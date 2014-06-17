#include "stdafx.h"

// ----------------------------------------------------------------------------------------------------

//  Forward definitions:
void DoCoCreate();
void DoRelease();
void DoShutdown();

// ----------------------------------------------------------------------------------------------------

// Data formatters:

enum HEX{};
ostream& operator<<( ostream& os, HEX h )
{
    return os << "0x" << hex << (int)h << dec;
}

enum HR{};
ostream& operator<<( ostream& os, HR hr )
{
    return os << "0x" << hex << (int)hr << dec;
}

ostream& operator<<( ostream& os, LPCWSTR wsz )
{
    if (wsz == NULL) return os << "(null)";
    while (*wsz != L'\0') os << (char)*wsz++;
    return os;
}

ostream& operator<<( ostream& os, const GUID& guid )
{
    WCHAR wsz[40];
    StringFromGUID2( guid, wsz, 40 );
    return os << wsz;
}

struct LINE {
	template<int n>
	explicit LINE( char (&buf)[n] ) : m_pc( buf ), m_cb( n ) {}

	char* m_pc;
	int m_cb;
	};

istream& operator>>( istream& is, LINE line )
{
	is.getline( line.m_pc, line.m_cb - 3 );
	is.putback( '\n' );
	return is;
}

#pragma warning ( suppress : 4512 ) // assignment operator could not be generated
struct BTA { BTH_ADDR& m; explicit BTA(const BTH_ADDR &a) : m(const_cast<BTH_ADDR&>(a)) {} };
istream& operator>>( istream& is, BTA& btAddr )
{
    char sz[80];
    is >> sz;

    btAddr.m = 0;
    for( char* pc = sz; *pc != L'\0'; pc++) {
        if ('0' <= *pc && *pc <= '9')
            btAddr.m = (btAddr.m << 4) + (*pc - '0');
        else if ('A' <= *pc && *pc <= 'F')
            btAddr.m = (btAddr.m << 4) + (*pc - 'A' + 10);
        else if ('a' <= *pc && *pc <= 'f')
            btAddr.m = (btAddr.m << 4) + (*pc - 'a' + 10);
    }

    return is;
}
ostream& operator<<( ostream& os, const BTA& btAddr )
{
    char sz[18];
    for (int i = 0; i < 6; i++)
    {
        int by = (int)(btAddr.m >> (8 * (5 - i)));
        sz[3 * i + 0] = "0123456789ABCDEF"[(by >> 4) & 0x0F];
        sz[3 * i + 1] = "0123456789ABCDEF"[by & 0x0F];
        sz[3 * i + 2] = ':';
    }
    sz[17] = '\0';
    return os << sz;
}

ostream& operator<<( ostream& os, BLUETOOTH_RADIO_STATE eState )
{
    os << (int)eState << '(';
    switch (eState) {
    case BRS_UNKNOWN:       os << "BRS_UNKNOWN";        break;
    case BRS_NO_HARDWARE:   os << "BRS_NO_HARDWARE";    break;
    case BRS_DISABLED:      os << "BRS_DISABLED";       break;
    case BRS_ENABLING:      os << "BRS_ENABLING";       break;
    case BRS_ENABLED:       os << "BRS_ENABLED";        break;
    case BRS_DISABLING:     os << "BRS_DISABLING";      break;
    default:                os << "invalid";            break;
    }
    return os << ')';
}

ostream& operator<<( ostream& os, BLUETOOTH_DEVICE_STATE eState )
{
    os << (int)eState << '(';
    switch (eState) {
    case BDS_NOT_VISIBLE:   os << "BDS_NOT_VISIBLE";    break;
    case BDS_VISIBLE:       os << "BDS_VISIBLE";        break;
    case BDS_PAIRED:        os << "BDS_PAIRED";         break;
    case BDS_PAIRING:       os << "BDS_PAIRING";        break;
    case BDS_CONNECTING:    os << "BDS_CONNECTING";     break;
    case BDS_CONNECTED:     os << "BDS_CONNECTED";      break;
    case BDS_DISCONNECTING: os << "BDS_DISCONNECTING";  break;
    case BDS_UNPAIRING:     os << "BDS_UNPAIRING";      break;
    default:                os << "invalid";            break;
    }
    return os << ')';
}

ostream& operator<<( ostream& os, BLUETOOTH_CONNECTION_STATE eState )
{
    os << (int)eState << '(';
    switch (eState) {
    case BCS_DISCONNECTED:  os << "BCS_DISCONNECTED";   break;
    case BCS_CONNECTING:    os << "BCS_CONNECTING";     break;
    case BCS_CONNECTED:     os << "BCS_CONNECTED";      break;
    case BCS_DISCONNECTING: os << "BCS_DISCONNECTING";  break;
    default:                os << "invalid";            break;
    }
    return os << ')';
}

// ----------------------------------------------------------------------------------------------------

// istream manipulators
istream& __cdecl clear (istream& is) { is.clear(); return is; }
//istream& __cdecl eol (istream& is) { SkipRestOfLine (is); return is; }
//istream& __cdecl wsnl (istream& is) { SkipWhiteSpaceNoEol (is); return is; }

istream& operator>>( istream& is, GUID& guid )
{
    char sz[80];
    WCHAR wsz[80];

    is >> sz;
    for (int i = 0; i < 80; i++) wsz[i] = (WCHAR) sz[i];

    HRESULT hr = IIDFromString( wsz, &guid );
    if (hr != S_OK) {
        cout << "Bad guid, using " << guid << endl;
    }

    return is;
}

// ----------------------------------------------------------------------------------------------------

// Global variables
IBtRadioController* g_pBtRadioController = NULL;
//IBtConnectionObserver* g_pBtConnectionObserver = NULL;
//IBtConnectionResponder* g_pBtConnectionResponder = NULL;
//
//int g_cAudio = 0;
//CAudio* g_apAudio[100];
//IBtScoAudioDevice* g_apScoAudioDevice[100];
//
//int g_cCommand = 0;
//CCommand* g_apCommand[100];
//
//int g_cObserver = 0;
//CObserver* g_apObserver[100];
//
//int g_cIncoming = 0;
//CIncoming* g_apIncoming[100];
//
//int g_cPairer = 0;
//CPairer* g_apPairer[100];
//IBtPairingRequest* g_apPairingRequest[100];
//
//int g_cResponder = 0;
//CResponder* g_apResponder[100];
//
//int g_cAsync = 0;

// ----------------------------------------------------------------------------------------------------


void DoCoCreate()
{
    DoRelease();

    HRESULT hr = CoCreateInstance( __uuidof(BtConnectionManager), NULL, CLSCTX_ALL, __uuidof(IBtRadioController), (void**)&g_pBtRadioController );
    //if (hr == S_OK) hr = g_pBtRadioController->QueryInterface( __uuidof(IBtConnectionObserver), (void**)&g_pBtConnectionObserver );
    //if (hr == S_OK) hr = g_pBtRadioController->QueryInterface( __uuidof(IBtConnectionResponder), (void**)&g_pBtConnectionResponder );

    if (hr !=- S_OK) cout << "Error " << HR(hr) << endl;
}

// ----------------------------------------------------------------------------------------------------

void DoRelease()
{
    if (g_pBtRadioController != NULL) g_pBtRadioController->Release();
    //if (g_pBtConnectionObserver != NULL) g_pBtConnectionObserver->Release();
    //if (g_pBtConnectionResponder != NULL) g_pBtConnectionResponder->Release();

    g_pBtRadioController = NULL;
    //g_pBtConnectionObserver = NULL;
    //g_pBtConnectionResponder = NULL;
}

// ----------------------------------------------------------------------------------------------------

void DoShutdown()
{
    HRESULT hr = g_pBtRadioController->Shutdown();
    if (hr != S_OK) cout << "Error " << HR(hr) << endl;
}


HRESULT Connect(BTH_ADDR btAddr)
{
    HRESULT hr = S_OK;

    hr = CoInitializeEx( NULL, COINIT_MULTITHREADED );
    if (FAILED(hr)) printf("CoInitializeEx");

    WSADATA wsaData = {};
    if (0 != WSAStartup( 0x0202, &wsaData )) printf( "WSAStartup" );

    DoCoCreate();

    hr = g_pBtRadioController->ConnectDevice( btAddr );

    if (g_pBtRadioController != NULL)
    {
        DoShutdown();
        DoRelease();
    }

    return hr;
}