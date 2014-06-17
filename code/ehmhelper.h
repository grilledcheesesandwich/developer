//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//
//
// Use of this source code is subject to the terms of the Microsoft
// premium shared source license agreement under which you licensed
// this source code. If you did not accept the terms of the license
// agreement, you are not authorized to use this source code.
// For the terms of the license, please see the license agreement
// signed by you and Microsoft.
// THE SOURCE CODE IS PROVIDED "AS IS", WITH NO WARRANTIES OR INDEMNITIES.
//
// Custom _ehmOnFail macro, uses wprintf and outputs __FUNCTION__
#ifndef _ehmOnFail
#define _ehmOnFail(eType, dwCode, pszFile, ulLine, pszMessage)    { wprintf(TEXT(" <> <> <> %S: %s FAILED (0x%08x) %s:%d\n"), __FUNCTION__, pszMessage, dwCode, pszFile, ulLine); }
#endif

#include <ehm.h>

// CBR with a string passed to wprintf() on fail
#define CBRL(fResult, printf_exp)               \
{                                               \
    if (!CHECK_CBR_TYPE(fResult))               \
    {                                           \
        _ehmOnFail(eBOOL, E_FAIL, TEXT(__FILE__), __LINE__, TEXT("CBRL(") TEXT( # fResult ) TEXT(")")); \
        wprintf printf_exp;                  \
        hr = E_FAIL;                            \
        goto Error;                             \
    }                                           \
}

// CHR with a string passed to wprintf() on fail
#define CHRL(hResult, printf_exp)   \
{                                   \
    hr = CHECK_CHR_TYPE(hResult);   \
    if (FAILED(hr))                 \
    {                               \
        _ehmOnFail(eHRESULT, hr, TEXT(__FILE__), __LINE__, TEXT("CHRL(") TEXT( # hResult ) TEXT(")")); \
        wprintf printf_exp;      \
        goto Error;                 \
    }                               \
}

// CBR that automatically calls GetLastError
#define CBRWIN(x) CBREx(x, HRESULT_FROM_WIN32(GetLastError()))

// Checks a windows result for ERROR_SUCCESS
#define CWIN(x) CBREx(ERROR_SUCCESS == x, HRESULT_FROM_WIN32(x))

// CBRL that automatically calls GetLastError
#define CBRLWIN(fResult, printf_exp)                \
{                                                   \
    if (!CHECK_CBR_TYPE(fResult))                   \
    {                                               \
        hr = HRESULT_FROM_WIN32(GetLastError());    \
        _ehmOnFail(eBOOL, hr, TEXT(__FILE__), __LINE__, TEXT("CBRLWIN(") TEXT( # fResult ) TEXT(")")); \
        wprintf printf_exp;                      \
        goto Error;                                 \
    }                                               \
}