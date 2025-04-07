using System.ComponentModel;
using System.Runtime.CompilerServices;
using CatUI.Data.Enums;

namespace CatUI.Data.Theming
{
    public class CatThemeSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// If dark mode is enabled or not (as an option). The default value is
        /// <see cref="PlatformOption.PlatformDependentFallbackDisabled"/>, so it tries to respect the platform choice,
        /// but if it can't, then dark mode will be disabled.
        /// </summary>
        public PlatformOption IsDarkModeEnabled
        {
            get => _isDarkModeEnabled;
            set
            {
                _isDarkModeEnabled = value;
                NotifyPropertyChanged();
            }
        }

        private PlatformOption _isDarkModeEnabled = PlatformOption.PlatformDependentFallbackDisabled;

        /// <summary>
        /// How to use the color contrast. By default, this respects the platform options (prefersPlatformOption is true),
        /// and the fallbackValue is <see cref="ColorContrastMode.Standard"/>.
        /// </summary>
        public CatSetting<ColorContrastMode> Contrast
        {
            get => _contrast;
            set
            {
                _contrast = value;
                NotifyPropertyChanged();
            }
        }

        private CatSetting<ColorContrastMode> _contrast = new(true, ColorContrastMode.Standard);

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
