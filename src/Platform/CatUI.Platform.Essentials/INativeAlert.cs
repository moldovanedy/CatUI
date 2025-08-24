using System;
using System.Threading.Tasks;
using CatUI.Utils;

namespace CatUI.Platform.Essentials
{
    /// <summary>
    /// Describes the interface for native platform alerts (message boxes) that might also display multiple buttons
    /// to present the user a choice.
    /// </summary>
    /// <remarks>
    /// On WebAssembly, this does not work. Use custom alerts instead.
    /// </remarks>
    public interface INativeAlert
    {
        /// <summary>
        /// Shows an alert using a custom title and message, with a predefined icon and one or more buttons.
        /// </summary>
        /// <param name="title">The alert title.</param>
        /// <param name="message">The alert message.</param>
        /// <param name="icon">An icon to show in the alert (this won't appear on iOS).</param>
        /// <param name="buttons">
        /// <para>
        /// One or more predefined buttons. All platforms support one or 2 buttons, but few support 3 buttons.
        /// If you pass more than 3 buttons, the function will throw an <see cref="ArgumentException"/>. If icon is
        /// <see cref="Icon.Question"/>, but you only pass one button, it will automatically convert the icon to
        /// <see cref="Icon.Info"/>.
        /// </para>
        /// <para>
        /// The order of the buttons is: (Yes, No, Cancel), (OK, Cancel), (Retry, Cancel), (Continue, Cancel),
        /// (Retry, Continue, Cancel), or this priority: (Yes, No, OK, Retry, Continue, Cancel).
        /// </para>
        /// </param>
        /// <param name="alertId">
        /// An object reference to the alert ID. Its value will be set directly when the function is called, so you can
        /// use the value directly after this call.
        /// </param>
        /// <param name="windowHandle">
        /// On supporting platforms, it will make the alert a popup window for the specified window, thus making the
        /// behavior more consistent with the runtime platform. Currently only used on Windows.
        /// </param>
        /// <returns>The button selected by the user or null if the dialog was timed out or forcefully closed by the
        /// user without selecting a button.
        /// </returns>
        Task<Button?> ShowAlertAsync(
            string title, string message, Icon icon = Icon.Info, Button buttons = Button.Ok,
            ObjectRef<Guid>? alertId = null, IntPtr windowHandle = 0);

        /// <summary>
        /// A more advanced alert showing function, supporting more customization like a custom icon and custom buttons
        /// text.
        /// </summary>
        /// <remarks>
        /// The layout of the buttons (left-to-right or right-to-left) is platform-specific, and you cannot override it.
        /// If icon is <see cref="Icon.Question"/>, but you only pass one button, it will automatically convert the
        /// icon to <see cref="Icon.Info"/>.
        /// </remarks>
        /// <param name="title">The alert title.</param>
        /// <param name="message">The alert message.</param>
        /// <param name="btn1Text">The custom text of the first button.</param>
        /// <param name="btn2Text">
        /// The custom text of the first button. Null means the alert will have a single button.
        /// </param>
        /// <param name="btn3Text">
        /// The custom text of the first button. Null means the alert will have two buttons. If this is non-null, but
        /// the second button is null, an <see cref="ArgumentException"/> will be thrown.
        /// </param>
        /// <param name="icon">An icon to show in the alert (this won't appear on iOS).</param>
        /// <param name="alertId">
        /// An object reference to the alert ID. Its value will be set directly when the function is called, so you can
        /// use the value directly after this call.
        /// </param>
        /// <param name="windowHandle">
        /// On supporting platforms, it will make the alert a popup window for the specified window, thus making the
        /// behavior more consistent with the runtime platform. Currently only used on Windows.
        /// </param>
        /// <returns>
        /// The 0-based index of the pressed button (first, second, or third) or null if the dialog was
        /// timed out or forcefully closed by the user without selecting a button. 
        /// </returns>
        Task<int?> ShowAlertAsync(
            string title, string message,
            string btn1Text, string? btn2Text = null, string? btn3Text = null,
            Icon icon = Icon.Info, ObjectRef<Guid>? alertId = null, IntPtr windowHandle = 0);

        /// <summary>
        /// Tries to close the alert using the given ID.
        /// </summary>
        /// <param name="id">The ID of the alert.</param>
        /// <returns>True if the closing was successful, false otherwise.</returns>
        bool CloseAlert(Guid id);

        /// <summary>
        /// Show an icon in the alert. All platforms except iOS support this; on iOS, the alert will not have any icon.
        /// </summary>
        enum Icon
        {
            /// <summary>
            /// Usually a circle with an "i" in the middle.
            /// </summary>
            Info = 0,

            /// <summary>
            /// Usually a circle with an "?" in the middle. Note that some platforms don't support this value, so this
            /// will try to draw a custom question sign if the platform supports it, otherwise will just fall back to
            /// <see cref="Warning"/>.
            /// </summary>
            Question = 1,

            /// <summary>
            /// Usually a yellow triangle with an "!" in the middle.
            /// </summary>
            Warning = 2,

            /// <summary>
            /// Usually a red circle with an "x" in the middle.
            /// </summary>
            Error = 3
        }

        /// <summary>
        /// Represents common buttons normally used in alerts. It is recommended to use those instead of custom ones
        /// whenever possible. The options prefixed with "Common" are standardized, so users are familiar with them,
        /// and those should display correctly on all platforms.
        /// </summary>
        [Flags]
        enum Button
        {
            Ok = 1,
            Cancel = 1 << 1,
            Yes = 1 << 2,
            No = 1 << 3,
            Retry = 1 << 4,
            Continue = 1 << 5,
            CommonYesNo = Yes | No,
            CommonYesNoCancel = Yes | No | Cancel,
            CommonOkCancel = Ok | Cancel,
            CommonRetryContinue = Retry | Continue,
            CommonRetryContinueCancel = Retry | Continue | Cancel
        }
    }
}
