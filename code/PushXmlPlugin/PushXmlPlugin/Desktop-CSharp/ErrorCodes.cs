using System;

namespace PushXmlPlugin
{
    /// <summary>
    /// Most Common error codes returned by the Config Manager
    /// </summary>    
    public enum ErrorCodes : uint
    {
        /// <summary>
        /// S_OK = 0x00000000
        /// </summary>          
        S_OK = 0x00000000,
        /// <summary>
        /// E_PENDING = 0x8000000A
        /// </summary>          
        E_PENDING = 0x8000000A,
        /// <summary>
        /// E_INVALIDARG = 0x80070057
        /// </summary>         
        E_INVALIDARG = 0x80070057,
        /// <summary>
        /// ERROR_INVALID_DATA = 0x8007000D
        /// </summary>         
        ERROR_INVALID_DATA = 0x8007000D,
        /// <summary>
        /// E_OUTOFMEMORY = 0x8007000E
        /// </summary>         
        E_OUTOFMEMORY = 0x8007000E,
        /// <summary>
        /// E_UNEXPECTED = 0x8000FFFF
        /// </summary>         
        E_UNEXPECTED = 0x8000FFFF,
        /// <summary>
        /// E_NOTIMPL = 0x80004001
        /// </summary>         
        E_NOTIMPL = 0x80004001,
        /// <summary>
        /// E_ABORT = 0x80004004
        /// </summary>         
        E_ABORT = 0x80004004,
        /// <summary>
        /// E_FAIL = 0x80004005
        /// </summary>         
        E_FAIL = 0x80004005,
        /// <summary>
        /// E_ACCESSDENIED = 0x80070005
        /// </summary>         
        E_ACCESSDENIED = 0x80070005,
        /// <summary>
        /// ERROR_CANCELLED = 0x800704C7
        /// </summary>         
        ERROR_CANCELLED = 0x800704C7,        /// <summary>
        /// CO_E_CLASSSTRING = 0x800401F3
        /// </summary>         
        CO_E_CLASSSTRING = 0x800401F3,
        /// <summary>
        /// CONFIG_E_OBJECTBUSY = 0x80042001
        /// </summary> 
        CONFIG_E_OBJECTBUSY = 0x80042001,
        /// <summary>
        /// CONFIG_E_CANCELTIMEOUT  = 0x80042002
        /// </summary>         
        CONFIG_E_CANCELTIMEOUT = 0x80042002,
        /// <summary>
        /// CONFIG_E_ENTRYNOTFOUND  = 0x80042004
        /// </summary>                
        CONFIG_E_ENTRYNOTFOUND = 0x80042004,
        /// <summary>
        /// CONFIG_E_CSPEXCEPTION = 0x80042007
        /// </summary>         
        CONFIG_E_CSPEXCEPTION = 0x80042007,
        /// <summary>
        /// CONFIG_E_TRANSACTIONINGFAILURE = 0x80042008
        /// </summary>         
        CONFIG_E_TRANSACTIONINGFAILURE = 0x80042008,
        /// <summary>
        /// CONFIG_E_BAD_XML = 0x80042009
        /// </summary>         
        CONFIG_E_BAD_XML = 0x80042009,
        /// <summary>
        /// CONFIG_E_CABVERSIONINVALID = 0x80042011
        /// </summary>         
        CONFIG_E_CABVERSIONINVALID = 0x80042011,
        /// <summary>
        /// CONFIG_E_CABPLATFORMINVALID = 0x80042012
        /// </summary>         
        CONFIG_E_CABPLATFORMINVALID = 0x80042012,
        /// <summary>
        /// CONFIG_E_CABPROCESSORINVALID = 0x80042013
        /// </summary>         
        CONFIG_E_CABPROCESSORINVALID = 0x80042013,
        /// <summary>
        /// CONFIG_E_CABOSVERSIONINVALID =0x80042014
        /// </summary>         
        CONFIG_E_CABOSVERSIONINVALID = 0x80042014,
        /// <summary>
        /// CONFIG_E_NOUNINSTALLREQUIRED = 0x80042015
        /// </summary>         
        CONFIG_E_NOUNINSTALLREQUIRED = 0x80042015,
        /// <summary>
        /// CONFIG_E_CABPLATFORMNOTSUPPORTED = 0x80042016
        /// </summary>         
        CONFIG_E_CABPLATFORMNOTSUPPORTED = 0x80042016,
        /// <summary>
        /// CONFIG_S_METABASEQUERY = 0x00042003
        /// </summary>         
        CONFIG_S_METABASEQUERY = 0x00042003,
        /// <summary>
        /// CONFIG_S_PROCESSINGCANCELED = 0x00042005
        /// </summary>         
        CONFIG_S_PROCESSINGCANCELED = 0x00042005,
        /// <summary>
        /// CONFIG_S_NOTFOUND = 0x00042006
        /// </summary>         
        CONFIG_S_NOTFOUND = 0x00042006,
        /// <summary>
        /// CONFIG_S_REBOOTREQUIRED = 0x00042010
        /// </summary>         
        CONFIG_S_REBOOTREQUIRED = 0x00042010,
        /// <summary>
        /// ERROR_INTERNET_NAME_NOT_RESOLVED = 0x80072EE7
        /// </summary>         
        ERROR_INTERNET_NAME_NOT_RESOLVED = 0x80072EE7,
        /// <summary>
        /// ERROR_INTERNET_CANNOT_CONNECT = 0x80072efd
        /// </summary>         
        ERROR_INTERNET_CANNOT_CONNECT = 0x80072efd,
        /// <summary>
        /// CERTSRV_E_UNSUPPORTED_CERT_TYPE = 0x80094800
        /// </summary>         
        CERTSRV_E_UNSUPPORTED_CERT_TYPE = 0x80094800,
        /// <summary>
        /// ERROR_INTERNET_INVALID_CA = 0x80072f0d
        /// </summary>         
        ERROR_INTERNET_INVALID_CA = 0x80072f0d,

        /* Error codes from cfgmgr2err.h */

        /// <summary>
        /// The node options provided are invalid.
        /// </summary>
        CFGMGR_E_INVALIDNODEOPTIONS = 0x86000000,

        /// <summary>
        /// The node options provided are invalid.
        /// </summary>
        CFGMGR_E_INVALIDDATATYPE = 0x86000001,

        /// <summary>
        /// The specified node doesn't exist
        /// </summary>
        CFGMGR_E_NODENOTFOUND = 0x86000002,

        /// <summary>
        /// The operation is illegal inside of a transaction
        /// </summary>
        CFGMGR_E_ILLEGALOPERATIONINATRANSACTION = 0x86000003,

        /// <summary>
        /// The operation is illegal outside of a transaction
        /// </summary>
        CFGMGR_E_ILLEGALOPERATIONOUTSIDEATRANSACTION = 0x86000004,

        /// <summary>
        /// One or more commands failed to Execute
        /// </summary>
        CFGMGR_E_ONEORMOREEXECUTIONFAILURES = 0x86000005,

        /// <summary>
        /// One or more commands failed to revert during the cancel
        /// </summary>
        CFGMGR_E_ONEORMORECANCELFAILURES = 0x86000006,

        /// <summary>
        /// The command was executed, but the transaction failed so the command was rolled back successfully
        /// </summary>
        CFGMGR_S_COMMANDFAILEDDUETOTRANSACTIONROLLBACK = 0x06000007,

        /// <summary>
        /// The transaction failed during the commit phase
        /// </summary>
        CFGMGR_E_COMMITFAILURE = 0x86000008,

        /// <summary>
        /// The transaction failed during the rollback phase
        /// </summary>
        CFGMGR_E_ROLLBACKFAILURE = 0x86000009,

        /// <summary>
        /// One or more commands failed during the cleanup phase after the transactions were committed
        /// </summary>
        CFGMGR_E_ONEORMORECLEANUPFAILURES = 0x8600000A,

        /// <summary>
        /// The IConfigNodeState interface may not be used after the validation call
        /// </summary>
        CFGMGR_E_CONFIGNODESTATEOBJECTNOLONGERVALID = 0x8600000B,

        /// <summary>
        /// The CSP registration in the registry is corrupted
        /// </summary>
        CFGMGR_E_CSPREGISTRATIONCORRUPT = 0x8600000C,

        /// <summary>
        /// The cancel operation failed on the node
        /// </summary>
        CFGMGR_E_NODEFAILEDTOCANCEL = 0x8600000D,

        /// <summary>
        /// The operation failed on the node because of a prior operation failure
        /// </summary>
        CFGMGR_E_DEPENDENTOPERATIONFAILURE = 0x8600000E,

        /// <summary>
        /// The requested command failed because the node is in an invalid state
        /// </summary>
        CFGMGR_E_CSPNODEILLEGALSTATE = 0x8600000F,

        /// <summary>
        /// The node must be internally transactioned to call this command
        /// </summary>
        CFGMGR_E_REQUIRESINTERNALTRANSACTIONING = 0x86000010,

        /// <summary>
        /// The requested command is not allowed on the target
        /// </summary>
        CFGMGR_E_COMMANDNOTALLOWED = 0x86000011,

        /// <summary>
        /// Inter-CSP copy and move operations are illegal
        /// </summary>
        CFGMGR_E_INTERCSPOPERATION = 0x86000012,

        /// <summary>
        /// The requested property is not supported by the node
        /// </summary>
        CFGMGR_E_PROPERTYNOTSUPPORTED = 0x86000013,

        /// <summary>
        /// The semantic type is invalid
        /// </summary>
        CFGMGR_E_INVALIDSEMANTICTYPE = 0x86000014,

        /// <summary>
        /// The node has been invalidated
        /// </summary>
        CFGMGR_E_NODEINVALIDATED = 0x86000015,

        /// <summary>
        /// The URI contains a forbidden segment
        /// </summary>
        CFGMGR_E_FORBIDDENURISEGMENT = 0x86000015,

        /// <summary>
        /// The requested read/write permission was not allowed
        /// </summary>
        CFGMGR_E_READWRITEACCESSDENIED = 0x86000016,

        /// <summary>
        /// The CfgMgr1 process has terminated
        /// </summary>
        /// <remarks>
        /// This error occurs when using the DmProcessConfigXml API and 
        /// (un)trustmarshaller.exe ends unexpectedly.
        /// </remarks>
        CFGMGR_E_PROCESSTERMINATED = 0x8001000A,

        /// <summary>
        /// Node not found value for CfgMgr1 TestCsp
        /// </summary>
        TESTCSP_CONFIG_S_NOTFOUND = 0x40042006,

        /// <summary>
        /// Error code generated by CfgMgr1 TestCsp
        /// </summary>
        TESTCSP_ERROR_CODE = 0xD00420FF,

        /// <summary>CRYPT_E_NOT_FOUND = 0x80092004</summary>
        CRYPT_E_NOT_FOUND = 0x80092004,

        /// <summary>
        /// No more handles could be generated at this time. 
        /// </summary>
        ERROR_INTERNET_OUT_OF_HANDLES = 12001,

        /// <summary>
        /// The request has timed out.
        /// </summary>
        ERROR_INTERNET_TIMEOUT = 12002,

        /// <summary>
        /// An extended error was returned from the server. This is typically a string or buffer containing a verbose error message. Call InternetGetLastResponseInfo to retrieve the error text. 
        /// </summary>
        ERROR_INTERNET_EXTENDED_ERROR = 12003,

        /// <summary>
        /// An internal error has occurred. 
        /// </summary>
        ERROR_INTERNET_INTERNAL_ERROR = 12004,

        /// <summary>
        /// The URL is invalid.
        /// </summary>
        ERROR_INTERNET_INVALID_URL = 12005,

        /// <summary>
        /// The operation was canceled, usually because the handle on which the request was operating was closed before the operation completed.
        /// </summary>
        ERROR_INTERNET_OPERATION_CANCELED = 12017,

        /// <summary>
        /// The required operation could not be completed because one or more requests are pending.
        /// </summary>
        ERROR_INTERNET_REQUEST_PENDING = 12026,

        /// <summary>
        /// The connection with the server has been terminated. 
        /// </summary>
        ERROR_INTERNET_CONNECTION_ABORTED = 12030,

        /// <summary>
        /// The connection with the server has been reset. 
        /// </summary>
        ERROR_INTERNET_CONNECTION_RESET = 12031,

        /// <summary>
        /// The function needs to redo the request.
        /// </summary>
        ERROR_INTERNET_FORCE_RETRY = 12032,

        /// <summary>
        /// The server response could not be parsed.
        /// </summary>
        ERROR_HTTP_INVALID_SERVER_RESPONSE = 12152,

        /// <summary>
        /// The redirection failed because either the scheme changed (for example, HTTP to FTP) or all attempts made to redirect failed (default is five attempts).
        /// </summary>
        ERROR_HTTP_REDIRECT_FAILED = 12156,

        /// <summary>
        /// The Web site or server indicated is unreachable.
        /// </summary>
        ERROR_INTERNET_SERVER_UNREACHABLE = 12164,

        /// <summary>
        /// The Internet connection has been lost. 
        /// </summary>
        ERROR_INTERNET_DISCONNECTED = 12163,

        /// <summary>
        /// The connection has been established.
        /// </summary>
        CONNMGR_STATUS_CONNECTED = 0x10,

        /// <summary>
        /// The connection has been established, but has been suspended.
        /// </summary>
        CONNMGR_STATUS_SUSPENDED = 0x11,

        /// <summary>
        /// Connection is disconnected
        /// </summary>
        CONNMGR_STATUS_DISCONNECTED = 0x20,

        /// <summary>
        /// The connection has failed and cannot be re-established. This may be because the connection request was terminated when a more secure connection was established.
        /// </summary>
        CONNMGR_STATUS_CONNECTIONFAILED = 0x21,

        /// <summary>
        /// The user aborted the connection, or the connection request was terminated because a more secure connection was established.
        /// </summary>
        CONNMGR_STATUS_CONNECTIONCANCELED = 0x22,

        /// <summary>
        /// Connection is ready to connect but disabled
        /// </summary>
        CONNMGR_STATUS_CONNECTIONDISABLED = 0x23,

        /// <summary>
        /// No path could be found to destination
        /// </summary>
        CONNMGR_STATUS_NOPATHTODESTINATION = 0x24,

        /// <summary>
        /// Waiting for a path to the destination
        /// </summary>
        CONNMGR_STATUS_WAITINGFORPATH = 0x25,

        /// <summary>
        /// Voice call is in progress
        /// </summary>
        CONNMGR_STATUS_WAITINGFORPHONE = 0x26,

        /// <summary>
        /// Phone resource needed and phone is off
        /// </summary>
        CONNMGR_STATUS_PHONEOFF = 0x27,

        /// <summary>
        /// the connection could not be established because it would multi-home an exclusive connection
        /// </summary>
        CONNMGR_STATUS_EXCLUSIVECONFLICT = 0x28,

        /// <summary>
        /// Failed to allocate resources to make the connection.
        /// </summary>
        CONNMGR_STATUS_NORESOURCES = 0x29,

        /// <summary>
        /// Connection link disconnected prematurely.
        /// </summary>
        CONNMGR_STATUS_CONNECTIONLINKFAILED = 0x2A,

        /// <summary>
        /// Failed to authenticate user.
        /// </summary>
        CONNMGR_STATUS_AUTHENTICATIONFAILED = 0x2B,

        /// <summary>
        /// Attempting to connect
        /// </summary>
        CONNMGR_STATUS_WAITINGCONNECTION = 0x40,

        /// <summary>
        /// Resource is in use by another connection
        /// </summary>
        CONNMGR_STATUS_WAITINGFORRESOURCE = 0x41,

        /// <summary>
        /// Network is used by higher priority thread or device is roaming.
        /// </summary>
        CONNMGR_STATUS_WAITINGFORNETWORK = 0x42,

        /// <summary>
        /// Connection is being brought down
        /// </summary>
        CONNMGR_STATUS_WAITINGDISCONNECTION = 0x80,

        /// <summary>
        /// Aborting connection attempt
        /// </summary>
        CONNMGR_STATUS_WAITINGCONNECTIONABORT = 0x81,

        /// <summary>Failure in InvariantChangeType</summary>
        DISP_E_TYPEMISMATCH = 0x80020005,


        /* Error codes from certenroll.h */

        /// <summary>
        /// The http page requested could not be found
        /// </summary>
        CERTENROLL_E_HTTPNOTFOUND = 0x801F0194,


        /* Error codes from vfwmsgs.h */

        /// <summary>E_PROP_ID_UNSUPPORTED = 0x80070490</summary>
        E_PROP_ID_UNSUPPORTED = 0x80070490,
    }
}