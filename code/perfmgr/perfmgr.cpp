#include <windows.h>
#include <stdio.h>
#include <ossvcs.h>
#include <auto_xxx.hxx>
#include <string.hxx>
#include <PerfScenario.h>
#include <qautils.h>

#define _ehmOnFail LOGEHM
#include "ehm.h"

// This is called whenever an EHM macro detects a failure
static void LOGEHM(eCodeType /*eType*/, DWORD dwCode, LPCTSTR pszFile, DWORD ulLine, LPCTSTR szMessage)
{
    wprintf(L"FAILURE %s:%d - %s  hr=0x%08x", pszFile, ulLine, szMessage, dwCode);
}

// {21C279FF-44DE-4e43-843D-36B7B093CB90}
static GUID s_guidSamplePerf = { 0x83854dc6, 0xec7d, 0x4272, { 0x88, 0x56, 0xcb, 0x1a, 0x44, 0xdb, 0x56, 0x5 } };

// MAIN FUNCTION
int WINAPI
WinMain(
    HINSTANCE   /*hInst*/,
    HINSTANCE   /*hInstPrev*/,
    LPTSTR      lpszCmdLine,
    int         /*nCmdShow*/ )
{
    HRESULT hr = S_OK;
    LPWSTR argv[10];
    ce::wstring szSessionName(L"MDPG\\");
    int argc = CreateArgvArgc(L"perfmgr", argv, lpszCmdLine);
    int cAction = toupper(argv[1][0]);
    CBR(argc == 3 && (cAction == 'O' || cAction == 'C'));

    szSessionName.append(argv[2]);

    CHR(PerfScenarioOpenSession(szSessionName));
    if (cAction == 'C')
    {
        CHR(PerfScenarioFlushMetrics(FALSE, &s_guidSamplePerf, L"PerfMgr_Namespace", szSessionName, L"\\release\\perfmgr_session.xml", NULL, NULL));
        CHR(PerfScenarioCloseSession(szSessionName));
    }

Error:
    if (FAILED(hr))
    {
        wprintf(L"USAGE: perfmgr.exe <o/c> MDPG\\<session>");
    }
    return FAILED(hr);
}
