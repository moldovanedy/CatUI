using CatUI.Platform.Essentials;

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
