using System;
using SkiaSharp;

namespace CatUI.Platform.CommonInterface
{
    public interface IWindowIcon
    {
        SKImage? GetWindowIcon(IntPtr windowHandle);
    }
}
