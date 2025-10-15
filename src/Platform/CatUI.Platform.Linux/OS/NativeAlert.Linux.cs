using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using CatUI.Platform.CommonInterface;
using CatUI.Utils;

namespace CatUI.Platform.Linux.OS;

/// <remarks>
/// Does not work properly on sandboxed environments like Flatpak, it also suffers from not being able to attach
/// the alert window to the main window, thus the user can just ignore the alert.
/// </remarks>
public class NativeAlertLinux : INativeAlert
{
    /// <summary>
    /// An immutable array that describes the "priority" of each backend. If a backend is not available (or not
    /// suitable for the desired behavior), the next one will be tried. If the array is empty, every call to show
    /// an alert will throw an <see cref="PlatformNotSupportedException"/>. You don't have to include all the
    /// backends. The default value is KDialog, then Zenity, then XMessage.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Changing this will only affect the next show alert call.
    /// </para>
    /// <para>
    /// As stated, <see cref="LinuxAlertBackend.XMessage"/> is very old and looks primitive. It's your choice if
    /// you prefer to have a weird-looking dialog or just throw an exception if nothing except XMessage is found
    /// by choosing to include/not include <see cref="LinuxAlertBackend.XMessage"/> in this list.
    /// </para>
    /// </remarks>
    public ImmutableArray<LinuxAlertBackend> BackendsPriority { get; set; } =
        [LinuxAlertBackend.KDialog, LinuxAlertBackend.Zenity, LinuxAlertBackend.XMessage];

    private readonly Dictionary<Guid, Process> _openAlerts = [];

    /// <summary>
    /// We need to determine whether we use Zenity's "extra" button (only with 3 buttons), as in that case Zenity
    /// writes the label in the stdout instead of returning an exit code.
    /// </summary>
    private string? _zenityExtraButtonLabel;

    /// <inheritdoc />
    /// <remarks>
    /// On Linux:
    /// <para>
    /// Zenity: only <see cref="INativeAlert.Icon.Question"/> can have 2 or 3 buttons, setting more than one button
    /// and something other than Question will convert the icon to a <see cref="INativeAlert.Icon.Question"/>.
    /// </para>
    /// <para>
    /// KDialog: <see cref="INativeAlert.Icon.Error"/> can only have one button, setting more than one button
    /// and Error will convert the icon to a <see cref="INativeAlert.Icon.Warning"/>.
    /// </para>
    /// <para>
    /// XMessage: icon is not supported at all, the name (Question, Info, Warning, Error) will be prepended to the
    /// message (along with ": ", of course).
    /// </para>
    /// </remarks>
    public async Task<INativeAlert.Button?> ShowAlertAsync(
        string title,
        string message,
        INativeAlert.Icon icon = INativeAlert.Icon.Info,
        INativeAlert.Button buttons = INativeAlert.Button.Ok,
        ObjectRef<Guid>? alertId = null,
        IntPtr windowHandle = 0)
    {
        if (alertId != null)
        {
            alertId.Value = Guid.NewGuid();
        }

        List<string> buttonsLabels = [];
        if ((buttons & INativeAlert.Button.Yes) != 0)
        {
            buttonsLabels.Add("Yes");
        }

        if ((buttons & INativeAlert.Button.No) != 0)
        {
            buttonsLabels.Add("No");
        }

        if ((buttons & INativeAlert.Button.Ok) != 0)
        {
            buttonsLabels.Add("OK");
        }

        if ((buttons & INativeAlert.Button.Retry) != 0)
        {
            buttonsLabels.Add("Retry");
        }

        if ((buttons & INativeAlert.Button.Continue) != 0)
        {
            buttonsLabels.Add("Continue");
        }

        if ((buttons & INativeAlert.Button.Cancel) != 0)
        {
            buttonsLabels.Add("Cancel");
        }

        if (buttonsLabels.Count == 0)
        {
            buttonsLabels.Add("OK");
        }

        if (buttonsLabels.Count > 3)
        {
            throw new ArgumentException("Too many buttons (more than 3).");
        }

        int? result = await ShowAlertAsync(
            title,
            message,
            buttonsLabels[0],
            buttonsLabels.Count >= 2 ? buttonsLabels[1] : null,
            buttonsLabels.Count >= 3 ? buttonsLabels[2] : null,
            icon);

        if (result == null || result < 0 || result > 2)
        {
            return null;
        }

        if (result >= buttonsLabels.Count)
        {
            return null;
        }

        switch (buttonsLabels[result.Value])
        {
            case "Yes":
                return INativeAlert.Button.Yes;
            case "No":
                return INativeAlert.Button.No;
            case "OK":
                return INativeAlert.Button.Ok;
            case "Retry":
                return INativeAlert.Button.Retry;
            case "Continue":
                return INativeAlert.Button.Continue;
            case "Cancel":
                return INativeAlert.Button.Cancel;
            default:
                return null;
        }
    }

    /// <inheritdoc />
    /// <remarks>
    /// On Linux:
    /// <para>
    /// Zenity: only <see cref="INativeAlert.Icon.Question"/> can have 2 or 3 buttons, setting more than one button
    /// and something other than Question will convert the icon to a <see cref="INativeAlert.Icon.Question"/>.
    /// </para>
    /// <para>
    /// KDialog: <see cref="INativeAlert.Icon.Error"/> can only have one button, setting more than one button
    /// and Error will convert the icon to a <see cref="INativeAlert.Icon.Warning"/>.
    /// </para>
    /// <para>
    /// XMessage: icon is not supported at all, the name (Question, Info, Warning, Error) will be prepended to the
    /// message (along with ": ", of course).
    /// </para>
    /// </remarks>
    public async Task<int?> ShowAlertAsync(
        string title,
        string message,
        string btn1Text,
        string? btn2Text = null,
        string? btn3Text = null,
        INativeAlert.Icon icon = INativeAlert.Icon.Info,
        ObjectRef<Guid>? alertId = null,
        IntPtr windowHandle = 0)
    {
        if (alertId != null)
        {
            alertId.Value = Guid.NewGuid();
        }

        Process? proc = FindAndUseDialogSystem(title, message, btn1Text, btn2Text, btn3Text, icon);
        if (proc == null)
        {
            throw new PlatformNotSupportedException("No dialog system (KDialog, Zenity, or XMessage) found.");
        }

        _openAlerts.Add(alertId?.Value ?? Guid.NewGuid(), proc);
        if (_zenityExtraButtonLabel != null)
        {
            char[] buffer = new char[_zenityExtraButtonLabel.Length];
            while (!proc.HasExited)
            {
                _ = await proc.StandardOutput.ReadAsync(buffer, 0, buffer.Length);
                if (_zenityExtraButtonLabel == new string(buffer))
                {
                    //2 is the index of the third button, that's always the "extra" button in Zenity
                    return 2;
                }
            }
        }
        else
        {
            await proc.WaitForExitAsync();
        }

        if (proc.ExitCode >= 0 && proc.ExitCode <= 2)
        {
            return proc.ExitCode;
        }

        return null;
    }

    public bool CloseAlert(Guid id)
    {
        if (_openAlerts.TryGetValue(id, out Process? proc))
        {
            proc.Close();
            return _openAlerts.Remove(id);
        }

        return false;
    }

    private Process? FindAndUseDialogSystem(
        string title,
        string message,
        string btn1Text,
        string? btn2Text = null,
        string? btn3Text = null,
        INativeAlert.Icon icon = INativeAlert.Icon.Info)
    {
        if (btn2Text == null && btn3Text != null)
        {
            throw new ArgumentException("Can't have button 3 without button 2.");
        }

        string[] userPathLocations = (Environment.GetEnvironmentVariable("PATH") ?? "").Split(':');

        while (true)
        {
            LinuxAlertBackend? foundBackend = null;
            string? fullExecutePath = null;

            foreach (string pathLocation in userPathLocations)
            {
                foreach (LinuxAlertBackend linuxBackends in BackendsPriority)
                {
                    string? programName = null;
                    // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
                    switch (linuxBackends)
                    {
                        case LinuxAlertBackend.KDialog:
                            programName = "kdialog";
                            break;
                        case LinuxAlertBackend.Zenity:
                            programName = "zenity";
                            break;
                        case LinuxAlertBackend.XMessage:
                            programName = "xmessage";
                            break;
                    }

                    if (programName == null)
                    {
                        continue;
                    }

                    if (File.Exists(Path.Join(pathLocation, programName)))
                    {
                        foundBackend = linuxBackends;
                        fullExecutePath = Path.Join(pathLocation, programName);
                        break;
                    }
                }

                if (foundBackend != null)
                {
                    break;
                }
            }

            if (foundBackend == null || fullExecutePath == null)
            {
                return null;
            }

            List<string> args = [];
            switch (foundBackend)
            {
                case LinuxAlertBackend.KDialog:
                    {
                        //logic error: it's a Question, but only one button is given
                        if (icon == INativeAlert.Icon.Question && btn2Text == null)
                        {
                            icon = INativeAlert.Icon.Info;
                        }

                        //KDialog limitation: can't have multiple buttons on an Error, so convert it to a Warning
                        if (icon == INativeAlert.Icon.Error && btn2Text != null)
                        {
                            icon = INativeAlert.Icon.Warning;
                        }

                        bool hasOnlyOk = false;
                        switch (icon)
                        {
                            default:
                            case INativeAlert.Icon.Info:
                                args.Add("--msgbox");
                                hasOnlyOk = true;
                                break;
                            case INativeAlert.Icon.Question:
                                args.Add(btn3Text == null ? "--yesno" : "--yesnocancel");
                                break;
                            case INativeAlert.Icon.Warning:
                                if (btn2Text == null)
                                {
                                    args.Add("--sorry");
                                    hasOnlyOk = true;
                                }
                                else if (btn3Text == null)
                                {
                                    args.Add("--warningyesno");
                                }
                                else
                                {
                                    args.Add("--warningyesnocancel");
                                }

                                break;
                            case INativeAlert.Icon.Error:
                                args.Add("--error");
                                hasOnlyOk = true;
                                break;
                        }

                        args.Add(message);

                        args.Add("--title");
                        args.Add(title);

                        if (hasOnlyOk)
                        {
                            args.Add("--ok-label");
                            args.Add(btn1Text);
                        }
                        else
                        {
                            args.Add("--yes-label");
                            args.Add(btn1Text);

                            if (btn2Text != null)
                            {
                                args.Add("--no-label");
                                args.Add(btn2Text);
                            }

                            if (btn3Text != null)
                            {
                                args.Add("--cancel-label");
                                args.Add(btn3Text);
                            }
                        }
                    }
                    break;
                case LinuxAlertBackend.Zenity:
                    {
                        //Zenity limitation: can't have multiple buttons on something else than Questions, so
                        // convert it to a Question
                        if (icon != INativeAlert.Icon.Question && btn2Text != null)
                        {
                            icon = INativeAlert.Icon.Question;
                        }

                        switch (icon)
                        {
                            default:
                            case INativeAlert.Icon.Info:
                                args.Add("--info");
                                break;
                            case INativeAlert.Icon.Question:
                                args.Add("--question");
                                break;
                            case INativeAlert.Icon.Warning:
                                args.Add("--warning");
                                break;
                            case INativeAlert.Icon.Error:
                                args.Add("--error");
                                break;
                        }

                        args.Add($"--title={title}");
                        args.Add($"--text={message}");
                        args.Add("--modal");

                        args.Add($"--ok-label={btn1Text}");

                        if (icon == INativeAlert.Icon.Question)
                        {
                            if (btn2Text != null)
                            {
                                args.Add($"--cancel-label={btn2Text}");
                            }

                            if (btn3Text != null)
                            {
                                args.Add($"--extra-button={btn3Text}");
                                _zenityExtraButtonLabel = btn3Text;
                            }
                        }
                    }
                    break;
                case LinuxAlertBackend.XMessage:
                    {
                        args.Add("-center");

                        args.Add("-title");
                        args.Add(title);

                        string buttonText = btn1Text + ":0";
                        if (btn2Text != null)
                        {
                            buttonText += $",{btn2Text}:1";
                        }

                        if (btn3Text != null)
                        {
                            buttonText += $",{btn3Text}:2";
                        }

                        args.Add("-buttons");
                        args.Add(buttonText);

                        string iconLabel;
                        switch (icon)
                        {
                            case INativeAlert.Icon.Info:
                                iconLabel = "Info: ";
                                break;
                            case INativeAlert.Icon.Question:
                                iconLabel = "Question: ";
                                break;
                            case INativeAlert.Icon.Warning:
                                iconLabel = "Warning: ";
                                break;
                            case INativeAlert.Icon.Error:
                                iconLabel = "Error: ";
                                break;
                            default:
                                iconLabel = "";
                                break;
                        }

                        args.Add($"{iconLabel}{message}");
                    }
                    break;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = fullExecutePath,
                UseShellExecute = false,
                RedirectStandardOutput = _zenityExtraButtonLabel != null,
                RedirectStandardError = _zenityExtraButtonLabel != null
            };

            foreach (string argument in args)
            {
                startInfo.ArgumentList.Add(argument);
            }

            Process? proc = Process.Start(startInfo);
            return proc;
        }
    }

    /// <summary>
    /// Represents the chosen backend for alerts. Since Linux is the only notable platform that doesn't have a
    /// built-in method to show alerts, we "fake" it through some common Linux applications. If the running system
    /// does not have any of these apps, showing alerts will throw an <see cref="PlatformNotSupportedException"/>.
    /// </summary>
    public enum LinuxAlertBackend
    {
        /// <summary>
        /// A lightweight, fast, modern dialog system. It's almost always installed on KDE systems (like KDE Plasma
        /// desktop).
        /// </summary>
        KDialog = 0,

        /// <summary>
        /// The most popular dialog system. It uses GTK and is default on GNOME systems. It might run slower than
        /// KDialog, though.
        /// </summary>
        Zenity = 1,

        /// <summary>
        /// A very old dialog system. It should be present on (almost) all Linux installations, but it has a very
        /// primitive look and feel (remember that it was built in the 90's). Avoid this if possible.
        /// </summary>
        XMessage = 2
    }
}
