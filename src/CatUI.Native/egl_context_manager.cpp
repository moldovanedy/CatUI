#include "egl_context_manager.h"

static char* eglLastError = nullptr;

char* getEglLastError() {
    return eglLastError;
}

int setupEgl(int renderingBackend, EGLContext* context, EGLDisplay* display, EGLSurface* surface, GLFWwindow* window) {
    *context = nullptr;
    const EGLAttrib platformAttributes[] = {
        EGL_PLATFORM_ANGLE_TYPE_ANGLE, EGL_PLATFORM_ANGLE_TYPE_DEFAULT_ANGLE,
        EGL_PLATFORM_ANGLE_MAX_VERSION_MAJOR_ANGLE, 1,
        EGL_PLATFORM_ANGLE_MAX_VERSION_MINOR_ANGLE, 1,
        EGL_NONE,
    };
    *display = eglGetPlatformDisplay(EGL_PLATFORM_ANGLE_ANGLE, EGL_DEFAULT_DISPLAY, platformAttributes);
    if (display == EGL_NO_DISPLAY) {
        return 0;
    }

    if (!eglInitialize(display, nullptr, nullptr)) {
        return 0;
    }

    EGLint configAttributes[] = {
        EGL_SURFACE_TYPE, EGL_WINDOW_BIT,
        EGL_RENDERABLE_TYPE, EGL_OPENGL_ES2_BIT,
        EGL_NONE
    };
    EGLConfig config;
    EGLint numConfigs;
    if (!eglChooseConfig(display, configAttributes, &config, 1, &numConfigs) || numConfigs < 1) {
        return 0;
    }

    EGLint contextAttributes[] = {
        EGL_CONTEXT_CLIENT_VERSION, 2,
        EGL_NONE
    };
    EGLContext ctx = eglCreateContext(display, config, EGL_NO_CONTEXT, contextAttributes);
    if (ctx == EGL_NO_CONTEXT) {
        return 0;
    }

    *context = ctx;
    EGLNativeWindowType nativeWindow = (EGLNativeWindowType)getNativeWindowHandle(window);
    *surface = eglCreateWindowSurface(display, config, nativeWindow, nullptr);
    if (surface == EGL_NO_SURFACE) {
        return 0;
    }

    if (!eglMakeCurrent(display, surface, surface, context)) {
        return 0;
    }

    return 1;
}

void swapBuffers(EGLDisplay* display, EGLSurface* surface) {
    eglSwapBuffers(display, surface);
}

void terminateEgl(EGLDisplay* display, EGLSurface* surface, EGLContext* context) {
    eglDestroySurface(display, surface);
    eglDestroyContext(display, context);
}