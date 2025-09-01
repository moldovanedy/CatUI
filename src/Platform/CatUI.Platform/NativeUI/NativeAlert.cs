using System;
using System.Threading.Tasks;
using CatUI.Platform.CommonInterface;
using CatUI.Utils;

namespace CatUI.Platform.NativeUI
{
    /// <summary>
    /// A native alert (or message box), specific to the runtime platform and using its native look and feel. It's
    /// a convenient wrapper around <see cref="INativeAlert"/>, so read the documentation for that mechanism, as well
    /// as for each implementation of it to get more information about platform support and limitations.
    /// </summary>
    public class NativeAlert
    {
        /// <summary>
        /// The alert title. Setting this while the alert is open won't change anything in the current alert, only for
        /// the next alert opening.
        /// </summary>
        public string Title { get; set; } = "";

        /// <summary>
        /// The alert message. Setting this while the alert is open won't change anything in the current alert,
        /// only for the next alert opening.
        /// </summary>
        public string Message { get; set; } = "";

        /// <summary>
        /// The alert icon, indicating the severity of the message. Note that iOS and Linux XMessage don't support
        /// graphical icons, instead the severity will be prepended to the message (e.g. "Warning: MESSAGE").
        /// Setting this while the alert is open won't change anything in the current alert, only for the next alert
        /// opening.
        /// </summary>
        public INativeAlert.Icon Icon { get; set; } = INativeAlert.Icon.Info;

        private Guid _usedId = Guid.Empty;

        public NativeAlert() { }

        /// <summary>
        /// Creates an alert object by setting <see cref="Title"/>, <see cref="Message"/>, and <see cref="Icon"/>.
        /// </summary>
        /// <param name="title">Sets <see cref="Title"/>.</param>
        /// <param name="message">Sets <see cref="Message"/>.</param>
        /// <param name="icon">Sets <see cref="Icon"/>.</param>
        public NativeAlert(string title, string message, INativeAlert.Icon icon = INativeAlert.Icon.Info)
        {
            Title = title;
            Message = message;
            Icon = icon;
        }

        /// <summary>
        /// Opens the alert with the specified buttons and waits for user action, returning the pressed button or null
        /// if the alert was closed, either by the user or by <see cref="Close"/>. This is just a convenient wrapper for
        /// <see cref="INativeAlert.ShowAlertAsync(string,string,INativeAlert.Icon,INativeAlert.Button,CatUI.Utils.ObjectRef{System.Guid}?,IntPtr)"/>,
        /// so also check that function for more info.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Some platforms have limitations regarding the number of buttons for a certain <see cref="Icon"/>; in
        /// that case, a different <see cref="Icon"/> will be presented so the requested buttons can appear normally.
        /// </para>
        /// <para>
        /// If the alert is already open, and you call this again while it is open, it will immediately return null.
        /// </para>
        /// </remarks>
        /// <param name="buttons">The buttons you want the user to have.</param>
        /// <returns>
        /// The pressed button or null if the user dismissed the alert (in can happen on some platforms) or it was
        /// closed by <see cref="Close"/>.
        /// </returns>
        /// <seealso cref="INativeAlert.ShowAlertAsync(string,string,INativeAlert.Icon,INativeAlert.Button,CatUI.Utils.ObjectRef{System.Guid}?,IntPtr)"/>
        public async Task<INativeAlert.Button?> OpenAsync(INativeAlert.Button buttons)
        {
            if (_usedId != Guid.Empty)
            {
                return null;
            }

            if (OS.NativeAlert == null)
            {
                throw new PlatformNotSupportedException("Native alerts are not supported on this platform.");
            }

            ObjectRef<Guid> idRef = new();
            Task<INativeAlert.Button?> alertTask = OS.NativeAlert.ShowAlertAsync(Title, Message, Icon, buttons, idRef);
            _usedId = idRef.Value;

            return await alertTask;
        }

        /// <summary>
        /// Opens the alert with the specified buttons and waits for user action, returning the index of the pressed
        /// button or null if the alert was closed, either by the user or by <see cref="Close"/>. This is just a
        /// convenient wrapper for
        /// <see cref="INativeAlert.ShowAlertAsync(string,string,string,string?,string?,INativeAlert.Icon,CatUI.Utils.ObjectRef{System.Guid}?,IntPtr)"/>,
        /// so also check that function for more info.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Some platforms have limitations regarding the number of buttons for a certain <see cref="Icon"/>; in
        /// that case, a different <see cref="Icon"/> will be presented so the requested buttons can appear normally.
        /// </para>
        /// <para>
        /// If the alert is already open, and you call this again while it is open, it will immediately return null.
        /// </para>
        /// </remarks>
        /// <param name="button1">The custom string for the first button.</param>
        /// <param name="button2">
        /// The custom string for the second button. Null means there will be no second button.
        /// </param>
        /// <param name="button3">
        /// The custom string for the third button. Null means there will be no third button. Having this non-null
        /// and the second button null will throw an <see cref="ArgumentException"/>.
        /// </param>
        /// <returns>
        /// The pressed button index (0-based) or null if the user dismissed the alert (in can happen on some platforms)
        /// or it was closed by <see cref="Close"/>.
        /// </returns>
        /// <seealso cref="INativeAlert.ShowAlertAsync(string,string,string,string?,string?,INativeAlert.Icon,CatUI.Utils.ObjectRef{System.Guid}?,IntPtr)"/>
        public async Task<int?> OpenAsync(string button1, string? button2 = null, string? button3 = null)
        {
            if (_usedId != Guid.Empty)
            {
                return null;
            }

            if (OS.NativeAlert == null)
            {
                throw new PlatformNotSupportedException("Native alerts are not supported on this platform.");
            }

            ObjectRef<Guid> idRef = new();
            Task<int?> alertTask =
                OS.NativeAlert.ShowAlertAsync(
                    Title,
                    Message,
                    button1,
                    button2,
                    button3,
                    Icon,
                    idRef);
            _usedId = idRef.Value;

            return await alertTask;
        }

        /// <summary>
        /// Closes an already open alert (it will return null on the Open functions) if one exists.
        /// </summary>
        /// <returns>True if the alert could be closed, false otherwise.</returns>
        public bool Close()
        {
            return OS.NativeAlert != null && OS.NativeAlert.CloseAlert(_usedId);
        }
    }
}
