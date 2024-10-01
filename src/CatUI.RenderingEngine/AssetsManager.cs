using System.Collections.Generic;
using System.IO;
using System.Reflection;
using CatUI.Data.Assets;

namespace CatUI.RenderingEngine
{
    public static class AssetsManager
    {
#if NET8_0_OR_GREATER
        private readonly static Dictionary<string, Asset> _cachedAssets = [];
#else
        private readonly static Dictionary<string, Asset> _cachedAssets = new Dictionary<string, Asset>();
#endif

        public static Asset? Load(Assembly mainAssembly, string assetPath, CacheMode cacheMode = CacheMode.Cache)
        {
            if (_cachedAssets.TryGetValue(assetPath, out Asset? asset))
            {
                return asset;
            }

            assetPath = assetPath.Replace('/', '.');
            string asmName = mainAssembly.GetName().ToString();
            asmName = asmName.Split(',')[0];

            Stream? fs =
                mainAssembly
                    .GetManifestResourceStream($"{asmName}{assetPath}");
            if (fs == null)
            {
                return null;
            }

            Image img = new Image();
            img.LoadFromRawData(fs);
            if (cacheMode == CacheMode.Cache)
            {
                _cachedAssets.Add(assetPath, img);
            }

            return img;
        }

        /// <summary>
        /// Specifies how the resources should be cached.
        /// </summary>
        public enum CacheMode
        {
            /// <summary>
            /// Does not cache the resource.
            /// </summary>
            Ignore = 0,
            /// <summary>
            /// Caches the resource.
            /// </summary>
            Cache = 1,
        }
    }
}
