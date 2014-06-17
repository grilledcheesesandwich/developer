#include <nt.h>
#include <ntrtl.h>
#include <nturtl.h>
#include <windows.h>
#include <chamberprofile.h>
#include <securityruntime.h>
#include <strsafe.h>
#include <wpcoreutil.h> // ShGetSpecialFolderPath

#define _ehmOnFail LOGEHM
#include <ehm.h>

void LOG(LPCTSTR szFormat, ...) 
{
   TCHAR szBuffer[1024];

   va_list pArgs; 
   va_start(pArgs, szFormat);
   StringCchVPrintf(szBuffer, _countof(szBuffer), szFormat, pArgs);
   va_end(pArgs);
   
   // no automatic newlines in Apollo
   wcscat_s(szBuffer, L"\n");

   wprintf(L"%s", szBuffer);
   OutputDebugString(szBuffer);
}

// This is called whenever an EHM macro detects a failure
static void LOGEHM(eCodeType /*eType*/, DWORD dwCode, LPCWSTR pszFile, DWORD ulLine, LPCTSTR szMessage)
{
    LOG(L"FAILURE %s:%d - %s  hr=0x%08x", pszFile, ulLine, szMessage, dwCode);
}

HRESULT WriteToChamberPrivateProfileRegistry(LPCWSTR szChamberName)
{
    HRESULT hr;
    HKEY hKey = NULL;
    HKEY hSubkey = NULL;
    LRESULT lStatus;
    LPCWSTR szDummyKey = L"WhoamiSubkey";
    LPCWSTR szDummyValue = L"WhoamiValue";
    DWORD dwValue = 0xABABABAB;
    
    hr = GetChamberRegistryLocation(szChamberName, KEY_READ|KEY_WRITE, &hKey);
    CHR(hr);

    ULONG cbKeyNameInfo = 0;
    CBR(STATUS_BUFFER_TOO_SMALL == NtQueryKey(
        hKey,
        KeyNameInformation,
        NULL,
        cbKeyNameInfo,
        &cbKeyNameInfo));

    PKEY_NAME_INFORMATION pKeyInfo = (PKEY_NAME_INFORMATION) new BYTE[cbKeyNameInfo];

    CBR(STATUS_SUCCESS == NtQueryKey(
        hKey,
        KeyNameInformation,
        pKeyInfo,
        cbKeyNameInfo,
        &cbKeyNameInfo));
    
    WCHAR tempBuffer[MAX_PATH] = {0};
    wcsncpy_s(tempBuffer, pKeyInfo->Name, pKeyInfo->NameLength / 2);

    LOG(L"profile registry path [%s]", tempBuffer);
    
    lStatus = RegCreateKeyEx(
        hKey,           // hKey
        szDummyKey,     // lpSubKey
        0,              // reserved
        0,              // lpClass
        0,              // dwOptions
        KEY_SET_VALUE,  // samDesired
        0,              // lpSecurityAttributes
        &hSubkey,       // phkResult
        0);             // lpdwDisposition
    CBREx(lStatus == ERROR_SUCCESS, HRESULT_FROM_WIN32(lStatus));
    
    // write dummy text to registry
    lStatus = RegSetValueEx(hSubkey, szDummyValue, 0, REG_DWORD, (BYTE*)&dwValue, sizeof(dwValue));
    CBREx(lStatus == ERROR_SUCCESS, HRESULT_FROM_WIN32(lStatus));
    
    LOG(L"Done writing to registry.");
    
Error:
    if (hKey != NULL)
    {
        RegCloseKey(hKey);
    }

    if (hSubkey != NULL)
    {
        RegCloseKey(hSubkey);
    }

    
    return hr;
}

HRESULT WriteToChamberPrivateDirectory(LPCWSTR szChamberName)
{
    HRESULT hr = S_OK;
    WCHAR szDataPath[MAX_PATH] = {0};
    WCHAR szCodePath[MAX_PATH] = {0};
    DWORD cbDataPath = _countof(szDataPath);
    DWORD cbCodePath = _countof(szCodePath);
    
    hr = GetChamberFolderPath(szChamberName,
             szDataPath,
             &cbDataPath,
             szCodePath, 
             &cbCodePath); // work around WP8 34508
    CHR(hr);
    
    LOG(L"profile data dir [%s]", szDataPath);
    LOG(L"profile code dir [%s]", szCodePath);
Error:
    return hr;    
    
}


HRESULT PrintEnvironmentVariable(LPCWSTR szVariable)
{
    HRESULT hr = S_OK;
    DWORD dwResult;
    WCHAR szBuffer[MAX_PATH] = {0};
    
    dwResult = GetEnvironmentVariable(szVariable, &szBuffer[0], _countof(szBuffer));
    if (dwResult == 0 && GetLastError() == ERROR_ENVVAR_NOT_FOUND)
    {
        LOG(L"%%%s%%= [%s]", szVariable, L"not set");
        return S_OK;
    }
    else
    {
        CBREx(dwResult != 0, HRESULT_FROM_WIN32(GetLastError()));
    }
    


    LOG(L"%%%s%%= [%s]", szVariable, szBuffer);

Error:
    return hr; 
}

HRESULT PrintCurrentDirectory()
{
    HRESULT hr = S_OK;
    DWORD dwResult;
    WCHAR szBuffer[MAX_PATH] = {0};
    //LPCSTR szEnv = getenv(szVariable);
    dwResult = GetCurrentDirectory(_countof(szBuffer), &szBuffer[0]);
    CBR(dwResult != 0);


    LOG(L"CWD= [%s]", szBuffer);

Error:
    return hr; 
}

HRESULT MakeChamberProfiles()
{
    HRESULT hr;
    LPCWSTR szChambers[] = {
        L"AppTestCapA",
        L"AppTestCapB",
        L"AppTestCapAB",
        L"AppTestCapNone",
    };
    
    for (DWORD i = 0; i < _countof(szChambers); i++)
    {
        hr = CreateChamberProfile(szChambers[i], 0);
        LOG(L"Creating chamber %a %s", szChambers[i], SUCCEEDED(hr) ? L"succeeded" : L"failed");
    }
    
    return S_OK;    
}


/*HRESULT PrintSpecialFolderPath(REFKNOWNFOLDERID rfid)
{
    LPCWSTR szPath;
    HRESULT hr;

    hr = ShGetKnownFolderPath(rfid, 0, NULL, &szPath);
    CHR(hr);

    LOG(L"Path was %s", szPath);

Error:
    if (szPath != NULL)
    {
        CoTaskMemFree(szPath);
    }

    return hr;
}*/
// print out my paths

HRESULT PrintRtlBnoPrefix()
{
    HRESULT hr = S_OK;
    UNICODE_STRING objectPath;
    NTSTATUS Status;

    Status = ::RtlGetAppContainerNamedObjectPath(
            NULL,           // _In_opt_ HANDLE Token,  
            NULL,           // _In_opt_ PSID AppContainerSid,  
            false,         //  BOOLEAN RelativePath,  
            &objectPath);   // PUNICODE_STRING ObjectPath  
    CBR(Status == STATUS_SUCCESS);
    CBR(objectPath.Buffer != NULL);

    LOG(L"RtlGetAppContainerNamedObjectPath absolute path [%s]", objectPath.Buffer);
    RtlFreeUnicodeString(&objectPath);
    
    Status = ::RtlGetAppContainerNamedObjectPath(
            NULL,           // _In_opt_ HANDLE Token,  
            NULL,           // _In_opt_ PSID AppContainerSid,  
            true,         //  BOOLEAN RelativePath,  
            &objectPath);   // PUNICODE_STRING ObjectPath  
    CBR(Status == STATUS_SUCCESS);
    CBR(objectPath.Buffer != NULL);

    LOG(L"RtlGetAppContainerNamedObjectPath relative path [%s]", objectPath.Buffer);
    
    
Error:
    if (objectPath.Buffer != NULL)
    {
        RtlFreeUnicodeString(&objectPath);
    }

    return hr;
}


HRESULT PrintBnoPrefix()
{

    NTSTATUS Status = STATUS_SUCCESS;
    ULONG ObjectPathLength = 0;
    LPWSTR ObjectPath = NULL;
    LPWSTR szAppName = L"DAC";
    HRESULT hr = S_OK;

    Status = GetApplicationBNOPrefixPath(szAppName, TRUE, ObjectPathLength, ObjectPath, &ObjectPathLength);
    CBR(Status == STATUS_BUFFER_TOO_SMALL);

    ObjectPath = new WCHAR[ObjectPathLength];
    Status = GetApplicationBNOPrefixPath(szAppName, TRUE, ObjectPathLength, ObjectPath, &ObjectPathLength);
    CBR(Status == STATUS_SUCCESS);
    CBR(ObjectPath[0] != L'\0');
    
    LOG(L"Relative BNO Prefix is [%s]", ObjectPath);
    
    // start over with the absolute path
    delete [] ObjectPath;
    ObjectPathLength = 0;
    
    Status = GetApplicationBNOPrefixPath(szAppName, FALSE, ObjectPathLength, ObjectPath, &ObjectPathLength);
    CBR(Status == STATUS_BUFFER_TOO_SMALL);

    ObjectPath = new WCHAR[ObjectPathLength];
    Status = GetApplicationBNOPrefixPath(szAppName, FALSE, ObjectPathLength, ObjectPath, &ObjectPathLength);
    CBR(Status == STATUS_SUCCESS);
    CBR(ObjectPath[0] != L'\0');
    
    LOG(L"Absolute BNO Prefix is [%s]", ObjectPath);

Error:

    if (ObjectPath != NULL) 
    {
        delete [] ObjectPath;
    }
    
    return hr;    
}

HRESULT PrintTempPath()
{
    HRESULT hr = S_OK;
    WCHAR szTempPath[MAX_PATH];
    DWORD dwRet;
    
    dwRet = GetTempPath(_countof(szTempPath), &szTempPath[0]);
    CBR(dwRet != 0);
    
    LOG(L"GetTempPath returned [%s]", szTempPath);
    
Error:
    return hr;    
}

HRESULT PrintFolderPaths()
{
    HRESULT hr = S_OK;
    PWSTR szPath;
    
    CHR(SHGetKnownFolderPath(FOLDERID_LocalAppData, NULL, NULL, &szPath));
    LOG(L"FOLDERID_LocalAppData = [%s]", szPath);
    CoTaskMemFree(szPath);

    CHR(SHGetKnownFolderPath(FOLDERID_LocalAppDataLow, NULL, NULL, &szPath));
    LOG(L"FOLDERID_LocalAppDataLow = [%s]", szPath);
    CoTaskMemFree(szPath);

    CHR(SHGetKnownFolderPath(FOLDERID_RoamingAppData, NULL, NULL, &szPath));
    LOG(L"FOLDERID_RoamingAppData = [%s]", szPath);
    CoTaskMemFree(szPath);

    CHR(SHGetKnownFolderPath(FOLDERID_TempAppData, NULL, NULL, &szPath));
    LOG(L"FOLDERID_TempAppData = [%s]", szPath);
    CoTaskMemFree(szPath);

    CHR(SHGetKnownFolderPath(FOLDERID_FrameworkTempAppData, NULL, NULL, &szPath));
    LOG(L"FOLDERID_FrameworkTempAppData = [%s]", szPath);
    CoTaskMemFree(szPath);

    CHR(SHGetKnownFolderPath(FOLDERID_LocalSharedAppData, NULL, NULL, &szPath));
    LOG(L"FOLDERID_LocalSharedAppData = [%s]", szPath);
    CoTaskMemFree(szPath);

    CHR(SHGetKnownFolderPath(FOLDERID_InternetCache, NULL, NULL, &szPath));
    LOG(L"FOLDERID_InternetCache  = [%s]", szPath);
    CoTaskMemFree(szPath);

    CHR(SHGetKnownFolderPath(FOLDERID_Cookies, NULL, NULL, &szPath));
    LOG(L"FOLDERID_Cookies = [%s]", szPath);
    CoTaskMemFree(szPath);

    CHR(SHGetKnownFolderPath(FOLDERID_History, NULL, NULL, &szPath));
    LOG(L"FOLDERID_Cookies = [%s]", szPath);
    CoTaskMemFree(szPath);
    
    
Error:
    return hr;    
}

// MAIN FUNCTION
int WINAPI
wWinMain(
    HINSTANCE   /*hInst*/,
    HINSTANCE   /*hInstPrev*/,
    LPWSTR      lpszCmdLine,
    int         /*nCmdShow*/ )
{
    HRESULT hr = S_OK;

    LOG(L"whoami compiled %S %S.", __DATE__, __TIME__);

    LOG(L"cmd line: [%s]", lpszCmdLine);
    
    //DebugBreak();
    
    // dump data
    if (wcslen(lpszCmdLine) == 0)
    {
        PrintCurrentDirectory();
        PrintEnvironmentVariable(L"TEMP");
        PrintEnvironmentVariable(L"TMP");
        PrintEnvironmentVariable(L"LOCALAPPDATA");
        PrintEnvironmentVariable(L"APPDATA");
        PrintEnvironmentVariable(L"USERPROFILE");
        
        PrintTempPath();
        
        PrintFolderPaths();
        
        LOG(L"Checking my profile");
        WriteToChamberPrivateDirectory(NULL);
        WriteToChamberPrivateProfileRegistry(NULL);
        
        PrintRtlBnoPrefix();
        //PrintBnoPrefix();
    }
    else
    {
        LOG(L"Checking profile for [%s]", lpszCmdLine);
        hr = CreateChamberProfile(lpszCmdLine, 0);
        // log it but keep going
        if (FAILED(hr)) 
        {
            LOG(L"Creating chamber profile for %s returned %08x", lpszCmdLine, hr);
        }
            
        WriteToChamberPrivateDirectory(lpszCmdLine);
        WriteToChamberPrivateProfileRegistry(lpszCmdLine);
        
    }
    
    CHR(hr);
    LOG(L"WHOAMI: Done.");

Error:
    return hr;
}
