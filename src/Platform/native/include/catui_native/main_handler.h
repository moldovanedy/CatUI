#ifndef MAIN_HANDLER_H_
#define MAIN_HANDLER_H_

extern "C"
{
    int OpenCommunication();
    void CloseCommunication();
    void WaitEventsWithTimeout(int timeout_millis);
}

#endif