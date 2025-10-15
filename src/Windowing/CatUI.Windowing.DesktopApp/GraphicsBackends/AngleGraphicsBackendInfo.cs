using CatUI.Data.Enums;
using CatUI.Windowing.Common;

namespace CatUI.Windowing.DesktopApp.GraphicsBackends;

public class AngleGraphicsBackendInfo : IGraphicsBackendInfo
{
    public GraphicsApi GetUsedGraphicsApi()
    {
        return GraphicsApi.OpenGlCoreAngle;
    }

    public string GetGraphicsApiVersion()
    {
        return "";
    }
}
