using CatUI.PlatformExtension;

namespace CatUI.Windowing.Desktop
{
    public class DesktopDispatcher : DispatcherBase
    {
        internal void CallActions()
        {
            CallOnUIThread();
        }
    }
}
