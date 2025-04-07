using System;

namespace CatUI.Platform.Essentials
{
    /// <summary>
    /// The base for all platform-controlled UI options like dark mode, high contrast, animation speed etc.
    /// </summary>
    /// <remarks>Note to inheritors: you only need to set the properties, no need to override anything.</remarks>
    public abstract class PlatformUiOptionsBase
    {
        /// <summary>
        /// Whether dark mode is enabled or not in the system. Supported by most platforms, except Linux, as there is no
        /// standard API for that. Support might still be available on some desktop environments on Linux.
        /// </summary>
        /// <remarks>Null means no support for the runtime platform or no implementation of CatUI exists for the platform.</remarks>
        public bool? IsDarkModeEnabled
        {
            get => _isDarkModeEnabled;
            protected set
            {
                _isDarkModeEnabled = value;
                IsDarkModeEnabledChanged?.Invoke(_isDarkModeEnabled);
            }
        }

        private bool? _isDarkModeEnabled;

        /// <summary>
        /// Fired when the dark mode preference changes. The parameter is the value of <see cref="IsDarkModeEnabled"/>
        /// after being set.
        /// </summary>
        public event Action<bool?>? IsDarkModeEnabledChanged;

        /// <summary>
        /// The color contrast preferred by the user. 0 is standard contrast, 1 is medium contrast (only on Android 14 or
        /// newer (API >= 34)) and 2 is high contrast (most platforms should support this).
        /// </summary>
        /// <remarks>Null means no support for the runtime platform or no implementation of CatUI exists for the platform.</remarks>
        public int? ColorContrast
        {
            get => _colorContrast;
            protected set
            {
                _colorContrast = value;
                ColorContrastChanged?.Invoke(_colorContrast);
            }
        }

        private int? _colorContrast;

        /// <summary>
        /// Fired when the contrast preference changes. The parameter is the value of <see cref="ColorContrast"/>
        /// after being set.
        /// </summary>
        public event Action<int?>? ColorContrastChanged;

        //TODO: animation scale, prefers reduced motion
    }
}
