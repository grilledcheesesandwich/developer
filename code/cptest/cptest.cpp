#include <windows.h>
#include <stdio.h>
#include <ehmhelper.h>
#include <SecurityRuntime.h>
#include <CreateProcessInChamber.h>

int __cdecl wmain(int argc, TCHAR *argv[])
{    
    HRESULT hr = S_OK;
    PROCESS_INFORMATION pi = {0};
    STARTUPINFO si = {0};
    LPWSTR wszApp = argv[1];

    for (int i = 0; i < argc; i++)
    {
        wprintf(L"%i: %s\n", i, argv[i]);
    }

    WCHAR szBuf[MAX_PATH] = {0};
    swprintf_s(szBuf, L"\"%s\"", wszApp);
    
    /*LPWSTR wszSid = NULL;
    CHR(GetChamberSidFromId(argv[2], &wszSid));
    wprintf(wszSid);*/
    wprintf(L"\n");
    WCHAR wszFullPath[MAX_PATH] = {0};
    CBR(GetFullPathName(wszApp, _countof(wszFullPath), wszFullPath, NULL));
    //CHR(CreateProcessInChamber(wszSid, NULL, wszApp, szBuf, FALSE, 0, NULL, &si, &pi));
    CBRWIN(CreateProcess(wszApp, szBuf, NULL, NULL, FALSE, 0, NULL, NULL, &si, &pi));
    CBRWIN(WAIT_OBJECT_0 == WaitForSingleObject(pi.hProcess, 20000));
    
    DWORD dwExitCode;
    CBRWIN(GetExitCodeProcess(pi.hProcess, &dwExitCode));
    CBR(dwExitCode == 0);

Error:

    if (pi.hProcess) { CloseHandle(pi.hProcess); }
    if (pi.hThread) { CloseHandle(pi.hThread); }

    return FAILED(hr);
}
