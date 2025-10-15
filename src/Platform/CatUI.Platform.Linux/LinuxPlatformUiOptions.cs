using CatUI.Platform.Essentials;

namespace CatUI.Platform.Linux;

public class LinuxPlatformUiOptions : PlatformUiOptionsBase
{
    public LinuxPlatformUiOptions()
    {
        XdgServices.OnDarkModePreferenceChanged += value =>
        {
            IsDarkModeEnabled = value;
        };
        XdgServices.OnHighContrastPreferenceChanged += value =>
        {
            ColorContrast = value == null ? null : value.Value ? 2 : 0;
        };
    }
}
