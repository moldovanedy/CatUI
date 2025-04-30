using CatUI.Platform.Essentials;

namespace CatUI.Windowing.Desktop.PlatformImplementations
{
    public class DesktopDispatcher : DispatcherBase
    {
        internal void CallActions()
        {
            CallOnUIThread();
        }
    }
}
