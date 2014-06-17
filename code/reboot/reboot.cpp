#pragma warning(push)
#pragma warning(disable:4115)     // C4115: named type definition in parentheses
#pragma warning(disable:4201)     // C4201: nonstandard extension used : nameless struct/union
#pragma warning(disable:4204)     // C4204: nonstandard extension used : non-constant aggregate initializer
#pragma warning(disable:4214)     // C4214: nonstandard extension used : bit field types other than int
#include <windows.h>
#include <stdio.h>
#include <aygshell.h>

#pragma warning(pop)

#include "ossvcs.h"
#define _ehmOnFail LOGEHM
#include "ehm.h"


void LOG(LPCTSTR szFormat, ...) 
{
   TCHAR szBuffer[1024];
   LPTSTR ptsz = szBuffer;
   size_t cchLeft = ARRAYSIZE(szBuffer);

   va_list pArgs; 
   va_start(pArgs, szFormat);
   StringCchVPrintfEx(ptsz, cchLeft, &ptsz, &cchLeft, STRSAFE_IGNORE_NULLS, szFormat, pArgs);
   va_end(pArgs);

   OutputDebugString(szBuffer);
}

// This is called whenever an EHM macro detects a failure
static void LOGEHM(eCodeType /*eType*/, DWORD dwCode, LPCTSTR pszFile, DWORD ulLine, LPCTSTR szMessage)
{
    LOG(L"FAILURE %s:%d - %s  hr=0x%08x", pszFile, ulLine, szMessage, dwCode);
}

// MAIN FUNCTION
int WINAPI
WinMain(
    HINSTANCE   /*hInst*/,
    HINSTANCE   /*hInstPrev*/,
    LPTSTR      lpszCmdLine,
    int         /*nCmdShow*/ )
{
    HRESULT         hr              = S_OK;

    if (*lpszCmdLine == 0)
        hr = ExitWindowsEx(EWX_REBOOT, NULL);
    else if ((*lpszCmdLine == 'd') || (*lpszCmdLine == 'D'))
        hr = ExitWindowsEx(EWX_REBOOT | EWX_DEFER, 0);
    else if ((*lpszCmdLine == 'p') || (*lpszCmdLine == 'P'))
        hr = ExitWindowsEx(EWX_REBOOT | EWX_PROMPT, 0);
    else if ((*lpszCmdLine == 'b') || (*lpszCmdLine == 'B'))
        hr = ExitWindowsEx(EWX_REBOOT | EWX_PROMPT | EWX_DEFER, 0);
    else
        wprintf(L"\nInvalid args, use \"reboot\", \"reboot d\" (to defer), \"reboot p\" (to prompt), or \"reboot b\" (to prompt and defer)\n");

    CHR(hr);
    return SUCCEEDED(hr);

Error:

    wprintf(L"\nERROR: Reboot call failed\n");
    return SUCCEEDED(hr);

}
