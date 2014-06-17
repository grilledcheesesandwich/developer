#include <unknwn.h>
#include <windbase_edb.h>
#include "PMCommon.h"

#pragma warning(disable:4005) // warning C4005: macro redefinition
#define _ehmOnFail LOGEHM
#pragma warning(default:4005)
#include "ehm.h"

#include <atlbase.h>
#include <xmllite.h>

#include <shlwapi.h>
#include <miscsvcs.h>
#include <miscsvcs_priv.h>
#include <ZipContainerApi.h>

// Security related headers.
#include <loaderverifier.h>
#include <sid.h>
#include <cepolicy.h>
#include <cepriority.h>
#include <secpol.h>
#include <peHlpLib.h>
#include <DeveloperUnlock.h>
#include "IEdbEnumFilter.h"

#include "ProgressListener.h"
#include "XapInfo.h"
#include "Activity.h"

#include "XDRMApp.h"
#include "LicenseChecker.h"

#include "PackageExtractor.h"
#include "ManifestElement.h"
#include "ManifestBase.h"
#include "ManifestParser.h"
#include "ActivityUtils.h"
#include "FolderManager.h"
#include "DatabaseInserter.h"
#include "SimplePolicyRuleLoader.h"
#include "SecurityManager.h"
#include "DefaultActivityFactory.h"
#include "LicenseActivities.h"

// Database access
#include <TableDescription.h>
#include <EdbTable.h>
#include <TableAccess.h>
#include <Database.h>

// This is called whenever an EHM macro detects a failure
static void LOGEHM(eCodeType /*eType*/, DWORD dwCode, LPCTSTR pszFile, DWORD ulLine, LPCTSTR szMessage)
{
    wprintf(L"FAILURE %s:%d - %s  hr=0x%08x", pszFile, ulLine, szMessage, dwCode);
}

int wmain(int argc, WCHAR** argv)
{
    HRESULT hr = S_OK;
    LARGE_INTEGER start;
    LARGE_INTEGER end;
    ce::auto_ptr<XapInfo> pXapInfo;

    LARGE_INTEGER frequency;
    QueryPerformanceFrequency(&frequency);

    pXapInfo = new XapInfo();
    CPR(pXapInfo);

    wprintf(L"Opening %s", argv[1]);
    hr = pXapInfo->Open(argv[1]);
    CHR(hr);

    wprintf(L"Extracting %s", argv[1]);
    QueryPerformanceCounter(&start);
    hr = pXapInfo->ExtractXap(argv[2], NULL);
    CHR(hr);
    QueryPerformanceCounter(&end);
    double total = ((double)(end.QuadPart - start.QuadPart)) / frequency.QuadPart * 1000;
    
    wprintf(L"Extracted %s in %f", argv[1], total);

    hr = pXapInfo->Close();
    CHR(hr);

Error:
    return FAILED(hr);
}
