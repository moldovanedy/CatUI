using System;
using System.Runtime.Versioning;
using CatUI.Data.Enums;
using CatUI.Windowing.Common;
using OpenTK.Graphics.OpenGL;

namespace CatUI.Windowing.DesktopApp.GraphicsBackends;

[SupportedOSPlatform("windows")]
[SupportedOSPlatform("macos")]
[SupportedOSPlatform("linux")]
public class OpenGlGraphicsBackendInfo : IGraphicsBackendInfo
{
    private int? _majorVersion;
    private int? _minorVersion;

    public GraphicsApi GetUsedGraphicsApi()
    {
        if (_majorVersion == null || _minorVersion == null)
        {
            string versionString = GL.GetString(StringName.Version);
            if (int.TryParse(versionString.AsSpan(0, 1), out int majVer))
            {
                _majorVersion = majVer;
            }

            if (int.TryParse(versionString.AsSpan(2, 1), out int minVer))
            {
                _minorVersion = minVer;
            }
        }

        return _majorVersion >= 3 && _minorVersion >= 2
            ? GraphicsApi.OpenGlCore
            : GraphicsApi.OpenGlCompatibility;
    }

    /// <summary>
    /// Will return the current OpenGL version as major_version.minor_version
    /// </summary>
    /// <returns></returns>
    public string GetGraphicsApiVersion()
    {
        return GL.GetString(StringName.Version).Split(' ')[0];
    }
}
