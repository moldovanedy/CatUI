#ifndef WINDOWING_H
#define WINDOWING_H

#include "utils.h"
#include <GLFW/glfw3.h>

//see https://stackoverflow.com/questions/2989810/which-cross-platform-preprocessor-defines-win32-or-win32-or-win32
//Windows
#if defined(_WIN32) || defined(_WIN64)
#define GLFW_EXPOSE_NATIVE_WIN32
#endif

//Unix
#if !defined(_WIN32) && (defined(__unix__) || defined(__unix) || (defined(__APPLE__) && defined(__MACH__)))
//BSD
#if defined(BSD)
#endif

//Linux
#if defined(__linux__)
//#define GLFW_EXPOSE_NATIVE_X11
//#define GLFW_EXPOSE_NATIVE_WAYLAND
#endif

//Mac OS
#if defined(__APPLE__) && defined(__MACH__)
#include <TargetConditionals.h>
#if TARGET_OS_MAC == 1
//#define GLFW_EXPOSE_NATIVE_COCOA
#endif
#endif

#endif
#include <GLFW/glfw3native.h>

extern "C" {
    EXPORT GLFWerrorfun setGlfwErrorCallback(GLFWerrorfun callback);
    EXPORT int initializeGlfw();
    EXPORT void terminateGlfw();

    EXPORT GLFWwindowclosefun setGlfwCloseRequestCallback(GLFWwindow* window, GLFWwindowclosefun callback);
    EXPORT GLFWwindow* createWindow(int width, int height, const char* title, int windowFlags);
    EXPORT void destroyWindow(GLFWwindow* window);
    EXPORT void* getNativeWindowHandle(GLFWwindow* window);

    EXPORT GLFWwindowsizefun setGlfwResizeCallback(GLFWwindow* window, GLFWwindowsizefun callback);
    EXPORT GLFWwindowposfun setGlfwWindowMovedCallback(GLFWwindow* window, GLFWwindowposfun callback);
    EXPORT void resizeWindow(GLFWwindow* window, int width, int height);
    EXPORT void getWindowSize(GLFWwindow* window, int* width, int* height);
    EXPORT void getWindowDecorationSize(GLFWwindow* window, int* left, int* top, int* right, int* bottom);
    EXPORT void setWindowSizeLimits(GLFWwindow* window, int minWidth, int minHeight, int maxWidth, int maxHeight);
    EXPORT void setWindowAspectRatio(GLFWwindow* window, int numerator, int denominator);
    EXPORT void getWindowPosition(GLFWwindow* window, int* xPosition, int* yPosition);
    EXPORT void setWindowPosition(GLFWwindow* window, int xPosition, int yPosition);

    EXPORT GLFWframebuffersizefun setFramebufferResizeCallback(GLFWwindow* window, GLFWframebuffersizefun callback);
    EXPORT GLFWwindowcontentscalefun setWindowContentScaleChangedCallback(GLFWwindow* window, GLFWwindowcontentscalefun callback);
    EXPORT GLFWwindowrefreshfun setWindowNeedsRefreshCallback(GLFWwindow* window, GLFWwindowrefreshfun callback);
    EXPORT void getFramebufferSize(GLFWwindow* window, int* width, int* height);
    EXPORT void getWindowContentScale(GLFWwindow* window, float* xScale, float* yScale);
    EXPORT int checkWindowFramebufferTransparency(GLFWwindow* window);
    EXPORT int setWindowOpacity(GLFWwindow* window, float opacity);

    EXPORT GLFWwindowfocusfun setWindowFocusCallback(GLFWwindow* window, GLFWwindowfocusfun callback);
    EXPORT void focusWindowForced(GLFWwindow* window);
    EXPORT void requestWindowAttention(GLFWwindow* window);

    EXPORT GLFWwindowmaximizefun setWindowMaximizationCallback(GLFWwindow* window, GLFWwindowmaximizefun callback);
    EXPORT void setWindowTitle(GLFWwindow* window, const char* title);
    EXPORT void setWindowIcon(GLFWwindow* window, int arraySize, const GLFWimage* images);
    EXPORT void resetWindowIconToDefault(GLFWwindow* window);
    EXPORT GLFWmonitor* getFullscreenWindowMonitor(GLFWwindow* window);
    EXPORT void setWindowMonitor(GLFWwindow* window, GLFWmonitor* monitor);
    EXPORT void windowEnterFullscreen(GLFWwindow* window);
    EXPORT void windowExitFullscreen(GLFWwindow* window, int xPosition, int yPosition, int width, int height);
    EXPORT void minimizeWindow(GLFWwindow* window);
    EXPORT void maximizeWindow(GLFWwindow* window);
    EXPORT void restoreWindowFromMinimizationOrMaximization(GLFWwindow* window);
    EXPORT void showWindow(GLFWwindow* window);
    EXPORT void hideWindow(GLFWwindow* window);

    EXPORT int receivedCloseRequest(GLFWwindow* window);
    EXPORT void pollEvents();
    EXPORT void waitForEvents();
}
#endif // !WINDOWING_H