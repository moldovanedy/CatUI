using System;

namespace CatUI.Data.Exceptions
{
    /// <summary>
    /// Generic error that indicates any error in the asset loading process. Can be any error, including insufficient
    /// permissions on the load stream/file, unsupported format, etc.
    /// </summary>
    public class AssetLoadException : Exception
    {
        public AssetLoadException(string message) : base(message) { }
        public AssetLoadException(string message, Exception innerException) : base(message, innerException) { }
    }
}
