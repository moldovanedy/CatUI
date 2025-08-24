using System;
using System.Threading.Tasks;
using CatUI.Platform.Essentials;
using CatUI.Platform.Windows.PInvoke;
using CatUI.Utils;

namespace CatUI.Platform.Windows.OS
{
    /// <remarks>
    /// On Windows:
    /// <list type="bullet">
    /// <item>
    /// Custom labels on buttons is not supported. The function will throw <see cref="PlatformNotSupportedException"/>.
    /// </item>
    /// <item> "Retry" is generally spelled as "Try Again".</item>
    /// <item>
    /// Even standard buttons might fail with  <see cref="PlatformNotSupportedException"/>, unless you use the values
    /// prefixed with Common* from  <see cref="INativeAlert.Button"/> (refer to win32 documentation regarding the
    /// MessageBox function).
    /// </item>
    /// <item>
    /// <see cref="CloseAlert"/> has no effect, as Windows blocks the entire application thread when opening dialogs.
    /// </item>
    /// </list>
    /// </remarks>
    public class NativeAlertWindows : INativeAlert
    {
        private bool _isAlertOpened;

        public Task<INativeAlert.Button?> ShowAlertAsync(
            string title,
            string message,
            INativeAlert.Icon icon = INativeAlert.Icon.Info,
            INativeAlert.Button buttons = INativeAlert.Button.Ok,
            ObjectRef<Guid>? alertId = null,
            IntPtr windowHandle = 0)
        {
            if (_isAlertOpened)
            {
                return Task.FromResult<INativeAlert.Button?>(null);
            }

            try
            {
                if (alertId != null)
                {
                    alertId.Value = Guid.NewGuid();
                    _isAlertOpened = true;
                }

                uint winIcon;
                switch (icon)
                {
                    default:
                    case INativeAlert.Icon.Info:
                        winIcon = (uint)User32.MessageBoxType.MB_ICONINFORMATION;
                        break;
                    case INativeAlert.Icon.Warning:
                        winIcon = (uint)User32.MessageBoxType.MB_ICONWARNING;
                        break;
                    case INativeAlert.Icon.Error:
                        winIcon = (uint)User32.MessageBoxType.MB_ICONERROR;
                        break;
                    case INativeAlert.Icon.Question:
                        winIcon = (uint)User32.MessageBoxType.MB_ICONQUESTION;
                        break;
                }

                uint winButtons;
                switch (buttons)
                {
                    case INativeAlert.Button.Ok:
                        winButtons = (uint)User32.MessageBoxType.MB_OK;
                        break;
                    case INativeAlert.Button.CommonOkCancel:
                        winButtons = (uint)User32.MessageBoxType.MB_OKCANCEL;
                        break;
                    case INativeAlert.Button.CommonYesNo:
                        winButtons = (uint)User32.MessageBoxType.MB_YESNO;
                        break;
                    case INativeAlert.Button.CommonYesNoCancel:
                        winButtons = (uint)User32.MessageBoxType.MB_YESNOCANCEL;
                        break;
                    case INativeAlert.Button.CommonRetryContinue:
                        winButtons = (uint)User32.MessageBoxType.MB_RETRYCANCEL;
                        break;
                    case INativeAlert.Button.CommonRetryContinueCancel:
                        winButtons = (uint)User32.MessageBoxType.MB_CANCELTRYCONTINUE;
                        break;
                    default:
                        throw new PlatformNotSupportedException("Unsupported button combination for Windows.");
                }

                int result = User32.MessageBox(windowHandle, message, title, winButtons | winIcon);

                INativeAlert.Button? buttonResult = null;
                switch (result)
                {
                    case (int)User32.MessageBoxResult.IDOK:
                        buttonResult = INativeAlert.Button.Ok;
                        break;
                    case (int)User32.MessageBoxResult.IDABORT:
                    case (int)User32.MessageBoxResult.IDCANCEL:
                        buttonResult = INativeAlert.Button.Cancel;
                        break;
                    case (int)User32.MessageBoxResult.IDYES:
                        buttonResult = INativeAlert.Button.Yes;
                        break;
                    case (int)User32.MessageBoxResult.IDNO:
                        buttonResult = INativeAlert.Button.No;
                        break;
                    case (int)User32.MessageBoxResult.IDTRYAGAIN:
                    case (int)User32.MessageBoxResult.IDRETRY:
                        buttonResult = INativeAlert.Button.Retry;
                        break;
                    case (int)User32.MessageBoxResult.IDCONTINUE:
                        buttonResult = INativeAlert.Button.Continue;
                        break;
                }

                return Task.FromResult(buttonResult);
            }
            finally
            {
                _isAlertOpened = false;
            }
        }

        public Task<int?> ShowAlertAsync(
            string title,
            string message,
            string btn1Text,
            string? btn2Text = null,
            string? btn3Text = null,
            INativeAlert.Icon icon = INativeAlert.Icon.Info,
            ObjectRef<Guid>? alertId = null,
            IntPtr windowHandle = 0)
        {
            throw new PlatformNotSupportedException(
                "Windows does not support custom labels for buttons. Use the overload with standard buttons instead.");
        }

        public bool CloseAlert(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
