namespace CatUI.Platform.CommonInterface;

public interface ISoftwareRenderer
{
    void Draw(
        nint nativeWindow, 
        nint pixelBuffer, 
        int framebufferWidth,
        int framebufferHeight,
        int bytesPerRow,
        int windowWidth,
        int windowHeight);
        
    void Resized(int newWidth, int newHeight);
}
