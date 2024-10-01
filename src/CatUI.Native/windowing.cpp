#include "windowing.h"

constexpr int WINDOW_MODE_WINDOWED = 0;
constexpr int WINDOW_MODE_MINIMIZED = 0b1;
constexpr int WINDOW_MODE_MAXIMIZED = 0b10;
constexpr int WINDOW_MODE_FULLSCREEN = 0b11;
constexpr int WINDOW_MODE_EXCLUSIVE_FULLSCREEN = 0b100;

//true by default
constexpr int WINDOW_HINT_RESIZABLE = 8;
constexpr int WINDOW_HINT_VISIBLE = 16;
constexpr int WINDOW_HINT_DECORATED = 32;
constexpr int WINDOW_HINT_DPI_AWARE = 64;
constexpr int WINDOW_HINT_FOCUSED = 128;

//false by default
constexpr int WINDOW_HINT_ALWAYS_ON_TOP = 256;
constexpr int WINDOW_TRANSPARENT_FRAMEBUFFER = 512;

GLFWerrorfun setGlfwErrorCallback(GLFWerrorfun callback) {
    return glfwSetErrorCallback(callback);
}

int initializeGlfw() {
    return glfwInit();
}

void terminateGlfw() {
    glfwTerminate();
}

#pragma region Window creation and destruction
GLFWwindowclosefun setGlfwCloseRequestCallback(GLFWwindow* window, GLFWwindowclosefun callback) {
    return glfwSetWindowCloseCallback(window, callback);
}

GLFWwindow* createWindow(int width, int height, const char* title, int windowFlags)
{
    GLFWwindow* window = nullptr;
    GLFWmonitor* monitor = glfwGetPrimaryMonitor();

    //we will be using EGL for context
    glfwWindowHint(GLFW_CLIENT_API, GLFW_NO_API);
    glfwWindowHint(GLFW_STEREO, GLFW_FALSE);

    glfwWindowHint(GLFW_COCOA_GRAPHICS_SWITCHING, GLFW_TRUE);
    glfwWindowHint(GLFW_COCOA_RETINA_FRAMEBUFFER, GLFW_TRUE);

    glfwWindowHint(GLFW_FOCUS_ON_SHOW, GLFW_TRUE);

    glfwWindowHint(GLFW_RESIZABLE, (windowFlags & WINDOW_HINT_RESIZABLE) != 0 ? GLFW_TRUE : GLFW_FALSE);
    glfwWindowHint(GLFW_VISIBLE, (windowFlags & WINDOW_HINT_VISIBLE) != 0 ? GLFW_TRUE : GLFW_FALSE);
    glfwWindowHint(GLFW_DECORATED, (windowFlags & WINDOW_HINT_DECORATED) != 0 ? GLFW_TRUE : GLFW_FALSE);
    glfwWindowHint(GLFW_SCALE_TO_MONITOR, (windowFlags & WINDOW_HINT_DPI_AWARE) != 0 ? GLFW_TRUE : GLFW_FALSE);
    glfwWindowHint(GLFW_FOCUSED, (windowFlags & WINDOW_HINT_FOCUSED) != 0 ? GLFW_TRUE : GLFW_FALSE);

    glfwWindowHint(GLFW_FLOATING, (windowFlags & WINDOW_HINT_ALWAYS_ON_TOP) != 0 ? GLFW_TRUE : GLFW_FALSE);
    glfwWindowHint(GLFW_TRANSPARENT_FRAMEBUFFER, (windowFlags & WINDOW_TRANSPARENT_FRAMEBUFFER) != 0 ? GLFW_TRUE : GLFW_FALSE);

    unsigned char windowMode = windowFlags & 0b111;
    switch (windowMode)
    {
    case WINDOW_MODE_WINDOWED:
    {
        window = glfwCreateWindow(width, height, title, nullptr, nullptr);
        break;
    }
    case WINDOW_MODE_MINIMIZED:
    {
        window = glfwCreateWindow(width, height, title, nullptr, nullptr);
        minimizeWindow(window);
        break;
    }
    case WINDOW_MODE_MAXIMIZED:
    {
        glfwWindowHint(GLFW_MAXIMIZED, GLFW_TRUE);
        window = glfwCreateWindow(width, height, title, nullptr, nullptr);
        break;
    }
    case WINDOW_MODE_FULLSCREEN:
    {
        const GLFWvidmode* mode = glfwGetVideoMode(monitor);
        glfwWindowHint(GLFW_RED_BITS, mode->redBits);
        glfwWindowHint(GLFW_GREEN_BITS, mode->greenBits);
        glfwWindowHint(GLFW_BLUE_BITS, mode->blueBits);
        glfwWindowHint(GLFW_REFRESH_RATE, mode->refreshRate);

        window = glfwCreateWindow(width, height, title, monitor, nullptr);
        break;
    }
    case WINDOW_MODE_EXCLUSIVE_FULLSCREEN:
    {
        window = glfwCreateWindow(width, height, title, glfwGetPrimaryMonitor(), nullptr);
        break;
    }
    default:
        window = glfwCreateWindow(width, height, title, nullptr, nullptr);
        break;
    }
    return window;
}

void destroyWindow(GLFWwindow* window) {
    glfwDestroyWindow(window);
}

void* getNativeWindowHandle(GLFWwindow* window) {
    int currentPlatform = glfwGetPlatform();
    switch (currentPlatform)
    {
    case GLFW_PLATFORM_WIN32:
        return glfwGetWin32Window(window);
    case GLFW_PLATFORM_X11:
        //return glfwGetX11Window(window);
    case GLFW_PLATFORM_WAYLAND:
        //return glfwGetWaylandWindow(window);
    case GLFW_PLATFORM_COCOA:
        //return glfwGetCocoaWindow(window);
    default:
        break;
    }
    return nullptr;
}
#pragma endregion


#pragma region Window sizing and positioning
GLFWwindowsizefun setGlfwResizeCallback(GLFWwindow* window, GLFWwindowsizefun callback) {
    return glfwSetWindowSizeCallback(window, callback);
}

GLFWwindowposfun setGlfwWindowMovedCallback(GLFWwindow* window, GLFWwindowposfun callback) {
    return glfwSetWindowPosCallback(window, callback);
}

void resizeWindow(GLFWwindow* window, int width, int height) {
    glfwSetWindowSize(window, width, height);
}

void getWindowSize(GLFWwindow* window, int* width, int* height) {
    glfwGetWindowSize(window, width, height);
}

void getWindowDecorationSize(GLFWwindow* window, int* left, int* top, int* right, int* bottom) {
    glfwGetWindowFrameSize(window, left, top, right, bottom);
}

void setWindowSizeLimits(GLFWwindow* window, int minWidth, int minHeight, int maxWidth, int maxHeight) {
    glfwSetWindowSizeLimits(window, minWidth, minHeight, maxWidth, maxHeight);
}

void setWindowAspectRatio(GLFWwindow* window, int numerator, int denominator) {
    glfwSetWindowAspectRatio(window, numerator, denominator);
}

void getWindowPosition(GLFWwindow* window, int* xPosition, int* yPosition) {
    glfwGetWindowPos(window, xPosition, yPosition);
}

void setWindowPosition(GLFWwindow* window, int xPosition, int yPosition) {
    glfwSetWindowPos(window, xPosition, yPosition);
}
#pragma endregion


#pragma region Window framebuffer and scaling
GLFWframebuffersizefun setFramebufferResizeCallback(GLFWwindow* window, GLFWframebuffersizefun callback) {
    return glfwSetFramebufferSizeCallback(window, callback);
}

GLFWwindowcontentscalefun setWindowContentScaleChangedCallback(GLFWwindow* window, GLFWwindowcontentscalefun callback) {
    return glfwSetWindowContentScaleCallback(window, callback);
}

//generally called only when the window is resized, so it doesn't offer much as there already is a callback for that
GLFWwindowrefreshfun setWindowNeedsRefreshCallback(GLFWwindow* window, GLFWwindowrefreshfun callback) {
    return glfwSetWindowRefreshCallback(window, callback);
}

void getFramebufferSize(GLFWwindow* window, int* width, int* height) {
    glfwGetFramebufferSize(window, width, height);
}

void getWindowContentScale(GLFWwindow* window, float* xScale, float* yScale) {
    glfwGetWindowContentScale(window, xScale, yScale);
}

int checkWindowFramebufferTransparency(GLFWwindow* window) {
    return glfwGetWindowAttrib(window, GLFW_TRANSPARENT_FRAMEBUFFER);
}

//returns 1 if the window opacity is respected on the current platform, 0 otherwise
int setWindowOpacity(GLFWwindow* window, float opacity) {
    glfwSetWindowOpacity(window, opacity);

    if (opacity != 1 && glfwGetWindowOpacity(window) == 1) {
        return 0;
    }
    else {
        return 1;
    }
}
#pragma endregion


#pragma region Window focus
GLFWwindowfocusfun setWindowFocusCallback(GLFWwindow* window, GLFWwindowfocusfun callback) {
    return glfwSetWindowFocusCallback(window, callback);
}

void focusWindowForced(GLFWwindow* window) {
    glfwFocusWindow(window);
}

void requestWindowAttention(GLFWwindow* window) {
    glfwRequestWindowAttention(window);
}
#pragma endregion

#pragma region Window presentation and monitor handling
GLFWwindowmaximizefun setWindowMaximizationCallback(GLFWwindow* window, GLFWwindowmaximizefun callback) {
    return glfwSetWindowMaximizeCallback(window, callback);
}

void setWindowTitle(GLFWwindow* window, const char* title) {
    glfwSetWindowTitle(window, title);
}

void setWindowIcon(GLFWwindow* window, int arraySize, const GLFWimage* images) {
    if (glfwGetPlatform() == GLFW_PLATFORM_COCOA || glfwGetPlatform() == GLFW_PLATFORM_WAYLAND) {
        return;
    }

    glfwSetWindowIcon(window, arraySize, images);
}

void resetWindowIconToDefault(GLFWwindow* window) {
    if (glfwGetPlatform() == GLFW_PLATFORM_COCOA || glfwGetPlatform() == GLFW_PLATFORM_WAYLAND) {
        return;
    }

    glfwSetWindowIcon(window, 0, nullptr);
}

GLFWmonitor* getFullscreenWindowMonitor(GLFWwindow* window) {
    return glfwGetWindowMonitor(window);
}

void setWindowMonitor(GLFWwindow* window, GLFWmonitor* monitor) {
    const GLFWvidmode* mode = glfwGetVideoMode(monitor);
    glfwSetWindowMonitor(window, monitor, 0, 0, mode->width, mode->height, mode->refreshRate);
}

void windowEnterFullscreen(GLFWwindow* window) {
    setWindowMonitor(window, glfwGetPrimaryMonitor());
}

void windowExitFullscreen(GLFWwindow* window, int xPosition, int yPosition, int width, int height) {
    glfwSetWindowMonitor(window, nullptr, xPosition, yPosition, width, height, 0);
}

void minimizeWindow(GLFWwindow* window) {
    glfwIconifyWindow(window);
}

void maximizeWindow(GLFWwindow* window) {
    glfwMaximizeWindow(window);
}

void restoreWindowFromMinimizationOrMaximization(GLFWwindow* window) {
    glfwRestoreWindow(window);
}

void showWindow(GLFWwindow* window) {
    glfwShowWindow(window);
}

void hideWindow(GLFWwindow* window) {
    glfwHideWindow(window);
}
#pragma endregion

#pragma region Window lifecycle
int receivedCloseRequest(GLFWwindow* window) {
    return glfwWindowShouldClose(window);
}

void pollEvents() {
    glfwPollEvents();
}

void waitForEvents() {
    glfwWaitEvents();
}
#pragma endregion
