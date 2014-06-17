#include <windows.h>
#include <stdio.h>
#include <auto_xxx.hxx>
#include "ossvcs.h"

#ifndef _ehmOnFail
#define _ehmOnFail(eType, dwCode, pszFile, ulLine, pszMessage)    { wprintf(L" <> <> <> Check Failed: %s (0x%08x) %s:%d", pszMessage, dwCode, pszFile, ulLine); }
#endif
#include "ehm.h"


// MAIN FUNCTION
int WINAPI
WinMain(
    HINSTANCE   /*hInst*/,
    HINSTANCE   /*hInstPrev*/,
    LPTSTR      lpszCmdLine,
    int         /*nCmdShow*/ )
{
    HRESULT         hr              = S_OK;

    CBR(SecureWipeAllVolumes());

Error:
    return SUCCEEDED(hr);
}
