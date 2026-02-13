using CatUI.Platform.Essentials;

namespace CatUI.Windowing.DesktopApp.PlatformImplementations;

public class DesktopDispatcher : DispatcherBase
{
    public void CallActions()
    {
        CallOnUIThread();
    }
}
