using System;
using System.Runtime.InteropServices;
using CatUI.Platform.CommonInterface;

//WARNING: Completely untested code! Taken from the internet and AI (= low chances for it to work properly)

// ReSharper disable InconsistentNaming
namespace CatUI.Platform.MacOS.OS
{
    public partial class SoftwareRendererMacOS : ISoftwareRenderer
    {
        #region Library imports
        // ----- Objective-C runtime -----
        [LibraryImport("/usr/lib/libobjc.A.dylib", StringMarshalling = StringMarshalling.Utf8)] 
        private static partial IntPtr objc_getClass(string name);
        [LibraryImport("/usr/lib/libobjc.A.dylib", StringMarshalling = StringMarshalling.Utf8)] 
        private static partial IntPtr sel_registerName(string name);
        [LibraryImport("/usr/lib/libobjc.A.dylib")] 
        private static partial IntPtr objc_msgSend(IntPtr recv, IntPtr sel);
        [LibraryImport("/usr/lib/libobjc.A.dylib")]
        private static partial void objc_msgSend_void(IntPtr recv, IntPtr sel);

        // ----- CoreGraphics -----
        [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        private static partial IntPtr CGColorSpaceCreateDeviceRGB();
    
        [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        private static partial void CGColorSpaceRelease(IntPtr cs);
    
        [Flags]
        enum CGBitmapInfo : uint
        {
            kCGImageAlphaNoneSkipFirst       = 1,
            kCGImageAlphaPremultipliedLast   = 1 << 1,
            kCGImageAlphaPremultipliedFirst  = 1 << 2,
            kCGBitmapByteOrderDefault        = 0,
            kCGBitmapByteOrder16Little       = 2 << 12,
            kCGBitmapByteOrder32Little       = 3 << 12,
            kCGBitmapByteOrder16Big          = 4 << 12,
            kCGBitmapByteOrder32Big          = 5 << 12,
        }
    
        [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        private static partial IntPtr CGDataProviderCreateWithData(IntPtr info, IntPtr data, nint size, IntPtr releaseCallback);
    
        [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        private static partial void CGDataProviderRelease(IntPtr provider);
    
        [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        private static partial IntPtr CGImageCreate(
            nint width, 
            nint height,
            nint bitsPerComponent, 
            nint bitsPerPixel, 
            nint bytesPerRow,
            IntPtr colorSpace, 
            CGBitmapInfo bitmapInfo,
            IntPtr provider, 
            IntPtr decode /* null */,
            [MarshalAs(UnmanagedType.Bool)]
            bool shouldInterpolate, 
            int renderingIntent /* kCGRenderingIntentDefault = 0 */);
    
        [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        private static partial void CGImageRelease(IntPtr image);
    
        [StructLayout(LayoutKind.Sequential)]
        private struct CGRect { public double x, y, width, height; }
    
        [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        private static partial void CGContextDrawImage(IntPtr ctx, CGRect rect, IntPtr image);
    
        #endregion
        
        
        #region NSGraphicsContext bridge
        // [NSGraphicsContext currentContext] -> NSGraphicsContext*
        private static IntPtr NSGraphicsContext_CurrentContext()
        {
            IntPtr cls = objc_getClass("NSGraphicsContext");
            IntPtr sel = sel_registerName("currentContext");
            return objc_msgSend(cls, sel);
        }

        // -[NSGraphicsContext CGContext] -> CGContextRef
        private static IntPtr NSGraphicsContext_GetCGContext(IntPtr nsGraphicsContext)
        {
            IntPtr sel = sel_registerName("CGContext");
            return objc_msgSend(nsGraphicsContext, sel);
        }

        // contentView = [window contentView]
        private static IntPtr GetContentView(IntPtr nsWindow)
        {
            IntPtr sel = sel_registerName("contentView");
            return objc_msgSend(nsWindow, sel);
        }

        private static void ViewLockFocus(IntPtr nsView)
        {
            IntPtr sel = sel_registerName("lockFocus");
            objc_msgSend_void(nsView, sel);
        }

        static void ViewUnlockFocus(IntPtr nsView)
        {
            IntPtr sel = sel_registerName("unlockFocus");
            objc_msgSend_void(nsView, sel);
        }
        
        #endregion
        public void Draw(
            nint nativeWindow, 
            nint pixelBuffer, 
            int framebufferWidth,
            int framebufferHeight,
            int windowWidth,
            int windowHeight)
        {
            int rowBytes = framebufferWidth + 4;
            if (nativeWindow == IntPtr.Zero || pixelBuffer == IntPtr.Zero)
            {
                return;
            }

            // 1) Wrap pixels into a CGImage (no copy)
            IntPtr cs = CGColorSpaceCreateDeviceRGB();
    
            // Data provider does NOT own the memory (no release callback)
            IntPtr provider = CGDataProviderCreateWithData(IntPtr.Zero, pixelBuffer,
                (nint)(rowBytes * framebufferHeight), IntPtr.Zero);
    
            // Match Skia: RGBA8888 premultiplied -> little-endian, premul last
            const int BitsPerComponent = 8;
            const int BitsPerPixel = 32;
    
            const CGBitmapInfo bitmapInfo = 
                CGBitmapInfo.kCGBitmapByteOrder32Little 
              | CGBitmapInfo.kCGImageAlphaPremultipliedLast;
    
            IntPtr cgImage = CGImageCreate(
                framebufferWidth, 
                framebufferHeight,
                BitsPerComponent, 
                BitsPerPixel, 
                rowBytes,
                cs, 
                bitmapInfo,
                provider, 
                IntPtr.Zero, 
                true, 
                0);
    
            // 2) Get CGContext from the window's contentView and draw
            IntPtr view = GetContentView(nativeWindow);
            if (view != IntPtr.Zero && cgImage != IntPtr.Zero)
            {
                // Ensure there is a graphics context; this pairs with unlockFocus
                ViewLockFocus(view);
                try
                {
                    IntPtr nsGc = NSGraphicsContext_CurrentContext();
                    IntPtr cg = NSGraphicsContext_GetCGContext(nsGc);
    
                    // Draw to a rect in *points* (CoreGraphics auto-scales the image)
                    var rect = new CGRect { x = 0, y = 0, width = windowWidth, height = windowHeight };
    
                    // CoreGraphics' default origin is bottom-left in a flipped coord space.
                    // Most NSViews are flipped top-left; if your view isn't flipped you may
                    // want to handle y-flip or use an NSView subclass. For a simple blit,
                    // this usually "just works" because NSGraphicsContext handles the flip.
                    CGContextDrawImage(cg, rect, cgImage);
                }
                finally
                {
                    ViewUnlockFocus(view);
                }
            }
    
            // 3) Cleanup temporary CG objects (we didn't own pixel memory)
            if (cgImage != IntPtr.Zero)
            {
                CGImageRelease(cgImage);
            }
    
            if (provider != IntPtr.Zero)
            {
                CGDataProviderRelease(provider);
            }
    
            if (cs != IntPtr.Zero)
            {
                CGColorSpaceRelease(cs);
            }
        }

        public void Resized(int newWidth, int newHeight)
        {
            //no-op
        }
    }
}
