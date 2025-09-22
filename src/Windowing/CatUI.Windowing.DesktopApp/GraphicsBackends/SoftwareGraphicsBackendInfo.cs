using CatUI.Data.Enums;
using CatUI.Windowing.Common;

namespace CatUI.Windowing.DesktopApp.GraphicsBackends
{
    public class SoftwareGraphicsBackendInfo : IGraphicsBackendInfo
    {
        public GraphicsApi GetUsedGraphicsApi()
        {
            return GraphicsApi.Software;
        }

        /// <summary>
        /// Will always return "1.0".
        /// </summary>
        /// <returns></returns>
        public string GetGraphicsApiVersion()
        {
            return "1.0";
        }
    }
}
