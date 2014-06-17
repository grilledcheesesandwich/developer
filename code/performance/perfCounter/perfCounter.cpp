#pragma warning(push)
#pragma warning(disable:4115)     // C4115: named type definition in parentheses
#pragma warning(disable:4201)     // C4201: nonstandard extension used : nameless struct/union
#pragma warning(disable:4204)     // C4204: nonstandard extension used : non-constant aggregate initializer
#pragma warning(disable:4214)     // C4214: nonstandard extension used : bit field types other than int
#include <windows.h>
#include <stdio.h>
#include <lvmod_pinvokes.cpp>

#pragma warning(pop)

#include "ossvcs.h"
#define _ehmOnFail LOGEHM
#include "ehm.h"

// QueryPerformanceCounter overhead for n reps
LONGLONG GetPerfOverhead(const DWORD reps)
{
    LARGE_INTEGER l1 = {0};
    LARGE_INTEGER l2 = {0};

    QueryPerformanceCounter(&l1);
	for (DWORD i = 0; i < reps; i++)
	{
		QueryPerformanceCounter(&l2);
	}
	return l2.QuadPart - l1.QuadPart;
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
    ce::auto_handle hTokenToLoad;
    ce::auto_hfile  hFile;
    ce::auto_handle hAuth(NULL);

    LARGE_INTEGER start;
    LARGE_INTEGER end;
    BYTE rgbHash[20] = {0};
    
    const DWORD REPS = 30000;
    LONGLONG overhead = 0;//GetPerfOverhead(REPS);
    
    LARGE_INTEGER frequency;
    QueryPerformanceFrequency(&frequency);

    printf("Overhead: %u", (DWORD)overhead);

    // convert our SID into a token
    //hTokenToLoad = CeCreateTokenFromAccount(L"S-1-5-112-545");
    hTokenToLoad = CeCreateTokenFromAccount(L"S-1-5-112-0-0X20");
    LV_AUTHORIZATION lvAuthz = LV_AUTHORIZATION_DENIED;
    const GUID *guidAuthClass;
    
    //// Open file
    //DWORD dwAttributes = GetFileAttributes(lpszCmdLine);
    //if (dwAttributes != 0xFFFFFFFF && dwAttributes & FILE_ATTRIBUTE_ROMMODULE)
    //{
    //    guidAuthClass = &LV_AUTHENTICATIONGUID_ROMMODULE;
    //}
    //else // file is not ROMMODULE
    //{
    //    guidAuthClass = &LV_AUTHENTICATIONGUID_PORTABLEEXECUTABLE;

    //    hFile = CreateFile(lpszCmdLine,
    //                       GENERIC_READ, 
    //                       FILE_SHARE_READ, 
    //                       NULL, 
    //                       OPEN_EXISTING, 
    //                       FILE_ATTRIBUTE_NORMAL, 
    //                       NULL);
    //    CBREx(hFile.valid(), HRESULT_FROM_WIN32(GetLastError()));
    //}
    
    double delta;
    double total = 0.0;

    // Do one authz first, this one should take longer because we haven't cached any results
    for (int i(0); i < 1; i++)
    {
        CHR(LoaderVerifierAuthenticateFile_Wrapper(lpszCmdLine, &hAuth));

        // Start timing
        QueryPerformanceCounter(&start);
        
        hr = LoaderVerifierAuthorize(
            hAuth,              //__in HLVMODAUTHENTICATIONDATA hlvauthnFile,
            GetCurrentToken(),  //__in HANDLE hTokenCaller,
            hTokenToLoad,       //__in HANDLE hTokenToLoad,
            &lvAuthz            //__out enum LV_AUTHORIZATION* plvauthz
        );
        CHR(hr);

        QueryPerformanceCounter(&end);

        delta = ((double)(end.QuadPart - start.QuadPart - overhead)) / frequency.QuadPart * 1000;
        total += delta;
        
        //printf("No cache: %f", delta);
        if (i % 200 == 0) printf("No cache: %4d: %f", i, delta);
    }

    // See if the Authorize was actually succeeding
    CBR(LV_AUTHORIZATION_DENIED != lvAuthz);

    printf("Total time for %u reps: %f", REPS, total);
    printf("Average time per rep  : %f", total / REPS);

    delta = total = 0.0;

    for (int i(0); i < REPS; i++)
    {
        // Start timing
        QueryPerformanceCounter(&start);

        //// Authenticate file
        //hr = LoaderVerifierAuthenticateFile(
        //    guidAuthClass,                  //__in const GUID*        guidAuthClass,
        //    hFile.valid() ? hFile : NULL,   //__in_opt HANDLE         hFile,
        //    lpszCmdLine,                    //__in LPCWSTR            szFilePath,
        //    NULL,                           //__in LPCWSTR            szHashHint,
        //    NULL,                           //__in_opt HANDLE         hauthnCatalog,
        //    &hAuth                          //__out LPHANDLE          phslauthnFile
        //);
        //CHR(hr);

        // Finally, authorize the authn\routing info as if we were launching it
        hr = LoaderVerifierAuthorize(
            hAuth,              //__in HLVMODAUTHENTICATIONDATA hlvauthnFile,
            GetCurrentToken(),  //__in HANDLE hTokenCaller,
            hTokenToLoad,       //__in HANDLE hTokenToLoad,
            &lvAuthz            //__out enum LV_AUTHORIZATION* plvauthz
        );

        //CHR(LoaderVerifierGetHash(hAuth, NULL, rgbHash, sizeof(rgbHash), NULL, NULL));

        // End timing
        QueryPerformanceCounter(&end);

        CHR(hr);

        delta = ((double)(end.QuadPart - start.QuadPart - overhead)) / frequency.QuadPart * 1000;
        total += delta;
        
        //printf("%f", delta);
        if (i % 200 == 0) printf("%4d: %f", i, delta);
    }

    // See if the Authorize was actually succeeding
    CBR(LV_AUTHORIZATION_DENIED != lvAuthz);

    printf("Total time for %u reps: %f", REPS, total);
    printf("Average time per rep  : %f", total / REPS);

Error:
    return SUCCEEDED(hr);
}