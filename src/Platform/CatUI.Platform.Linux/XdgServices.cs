using System;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Desktop.DBus;
using Tmds.DBus.Protocol;

namespace CatUI.Platform.Linux;

[SupportedOSPlatform("linux")]
internal static class XdgServices
{
    internal static ServiceStatus Status { get; private set; } = ServiceStatus.Uninitialized;

    private static bool _isInitialized;
    private const string XDG_PORTAL_PATH = "/org/freedesktop/portal/desktop";

    internal static async Task Init()
    {
        if (_isInitialized)
        {
            return;
        }

        try
        {
            string? sessionBusAddress = Address.Session;
            if (sessionBusAddress is null)
            {
                Status = ServiceStatus.Unavailable;
                return;
            }

            var conn = new Connection(Address.Session!);
            await conn.ConnectAsync();

            DesktopService xdgService = new(conn, "org.freedesktop.portal.Desktop");
            await SetupSettingsService(xdgService);
            SetupFilePickerService(xdgService);

            Status = ServiceStatus.Operational;

            OnDarkModePreferenceChanged?.Invoke(await GetIsDarkModeEnabledAsync());
            OnHighContrastPreferenceChanged?.Invoke(await GetIsHighContrastEnabledAsync());

            _isInitialized = true;
        }
        catch
        {
            Status = ServiceStatus.Unavailable;
        }
    }

    #region Settings

    internal static Settings? SettingsService { get; private set; }

    internal static event Action<bool?>? OnDarkModePreferenceChanged;
    internal static event Action<bool?>? OnHighContrastPreferenceChanged;

    internal static async Task<bool?> GetIsDarkModeEnabledAsync()
    {
        if (SettingsService == null)
        {
            return null;
        }

        try
        {
            VariantValue rawValue = await SettingsService.ReadOneAsync(
                "org.freedesktop.appearance", "color-scheme");
            //1 is dark, 2 is light, anything else is "unspecified", so treated as light
            return rawValue.GetUInt32() == 1;
        }
        catch
        {
            return null;
        }
    }

    internal static async Task<bool?> GetIsHighContrastEnabledAsync()
    {
        if (SettingsService == null)
        {
            return null;
        }

        try
        {
            VariantValue rawValue = await SettingsService.ReadOneAsync(
                "org.freedesktop.appearance", "contrast");
            //1 is high contrast, 0 is normal, anything else is "unspecified", so treated as normal
            return rawValue.GetUInt32() == 1;
        }
        catch
        {
            return null;
        }
    }

    internal static async Task SetupSettingsService(DesktopService xdgService)
    {
        SettingsService = xdgService.CreateSettings(XDG_PORTAL_PATH);

        await SettingsService.WatchSettingChangedAsync((exception, data) =>
        {
            if (
                exception != null ||
                data.Namespace != "org.freedesktop.appearance" ||
                data.Value.ValueType != VariantValueType.UInt32)
            {
                return;
            }

            switch (data.Key)
            {
                case "color-scheme":
                    OnDarkModePreferenceChanged?.Invoke(data.Value.GetUInt32() == 1);
                    break;
                case "contrast":
                    OnHighContrastPreferenceChanged?.Invoke(data.Value.GetUInt32() == 1);
                    break;
            }
        });
    }

    #endregion

    #region File picker

    internal static FileChooser? FilePickerService { get; private set; }

    internal static void SetupFilePickerService(DesktopService xdgService)
    {
        FilePickerService = xdgService.CreateFileChooser(XDG_PORTAL_PATH);
    }

    #endregion


    public enum ServiceStatus
    {
        Uninitialized = 0,
        Operational = 1,
        Unavailable = 2
    }
}
