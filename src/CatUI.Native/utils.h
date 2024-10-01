#ifdef _WIN32
#define EXPORT __declspec(dllexport)
#elif
#define EXPORT
#endif

//#define BYTE unsigned char