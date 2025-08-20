using System;
using SkiaSharp;

namespace CatUI.Platform.Essentials
{
    public interface IWindowIcon
    {
        SKImage? GetWindowIcon(IntPtr windowHandle);
    }
}
