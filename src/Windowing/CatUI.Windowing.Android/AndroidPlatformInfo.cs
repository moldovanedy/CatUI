using CatUI.Data;
using CatUI.Windowing.Android.PlatformImplementations;

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
