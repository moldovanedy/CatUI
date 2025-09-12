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
        private static partial IntPtr CGDataProviderCreateWithData(IntPtr info, IntPtr data, int size, IntPtr releaseCallback);
    
        [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        private static partial void CGDataProviderRelease(IntPtr provider);
    
        [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        private static partial void CGImageRelease(IntPtr image);
    
        [StructLayout(LayoutKind.Sequential)]
        private struct CGRect { public double x, y, width, height; }
    
        [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        private static partial void CGContextDrawImage(IntPtr ctx, CGRect rect, IntPtr image);
    
        
        [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        private static partial IntPtr CGBitmapContextCreate(
            IntPtr data,
            int width, 
            int height,
            int bitsPerComponent,
            int bytesPerRow,
            IntPtr colorspace,
            CGBitmapInfo bitmapInfo);

        [LibraryImport("/System/Library/Frameworks/CoreGraphics.framework/CoreGraphics")]
        private static partial IntPtr CGBitmapContextCreateImage(IntPtr context);
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
            int bytesPerRow,
            int windowWidth,
            int windowHeight)
        {
            if (nativeWindow == IntPtr.Zero || pixelBuffer == IntPtr.Zero)
            {
                return;
            }

            IntPtr cs = CGColorSpaceCreateDeviceRGB();
            IntPtr provider = CGDataProviderCreateWithData(
                IntPtr.Zero, 
                pixelBuffer,
                bytesPerRow * framebufferHeight,
                IntPtr.Zero);
    
            //match Skia: RGBA8888 premultiplied -> big-endian, premul last
            const int BitsPerComponent = 8;
            const CGBitmapInfo bitmapInfo = 
                CGBitmapInfo.kCGBitmapByteOrder32Big
              | CGBitmapInfo.kCGImageAlphaPremultipliedLast;
            
            //TODO: fix this, as this does not work; it returns null every time
            IntPtr ctx = CGBitmapContextCreate(
                pixelBuffer,
                framebufferWidth, 
                framebufferHeight,
                BitsPerComponent, 
                bytesPerRow,
                cs, 
                bitmapInfo);
            Console.WriteLine($"ctx: {ctx != nint.Zero}");
            IntPtr cgImage = CGBitmapContextCreateImage(ctx);
    
            //get CGContext from the window's contentView and draw
            IntPtr view = GetContentView(nativeWindow);
            if (view != IntPtr.Zero && cgImage != IntPtr.Zero)
            {
                ViewLockFocus(view);
                try
                {
                    IntPtr nsGc = NSGraphicsContext_CurrentContext();
                    IntPtr cg = NSGraphicsContext_GetCGContext(nsGc);
    
                    // draw to a rect in *points* (CoreGraphics auto-scales the image)
                    var rect = new CGRect { x = 0, y = 0, width = windowWidth, height = windowHeight };
                    CGContextDrawImage(cg, rect, cgImage);
                }
                finally
                {
                    ViewUnlockFocus(view);
                }
            }
            else
            {
                //ERROR
                Console.WriteLine($"view: {view != nint.Zero}, cgImage: {cgImage != nint.Zero}");
            }
    
            //cleanup
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
