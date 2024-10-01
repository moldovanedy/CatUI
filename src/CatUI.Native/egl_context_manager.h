#ifndef EGL_CONNTEXT_MANAGER_H
#define EGL_CONNTEXT_MANAGER_H

#include "utils.h"
#include <EGL/egl.h>
#include <EGL/eglext.h>
#include <EGL/eglext_angle.h>

#include "windowing.h"

extern "C" {
    EXPORT char* getEglLastError();

    EXPORT int setupEgl(int renderingBackend, EGLContext* context, EGLDisplay* display, EGLSurface* surface, GLFWwindow* window);
    EXPORT void swapBuffers(EGLDisplay* display, EGLSurface* surface);
    EXPORT void terminateEgl(EGLDisplay* display, EGLSurface* surface, EGLContext* context);
}
#endif //EGL_CONNTEXT_MANAGER_H
