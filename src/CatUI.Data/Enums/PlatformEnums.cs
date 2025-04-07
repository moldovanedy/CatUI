using System;

namespace CatUI.Data.Enums
{
    public enum RuntimePlatform
    {
        // ReSharper disable InconsistentNaming
        Windows = 0,

        /// <summary>
        /// This is returned for both macOS and MacCatalyst. Use <see cref="OperatingSystem.IsMacCatalyst"/> to
        /// differentiate one from another if needed.
        /// </summary>
        MacOS = 1,
        Linux = 2,
        FreeBSD = 3,
        Android = 4,
        iOS = 5,
        Browser = 6,

        TvOS = 7,
        WatchOS = 8,

        Unknown = 255
        // ReSharper restore InconsistentNaming
    }

    /// <summary>
    /// Describes an option that can be either set at runtime by the runtime platform or set by you to either enabled or
    /// disabled. At runtime, it can have 2 options: enabled or disabled.
    /// </summary>
    public enum PlatformOption
    {
        /// <summary>
        /// This will always be disabled, regardless of platform settings.
        /// </summary>
        Disabled = 0,

        /// <summary>
        /// This will always be enabled, regardless of platform settings.
        /// </summary>
        Enabled = 1,

        /// <summary>
        /// This will respect the platform settings, but if that is not available (because for example that option doesn't
        /// exist on the runtime platform, or it exists, but CatUI currently doesn't have a way to get that option), it
        /// will fall back to "disabled".
        /// </summary>
        PlatformDependentFallbackDisabled = 0b10,

        /// <summary>
        /// This will respect the platform settings, but if that is not available (because for example that option doesn't
        /// exist on the runtime platform, or it exists, but CatUI currently doesn't have a way to get that option), it
        /// will fall back to "enabled".
        /// </summary>
        PlatformDependentFallbackEnabled = 0b11
    }
}
