// 
// Copyright © 2010-2025, Sinclair Community College
// Licensed under the GNU General Public License, version 3.
// See the LICENSE file in the project root for full license information.  
//
// This file is part of Make Me Admin.
//
// Make Me Admin is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, version 3.
//
// Make Me Admin is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
//
// You should have received a copy of the GNU General Public License
// along with Make Me Admin. If not, see <http://www.gnu.org/licenses/>.
//

namespace LsaLogonSessions
{
    /// <summary>
    /// Contains values that indicate the type of session information to retrieve in a call to the
    /// WTSQuerySessionInformation() function.
    /// </summary>
    internal enum WTS_INFO_CLASS
    {
        /// <summary>
        /// A null-terminated string containing the name of the initial program that Remote Desktop Services runs when the user logs on.
        /// </summary>
        WTSInitialProgram = 0,

        /// <summary>
        /// A null-terminated string containing the published name of the application that the session is running.
        /// </summary>
        WTSApplicationName = 1,

        /// <summary>
        /// A null-terminated string containing the default directory used when launching the initial program.
        /// </summary>
        WTSWorkingDirectory = 2,

        /// <summary>
        /// This value is not used.
        /// </summary>
        WTSOEMId = 3,

        /// <summary>
        /// A ULONG value containing the session identifier.
        /// </summary>
        WTSSessionId = 4,

        /// <summary>
        /// A null-terminated string containing the name of the user associated with the session.
        /// </summary>
        WTSUserName = 5,

        /// <summary>
        /// A null-terminated string containing the name of the Remote Desktop Services session.
        /// </summary>
        /// <remarks>
        /// Despite its name, specifying this type does not return the window station name. Rather, it returns
        /// the name of the Remote Desktop Services session. Each Remote Desktop Services session is associated
        /// with an interactive window station. Currently, since the only supported window station name for an
        /// interactive window station is "WinSta0", each session is associated with its own "WinSta0" window
        /// station. For more information, see http://msdn.microsoft.com/en-us/library/ms687096(v=VS.85).aspx.
        /// </remarks>
        WTSWinStationName = 6,

        /// <summary>
        /// A null-terminated string containing the name of the domain to which the logged-on user belongs.
        /// </summary>
        WTSDomainName = 7,

        /// <summary>
        /// The session's current connection state. For more information, see WTS_CONNECTSTATE_CLASS on MSDN.
        /// </summary>
        WTSConnectState = 8,

        /// <summary>
        /// A ULONG value containing the build number of the client.
        /// </summary>
        WTSClientBuildNumber = 9,

        /// <summary>
        /// A null-terminated string containing the name of the client.
        /// </summary>
        WTSClientName = 10,

        /// <summary>
        /// A null-terminated string containing the directory in which the client is installed.
        /// </summary>
        WTSClientDirectory = 11,

        /// <summary>
        /// A USHORT client-specific product identifier.
        /// </summary>
        WTSClientProductId = 12,

        /// <summary>
        /// A ULONG value containing a client-specific hardware identifier.
        /// </summary>
        WTSClientHardwareId = 13,

        /// <summary>
        /// The network type and network address of the client.
        /// </summary>
        /// <remarks>
        /// The IP address is offset by two bytes from the start of the Address member of the WTS_CLIENT_ADDRESS structure.
        /// For more information, see WTS_CLIENT_ADDRESS on MSDN.
        /// </remarks>
        WTSClientAddress = 14,

        /// <summary>
        /// Information about the display resolution of the client. For more information, see WTS_CLIENT_DISPLAY on MSDN.
        /// </summary>
        WTSClientDisplay = 15,

        /// <summary>
        /// A USHORT value specifying information about the protocol type for the session.
        /// </summary>
        /// <remarks>
        /// 0 = The console session.
        /// 1 = This value is retained for legacy purposes.
        /// 2 = The RDP protocol.
        /// </remarks>
        WTSClientProtocolType = 16,

        /// <summary>
        /// This value returns FALSE. If you call GetLastError() to get extended error information, GetLastError returns ERROR_NOT_SUPPORTED.
        /// </summary>
        WTSIdleTime = 17,

        /// <summary>
        /// This value returns FALSE. If you call GetLastError() to get extended error information, GetLastError returns ERROR_NOT_SUPPORTED.
        /// </summary>
        WTSLogonTime = 18,

        /// <summary>
        /// This value returns FALSE. If you call GetLastError() to get extended error information, GetLastError returns ERROR_NOT_SUPPORTED.
        /// </summary>
        WTSIncomingBytes = 19,

        /// <summary>
        /// This value returns FALSE. If you call GetLastError() to get extended error information, GetLastError returns ERROR_NOT_SUPPORTED.
        /// </summary>
        WTSOutgoingBytes = 20,

        /// <summary>
        /// This value returns FALSE. If you call GetLastError() to get extended error information, GetLastError returns ERROR_NOT_SUPPORTED.
        /// </summary>
        WTSIncomingFrames = 21,

        /// <summary>
        /// This value returns FALSE. If you call GetLastError() to get extended error information, GetLastError returns ERROR_NOT_SUPPORTED.
        /// </summary>
        WTSOutgoingFrames = 22,

        /// <summary>
        /// Information about a Remote Desktop Connection (RDC) client. For more information, see WTSCLIENT on MSDN.
        /// </summary>
        /// <remarks>
        /// Windows Vista, Windows Server 2003, Windows XP, and Windows 2000: This value is not supported. This value is supported
        /// for Windows Server 2008 and Windows Vista with SP1.
        /// </remarks>
        WTSClientInfo = 23,

        /// <summary>
        /// Information about a client session on an RD Session Host server. For more information, see WTSINFO on MSDN.
        /// </summary>
        /// <remarks>
        /// Windows Vista, Windows Server 2003, Windows XP, and Windows 2000: This value is not supported. This value is supported
        /// for Windows Server 2008 and Windows Vista with SP1.
        /// </remarks>
        WTSSessionInfo = 24,

        /// <summary>
        /// Extended information about a session on an RD Session Host server. For more information, see WTSINFOEX on MSDN.
        /// </summary>
        /// <remarks>
        /// Windows Server 2008, Windows Vista, Windows Server 2003, Windows XP, and Windows 2000: This value is not supported.
        /// </remarks>
        WTSSessionInfoEx = 25,

        /// <summary>
        /// Information about the configuration of an RD Session Host server.
        /// </summary>
        /// <remarks>
        /// Windows Server 2008, Windows Vista, Windows Server 2003, Windows XP, and Windows 2000: This value is not supported.
        /// </remarks>
        WTSConfigInfo = 26,

        /// <summary>
        /// This value is not supported.
        /// </summary>
        WTSValidationInfo = 27,

        /// <summary>
        /// A WTS_SESSION_ADDRESS structure that contains the IPv4 address assigned to the session. If the session does not have
        /// a virtual IP address, the WTSQuerySessionInformation function returns ERROR_NOT_SUPPORTED.
        /// </summary>
        /// <remarks>
        /// Windows Server 2008, Windows Vista, Windows Server 2003, Windows XP, and Windows 2000: This value is not supported.
        /// </remarks>
        WTSSessionAddressV4 = 28,

        /// <summary>
        /// Information about whether the session is a remote session. The WTSQuerySessionInformation function returns a value
        /// of TRUE to indicate that the session is a remote session, and FALSE to indicate that the session is a local session.
        /// </summary>
        /// <remarks>
        /// Windows Server 2008, Windows Vista, Windows Server 2003, Windows XP, and Windows 2000: This value is not supported.
        /// </remarks>
        WTSIsRemoteSession = 29
    }
}
