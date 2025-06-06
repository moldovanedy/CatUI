#ifndef SCROLL_HANDLER_H_
#define SCROLL_HANDLER_H_

const int kDeviceMouse = 0;
const int kDeviceTouchpad = 1;

typedef void (*OnScroll)(int, int);

void AddOnScrollCallback(OnScroll *scroll_callback);

#pragma region Internal

void InvokeOnScrollCallbacks(int value, int device_type);

void ScrollHandlerFreeResources();

#pragma endregion

#endif