namespace CatUI.Data
{
    /// <summary>
    /// Represents a setting for internal CatUI components that will be affected by what you set here. This object alone
    /// won't do much, as it only specifies settings for internal components, so those components will provide something
    /// meaningful using this data. Note that this is for values that are generally also set by the runtime platform,
    /// and subsequently, your application should use them. 
    /// </summary>
    /// <typeparam name="T">The type of value used.</typeparam>
    public class CatPlatformDependentSetting<T>
    {
        /// <summary>
        /// If true, it will respect the platform option if possible (using <see cref="FallbackValue"/> if it can't do this).
        /// If false, it will always use <see cref="FallbackValue"/>.
        /// </summary>
        public bool PrefersPlatformOption { get; }

        /// <summary>
        /// The value that will be used when <see cref="PrefersPlatformOption"/> is false OR when it's true, but
        /// the runtime platform doesn't have this option.
        /// </summary>
        /// <remarks>All settings used by CatUI will have this as an immutable value.</remarks>
        public T? FallbackValue { get; }

        public CatPlatformDependentSetting(bool prefersPlatformOption, T? fallbackValue)
        {
            PrefersPlatformOption = prefersPlatformOption;
            FallbackValue = fallbackValue;
        }
    }
}
