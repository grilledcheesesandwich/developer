#include <windows.h>
#include <stdio.h>
#include <AclAPI.h>
#include <ehmhelper.h>

DWORD SetPrivilege(
    __in LPCTSTR PrivilegeName,     // name of privilege to enable/disable
    BOOL bEnablePrivilege           // to enable or disable privilege
    ) 
{
    TOKEN_PRIVILEGES tp = {0};
    LUID luid = {0};
    HANDLE hToken;
    DWORD dwRes = ERROR_SUCCESS;

    if (!OpenProcessToken(GetCurrentProcess(), TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, &hToken)) {
        dwRes = GetLastError();
        goto Cleanup;
    }

    if ( !LookupPrivilegeValue( 
            NULL,            // lookup privilege on local system
            PrivilegeName,   // privilege to lookup 
            &luid ) ) {      // receives LUID of privilege
        dwRes = GetLastError();
        goto Cleanup;
    }

    tp.PrivilegeCount = 1;
    tp.Privileges[0].Luid = luid;
    if (bEnablePrivilege) {
        tp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;
    }
    else {
        tp.Privileges[0].Attributes = 0;
    }

    // Enable the privilege or disable all privileges.
    if ( !AdjustTokenPrivileges(
           hToken, 
           FALSE, 
           &tp, 
           sizeof(TOKEN_PRIVILEGES), 
           (PTOKEN_PRIVILEGES) NULL, 
           (PDWORD) NULL) ) { 
        dwRes = GetLastError();
        goto Cleanup;
    } 

    if (GetLastError() == ERROR_NOT_ALL_ASSIGNED) {
        dwRes = ERROR_NOT_ALL_ASSIGNED;
        goto Cleanup;
    } 

Cleanup:
    if (hToken) {
        CloseHandle(hToken);
    }
    return dwRes;
}

BOOL AccessChk(PCWSTR szObjectPath)
{
    STARTUPINFO si = {0};
    PROCESS_INFORMATION pi = {0};
    if (!CreateProcess(
        L"C:\\windows\\system32\\Sleep.exe",
        L"C:\\windows\\system32\\Sleep.exe 30000",
        NULL,
        NULL,
        false,
        0,
        NULL,
        NULL,
        &si,
        &pi))
    {
        wprintf(L"\nCreateProcess failed - %d\n", GetLastError());
        return FALSE;
    }

    HANDLE hToken = NULL;
    if (!OpenProcessToken(pi.hProcess, TOKEN_ALL_ACCESS, &hToken))
    {
        wprintf(L"\nOpenProcessToken failed - %d\n", GetLastError());
        return FALSE;
    }

    PSID pOwner = NULL;
    PSID pGroup = NULL;
    PACL pDacl = NULL;
    PACL pSacl = NULL;
    PSECURITY_DESCRIPTOR pSD = NULL;
    DWORD returnValue = GetNamedSecurityInfo(
        szObjectPath,
        SE_FILE_OBJECT,
        0xFFFFFFFF,
        &pOwner,
        &pGroup,
        &pDacl,
        &pSacl,
        &pSD);
    if (returnValue != ERROR_SUCCESS)
    {
        wprintf(L"\nGetNamedSecurityInfo error - %d\n", returnValue);
        return FALSE;
    }

    GENERIC_MAPPING mapping;
    mapping.GenericAll = KEY_ALL_ACCESS;
    mapping.GenericWrite = KEY_WRITE;
    mapping.GenericRead = KEY_READ;
    mapping.GenericExecute = KEY_EXECUTE;
    
    DWORD dwGrantedAccess = 0;
    BOOL fAccessStatus;
    if (!AccessCheck(
        pSD,
        hToken,
        MAXIMUM_ALLOWED,
        &mapping,
        NULL,
        0,
        &dwGrantedAccess,
        &fAccessStatus))
    {
        wprintf(L"\nAccessCheck error - %d\n", GetLastError());
        return FALSE;
    }
    if (!fAccessStatus)
    {
        wprintf(L"\nAccess granted was insufficient - %d\n", GetLastError());
    }
    else
    {
        wprintf(L"Access granted was: %d\n", dwGrantedAccess);
    }

    return TRUE;
}

int __cdecl wmain(int /*argc*/, WCHAR *argv[])
{
    HRESULT hr = S_OK;

    CBR(ERROR_SUCCESS == SetPrivilege(SE_SECURITY_NAME, TRUE));
    CBR(ERROR_SUCCESS == SetPrivilege(SE_BACKUP_NAME, TRUE));
    CBR(ERROR_SUCCESS == SetPrivilege(SE_RESTORE_NAME, TRUE));

    CBR(AccessChk(argv[1]));

    CHR(hr);

Error:
    return SUCCEEDED(hr);
}
