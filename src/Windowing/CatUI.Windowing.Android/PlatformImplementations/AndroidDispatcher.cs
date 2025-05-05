using CatUI.Platform.Essentials;

namespace CatUI.Windowing.Android.PlatformImplementations
{
    public class AndroidDispatcher : DispatcherBase
    {
        internal void CallActions()
        {
            CallOnUIThread();
        }
    }
}
