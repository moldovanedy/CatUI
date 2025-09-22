using CatUI.Data.Enums;
using CatUI.Windowing.Common;

namespace CatUI.Windowing.Android
{
    /// <summary>
    /// All operations are no-op because all the graphics are managed by <c>SkiaSharp.Views.Android</c>.
    /// </summary>
    internal class AndroidGraphicsBackendInfo : IGraphicsBackendInfo
    {
        public GraphicsApi GetUsedGraphicsApi()
        {
            return GraphicsApi.OpenGlEs;
        }

        public string GetGraphicsApiVersion()
        {
            return "";
        }
    }
}
