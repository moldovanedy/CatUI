namespace CatUI.Data.Enums
{
    public enum GraphicsApi
    {
        /// <summary>
        /// Native OpenGL core profile (version 3.2+) (desktop only).
        /// </summary>
        OpenGlCore = 0,

        /// <summary>
        /// Native OpenGL compatibility profile (version 2.1-3.2) (desktop only).
        /// </summary>
        OpenGlCompatibility = 1,

        /// <summary>
        /// Native Vulkan backend (not implemented yet) (Windows, Linux, and Android only).
        /// </summary>
        Vulkan = 2,

        /// <summary>
        /// Native Metal backend (not implemented yet) (macOS/iOS only).
        /// </summary>
        Metal = 3,

        /// <summary>
        /// Native OpenGL ES 3.0 backend (not implemented yet) (Android only).
        /// </summary>
        OpenGlEs = 4,

        /// <summary>
        /// OpenGL core through ANGLE (desktop only).
        /// </summary>
        OpenGlCoreAngle = 0x80 | 0,

        /// <summary>
        /// Vulkan through ANGLE (Windows, Linux, and Android only).
        /// </summary>
        VulkanAngle = 0x80 | 2,

        /// <summary>
        /// Metal through ANGLE (macOS/iOS only).
        /// </summary>
        VulkanMetal = 0x80 | 3,

        /// <summary>
        /// DirectX 9 through ANGLE (Windows only).
        /// </summary>
        Dx9Angle = 0x80 | 5,

        /// <summary>
        /// DirectX 11 through ANGLE (Windows only).
        /// </summary>
        Dx11Angle = 0x80 | 6,

        /// <summary>
        /// Software rendering, no GPU acceleration. Should work on any platform as long as an implementation is
        /// available on that platform (Windows and Linux at the moment), but is very slow for complex UIs.
        /// </summary>
        Software = 0xff
    }
}
