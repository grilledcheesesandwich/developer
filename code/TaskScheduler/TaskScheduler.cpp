#include <windows.h>
#include <stdio.h>
#include <strsafe.h>
#include <TaskSchedulerAPI.h>
#include <wpnfnames.h>
#include <ehmhelper.h>

int __cdecl wmain(int argc, __in_ecount(argc) WCHAR* argv[])
{
    /*HANDLE hBinding = NULL;
    HRESULT hr = TaskSchedulerInit(&hBinding);
    if (FAILED(hr)) {
        wprintf(L"ERROR: BindToRPCServer failed with hr=0x%8.8lx", hr);
        return 1;
    }
    return 0;*/

    if (argc != 2)
    {
        printf("USAGE: TaskScheduler.exe <binpath>");
        return 1;
    }

    HRESULT hr = S_OK;
	HANDLE hBinding = NULL;
   
	DWORD MaxRunCount = (DWORD)-1;
	DWORD dataValue = 1;
    DWORD Flags = TASK_FLAGS_PERSIST;

	SYSTEMTIME stTime = {0};

    GUID ScheduleGuid = 
		{ 0x4836606, 0xd82d, 0x4133, { 0xbc, 0xfb, 0x84, 0x3b, 0x9b, 0xf0, 0x83, 0x43 } };

    WCHAR* ScheduleName = L"ActionsSchedule";

    TASK_ACTION_LAUNCH launch = { argv[1], NULL };
    TASK_ACTION action = {TASK_ACTION_TYPE_LAUNCH, &launch};

	TASK_TRIGGER_WNFSTATE wnf = {WNF_OS_CRITICAL_BOOT_SERVICES_READY, TASK_STATE_EQUAL, sizeof(dataValue),(BYTE*)&dataValue, 0, 0};
    TASK_TRIGGER wnfstatetrigger = {TASK_TRIGGER_TYPE_WNFSTATE, reinterpret_cast<PTASK_TRIGGER_PERIODIC>(&wnf)};
    TASK_SCHEDULE schedule = {&ScheduleGuid, Flags, ScheduleName, stTime, stTime, MaxRunCount, &wnfstatetrigger, &action};

    CHR(TaskSchedulerInit(&hBinding));
    CHR(TaskSchedulerCreateSchedule(hBinding, &schedule, NULL));

Error:
    TaskSchedulerDeinit(&hBinding);
    hBinding = NULL;

    return hr != S_OK;
}

// Required MIDL memory allocation function
EXTERN_C PVOID MIDL_user_allocate(size_t BufferSize)
{
    return LocalAlloc(LPTR, BufferSize);
}

// Required MIDL memory deallocation function
EXTERN_C VOID MIDL_user_free(_In_ PVOID Buffer)
{
    LocalFree(Buffer);
}