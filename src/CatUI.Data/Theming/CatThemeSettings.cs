using System.ComponentModel;
using System.Runtime.CompilerServices;
using CatUI.Data.Enums;

namespace CatUI.Data.Theming
{
    public class CatThemeSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// If dark mode is enabled or not (as an option). By default, this respects the platform options
        /// (prefersPlatformOption is true), and the fallbackValue is false.
        /// </summary>
        public CatPlatformDependentSetting<bool> IsDarkModeEnabled
        {
            get => _isDarkModeEnabled;
            set
            {
                _isDarkModeEnabled = value;
                NotifyPropertyChanged();
            }
        }

        private CatPlatformDependentSetting<bool> _isDarkModeEnabled = new(true, false);

        /// <summary>
        /// How to use the color contrast. By default, this respects the platform options (prefersPlatformOption is true),
        /// and the fallbackValue is <see cref="ColorContrastMode.Standard"/>.
        /// </summary>
        public CatPlatformDependentSetting<ColorContrastMode> Contrast
        {
            get => _contrast;
            set
            {
                _contrast = value;
                NotifyPropertyChanged();
            }
        }

        private CatPlatformDependentSetting<ColorContrastMode> _contrast = new(true, ColorContrastMode.Standard);

        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
