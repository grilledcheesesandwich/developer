#include <windows.h>

// MAIN FUNCTION
int WINAPI
WinMain(
    HINSTANCE   /*hInst*/,
    HINSTANCE   /*hInstPrev*/,
    LPTSTR      /*lpszCmdLine*/,
    int         /*nCmdShow*/ )
{
    MessageBox(NULL, L"I can run", L"Whitelist Demo", MB_OK);

    return 0;
}
