using CatUI.Windowing.Common;

namespace CatUI.Windowing.DesktopApp.GraphicsBackends
{
    public class AngleGraphicsBackendInfo : IGraphicsBackendInfo
    {
        public IGraphicsBackendInfo.GraphicsApi GetUsedGraphicsApi()
        {
            return IGraphicsBackendInfo.GraphicsApi.OpenGlCoreAngle;
        }

        public string GetGraphicsApiVersion()
        {
            return "";
        }
    }
}
