#ifndef UNICODE
#define UNICODE
#endif
#pragma comment(lib, "netapi32.lib")

#include <nt.h>
#include <ntrtl.h>
#include <nturtl.h>
#include <windows.h>
#include <assert.h>
#include <sddl.h>
#include <ehmhelper.h>
#include <sectesthlp.h>
#include <lm.h>
#include <chamberprofile.h>
#include <CreateProcessInChamber.h>
#include <SecurityRuntime.h>
#include <GwpCpc.h>
#include <wpcoreutil.h>
#include <GetDeviceUniqueID.h>
#include <wincrypt.h>

DWORD PrintSidFromAccountName(LPCWSTR szAccountName)
{
    DWORD dwRet = ERROR_SUCCESS;
    PSID pSid = NULL;
    DWORD cbSid = 0;
    LPWSTR szDomainName = NULL;
    DWORD cchReferencedDomainName = 0;
    SID_NAME_USE peUse;
    BOOL fSuccess = LookupAccountNameLocal(szAccountName, pSid, &cbSid, szDomainName, &cchReferencedDomainName, &peUse);
    if (!fSuccess)
    {
        dwRet = GetLastError();
        if (dwRet == ERROR_INSUFFICIENT_BUFFER)
        {
            pSid = new BYTE[cbSid];
            szDomainName = new WCHAR[cchReferencedDomainName];
            fSuccess = LookupAccountNameLocal(szAccountName, pSid, &cbSid, szDomainName, &cchReferencedDomainName, &peUse);
            if (!fSuccess)
            {
                dwRet = GetLastError();
                goto Cleanup;
            }
        }
        else
        {
            goto Cleanup;
        }
    }
    LPWSTR szSid = NULL;
    fSuccess = ConvertSidToStringSid(pSid, &szSid);
    if (!fSuccess)
    {
        dwRet = GetLastError();
        goto Cleanup;
    }
    wprintf(szSid);

Cleanup:

    return dwRet;
}

HRESULT GetAppContainerNamedObjectPathTest(LPCTSTR StringSid)
{
    HRESULT hr = S_OK;
    HANDLE hProcess = GetCurrentProcess();
    HANDLE hToken = NULL;
    CBR(OpenProcessToken(hProcess, TOKEN_ALL_ACCESS, &hToken));
    PSID pSid = NULL;
    CBRWIN(ConvertStringSidToSid(StringSid, &pSid));
    ULONG ReturnLength = 0;
    WCHAR ObjectPath[MAX_PATH] = {0};
    DWORD dwResult = GetAppContainerNamedObjectPath(NULL, pSid, _countof(ObjectPath), ObjectPath, &ReturnLength);
    printf("%d", dwResult);
    printf("ReturnLength: %d", ReturnLength);
    wprintf(ObjectPath);
    CBR(ERROR_SUCCESS == dwResult);

Error:
    return hr;
}

int EnumerateAccounts(int argc, wchar_t *argv[])
{
   LPUSER_INFO_0 pBuf = NULL;
   LPUSER_INFO_0 pTmpBuf;
   DWORD dwLevel = 0;
   DWORD dwPrefMaxLen = MAX_PREFERRED_LENGTH;
   DWORD dwEntriesRead = 0;
   DWORD dwTotalEntries = 0;
   DWORD dwResumeHandle = 0;
   DWORD i;
   DWORD dwTotalCount = 0;
   NET_API_STATUS nStatus;
   LPTSTR pszServerName = NULL;

   if (argc > 2)
   {
      fwprintf(stderr, L"Usage: %s [\\\\ServerName]\n", argv[0]);
      exit(1);
   }
   // The server is not the default local computer.
   //
   if (argc == 2)
      /*pszServerName =  (LPTSTR) argv[1];*/
      dwLevel = _wtoi(argv[1]);
   wprintf(L"\nUser account on %s: \n", pszServerName);
   //
   // Call the NetUserEnum function, specifying level 0; 
   //   enumerate global user account types only.
   //
   do // begin do
   {
      nStatus = NetUserEnum((LPCWSTR) pszServerName,
                            dwLevel,
                            0, // global users
                            (LPBYTE*)&pBuf,
                            dwPrefMaxLen,
                            &dwEntriesRead,
                            &dwTotalEntries,
                            &dwResumeHandle);
      //
      // If the call succeeds,
      //
      if ((nStatus == NERR_Success) || (nStatus == ERROR_MORE_DATA))
      {
         if ((pTmpBuf = pBuf) != NULL)
         {
            //
            // Loop through the entries.
            //
            for (i = 0; (i < dwEntriesRead); i++)
            {
               assert(pTmpBuf != NULL);

               if (pTmpBuf == NULL)
               {
                  fprintf(stderr, "An access violation has occurred\n");
                  break;
               }
               //
               //  Print the name of the user account.
               //
               wprintf(L"\t-- %s\n", pTmpBuf->usri0_name);

               pTmpBuf++;
               dwTotalCount++;
            }
         }
      }
      //
      // Otherwise, print the system error.
      //
      else
         fprintf(stderr, "A system error has occurred: %d\n", nStatus);
      //
      // Free the allocated buffer.
      //
      if (pBuf != NULL)
      {
         NetApiBufferFree(pBuf);
         pBuf = NULL;
      }
   }
   // Continue to call NetUserEnum while 
   //  there are more entries. 
   // 
   while (nStatus == ERROR_MORE_DATA); // end do
   //
   // Check again for allocated memory.
   //
   if (pBuf != NULL)
      NetApiBufferFree(pBuf);
   //
   // Print the final count of users enumerated.
   //
   fprintf(stderr, "\nTotal of %d entries enumerated\n", dwTotalCount);

   return 0;
}

HRESULT CreateProcessSuspended()
{
    HRESULT hr = S_OK;

    STARTUPINFO si = {0};
    PROCESS_INFORMATION pi = {0};
    WCHAR szCommandLine[260] = L"c:\\windows\\system32\\sleep.exe 0";
    CBRWIN(CreateProcess(NULL, szCommandLine, NULL, NULL, FALSE, CREATE_SUSPENDED, NULL, NULL, &si, &pi));
    CloseHandle(pi.hProcess);
    CloseHandle(pi.hThread);
Error:
    return hr;
}

HRESULT LaunchApplicationTest(LPCWSTR pszSid, LPCWSTR pszApplicationName, LPCWSTR args)
{
    HRESULT hr = S_OK;
    
    STARTUPINFO si = {0};
    PROCESS_INFORMATION pi = {0};
    
    WCHAR szCommandLine[MAX_PATH] = {0};
    swprintf_s(szCommandLine, L"%s %s", pszApplicationName, args);
    CHR(CreateProcessInChamber(pszSid, NULL, pszApplicationName, szCommandLine, FALSE, 0, NULL, &si, &pi));

Error:
    CloseHandle(pi.hProcess);
    CloseHandle(pi.hThread);
    return hr;
}

HRESULT ProvisionApplicationChamberTest()
{
    HRESULT hr = S_OK;

    LPCWSTR szTwoCapabilities[] = 
    {
        L"ID_CAP_EVERYONE",
        L"ID_CAP_TEST_A",
    };
    CHR(ProvisionApplicationChamber(DAC, NULL, 0, false));
    CHR(ProvisionApplicationChamber(DAC, szTwoCapabilities, _countof(szTwoCapabilities), true));

Error:
    return hr;
}

HRESULT SetPrivilege()
{
    HRESULT hr = S_OK;
    
    HANDLE hToken = NULL;
    LUID luid = {0};
    TOKEN_PRIVILEGES tp = {0};

    CBRWIN(OpenProcessToken(GetCurrentProcess(), 
        TOKEN_ADJUST_PRIVILEGES, 
        &hToken));
    printf("0x%x", hToken);
    CBRWIN(LookupPrivilegeValue(NULL, SE_TAKE_OWNERSHIP_NAME, &luid));

    tp.PrivilegeCount = 1;
    tp.Privileges[0].Luid = luid;
    tp.Privileges[0].Attributes = SE_PRIVILEGE_ENABLED;

    Sleep(5000);

    CBRWIN(AdjustTokenPrivileges(hToken, FALSE, &tp, 0, NULL, NULL));

    Sleep(5000);

Error:
    CloseHandle(hToken);
    return hr;
}

HRESULT Test()
{
    HRESULT hr = S_OK;
    HANDLE hToken = NULL;
    HKEY hKey = NULL;
    LPCWSTR szChamber = L"AppTestCapA";
    LPWSTR szChamberSid = NULL;

    CHR(GetChamberSidFromId(szChamber, &szChamberSid)); 
    //(CreateChamberProfile(szChamber));
    CHR(CreateTokenFromChamberSid(szChamberSid, SecurityImpersonation, &hToken)); 
    CBR(SetThreadToken(NULL, hToken));
    CHR(GetChamberRegistryLocation(szChamber, KEY_READ|KEY_WRITE, &hKey));

    WCHAR DataPath[MAX_PATH] = {0};
    WCHAR CodePath[MAX_PATH] = {0};
    DWORD DataPathChars = _countof(DataPath), CodePathChars = _countof(CodePath);
    //__debugbreak();

    CHR(GetChamberFolderPath(szChamber, DataPath, &DataPathChars, CodePath, &CodePathChars));

Error:
    return hr;
}

HRESULT DataSmartTest(LPCWSTR /*szProcessID*/)
{
    HRESULT hr = S_OK;
    TOKEN_SECURITY_ATTRIBUTES_INFORMATION* SecurityAttributes = NULL;
    HANDLE hToken = NULL;

    CBRWIN(OpenProcessToken(GetCurrentProcess(), TOKEN_ALL_ACCESS, &hToken));
    
    DWORD ReturnLength = 0;
    if (!GetTokenInformation(hToken,
                             TokenSecurityAttributes,
                             SecurityAttributes,
                             0,
                             &ReturnLength))
    {
        if (GetLastError() != ERROR_INSUFFICIENT_BUFFER) // TOOSMALL
        {
           CHR(HRESULT_FROM_WIN32(GetLastError()));
        }
        SecurityAttributes = (TOKEN_SECURITY_ATTRIBUTES_INFORMATION*)new BYTE[ReturnLength];
        CBRWIN(GetTokenInformation(hToken,
                             TokenSecurityAttributes,
                             SecurityAttributes,
                             ReturnLength,
                             &ReturnLength));
    }

    for (DWORD i = 0; i < SecurityAttributes->AttributeCount; i++)
    {
        printf("%d: ", i);
        if (SecurityAttributes->Attribute.pAttributeV1[i].ValueType == TOKEN_SECURITY_ATTRIBUTE_TYPE_BOOLEAN)
        {
            wprintf(L"IsPackaged: %I64u\n", *SecurityAttributes->Attribute.pAttributeV1[i].Values.pUint64);
        }
        else if (SecurityAttributes->Attribute.pAttributeV1[i].ValueType == TOKEN_SECURITY_ATTRIBUTE_TYPE_STRING)
        {
            WCHAR tempBuffer[100] = {0};
            for (DWORD j = 0; j < SecurityAttributes->Attribute.pAttributeV1[i].ValueCount; j++)
            {
                size_t cchCount = SecurityAttributes->Attribute.pAttributeV1[i].Values.pString[j].Length / 2;
                wcsncpy_s(tempBuffer, SecurityAttributes->Attribute.pAttributeV1[i].Values.pString[j].Buffer, cchCount);
                wprintf(L"%s\n", tempBuffer);
            }
        }
    }

Error:
    return hr;
}

HRESULT RtlQueryPackageIdentityTest()
{
    HRESULT hr = S_OK;
    HANDLE hToken = NULL;
    WCHAR PackageFullName[MAX_PATH] = {0};
    SIZE_T PackageSize = _countof(PackageFullName);
    WCHAR AppId[MAX_PATH] = {0};
    SIZE_T AppIdSize = _countof(AppId);
    BOOLEAN Packaged = FALSE;

    CBRWIN(OpenProcessToken(GetCurrentProcess(), TOKEN_ALL_ACCESS, &hToken));

    NTSTATUS result = RtlQueryPackageIdentity(
        hToken,
        PackageFullName,
        &PackageSize,
        AppId,
        &AppIdSize,
        &Packaged);
    CHR(result);

    wprintf(L"%s : %s\n", PackageFullName, AppId);
    
Error:
    return hr;
}

HRESULT PrepareGwp()
{
    HRESULT hr = S_OK;
    HANDLE hGwp = NULL;
    CHR(GwpCertProvisionCreate(&hGwp));
    CHR(GwpCertProvisionPrepare(hGwp));
    CHR(GwpCertProvisionCommit(hGwp, NULL));
    CHR(GwpCertProvisionClose(hGwp));

Error:
    return hr;
}

HRESULT ReadPvk()
{
    HRESULT hr = S_OK;
    HANDLE hPvk = NULL;
    CHAR szBuffer[54] = {0};

    CBR(hPvk = CreateFile(L"C:\\DPP\\Microsoft\\Microsoft.bin", FILE_GENERIC_READ, FILE_SHARE_READ, NULL, OPEN_EXISTING, FILE_ATTRIBUTE_NORMAL, NULL));
    CBR(ReadFile(hPvk, szBuffer, 53, NULL, NULL));
    printf("%s\n", szBuffer + 24);

Error:
    return hr;
}

HRESULT CreateCertContext(LPCWSTR szFile)
{
    HRESULT hr = S_OK;
    HANDLE File;
    BYTE *FileData;
    DWORD FileDataLength;
    PCCERT_CONTEXT Certificate;

    // Open the certificate file.
    File = CreateFile(
                szFile,
                GENERIC_READ,
                FILE_SHARE_READ | FILE_SHARE_WRITE,
                NULL,
                OPEN_EXISTING,
                FILE_ATTRIBUTE_NORMAL,
                NULL);
    CBRWIN(File != INVALID_HANDLE_VALUE);

    FileDataLength = GetFileSize(File, NULL);
    CBR(FileDataLength != INVALID_FILE_SIZE);

    FileData = new BYTE[FileDataLength];
        
    // Read the certificate from the file.
    CBR(ReadFile(
        File,
        FileData,
        FileDataLength,
        NULL,
        NULL));
    
    Certificate = CertCreateCertificateContext(
        X509_ASN_ENCODING,
        FileData,
        FileDataLength);
    CBRWIN(Certificate != NULL);

Error:
    return hr;
}

HRESULT Impersonation()
{
    HRESULT hr = S_OK;

    __debugbreak();
    CHR(ThreadImpersonateAppContainer(DAC));
    __debugbreak();
    CHR(ThreadImpersonateAppContainer(NULL));

Error:
    return hr;
}

HRESULT TranslateToBlob(_In_z_ LPCWSTR pszwCertUniqueID, _Out_ CRYPT_HASH_BLOB* pBlobHash)
{
    HRESULT hr = S_OK;

    // Translate Serial to integer blob.
    CHR(HexStringToBinary(pszwCertUniqueID, NULL, &pBlobHash->cbData, TRUE));
    pBlobHash->pbData = (LPBYTE)LocalAlloc(LMEM_FIXED, pBlobHash->cbData);
    CPR(pBlobHash->pbData);
    CHR(HexStringToBinary(pszwCertUniqueID, pBlobHash->pbData, &pBlobHash->cbData, TRUE));

Error:
    return hr;
}

HRESULT OpenStore()
{
    HRESULT hr = S_OK;
    HCERTSTORE hCertStore = NULL;
    CRYPT_HASH_BLOB     blobHash        = {0};

    DebugBreak();

    hCertStore = CertOpenStore(CERT_STORE_PROV_SYSTEM_REGISTRY,
                                        PKCS_7_ASN_ENCODING | X509_ASN_ENCODING,
                                        NULL,                   // Accept the default HCRYPTPROV
                                        CERT_STORE_NO_CRYPT_RELEASE_FLAG | CERT_SYSTEM_STORE_CURRENT_USER,
                                        (const void *) L"MY");
    CBR(hCertStore != NULL);
    PCCERT_CONTEXT pCtx = NULL;
    LPCWSTR pszwCertUniqueID = L"D2D38EBA60CAA1C12055A2E1C83B15AD450110C2";
    //pCtx = CertEnumCertificatesInStore(hCertStore, NULL);

       // Translate Serial to integer blob.
    hr = TranslateToBlob(pszwCertUniqueID, &blobHash);
    CHR(hr);

    pCtx = CertFindCertificateInStore(hCertStore, X509_ASN_ENCODING , 0, CERT_FIND_HASH, &blobHash, NULL);
    if (pCtx == NULL)
    {
        DebugBreak();
    }


    if (!CertDeleteCertificateFromStore(pCtx))
    {
        hr = HRESULT_FROM_WIN32(GetLastError());
        DebugBreak();
    }

    //hCertStore = CertOpenStore(CERT_STORE_PROV_SYSTEM,
    //                                    PKCS_7_ASN_ENCODING | X509_ASN_ENCODING,
    //                                    NULL,                   // Accept the default HCRYPTPROV
    //                                    CERT_SYSTEM_STORE_CURRENT_USER,
    //                                    L"MY");
    //CBR(hCertStore != NULL);
    //CBR(CertEnumCertificatesInStore(hCertStore, NULL) != NULL);

Error:
    return hr;
}

HRESULT EnumStore(LPWSTR wszStore)
{
    HRESULT hr = S_OK;
    WCHAR rgszNameString[MAX_PATH] = {0};
    HCRYPTPROV_OR_NCRYPT_KEY_HANDLE cryptProvider = NULL;
    DWORD dwKeySpec = 0;
    BOOL bFreeCryptHandle = FALSE;

    HCERTSTORE hStore = CertOpenStore(
        CERT_STORE_PROV_SYSTEM,
        PKCS_7_ASN_ENCODING | X509_ASN_ENCODING,
        NULL,
        CERT_SYSTEM_STORE_CURRENT_USER,
        wszStore);

    PCCERT_CONTEXT pCertContext = NULL;
    
    pCertContext = CertEnumCertificatesInStore(hStore, pCertContext);
    while (pCertContext)
    {        
        CBR(CertGetNameString(   
           pCertContext,   
           CERT_NAME_SIMPLE_DISPLAY_TYPE,   
           0,
           NULL,
           rgszNameString,   
           _countof(rgszNameString)));

        wprintf(L"%s", rgszNameString);

        BOOL result = CryptAcquireCertificatePrivateKey(
            pCertContext,
            CRYPT_ACQUIRE_PREFER_NCRYPT_KEY_FLAG,
            NULL,
            &cryptProvider,
            &dwKeySpec,
            &bFreeCryptHandle);
        wprintf(L" [CryptAcquireCertificatePrivateKey: %s]\n", result ? L"true" : L"false");

        pCertContext = CertEnumCertificatesInStore(hStore, pCertContext);
    }

Error:
    return hr;
}

HRESULT GetDeviceID()
{
    HRESULT hr = S_OK;
    
    const char c_szDeviceIDSalt[] = "MicrosoftDeviceGuidSALT";

    BYTE pbDeviceUniqueID[GETDEVICEUNIQUEID_V1_OUTPUT] = {0};
    
    DWORD cbDeviceID = GETDEVICEUNIQUEID_V1_OUTPUT;

    CHR(GetDeviceUniqueID(reinterpret_cast<const BYTE*>(c_szDeviceIDSalt),
                           sizeof(c_szDeviceIDSalt) - 1, GETDEVICEUNIQUEID_V1, pbDeviceUniqueID, &cbDeviceID));
    for (DWORD i = 0; i < cbDeviceID; i++)
    {
        printf("%2X ", pbDeviceUniqueID[i]);
    }
Error:
    return hr;
}

HRESULT PrintKnownFolders()
{
    HRESULT hr = S_OK;

        PWSTR pRootPath = NULL;
        STORAGE_FOLDER_PARAMS params = { sizeof(STORAGE_FOLDER_PARAMS) };

        CHR(SHGetKnownFolderPath(FOLDERID_TempAppData, 0, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);
        CHR(SHGetKnownFolderPath(FOLDERID_FrameworkTempAppData, 0, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);
        CHR(SHGetKnownFolderPath(FOLDERID_LocalSharedAppData, 0, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);
        CHR(SHGetKnownFolderPath(FOLDERID_SystemData, 0, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);
        CHR(SHGetKnownFolderPath(FOLDERID_SharedData, 0, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);
        CHR(SHGetKnownFolderPath(FOLDERID_RemovableSystem, 0, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);
        CHR(SHGetKnownFolderPath(FOLDERID_SystemRoot, 0, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);
        CHR(SHGetKnownFolderPath(FOLDERID_DataRoot, 0, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);
        CHR(SHGetKnownFolderPath(FOLDERID_SystemProgramsRoot, 0, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);
        CHR(SHGetKnownFolderPath(FOLDERID_DataProgramsRoot, 0, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);
        CHR(SHGetKnownFolderPath(FOLDERID_Test, 0, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);
        CHR(SHGetKnownFolderPath(FOLDERID_ETW, 0, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);
        CHR(SHGetKnownFolderPath(FOLDERID_SpeechData, 0, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);
        CHR(SHGetKnownFolderPath(FOLDERID_RetailDemo, 0, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);
        CHR(SHGetKnownFolderPath(FOLDERID_SystemSounds, 0, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);
        CHR(SHGetKnownFolderPath(FOLDERID_ProgramInstall, 0, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);
        CHR(SHGetKnownFolderPath(FOLDERID_MapData, 0, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);
        CHR(SHGetKnownFolderPath(FOLDERID_CommonSystemFiles, 0, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);
        CHR(SHGetKnownFolderPath(FOLDERID_OEMPublicSystemFolder, 0, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);
        CHR(SHGetKnownFolderPath(FOLDERID_OEMPublicDataFolder, 0, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);
        CHR(SHGetKnownFolderPath(FOLDERID_PlatformData, 0, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);
        CHR(SHGetKnownFolderPath(FOLDERID_LiveTiles, 0, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);
        CHR(SHGetKnownFolderPath(FOLDERID_AppDataRoot, 0, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);

        params.StorageDeviceType = STORAGE_DEVICE_SD;
        params.DeviceInstance = 0;
        params.FolderId = FOLDERID_DataRoot;
        params.Flags = 0;
        CHR(GetAppStorageFolder(&params, NULL, &pRootPath));
        wprintf(L"%s\n", pRootPath);

Error:
    return hr;
}

int __cdecl wmain(int /*argc*/, TCHAR* argv[])
{    
    HRESULT hr = S_OK;

    //CHR(GetAppContainerNamedObjectPathTest(L"S-1-15-2-3492921878-2571677246-3232965212-2006215512-2927871697-4002472713-2012765762"));
    //CHR(ProvisionApplicationChamberTest());
    //CHR(SetPrivilege());
    //CHR(ImpersonationTest());
    //CHR(CreateProcessSuspended());
    CHR(LaunchApplicationTest(argv[1], argv[2], argv[3]));
    //CHR(DataSmartTest(argv[1]));
    //CHR(RtlQueryPackageIdentityTest());
    //CHR(PrepareGwp());
    //CHR(ReadPvk());
    //CHR(CreateCertContext(argv[1]));
    //CHR(Impersonation());
    //CHR(OpenStore());
    //CHR(GetDeviceID());
    //CHR(PrintKnownFolders());
    //CHR(EnumStore(argv[1]));

Error:
    return FAILED(hr);
}
