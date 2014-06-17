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
// 
//

#include "UTTest.h"
#include <testsid.h>
#include <hash.hxx>

//
// The Unit Tests
//

/// <summary>
///     Test
/// </summary>
/// <returns>
///     A standard HRESULT to indicate pass/fail.
/// </returns>
HRESULT Test(void)
{
    HRESULT hr = S_OK;

    UTLogMessage(L"Running Test");
    
    ////LVModProvisionSecurityForApplication(NULL, NULL, NULL, 0, NULL, true);
    //DWORD dwAccessGranted;

    //DWORD start = GetTickCount();
    //CePolicyGetAccessGranted_Wrapper(L"/RESOURCES/GLOBAL/CAPABILITYTEST/READRESOURCE", SID_TEST_DYNAMICLPC, &dwAccessGranted);

    //printf("%d", GetTickCount() - start);

    WCHAR szSID[] = L"Union"; 
    LPCWSTR pszCaps[] = {  L"CAP_TESTPUBLIC1",
                           L"CAP_TESTPUBLIC2",
                           L"CAP_TESTPRIVATE1",
                           L"CAP_TESTPRIVATE2" };

    ce::wstring szSid(szSID);
    ce::vector<ce::wstring> strCaps;
    for (DWORD i = 0; i < _countof(pszCaps); i++)
    {
        strCaps.push_back(ce::wstring(pszCaps[i]));
    }

    wprintf(szSid);
    for (DWORD i = 0; i < strCaps.size();  i++)
    {
        wprintf(strCaps[i]);
    }

    return hr;
}




/// <summary>
///     This function is called one time at DLL initialization before any
///     other tests in this DLL are run.
/// </summary>
/// <returns>
///     A standard HRESULT to indicate pass/fail.
/// </returns>
HRESULT SuiteSetup(void)
{
    HRESULT hr = S_OK;
    
    return hr;
}

/// <summary>
///     This function is called one time after all tests in the DLL are run.
/// </summary>
/// <returns>
///     A standard HRESULT to indicate pass/fail.
/// </returns>
HRESULT SuiteTeardown(void)
{
    HRESULT hr = S_OK;
    
    return hr;
}

/// <summary>
///     This function parses the -C tux command line.  The full command line
///     is passed to this function.
/// </summary>
/// <param name="argc">An integer that contains the count of arguments that follow in argv.</param>
/// <param name="argv">An array of null-terminated strings representing command-line arguments</param>
/// <returns>
///     void
/// </returns>
void ParseCommandLine(int argc, WCHAR* argv[])
{
    // TODO: Parse any command line options here
}


//
// Add an entry to this structure to add a new unit test.  It is mandatory to include
// a test name and test function.  You can optionally add a space delimited list of
// groups the test will run in, the number of threads to concurrently run the test,
// and a DWORD parameter that the test can access via GetCurrentTestParam().
// 
static UnitTest s_UnitTests [] =
{
    // {L"TestName", TestFunction, L"GroupName GroupName2", NumThreads, Param},
    {L"Test", Test}
    
};
static const UINT s_cUnitTests = _countof(s_UnitTests);

//
// To use groups in your unit tests, you can add group setup/teardown and test
// setup/teardown functions on a per group basis.  To support groups uncomment the
// following block of code and remove the stub variables below that.
//
// static GroupFunction s_Groups [] = 
// {
//    // {L"GroupName", GroupFunction, FunctionType}
//        
// }; 
// static const UINT s_cGroupFunctions = _countof(s_Groups);
static GroupFunction* s_Groups = NULL;
static const UINT s_cGroupFunctions = 0;

//
// This variable sets the TUX index that the unit tests defined above will be
// based from.  This is useful if you have multiple files in your unit test project 
// and want to have deterministic test identifiers through TUX's "-x" parameter.
//
static const UINT s_TestIndexBase = 1;

//
// This statement will build up the list of unit test and group functions.
// Note: There should be no need to modify this!
//
static BuiltUTTable s_UTTable(s_UnitTests, s_cUnitTests, s_Groups, s_cGroupFunctions, __FILE__, s_TestIndexBase);

