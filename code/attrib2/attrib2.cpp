#include <windows.h>
#include <stdio.h>
#include <auto_xxx.hxx>
#include <ossvcs.h>

#ifndef _ehmOnFail
#define _ehmOnFail(eType, dwCode, pszFile, ulLine, pszMessage)    { wprintf(L" <> <> <> Check Failed: %s (0x%08x) %s:%d", pszMessage, dwCode, pszFile, ulLine); }
#endif
#include "ehm.h"

int wmain(int argc, WCHAR** argv)
{
    HRESULT hr = S_OK;
    LPTSTR szArg = NULL;

    if (argc < 2)
    {
        wprintf(L"USAGE: attrib2 <filename> [(+|-)r]");
        return 1;
    }
    
    LPWSTR szFilename = argv[1];
    DWORD dwAttribs = GetFileAttributes(szFilename);
    CBREx(dwAttribs != INVALID_FILE_ATTRIBUTES, HRESULT_FROM_WIN32(GetLastError()));

    if (argc < 3)
    {
        wprintf(L"\nFile Attribute Mask: 0x%08X", dwAttribs);

        if (dwAttribs & FILE_ATTRIBUTE_READONLY)              wprintf(L"\nFILE_ATTRIBUTE_READONLY");
        if (dwAttribs & FILE_ATTRIBUTE_HIDDEN)                wprintf(L"\nFILE_ATTRIBUTE_HIDDEN");
        if (dwAttribs & FILE_ATTRIBUTE_SYSTEM)                wprintf(L"\nFILE_ATTRIBUTE_SYSTEM");
        if (dwAttribs & FILE_ATTRIBUTE_DIRECTORY)             wprintf(L"\nFILE_ATTRIBUTE_DIRECTORY");
        if (dwAttribs & FILE_ATTRIBUTE_ARCHIVE)               wprintf(L"\nFILE_ATTRIBUTE_ARCHIVE");
        if (dwAttribs & FILE_ATTRIBUTE_INROM)                 wprintf(L"\nFILE_ATTRIBUTE_INROM");
        if (dwAttribs & FILE_ATTRIBUTE_NORMAL)                wprintf(L"\nFILE_ATTRIBUTE_NORMAL");
        if (dwAttribs & FILE_ATTRIBUTE_TEMPORARY)             wprintf(L"\nFILE_ATTRIBUTE_TEMPORARY");
        if (dwAttribs & FILE_ATTRIBUTE_SPARSE_FILE)           wprintf(L"\nFILE_ATTRIBUTE_SPARSE_FILE");
        if (dwAttribs & MODULE_ATTR_NOT_TRUSTED)              wprintf(L"\nMODULE_ATTR_NOT_TRUSTED");
        if (dwAttribs & FILE_ATTRIBUTE_REPARSE_POINT)         wprintf(L"\nFILE_ATTRIBUTE_REPARSE_POINT");
        if (dwAttribs & MODULE_ATTR_NODEBUG)                  wprintf(L"\nMODULE_ATTR_NODEBUG");
        if (dwAttribs & FILE_ATTRIBUTE_COMPRESSED)            wprintf(L"\nFILE_ATTRIBUTE_COMPRESSED");
        if (dwAttribs & FILE_ATTRIBUTE_OFFLINE)               wprintf(L"\nFILE_ATTRIBUTE_OFFLINE");
        if (dwAttribs & FILE_ATTRIBUTE_ROMSTATICREF)          wprintf(L"\nFILE_ATTRIBUTE_ROMSTATICREF");
        if (dwAttribs & FILE_ATTRIBUTE_NOT_CONTENT_INDEXED)   wprintf(L"\nFILE_ATTRIBUTE_NOT_CONTENT_INDEXED");
        if (dwAttribs & FILE_ATTRIBUTE_ROMMODULE)             wprintf(L"\nFILE_ATTRIBUTE_ROMMODULE");
        if (dwAttribs & FILE_ATTRIBUTE_ENCRYPTED)             wprintf(L"\nFILE_ATTRIBUTE_ENCRYPTED");
    }
    else
    {
        if (argv[2][0] == '-')
        {
            if (argv[2][1] == 'r')
            {
	            dwAttribs ^= FILE_ATTRIBUTE_READONLY;
                wprintf(L"Unsetting FILE_ATTRIBUTE_READONLY");
            }
        }
        else
        {
	        if (argv[2][1] == 'r')
            {
	            dwAttribs |= FILE_ATTRIBUTE_READONLY;
                wprintf(L"Setting FILE_ATTRIBUTE_READONLY");
            }
        }

        CBREx(SetFileAttributes(szFilename, dwAttribs), HRESULT_FROM_WIN32(GetLastError()));
    }

Error:
    return FAILED(hr);
}
