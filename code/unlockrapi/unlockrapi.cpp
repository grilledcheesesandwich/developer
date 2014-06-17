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

#define CBRWIN(x) CBREx(x, HRESULT_FROM_WIN32(GetLastError()))

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
    LPTSTR      /*lpszCmdLine*/,
    int         /*nCmdShow*/ )
{
    HRESULT hr;
    HKEY hKey;
    LONG lRes;

    lRes = RegOpenKeyEx(HKEY_LOCAL_MACHINE, L"\\Security\\Policies\\Policies", 0, 0, &hKey);
    CBRWIN(ERROR_SUCCESS == lRes);

    DWORD dwType = 0;
    DWORD dwData = 0;
    DWORD cbData = sizeof(dwData);

    lRes = RegQueryValueEx(hKey, L"00001001", 0, &dwType, (LPBYTE)&dwData, &cbData);
    CBRWIN(ERROR_SUCCESS == lRes);

    const BYTE dwVal = 1;

    lRes = RegSetValueEx(hKey, L"00001001", 0, REG_DWORD, &dwVal, sizeof(dwVal));
    CBRWIN(ERROR_SUCCESS == lRes);

    lRes = RegCloseKey(hKey);
    CBRWIN(ERROR_SUCCESS == lRes);

    return ERROR_SUCCESS == lRes;

Error:

    wprintf(L"\nERROR: Reg change failed\n");
    return ERROR_SUCCESS == lRes;
}
