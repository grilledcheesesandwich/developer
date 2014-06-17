// Test executable created solely for the purpose of loading credsvc.dll for testing purposes
// The following is the header comment for services.exe, probably still applies
//
// ServCmd.cpp - handles services.exe command line options to make 
// managing services easier.  
//
// NOTE this process MUST run at lowest possible security privelege on systems that
// enable trust.  A malicious application can call CreateProcess("services.exe","command...",).
// When the service received the call, it would check the trust of services.exe
// itself and NOT the process that initiated the Create Process.
//
// Actual services themselves are now hosted in servicesd.exe.  Services.exe is
// just the command line wrapper to avoid the trust issues, keeping name same
// to avoid BC issues.
//

#include <windows.h>
#include <winbase.h>

int wmain(int argc, WCHAR **argv)
{
	DEVMGR_DEVICE_INFORMATION pdi;

    // Load a udevice
	HANDLE h = FindFirstDevice(DeviceSearchByDeviceName, argv[1], &pdi);
	
    if (h == INVALID_HANDLE_VALUE) {
		printf("\nError loading device: [%u]", GetLastError());		
    }

    BOOL bRet = DeactivateDevice(pdi.hDevice);

    if (!bRet) {
		printf("\nError unloading device: [%u]", GetLastError());		
    }

    CloseHandle(h);

	return 0;
}
