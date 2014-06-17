#pragma warning(push)
#pragma warning(disable:4115)     // C4115: named type definition in parentheses
#pragma warning(disable:4201)     // C4201: nonstandard extension used : nameless struct/union
#pragma warning(disable:4204)     // C4204: nonstandard extension used : non-constant aggregate initializer
#pragma warning(disable:4214)     // C4214: nonstandard extension used : bit field types other than int
#include <windows.h>
#include <stdio.h>

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

void function(int a, int b, int c) {
   char buffer1[5];
   char buffer2[10];
   int *ret;

   ret = ((int *)buffer1) + 12;
   (*ret) += 4;
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

    int x;
    x = 0;
    function(1,2,3);
    x = 1;
    printf("%d\n",x);

    CHR(hr);

Error:
    return SUCCEEDED(hr);
}