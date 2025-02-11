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
}
