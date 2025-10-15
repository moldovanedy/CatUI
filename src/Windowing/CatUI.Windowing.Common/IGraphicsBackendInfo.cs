using CatUI.Data.Enums;

namespace CatUI.Windowing.Common;

public interface IGraphicsBackendInfo
{
    GraphicsApi GetUsedGraphicsApi();
    string GetGraphicsApiVersion();
}
