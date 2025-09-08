using System.Collections.Generic;
using System.Runtime.InteropServices;
using CatUI.Data;
using CatUI.Platform.CommonInterface;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace CatUI.Platform.DesktopCommon.OS
{
    public class CursorProviderDesktop : ICursorProvider
    {
        private readonly Dictionary<int, Image> _customCursors = new();
        private bool _canDrawFakeCursor;

        public void SetBuiltInCursorFakeMode(ICursorProvider.FakeCursorMode fakeCursorMode)
        {
            _canDrawFakeCursor = fakeCursorMode == ICursorProvider.FakeCursorMode.DrawFakeCursor;
        }


        public unsafe CursorIcon GetDefaultCursorIcon(Size size, Point2D hotspot)
        {
            return new CursorIcon(
                0,
                size,
                hotspot,
                false,
                (nint)GLFW.CreateStandardCursor(CursorShape.Arrow));
        }

        public unsafe CursorIcon? CreateCursor(
            int id, Size size, Point2D hotspot, bool isBuiltIn, byte[]? pixelData = null)
        {
            switch (id)
            {
                case >= 256:
                    {
                        if (pixelData == null)
                        {
                            return null;
                        }

                        byte* pixelsRaw = (byte*)Marshal.AllocHGlobal(pixelData.Length);
                        for (int i = 0; i < pixelData.Length; i++)
                        {
                            pixelsRaw[i] = pixelData[i];
                        }

                        Image glfwCursorImage = new()
                        {
                            Width = (int)size.Width, Height = (int)size.Height, Pixels = pixelsRaw
                        };
                        _customCursors.TryAdd(id, glfwCursorImage);

                        Cursor* glfwCursor = GLFW.CreateCursor(glfwCursorImage, (int)hotspot.X, (int)hotspot.Y);
                        return glfwCursor == null ? null : new CursorIcon(id, size, hotspot, false, (nint)glfwCursor);
                    }
                case < CursorIcon.BUILT_IN_CURSOR_LENGTH:
                    {
                        CursorShape? glfwCursorShape = GetGlfwCursorIdFromCatId(id);
                        if (glfwCursorShape == null)
                        {
                            //TODO: draw a custom cursor if _canDrawFakeCursor is true
                            glfwCursorShape = CursorShape.Arrow;
                        }

                        Cursor* glfwCursor = GLFW.CreateStandardCursor(glfwCursorShape.Value);
                        return glfwCursor == null
                            ? null
                            : new CursorIcon(id, new Size(), Point2D.Zero, false, (nint)glfwCursor);
                    }
                default:
                    return null;
            }
        }

        public unsafe void DestroyCursor(CursorIcon cursorIcon)
        {
            if (cursorIcon.InternalPointerData is not nint glfwCursorPtr)
            {
                return;
            }

            Cursor* glfwCursor = (Cursor*)glfwCursorPtr;
            if (_customCursors.TryGetValue(cursorIcon.Id, out Image glfwCursorImage))
            {
                Marshal.FreeHGlobal((nint)glfwCursorImage.Pixels);
                _customCursors.Remove(cursorIcon.Id);
            }

            GLFW.DestroyCursor(glfwCursor);
        }

        public unsafe bool SetCursorAsActive(object? windowIdentifier, CursorIcon cursorIcon)
        {
            if (windowIdentifier is not nint glfwWindowPtr || cursorIcon.InternalPointerData is not nint glfwCursorPtr)
            {
                return false;
            }

            GLFW.SetCursor((Window*)glfwWindowPtr, (Cursor*)glfwCursorPtr);
            return true;
        }

        public unsafe void SetCursorMode(object? windowIdentifier, ICursorProvider.CursorMode cursorMode)
        {
            if (windowIdentifier is not nint glfwWindowPtr)
            {
                return;
            }

            CursorModeValue mode;
            switch (cursorMode)
            {
                default:
                case ICursorProvider.CursorMode.Visible:
                    mode = CursorModeValue.CursorNormal;
                    break;
                case ICursorProvider.CursorMode.Hidden:
                    mode = CursorModeValue.CursorHidden;
                    break;
                case ICursorProvider.CursorMode.Locked:
                    mode = CursorModeValue.CursorDisabled;
                    break;
            }

            GLFW.SetInputMode((Window*)glfwWindowPtr, CursorStateAttribute.Cursor, mode);
        }

        private static CursorShape? GetGlfwCursorIdFromCatId(int id)
        {
            switch (id)
            {
                case CursorIcon.CURSOR_ARROW:
                    return CursorShape.Arrow;
                case CursorIcon.CURSOR_TEXT:
                    return CursorShape.IBeam;
                case CursorIcon.CURSOR_CROSSHAIR:
                    return CursorShape.Crosshair;
                case CursorIcon.CURSOR_POINTING_HAND:
                    return CursorShape.PointingHand;
                case CursorIcon.CURSOR_EW_RESIZE:
                    return CursorShape.ResizeEW;
                case CursorIcon.CURSOR_NS_RESIZE:
                    return CursorShape.ResizeNS;
                case CursorIcon.CURSOR_NWSE_RESIZE:
                    return CursorShape.ResizeNWSE;
                case CursorIcon.CURSOR_NESW_RESIZE:
                    return CursorShape.ResizeNESW;
                case CursorIcon.CURSOR_ALL_RESIZE:
                    return CursorShape.ResizeAll;
                case CursorIcon.CURSOR_NOT_ALLOWED:
                    return CursorShape.NotAllowed;
                default:
                    return null;
            }
        }
    }
}
