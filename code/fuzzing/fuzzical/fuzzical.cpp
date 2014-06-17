#include <objbase.h>
#define INITGUID
#include <initguid.h>
#include "atlbase.h"

#ifndef INCLUDE_VCARD_UTILS
#define INCLUDE_VCARD_UTILS
#pragma warning(push,3)
#include <vCardUtils.cpp>
#pragma warning(pop)
#include "Poom2QA.h"
#undef INCLUDE_VCARD_UTILS
#endif

#include "sharedfuzz.h"

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

/*
    Consume one file.
*/
HRESULT LoadOneFile(LPCTSTR szFilename)
{
    HRESULT hr = S_OK;
    
    ATL::CComPtr<IIcalParser> pParser;
    ATL::CComPtr<IStream> pStream;
    ATL::CComPtr<IDispatch> pDispatch;
    ATL::CComQIPtr<IContact> pContact;    
    ATL::CComPtr<IItem> pItem;
    vCardUtils vcu;

    hr = vcu.ImportCard(const_cast<LPWSTR>(szFilename), &pItem);
    CHR(hr);

Error:
    
    LOG(L"Parsing of ical entry: %s", SUCCEEDED(hr) ? L"PASS" : L"FAIL");

    return 0;
}

int WINAPI
WinMain(
    HINSTANCE   /*hInst*/,
    HINSTANCE   /*hInstPrev*/,
    LPTSTR      lpszCmdLine,
    int         /*nCmdShow*/ )
{
    HRESULT hr = E_FAIL;
    
    // Do any cleanup necessary to recover from prior crashes here
    
    // Do any setup necessary to begin new runs here
    
    hr = CoInitializeEx(0, COINIT_MULTITHREADED);
    if (FAILED(hr))
    {
        return 1;
    }

    // transfer execution into the shared fuzzer library
    hr = FuzzMain(lpszCmdLine);

    CoUninitialize();
    
    return hr;
}
