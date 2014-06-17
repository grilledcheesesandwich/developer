// RapiConfig.cpp : Defines the entry point for the console application.
//

#include <stdio.h>
#include <windows.h>
#include <objbase.h>
#include <msxml2.h>
#include <assert.h>
#include <strsafe.h>
#include "rapi.h"

#define ARRAYSIZE(x) (sizeof(x)/sizeof(x[0]))

static void SearchSubPaths(wchar_t* path, wchar_t* searchString);

static int wmain(int argc, wchar_t* argv[])
{
    if (2 != argc)
    {
        printf("Searches for a file or files on a device using ActiveSync\n\n");
        printf("RAPIFIND query\n\n");
        printf("For example: Rapifind *.vol\n\n");
        printf("Created by Union Palenshus [unionp@microsoft.com]\n");
        return 1;
    }

    HRESULT hr = S_OK;

    hr = CeRapiInit();

    SearchSubPaths(L"", argv[1]);

    return 0;
}

static void SearchSubPaths(wchar_t* path, wchar_t* searchString)
{
    // Concatenate path and searchString
    WCHAR searchPathAndString[MAX_PATH];
    swprintf_s(searchPathAndString, MAX_PATH, L"%s\\%s", path, searchString);

    // Try and find file in path first, if found, print the path
    CE_FIND_DATA FindFileData;
    ZeroMemory(&FindFileData, sizeof(CE_FIND_DATA));

    if (HANDLE hFindFile = CeFindFirstFile(searchPathAndString, &FindFileData))
    {
        if (INVALID_HANDLE_VALUE != hFindFile)
        {
            do
            {
                wprintf(L"%s\\%s\n", path, FindFileData.cFileName);
            } while (CeFindNextFile(hFindFile, &FindFileData));

            //DWORD dwErr = CeGetLastError();
            //DWORD dwErrRapi = CeRapiGetError();
            
            CeFindClose(hFindFile);
        }
    }

    // Now get all sub-folders in current directory
    WCHAR searchPath[MAX_PATH];
    swprintf_s(searchPath, MAX_PATH, L"%s\\*", path);
    
    LPCE_FIND_DATA lpFindDataArray;
    DWORD dwFoundCount = 0;

    if(!CeFindAllFiles(searchPath, FAF_FOLDERS_ONLY | FAF_ATTRIB_CHILDREN | FAF_NAME | FAF_ATTRIBUTES, &dwFoundCount, &lpFindDataArray))
    {
        wprintf(L"*** CeFindAllFiles failed. ***\n");
        return;
    }
    
    if(!dwFoundCount)
        return;

    // Now recurse through all the subdirs looking for the file
    for(UINT i = 0; i < dwFoundCount; i++)
    {
        if(lpFindDataArray[i].dwFileAttributes & FILE_ATTRIBUTE_DIRECTORY)
        {
            swprintf_s(searchPath, MAX_PATH, L"%s\\%s", path, lpFindDataArray[i].cFileName);

            SearchSubPaths(searchPath, searchString);
        }
    }

    if (lpFindDataArray)
    {
        RapiFreeBuffer(lpFindDataArray);
    }
}