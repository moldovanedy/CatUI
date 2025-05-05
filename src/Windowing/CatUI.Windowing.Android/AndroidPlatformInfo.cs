using CatUI.Data;
using CatUI.Windowing.Android.PlatformImplementations;
using CatUI.Windowing.Common;

namespace CatUI.Windowing.Android
{
    public class AndroidPlatformInfo : PlatformInfo
    {
        public override CatApplicationInitializer AppInitializer => new(
            new AndroidDispatcher(),
            new AndroidPlatformUiOptions(),
            () => { });
    }
}
