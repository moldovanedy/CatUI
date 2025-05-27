#ifndef MAIN_HANDLER_H_
#define MAIN_HANDLER_H_

extern "C"
{
    const char *GetLastError();

    int OpenCommunication();
    void CloseCommunication();
    void WaitEventsWithTimeout(int timeout_millis);

    const int kDeviceMouse = 0;
    const int kDeviceTouchpad = 1;

    typedef void (*OnScroll)(int, int);

    void AddOnScrollCallback(OnScroll *scroll_callback);
}

#endif