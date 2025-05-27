#include <iostream>
#include <string>
#include <vector>

#include "catui_native/main_handler.h"
#include "catui_native/linux/wl_setup.h"

const char *m_err = "";

static std::vector<OnScroll *> m_scroll_callbacks = std::vector<OnScroll *>();

const char *GetLastError()
{
    return m_err;
}

int OpenCommunication()
{
    return 1;
}

void CloseCommunication()
{
}

void WaitEventsWithTimeout(int timeout_millis)
{
}

void AddOnScrollCallback(OnScroll *scroll_callback)
{
}