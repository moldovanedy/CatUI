using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CatUI.Platform.Essentials;
using CatUI.Utils;
using Tmds.DBus.Protocol;

#if !DEBUG && TRACE
using System.Diagnostics;
#endif

namespace CatUI.Platform.Linux.OS
{
    public class FilePickerLinux : IFilePicker
    {
        public async Task<IFilePicker.OpenFilesResponse?> OpenFilesAsync(
            string dialogTitle,
            bool canSelectMultiple,
            IFilePicker.FileFiltersArgument? filterPattern = null,
            Uri? initialLocation = null,
            string? customSubmitButtonText = null,
            IFilePicker.PickerChoicesRequest[]? choices = null,
            object? windowIdentifier = null,
            CancellationToken? cancellationToken = null)
        {
            cancellationToken?.ThrowIfCancellationRequested();
            IFilePicker.OpenFilesResponse? result;

            if (XdgServices.FilePickerService != null)
            {
                result = await XdgGetOpenResponseAsync(
                    false,
                    dialogTitle,
                    canSelectMultiple,
                    filterPattern,
                    initialLocation,
                    customSubmitButtonText,
                    choices,
                    windowIdentifier,
                    cancellationToken);
            }
            else
            {
                result = await DialogProcGetOpenResponseAsync(
                    false,
                    dialogTitle,
                    canSelectMultiple,
                    false,
                    filterPattern,
                    initialLocation,
                    cancellationToken);
            }

            return result;
        }

        public async Task<IFilePicker.OpenDirectoriesResponse?> OpenDirectoriesAsync(
            string dialogTitle,
            bool canSelectMultiple,
            Uri? initialLocation = null,
            string? customSubmitButtonText = null,
            IFilePicker.PickerChoicesRequest[]? choices = null,
            object? windowIdentifier = null,
            CancellationToken? cancellationToken = null)
        {
            cancellationToken?.ThrowIfCancellationRequested();
            IFilePicker.OpenFilesResponse? result;

            if (XdgServices.FilePickerService != null)
            {
                result = await XdgGetOpenResponseAsync(
                    true,
                    dialogTitle,
                    canSelectMultiple,
                    null,
                    initialLocation,
                    customSubmitButtonText,
                    choices,
                    windowIdentifier,
                    cancellationToken);
            }
            else
            {
                result = await DialogProcGetOpenResponseAsync(
                    true,
                    dialogTitle,
                    canSelectMultiple,
                    false,
                    null,
                    initialLocation,
                    cancellationToken);
            }

            return
                result != null
                    ? new IFilePicker.OpenDirectoriesResponse(result.FileUris, result.PickerChoicesResponse)
                    : null;
        }

        public async Task<IFilePicker.SaveFileResponse?> SaveFileAsync(
            string dialogTitle,
            string fileName,
            IFilePicker.FileFiltersArgument? filterPattern = null,
            Uri? initialLocation = null,
            string? customSubmitButtonText = null,
            IFilePicker.PickerChoicesRequest[]? choices = null,
            object? windowIdentifier = null,
            CancellationToken? cancellationToken = null)
        {
            CommonSaveFilesResponse result;
            if (XdgServices.FilePickerService != null)
            {
                result =
                    await XdgGetSaveResponseAsync(
                        false,
                        dialogTitle,
                        [fileName],
                        filterPattern,
                        initialLocation,
                        customSubmitButtonText,
                        choices,
                        windowIdentifier,
                        cancellationToken);
            }
            else
            {
                result =
                    await DialogProcGetSaveResponseAsync(
                        dialogTitle,
                        false,
                        filterPattern,
                        initialLocation,
                        cancellationToken);
            }

            if (result.MainResponse == null)
            {
                return null;
            }

            return new IFilePicker.SaveFileResponse(
                result.MainResponse.FileUris.Length >= 1 ? result.MainResponse.FileUris[0] : new Uri(""),
                result.FileFilter,
                result.MainResponse.PickerChoicesResponse);
        }

        public async Task<IFilePicker.SaveFilesInDirectoryResponse?> SaveFilesInDirectoryAsync(
            string dialogTitle,
            string[] fileNames,
            Uri? initialLocation = null,
            string? customSubmitButtonText = null,
            IFilePicker.PickerChoicesRequest[]? choices = null,
            object? windowIdentifier = null,
            CancellationToken? cancellationToken = null)
        {
            if (XdgServices.FilePickerService != null)
            {
                CommonSaveFilesResponse result =
                    await XdgGetSaveResponseAsync(
                        true,
                        dialogTitle,
                        fileNames,
                        null,
                        initialLocation,
                        customSubmitButtonText,
                        choices,
                        windowIdentifier,
                        cancellationToken);

                return result.MainResponse;
            }

            return null;
        }

        #region XDG

        private static async Task<IFilePicker.OpenFilesResponse?> XdgGetOpenResponseAsync(
            bool isDirectoryPicker,
            string dialogTitle,
            bool canSelectMultiple,
            IFilePicker.FileFiltersArgument? filterPattern = null,
            Uri? initialLocation = null,
            string? customSubmitButtonText = null,
            IFilePicker.PickerChoicesRequest[]? choices = null,
            object? windowIdentifier = null,
            CancellationToken? cancellationToken = null)
        {
            if (XdgServices.FilePickerService == null)
            {
                return null;
            }

            Dictionary<string, VariantValue> options = new();
            if (isDirectoryPicker)
            {
                options.Add("directory", isDirectoryPicker);
            }

            if (customSubmitButtonText != null)
            {
                options.Add("accept_button", customSubmitButtonText);
            }

            if (canSelectMultiple)
            {
                options.Add("multiple", true);
            }

            if (initialLocation != null)
            {
                options.Add("current_folder", GetNullTerminatedString(initialLocation.AbsolutePath));
            }

            if (filterPattern != null)
            {
                options.Add("filters", GetFilters(filterPattern, out VariantValue? defaultFilter));
                if (defaultFilter != null)
                {
                    options.Add("current_filter", defaultFilter.Value);
                }
            }

            if (choices != null)
            {
                options.Add("choices", GetPickerChoices(choices));
            }

            if (windowIdentifier is not string windowId)
            {
                windowId = "";
            }

            ObjectPath responsePath =
                await XdgServices.FilePickerService.OpenFileAsync(windowId, dialogTitle, options);

            CancellationToken token = cancellationToken ?? CancellationToken.None;
            TaskCompletionSource<IFilePicker.OpenFilesResponse?> tcs =
                new(TaskCreationOptions.RunContinuationsAsynchronously);

            await using CancellationTokenRegistration ctr = token.Register(() => tcs.TrySetCanceled(token));
            bool isInErrorState = false;

            IDisposable watcher = await XdgServices.FilePickerService.WatchSignalAsync(
                XdgServices.FilePickerService.Service.Destination,
                "org.freedesktop.portal.Request",
                responsePath,
                "Response",
                (msg, _) =>
                {
                    Reader reader = msg.GetBodyReader();

                    uint code = reader.ReadUInt32();
                    Dictionary<string, VariantValue> results =
                        reader.ReadDictionaryOfStringToVariantValue();

                    return (code, results);
                },
                (Exception? ex, (uint Code, Dictionary<string, VariantValue> Results) data) =>
                {
                    if (ex != null)
                    {
#if DEBUG
                        Console.WriteLine(
                            $"[ERROR ({DateTime.Now:HH:mm:ss.fff})]: Exception (at open file): {ex}");
#elif TRACE
                                Trace.WriteLine(
                                    $"[ERROR ({DateTime.Now:HH:mm:ss.fff})]: Exception (at open file): {ex}");
#endif
                        tcs.TrySetResult(null);
                        return;
                    }

                    uint responseCode = data.Code;
                    //if not 0, it means it's dismissed or in an error state
                    switch (responseCode)
                    {
                        case 1:
                            tcs.TrySetResult(null);
                            return;
                        case 2:
                            tcs.TrySetResult(null);
                            isInErrorState = true;
                            return;
                    }

                    Dictionary<string, VariantValue> results = data.Results;

                    // Read URIs
                    List<Uri> selectedUris = [];
                    if (results.TryGetValue("uris", out VariantValue urisValue))
                    {
                        foreach (string uriString in urisValue.GetArray<string>())
                        {
                            if (Uri.TryCreate(uriString, UriKind.Absolute, out Uri? uri))
                            {
                                selectedUris.Add(uri);
                            }
                        }
                    }

                    Dictionary<string, string> selectedChoices = new();
                    if (results.TryGetValue("choices", out VariantValue choicesValue))
                    {
                        VariantValue[] selectedOptions = choicesValue.GetArray<VariantValue>();
                        foreach (VariantValue selectedOption in selectedOptions)
                        {
                            selectedChoices[selectedOption.GetItem(0).GetString()] =
                                selectedOption.GetItem(1).GetString();
                        }
                    }

                    string selectedFilterLabel = "";
                    List<string> selectedFilterPatterns = [];

                    if (results.TryGetValue("current_filter", out VariantValue selectedFilterValue))
                    {
                        selectedFilterLabel = selectedFilterValue.GetItem(0).GetString();

                        VariantValue[] patterns = selectedFilterValue.GetItem(1).GetArray<VariantValue>();
                        foreach (VariantValue pattern in patterns)
                        {
                            uint type = pattern.GetItem(0).GetUInt32();
                            if (type != 0)
                            {
                                //TODO: get pattern from MIME type
                                throw new NotImplementedException("Pattern to MIME type not implemented.");
                            }

                            selectedFilterPatterns.Add(pattern.GetItem(1).GetString());
                        }
                    }

                    IFilePicker.OpenFilesResponse catResponse = new(
                        selectedUris.ToArray(),
                        new IFilePicker.FileFilter(
                            selectedFilterLabel,
                            new FileGlobPattern(selectedFilterPatterns.ToArray())),
                        selectedChoices);

                    tcs.TrySetResult(catResponse);
                },
                false,
                ObserverFlags.None);

            try
            {
                IFilePicker.OpenFilesResponse? result = await tcs.Task.ConfigureAwait(false);
                watcher.Dispose();

                if (watcher is IAsyncDisposable ad)
                {
                    await ad.DisposeAsync().ConfigureAwait(false);
                }

                if (isInErrorState)
                {
                    throw new PlatformNotSupportedException("File picker failed.");
                }

                return result;
            }
            catch (OperationCanceledException)
            {
                watcher.Dispose();
                if (watcher is IAsyncDisposable ad)
                {
                    await ad.DisposeAsync().ConfigureAwait(false);
                }

                throw;
            }
        }

        private static async Task<CommonSaveFilesResponse> XdgGetSaveResponseAsync(
            bool isSaveDirectoryPicker,
            string dialogTitle,
            string[] fileNames,
            IFilePicker.FileFiltersArgument? singleFileFilterPattern = null,
            Uri? initialLocation = null,
            string? customSubmitButtonText = null,
            IFilePicker.PickerChoicesRequest[]? choices = null,
            object? windowIdentifier = null,
            CancellationToken? cancellationToken = null)
        {
            if (XdgServices.FilePickerService == null)
            {
                return new CommonSaveFilesResponse(null, null);
            }

            Dictionary<string, VariantValue> options = new();
            if (customSubmitButtonText != null)
            {
                options.Add("accept_button", customSubmitButtonText);
            }

            if (initialLocation != null)
            {
                options.Add("current_folder", GetNullTerminatedString(initialLocation.AbsolutePath));
            }

            if (singleFileFilterPattern != null && !isSaveDirectoryPicker)
            {
                options.Add("filters", GetFilters(singleFileFilterPattern, out VariantValue? defaultFilter));
                if (defaultFilter != null)
                {
                    options.Add("current_filter", defaultFilter.Value);
                }
            }

            if (choices != null)
            {
                options.Add("choices", GetPickerChoices(choices));
            }

            switch (fileNames.Length)
            {
                case 1:
                    options.Add("current_name", fileNames[0]);
                    break;
                case > 1:
                    {
                        Array<Array<byte>> rawFileLocations = [];
                        foreach (string fileName in fileNames)
                        {
                            rawFileLocations.Add(GetNullTerminatedString(fileName));
                        }

                        options.Add("files", rawFileLocations.AsVariantValue());
                    }
                    break;
            }

            if (windowIdentifier is not string windowId)
            {
                windowId = "";
            }


            ObjectPath responsePath;
            if (isSaveDirectoryPicker)
            {
                responsePath = await XdgServices.FilePickerService.SaveFilesAsync(windowId, dialogTitle, options);
            }
            else
            {
                responsePath = await XdgServices.FilePickerService.SaveFileAsync(windowId, dialogTitle, options);
            }

            CancellationToken token = cancellationToken ?? CancellationToken.None;
            TaskCompletionSource<CommonSaveFilesResponse?>
                tcs = new(TaskCreationOptions.RunContinuationsAsynchronously);

            await using CancellationTokenRegistration ctr = token.Register(() => tcs.TrySetCanceled(token));

            IDisposable watcher = await XdgServices.FilePickerService.WatchSignalAsync(
                XdgServices.FilePickerService.Service.Destination,
                "org.freedesktop.portal.Request",
                responsePath,
                "Response",
                (msg, _) =>
                {
                    Reader reader = msg.GetBodyReader();

                    uint code = reader.ReadUInt32();
                    Dictionary<string, VariantValue> results =
                        reader.ReadDictionaryOfStringToVariantValue();

                    return (code, results);
                },
                (Exception? ex, (uint Code, Dictionary<string, VariantValue> Results) data) =>
                {
                    if (ex != null)
                    {
#if DEBUG
                        Console.WriteLine(
                            $"[ERROR ({DateTime.Now:HH:mm:ss.fff})]: Exception (at save file): {ex}");
#elif TRACE
                        Trace.WriteLine(
                            $"[ERROR ({DateTime.Now:HH:mm:ss.fff})]: Exception (at save file): {ex}");
#endif
                        tcs.TrySetResult(new CommonSaveFilesResponse(null, null));
                        return;
                    }

                    uint responseCode = data.Code;
                    //if not 0, it means it's dismissed or in an error state
                    switch (responseCode)
                    {
                        case 1:
                            tcs.TrySetResult(new CommonSaveFilesResponse(null, null));
                            return;
                        case 2:
                            tcs.TrySetResult(null);
                            return;
                    }

                    Dictionary<string, VariantValue> results = data.Results;

                    // Read URIs
                    List<Uri> selectedUris = [];
                    if (results.TryGetValue("uris", out VariantValue urisValue))
                    {
                        foreach (string uriString in urisValue.GetArray<string>())
                        {
                            if (Uri.TryCreate(uriString, UriKind.Absolute, out Uri? uri))
                            {
                                selectedUris.Add(uri);
                            }
                        }
                    }

                    Dictionary<string, string> selectedChoices = new();
                    if (results.TryGetValue("choices", out VariantValue choicesValue))
                    {
                        VariantValue[] selectedOptions = choicesValue.GetArray<VariantValue>();
                        foreach (VariantValue selectedOption in selectedOptions)
                        {
                            selectedChoices[selectedOption.GetItem(0).GetString()] =
                                selectedOption.GetItem(1).GetString();
                        }
                    }

                    string selectedFilterLabel = "";
                    List<string> selectedFilterPatterns = [];

                    if (results.TryGetValue("current_filter", out VariantValue selectedFilterValue))
                    {
                        selectedFilterLabel = selectedFilterValue.GetItem(0).GetString();

                        VariantValue[] patterns = selectedFilterValue.GetItem(1).GetArray<VariantValue>();
                        foreach (VariantValue pattern in patterns)
                        {
                            uint type = pattern.GetItem(0).GetUInt32();
                            if (type != 0)
                            {
                                //TODO: get pattern from MIME type
                                throw new NotImplementedException("Pattern to MIME type not implemented.");
                            }

                            selectedFilterPatterns.Add(pattern.GetItem(1).GetString());
                        }
                    }

                    IFilePicker.SaveFilesInDirectoryResponse catResponse = new(
                        selectedUris.ToArray(),
                        selectedChoices);

                    var catFiltersResponse = new IFilePicker.FileFilter(
                        selectedFilterLabel,
                        new FileGlobPattern(selectedFilterPatterns.ToArray()));
                    tcs.TrySetResult(new CommonSaveFilesResponse(catResponse, catFiltersResponse));
                },
                false,
                ObserverFlags.None);

            try
            {
                CommonSaveFilesResponse? result = await tcs.Task.ConfigureAwait(false);
                watcher.Dispose();

                if (result == null)
                {
                    throw new PlatformNotSupportedException("File picker failed.");
                }

                if (watcher is IAsyncDisposable ad)
                {
                    await ad.DisposeAsync().ConfigureAwait(false);
                }

                return result;
            }
            catch (OperationCanceledException)
            {
                watcher.Dispose();
                if (watcher is IAsyncDisposable ad)
                {
                    await ad.DisposeAsync().ConfigureAwait(false);
                }

                throw;
            }
        }

        private static Array<byte> GetNullTerminatedString(string str)
        {
            byte[] locBytes = Encoding.UTF8.GetBytes(str);
            byte[] finalBytes = new byte[locBytes.Length + 1];
            Array.Copy(locBytes, finalBytes, locBytes.Length);

            return new Array<byte>(finalBytes);
        }

        private static VariantValue GetFilters(
            IFilePicker.FileFiltersArgument filterArgument,
            out VariantValue? currentFilter)
        {
            currentFilter = null;
            Array<Struct<string, Array<Struct<uint, string>>>> xdgFilters = [];
            int defaultFilterIdx = Math.Clamp(filterArgument.DefaultFilterIndex, 0, filterArgument.Filters.Length);

            for (int i = 0; i < filterArgument.Filters.Length; i++)
            {
                IFilePicker.FileFilter filter = filterArgument.Filters[i];

                Array<Struct<uint, string>> patterns = [];
                foreach (string catPattern in filter.Pattern.GetPatternsDirectly())
                {
                    patterns.Add(Struct.Create(0u, catPattern));
                }

                xdgFilters.Add(Struct.Create(filter.Label, patterns));

                if (i == defaultFilterIdx)
                {
                    currentFilter = xdgFilters[i].AsVariantValue();
                }
            }

            return xdgFilters.AsVariantValue();
        }

        private static VariantValue GetPickerChoices(IFilePicker.PickerChoicesRequest[] pickerChoicesRequest)
        {
            Array<Struct<string, string, Array<Struct<string, string>>, string>> xdgChoices = [];

            foreach (IFilePicker.PickerChoicesRequest choice in pickerChoicesRequest)
            {
                Array<Struct<string, string>> selectableOptions = [];
                string defaultOption = "";

                //if true, this will be a boolean (checkbox)
                if (choice.Options.Length == 0)
                {
                    defaultOption = choice.DefaultOptionIndex != 0 ? "true" : "false";
                }
                else
                {
                    int defaultOptionIdx = Math.Clamp(choice.DefaultOptionIndex, 0, choice.Options.Length);

                    for (int i = 0; i < choice.Options.Length; i++)
                    {
                        selectableOptions.Add(
                            Struct.Create(choice.Options[i].Item1, choice.Options[i].Item2));

                        if (i == defaultOptionIdx)
                        {
                            defaultOption = choice.Options[i].Item1;
                        }
                    }
                }

                xdgChoices.Add(
                    Struct.Create(
                        choice.ID,
                        choice.Label,
                        selectableOptions,
                        defaultOption));
            }

            return xdgChoices.AsVariantValue();
        }

        #endregion

        #region KDialog & Zenity

        private static async Task<IFilePicker.OpenFilesResponse?> DialogProcGetOpenResponseAsync(
            bool isDirectoryPicker,
            string dialogTitle,
            bool canSelectMultiple,
            bool hasZenityPriority = false,
            IFilePicker.FileFiltersArgument? filterPattern = null,
            Uri? initialLocation = null,
            CancellationToken? cancellationToken = null)
        {
            cancellationToken?.ThrowIfCancellationRequested();
            try
            {
                (AvailableDialogSystem, string?) response = TryFindProcess(hasZenityPriority);
                List<string> args = [];

                switch (response.Item1)
                {
                    default:
                    case AvailableDialogSystem.None:
                        throw new PlatformNotSupportedException("No dialog system (KDialog or Zenity) found.");
                    case AvailableDialogSystem.KDialog:
                        {
                            args.Add("--title");
                            args.Add(dialogTitle);

                            args.Add("--separate-output");

                            if (canSelectMultiple)
                            {
                                args.Add("--multiple");
                            }

                            args.Add(isDirectoryPicker ? "--getexistingdirectory" : "--getopenfilename");
                            args.Add(initialLocation?.AbsolutePath ?? "");

                            if (!isDirectoryPicker && filterPattern != null)
                            {
                                args.Add(GetKDialogFiltersString(filterPattern));
                            }
                        }
                        break;
                    case AvailableDialogSystem.Zenity:
                        {
                            args.Add($"--title={dialogTitle}");
                            args.Add("--modal");

                            if (canSelectMultiple)
                            {
                                args.Add("--multiple");
                            }

                            if (isDirectoryPicker)
                            {
                                args.Add("--directory");
                            }

                            args.Add("--separator=\n");

                            if (!isDirectoryPicker && filterPattern != null)
                            {
                                args.Add(GetZenityFiltersString(filterPattern));
                            }
                        }
                        break;
                }

                if (string.IsNullOrWhiteSpace(response.Item2))
                {
                    throw new PlatformNotSupportedException("The executable path is missing (dialog systems).");
                }

                Process? proc = StartProcess(response.Item2, args);
                if (proc == null)
                {
                    throw new PlatformNotSupportedException("The process could not be started.");
                }

                List<Uri> files = [];
                char[] buffer = new char[4096];
                while (!proc.HasExited)
                {
                    int bytesRead = await proc.StandardOutput.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0 && buffer[0] == '/')
                    {
                        files.Add(new Uri(new string(buffer, 0, bytesRead)));
                    }
                }

                return new IFilePicker.OpenFilesResponse(files.ToArray());
            }
            catch (OperationCanceledException)
            {
                return null;
            }
        }

        private static async Task<CommonSaveFilesResponse> DialogProcGetSaveResponseAsync(
            string dialogTitle,
            bool hasZenityPriority = false,
            IFilePicker.FileFiltersArgument? singleFileFilterPattern = null,
            Uri? initialLocation = null,
            CancellationToken? cancellationToken = null)
        {
            cancellationToken?.ThrowIfCancellationRequested();
            try
            {
                (AvailableDialogSystem, string?) response = TryFindProcess(hasZenityPriority);
                List<string> args = [];

                switch (response.Item1)
                {
                    default:
                    case AvailableDialogSystem.None:
                        throw new PlatformNotSupportedException("No dialog system (KDialog or Zenity) found.");
                    case AvailableDialogSystem.KDialog:
                        {
                            args.Add("--title");
                            args.Add(dialogTitle);

                            args.Add("--separate-output");

                            args.Add("--getsavefilename");
                            args.Add(initialLocation?.AbsolutePath ?? "");

                            if (singleFileFilterPattern != null)
                            {
                                args.Add(GetKDialogFiltersString(singleFileFilterPattern));
                            }
                        }
                        break;
                    case AvailableDialogSystem.Zenity:
                        {
                            args.Add($"--title={dialogTitle}");
                            args.Add("--modal");
                            args.Add("--save");

                            if (singleFileFilterPattern != null)
                            {
                                args.Add(GetZenityFiltersString(singleFileFilterPattern));
                            }
                        }
                        break;
                }

                if (string.IsNullOrWhiteSpace(response.Item2))
                {
                    throw new PlatformNotSupportedException("The executable path is missing (dialog systems).");
                }

                Process? proc = StartProcess(response.Item2, args);
                if (proc == null)
                {
                    throw new PlatformNotSupportedException("The process could not be started.");
                }

                Uri? file = null;
                char[] buffer = new char[4096];
                while (!proc.HasExited)
                {
                    int bytesRead = await proc.StandardOutput.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead > 0 && buffer[0] == '/')
                    {
                        file = new Uri(new string(buffer, 0, bytesRead));
                    }
                }

                return new CommonSaveFilesResponse(
                    new IFilePicker.SaveFilesInDirectoryResponse(
                        file != null ? [file] : []),
                    null);
            }
            catch (OperationCanceledException)
            {
                return new CommonSaveFilesResponse(null, null);
            }
        }

        private static (AvailableDialogSystem, string?) TryFindProcess(bool hasZenityPriority)
        {
            string[] userPathLocations = (Environment.GetEnvironmentVariable("PATH") ?? "").Split(':');
            AvailableDialogSystem[] dialogSystems =
                hasZenityPriority
                    ? [AvailableDialogSystem.Zenity, AvailableDialogSystem.KDialog]
                    : [AvailableDialogSystem.KDialog, AvailableDialogSystem.Zenity];

            foreach (AvailableDialogSystem dialogSystem in dialogSystems)
            {
                string programName = dialogSystem == AvailableDialogSystem.Zenity ? "zenity" : "kdialog";

                foreach (string pathLocation in userPathLocations)
                {
                    if (File.Exists(Path.Join(pathLocation, programName)))
                    {
                        return (dialogSystem, Path.Join(pathLocation, programName));
                    }
                }
            }

            return (AvailableDialogSystem.None, null);
        }

        private static Process? StartProcess(string execPath, List<string> args)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = execPath,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            foreach (string argument in args)
            {
                startInfo.ArgumentList.Add(argument);
            }

            Process? proc = Process.Start(startInfo);
            return proc;
        }

        private static string GetKDialogFiltersString(IFilePicker.FileFiltersArgument filterPattern)
        {
            int defaultFilterIndex = Math.Clamp(filterPattern.DefaultFilterIndex, 0, filterPattern.Filters.Length);
            if (defaultFilterIndex != 0)
            {
                //swap
                (filterPattern.Filters[defaultFilterIndex], filterPattern.Filters[0]) =
                    (filterPattern.Filters[0], filterPattern.Filters[defaultFilterIndex]);
            }

            StringBuilder sb = new();
            foreach (IFilePicker.FileFilter filter in filterPattern.Filters)
            {
                sb.Append(filter.Label);
                sb.Append('(');

                foreach (string pattern in filter.Pattern.GetPatternsDirectly())
                {
                    sb.Append(pattern);
                    sb.Append(' ');
                }

                sb.Remove(sb.Length - 1, 1);
                sb.Append(") | ");
            }

            sb.Remove(sb.Length - 3, 3);
            return sb.ToString();
        }

        private static string GetZenityFiltersString(IFilePicker.FileFiltersArgument filterPattern)
        {
            int defaultFilterIndex = Math.Clamp(filterPattern.DefaultFilterIndex, 0, filterPattern.Filters.Length);
            if (defaultFilterIndex != 0)
            {
                //swap
                (filterPattern.Filters[defaultFilterIndex], filterPattern.Filters[0]) =
                    (filterPattern.Filters[0], filterPattern.Filters[defaultFilterIndex]);
            }

            StringBuilder sb = new();
            foreach (IFilePicker.FileFilter filter in filterPattern.Filters)
            {
                sb.Append("--file-filter=");
                sb.Append(filter.Label);
                sb.Append(" | ");

                foreach (string pattern in filter.Pattern.GetPatternsDirectly())
                {
                    sb.Append(pattern);
                    sb.Append(' ');
                }

                sb.Remove(sb.Length - 1, 1);
            }

            return sb.ToString();
        }

        private enum AvailableDialogSystem
        {
            None = 0,
            KDialog = 1,
            Zenity = 2
        }

        #endregion

        private record CommonSaveFilesResponse(
            IFilePicker.SaveFilesInDirectoryResponse? MainResponse,
            IFilePicker.FileFilter? FileFilter);
    }
}
