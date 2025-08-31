using System;
using System.Threading.Tasks;
using CatUI.Data;
using CatUI.Data.Brushes;
using CatUI.Data.Containers.LinearContainers;
using CatUI.Data.ElementData;
using CatUI.Data.Enums;
using CatUI.Data.Theming;
using CatUI.Elements.Buttons;
using CatUI.Elements.Containers.Linear;
using CatUI.Elements.Containers.Scroll;
using CatUI.Elements.Text;
using CatUI.Elements.Utils;
using CatUI.Platform.Essentials;
using CatUI.Utils;

namespace CatUISample.UI.Pages.NativeUi
{
    public class FilePickingExamples : ScrollContainer
    {
        public FilePickingExamples()
        {
            ElementLayout _buttonsLayout = new ElementLayout().SetFixedWidth(200).SetFixedHeight(32);

            Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%");

            Content = new PaddingElement(new EdgeInset(0, 5))
            {
                Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                Children =
                [
                    new ColumnContainer
                    {
                        Layout = new ElementLayout().SetFixedWidth("100%").SetFixedHeight("100%"),
                        Arrangement = LinearArrangement.SpacedBy(5),
                        Children =
                        [
                            new Label("File picking examples", TextAlignmentType.Center)
                            {
                                Layout = new ElementLayout().SetMinMaxAndPreferredWidth("100%", 0, "100%"),
                                FontSize = 32,
                                TextBrush = new ColorBrush(CatTheme.Colors.OnSurface)
                            },
                            new Button("Open file", 16, new ColorBrush(CatTheme.Colors.OnPrimary))
                            {
                                Layout = _buttonsLayout,
                                Background = new ColorBrush(CatTheme.Colors.Primary),
                                OnClick = async void (_, _) =>
                                {
                                    try
                                    {
                                        Task<IFilePicker.OpenFilesResponse?>? task = OS.FilePicker?.OpenFilesAsync(
                                            "Open files (custom title)",
                                            true,
                                            new IFilePicker.FileFiltersArgument(
                                            [
                                                new IFilePicker.FileFilter(
                                                    "All files",
                                                    new FileGlobPattern("*.*")),
                                                new IFilePicker.FileFilter(
                                                    "Image files",
                                                    new FileGlobPattern(["*.png", "*.jpg"]))
                                            ], 1),
                                            //new Uri("/bin"),
                                            null,
                                            "Get file");

                                        if (task == null)
                                        {
                                            CatLogger.LogError("Failed to open file picker.");
                                            return;
                                        }

                                        IFilePicker.OpenFilesResponse? result = await task;
                                        if (result == null)
                                        {
                                            CatLogger.LogError("Picking aborted.");
                                            return;
                                        }

                                        CatLogger.LogDebug("Success! Files:");
                                        foreach (Uri fileUri in result.FileUris)
                                        {
                                            CatLogger.LogDebug(fileUri.ToString());
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        CatLogger.LogException(ex);
                                    }
                                }
                            },
                            new Button("Open directory", 16, new ColorBrush(CatTheme.Colors.OnPrimary))
                            {
                                Layout = _buttonsLayout,
                                Background = new ColorBrush(CatTheme.Colors.Primary),
                                OnClick = async void (_, _) =>
                                {
                                    try
                                    {
                                        Task<IFilePicker.OpenDirectoriesResponse?>? task =
                                            OS.FilePicker?.OpenDirectoriesAsync(
                                                "Open directory (custom title)",
                                                true);

                                        if (task == null)
                                        {
                                            CatLogger.LogError("Failed to open file picker.");
                                            return;
                                        }

                                        IFilePicker.OpenDirectoriesResponse? result = await task;
                                        if (result == null)
                                        {
                                            CatLogger.LogError("Picking aborted.");
                                            return;
                                        }

                                        CatLogger.LogDebug("Success! Directories:");
                                        foreach (Uri fileUri in result.DirectoryUris)
                                        {
                                            CatLogger.LogDebug(fileUri.ToString());
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        CatLogger.LogException(ex);
                                    }
                                }
                            },
                            new Button("Open files with choices", 16, new ColorBrush(CatTheme.Colors.OnPrimary))
                            {
                                Layout = _buttonsLayout,
                                Background = new ColorBrush(CatTheme.Colors.Primary),
                                OnClick = async void (_, _) =>
                                {
                                    try
                                    {
                                        Task<IFilePicker.OpenFilesResponse?>? task = OS.FilePicker?.OpenFilesAsync(
                                            "File picker with choices (custom title)",
                                            true,
                                            null,
                                            null,
                                            "Get file",
                                            [
                                                new IFilePicker.PickerChoicesRequest(
                                                    "opt1",
                                                    "Option 1",
                                                    [
                                                        ("opt1-val1", "Value 1"),
                                                        ("opt1-val2", "Value 2"),
                                                        ("opt1-val3", "Value 3")
                                                    ],
                                                    0),
                                                new IFilePicker.PickerChoicesRequest(
                                                    "opt2",
                                                    "Option 2",
                                                    [],
                                                    0)
                                            ]
                                        );

                                        if (task == null)
                                        {
                                            CatLogger.LogError("Failed to open file picker.");
                                            return;
                                        }

                                        IFilePicker.OpenFilesResponse? result = await task;
                                        if (result == null)
                                        {
                                            CatLogger.LogError("Picking aborted.");
                                            return;
                                        }

                                        CatLogger.LogDebug("Success! Files:");
                                        foreach (Uri fileUri in result.FileUris)
                                        {
                                            CatLogger.LogDebug(fileUri.ToString());
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        CatLogger.LogException(ex);
                                    }
                                }
                            },
                            new Button("Save file", 16, new ColorBrush(CatTheme.Colors.OnPrimary))
                            {
                                Layout = _buttonsLayout,
                                Background = new ColorBrush(CatTheme.Colors.Primary),
                                OnClick = async void (_, _) =>
                                {
                                    try
                                    {
                                        Task<IFilePicker.SaveFileResponse?>? task = OS.FilePicker?.SaveFileAsync(
                                            "Save file (custom title)",
                                            "file1.png");

                                        if (task == null)
                                        {
                                            CatLogger.LogError("Failed to open file picker.");
                                            return;
                                        }

                                        IFilePicker.SaveFileResponse? result = await task;
                                        if (result == null)
                                        {
                                            CatLogger.LogError("Picking aborted.");
                                            return;
                                        }

                                        CatLogger.LogDebug($"Success! File: {result.FileUri}");
                                    }
                                    catch (Exception ex)
                                    {
                                        CatLogger.LogException(ex);
                                    }
                                }
                            },
                            new Button("Save multiple files", 16, new ColorBrush(CatTheme.Colors.OnPrimary))
                            {
                                Layout = _buttonsLayout,
                                Background = new ColorBrush(CatTheme.Colors.Primary),
                                OnClick = async void (_, _) =>
                                {
                                    try
                                    {
                                        Task<IFilePicker.SaveFilesInDirectoryResponse?>? task =
                                            OS.FilePicker?.SaveFilesInDirectoryAsync(
                                                "Save multiple files (custom title)",
                                                ["file1.png", "file2.png", "file3.png"]);

                                        if (task == null)
                                        {
                                            CatLogger.LogError("Failed to open file picker.");
                                            return;
                                        }

                                        IFilePicker.SaveFilesInDirectoryResponse? result = await task;
                                        if (result == null)
                                        {
                                            CatLogger.LogError("Picking aborted.");
                                            return;
                                        }

                                        CatLogger.LogDebug("Success! Files:");
                                        foreach (Uri fileUri in result.FileUris)
                                        {
                                            CatLogger.LogDebug(fileUri.ToString());
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        CatLogger.LogException(ex);
                                    }
                                }
                            }
                        ]
                    }
                ]
            };
        }
    }
}
