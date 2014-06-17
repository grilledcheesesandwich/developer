//#include <windows.h>
//#include <lmaccess.h>
//#include <lmerr.h>
//#include <stdio.h>
//#include <sddl.h>
//
//#define USER_ACCNT_FLAGS (UF_PASSWD_CANT_CHANGE | UF_DONT_EXPIRE_PASSWD | UF_NOT_DELEGATED | UF_NORMAL_ACCOUNT)
//
//typedef struct _SECURITY_CAPABILITIES {
//    PSID AppContainerSid;
//    PSID_AND_ATTRIBUTES Capabilities;
//    DWORD CapabilityCount;
//    DWORD Reserved;
//} SECURITY_CAPABILITIES, *PSECURITY_CAPABILITIES, *LPSECURITY_CAPABILITIES;
//
//BOOL WINAPI UpdateProcThreadAttribute2(
//  __inout    LPPROC_THREAD_ATTRIBUTE_LIST lpAttributeList,
//  __in       DWORD dwFlags,
//  __in       DWORD_PTR Attribute,
//  __in       PVOID lpValue,
//  __in       SIZE_T cbSize,
//  __out_opt  PVOID lpPreviousValue,
//  __in_opt   PSIZE_T lpReturnSize
//)
//{
//    printf("hello");
//    return TRUE;
//}
//
//#define PROC_THREAD_ATTRIBUTE_SECURITY_CAPABILITIES \
//    ProcThreadAttributeValue (9, FALSE, TRUE, FALSE)
//
//int __cdecl wmain2(int argc, __in_ecount(argc) WCHAR* argv[])
//{
//    DWORD AttributesLength = 0;
//    LPPROC_THREAD_ATTRIBUTE_LIST AttributeList = NULL;
//    LPCWSTR pszCapabilitySids[] = {L"S-1-15-3-1", L"S-1-15-3-2"};
//    PSID* CapabilitySids = NULL;
//    const DWORD CapabilitySidCount = _countof(pszCapabilitySids);
//    PSID_AND_ATTRIBUTES Capabilities = NULL;
//    NET_API_STATUS NASError;
//    LPCWSTR pszPackageSid = L"S-1-15-2-1234";
//    PSID PackageSid = NULL;
//    LPWSTR Password = L"password";
//    PROCESS_INFORMATION ProcessInfo = {0};
//    HRESULT Result = S_OK;
//    SECURITY_CAPABILITIES SecurityCapabilities = {0};
//    STARTUPINFOEX StartupInfo = {0};
//    HANDLE Token = NULL;
//    USER_INFO_1 UserInfo;
//    const DWORD UserInfoLevel = 1;
//    LPCWSTR Username = L"TestApplication1";
//
//    //if (argc != 2) {
//    //    Result = E_INVALIDARG;
//    //    goto Cleanup;
//    //}
//
//    //// Reset the password of the application "user" account
//    //memset(&UserInfo, 0, sizeof(UserInfo));
//
//    //UserInfo.usri1_password = Password;
//    //UserInfo.usri1_priv = USER_PRIV_USER;
//    //UserInfo.usri1_flags = USER_ACCNT_FLAGS;
//
//    //NASError = NetUserSetInfo(NULL, Username, UserInfoLevel, (PBYTE)&UserInfo, NULL);
//    //if (NASError != NERR_Success) {
//    //    Result = E_FAIL;
//    //    goto Cleanup;
//    //}
//
//    //// Logon the application "user"
//    //if (!LogonUserExExW(
//    //                Username,
//    //                NULL,
//    //                Password,
//    //                LOGON32_LOGON_INTERACTIVE,
//    //                LOGON32_PROVIDER_DEFAULT,
//    //                NULL,
//    //                &Token,
//    //                NULL,
//    //                NULL,
//    //                NULL,
//    //                NULL)) {
//    //    Result = HRESULT_FROM_WIN32(GetLastError());
//    //}
//
//    // Set the security capabilities
//    if (!InitializeProcThreadAttributeList(
//                    NULL,
//                    1,
//                    0,
//                    &AttributesLength)) {
//        if (GetLastError() != ERROR_INSUFFICIENT_BUFFER) {
//            Result = HRESULT_FROM_WIN32(GetLastError());
//            goto Cleanup;
//        }
//    }
//
//    AttributeList = (LPPROC_THREAD_ATTRIBUTE_LIST)LocalAlloc(LPTR, AttributesLength);
//    if (AttributeList == NULL) {
//        Result = E_OUTOFMEMORY;
//        goto Cleanup;
//    }
//
//    if (!InitializeProcThreadAttributeList(
//                    AttributeList,
//                    1,
//                    0,
//                    &AttributesLength)) {
//        Result = HRESULT_FROM_WIN32(GetLastError());
//        goto Cleanup;
//    }
//
//    if (!ConvertStringSidToSid(
//                    pszPackageSid,
//                    &PackageSid)) {
//        Result = HRESULT_FROM_WIN32(GetLastError());
//        goto Cleanup;
//    }
//    
//    CapabilitySids = (PSID*)LocalAlloc(LPTR, CapabilitySidCount * sizeof(PSID));
//    if (CapabilitySids == NULL) {
//        Result = E_OUTOFMEMORY;
//        goto Cleanup;
//    }
//
//    Capabilities = (PSID_AND_ATTRIBUTES)LocalAlloc(LPTR, CapabilitySidCount * sizeof(SID_AND_ATTRIBUTES));
//    if (Capabilities == NULL) {
//        Result = E_OUTOFMEMORY;
//        goto Cleanup;
//    }
//
//    for (DWORD i = 0; i < CapabilitySidCount; i++)
//    {
//        if (!ConvertStringSidToSid(
//                        pszCapabilitySids[i],
//                        &CapabilitySids[i]))
//        {
//            Result = HRESULT_FROM_WIN32(GetLastError());
//            goto Cleanup;
//        }
//
//        Capabilities[i].Sid = CapabilitySids[i];
//        Capabilities[i].Attributes = SE_GROUP_ENABLED;
//    }
//
//    SecurityCapabilities.CapabilityCount = CapabilitySidCount;
//    SecurityCapabilities.Capabilities = Capabilities;
//    SecurityCapabilities.AppContainerSid = PackageSid;
//    SecurityCapabilities.Reserved = 0;
//    
//    if (!UpdateProcThreadAttribute2(
//                    AttributeList,
//                    0,
//                    PROC_THREAD_ATTRIBUTE_SECURITY_CAPABILITIES,
//                    &SecurityCapabilities,
//                    sizeof(SecurityCapabilities),
//                    NULL,
//                    NULL)) {
//        Result = HRESULT_FROM_WIN32(GetLastError());
//        goto Cleanup;
//    }
//
//    StartupInfo.StartupInfo.cb = sizeof(StartupInfo);
//    StartupInfo.lpAttributeList = AttributeList;
//
//    if (!CreateProcessAsUser(
//                    Token,
//                    argv[1],
//                    NULL,
//                    NULL,
//                    NULL,
//                    TRUE,
//                    EXTENDED_STARTUPINFO_PRESENT,
//                    NULL,
//                    NULL,
//                    (LPSTARTUPINFO)&StartupInfo,
//                    &ProcessInfo)) {
//        Result = HRESULT_FROM_WIN32(GetLastError());
//        goto Cleanup;
//    }
//
//Cleanup:
//    if (Token != NULL) {
//        CloseHandle(Token);
//    }
//
//    if (AttributeList != NULL) {
//        LocalFree(AttributeList);
//    }
//
//    if (PackageSid != NULL) {
//        LocalFree(PackageSid);
//    }
//
//    if (CapabilitySids != NULL) {
//        for (DWORD i = 0; i < CapabilitySidCount; i++) {
//            if (CapabilitySids[i] != NULL) {
//                LocalFree(CapabilitySids[i]);
//            }
//        }
//        
//        LocalFree(CapabilitySids);
//    }
//
//    if (Capabilities != NULL) {
//        LocalFree(Capabilities);
//    }
//
//    if (ProcessInfo.hProcess != NULL) {
//        CloseHandle(ProcessInfo.hProcess);
//    }
//
//    if (ProcessInfo.hThread != NULL) {
//        CloseHandle(ProcessInfo.hThread);
//    }
//    
//    return SUCCEEDED(Result)? 0: -1;
//}
