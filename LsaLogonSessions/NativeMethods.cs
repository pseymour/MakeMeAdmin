// 
// Copyright © 2010-2019, Sinclair Community College
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
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Win32 P/Invoke stuff.
    /// </summary>
    internal class NativeMethods
    {
        /// <summary>
        /// A Remote Desktop Session Host server value for the computer on which the application is running.
        /// </summary>
        internal static IntPtr WTS_CURRENT_SERVER_HANDLE = IntPtr.Zero;

        /*
        [DllImport("wtsapi32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        internal static extern IntPtr WTSOpenServer([MarshalAs(UnmanagedType.LPWStr)] string serverName);
        */

        /// <summary>
        /// Retrieves a list of sessions on a specified Remote Desktop Session Host (RD Session Host) server.
        /// </summary>
        /// <param name="serverHandle">
        /// A handle to an RD Session Host server.
        /// </param>
        /// <param name="reserved">
        /// This parameter is reserved and must have a value of zero (0).
        /// </param>
        /// <param name="version">
        /// The version of the enumeration request. This parameter must be one (1).
        /// </param>
        /// <param name="sessionInfoPointer">
        /// A pointer to a variable that receives a pointer to an array of WTS_SESSION_INFO structures.
        /// </param>
        /// <param name="count">
        /// The number of WTS_SESSION_INFO structures returned in the buffer pointed to by sessionInfoPointer.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is a non-zero value. Otherwise, the return value is zero.
        /// </returns>
        /// <remarks>
        /// For the server handle parameter, specify a handle opened by the WTSOpenServer or WTSOpenServerEx
        /// function, or specify WTS_CURRENT_SERVER_HANDLE to indicate the RD Session Host server on which
        /// your application is running.
        /// </remarks>
        [DllImport("wtsapi32.dll", SetLastError = true)]
        internal static extern int WTSEnumerateSessions(System.IntPtr serverHandle, [MarshalAs(UnmanagedType.U4)] int reserved, [MarshalAs(UnmanagedType.U4)] int version, ref IntPtr sessionInfoPointer, [MarshalAs(UnmanagedType.U4)] ref int count);

        /*
        /// <summary>
        /// Retrieves session information for the specified session on the specified Remote Desktop Session
        /// Host (RD Session Host) server. It can be used to query session information on local and remote
        /// RD Session Host servers.
        /// </summary>
        /// <param name="serverHandle">
        /// A handle to an RD Session Host server.
        /// </param>
        /// <param name="sessionId">
        /// A Remote Desktop Services session identifier.
        /// </param>
        /// <param name="wtsInfoClass">
        /// A value of the WTS_INFO_CLASS enumeration that indicates the type of session information to retrieve.
        /// </param>
        /// <param name="ppBuffer">
        /// A pointer to a variable that receives a pointer to the requested information. The format and contents
        /// of the data depend on the information class specified in the WTSInfoClass parameter.
        /// </param>
        /// <param name="pBytesReturned">
        /// The size, in bytes, of the data returned in ppBuffer.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is a non-zero value. Otherwise, the return value is zero.
        /// </returns>
        [DllImport("wtsapi32.dll", SetLastError = true)]
        internal static extern int WTSQuerySessionInformation(IntPtr serverHandle, int sessionId, WTS_INFO_CLASS wtsInfoClass, out IntPtr ppBuffer, out int pBytesReturned);
        */

        /// <summary>
        /// Obtains the primary access token of the logged-on user specified by the session ID.
        /// </summary>
        /// <remarks>
        ///  To call this function successfully, the calling application must be running within the context of the
        ///  LocalSystem account and have the SE_TCB_NAME privilege.
        /// </remarks>
        /// <param name="sessionId">
        /// A Remote Desktop Services session identifier. Any program running in the context of a service
        /// will have a session identifier of zero (0). You can use the WTSEnumerateSessions function
        /// to retrieve the identifiers of all sessions on a specified RD Session Host server.
        /// To be able to query information for another user's session, you need to have the Query Information
        /// permission. For more information, see Remote Desktop Services Permissions. To modify permissions
        /// on a session, use the Remote Desktop Services Configuration administrative tool.
        /// /// </param>
        /// <param name="phToken">
        /// If the function succeeds, receives a pointer to the token handle for the logged-on user.
        /// Note that you must call the CloseHandle function to close this handle.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is a nonzero value, and the phToken parameter
        /// points to the primary token of the user.
        /// If the function fails, the return value is zero.To get extended error information, call
        /// GetLastError.
        /// </returns>
        [DllImport("wtsapi32.dll", SetLastError = true)]
        internal static extern int WTSQueryUserToken([MarshalAs (UnmanagedType.U4)] [In] int sessionId, [Out] out IntPtr phToken);

        /// <summary>
        /// Frees memory allocated by a Remote Desktop Services function.
        /// </summary>
        /// <param name="memory">
        /// Pointer to the memory to free.
        /// </param>
        [DllImport("wtsapi32.dll", ExactSpelling = true, SetLastError = false)]
        internal static extern void WTSFreeMemory(IntPtr memory);

        /*
        [DllImport("wtsapi32.dll", SetLastError = true)]
        internal static extern void WTSCloseServer(IntPtr hServer);
        */

        /// <summary>
        /// Closes an open object handle.
        /// </summary>
        /// <param name="hObject">
        /// A valid handle to an open object.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// If the application is running under a debugger, the function will throw an exception if it receives
        /// either a handle value that is not valid or a pseudo-handle value. This can happen if you close a handle
        /// twice, or if you call CloseHandle on a handle returned by the FindFirstFile function instead of calling
        /// the FindClose function.
        /// </returns>
        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern bool CloseHandle(IntPtr hObject);

        /// <summary>
        /// Retrieves a specified type of information about an access token.
        /// </summary>
        /// <param name="TokenHandle">
        /// A handle to an access token from which information is retrieved. If TokenInformationClass
        /// specifies TokenSource, the handle must have TOKEN_QUERY_SOURCE access. For all other
        /// TokenInformationClass values, the handle must have TOKEN_QUERY access.
        /// </param>
        /// <param name="TokenInformationClass">
        /// Specifies a value from the TOKEN_INFORMATION_CLASS enumerated type to identify the type
        /// of information the function retrieves.
        /// </param>
        /// <param name="TokenInformation">
        /// A pointer to a buffer the function fills with the requested information. The structure
        /// put into this buffer depends upon the type of information specified by the
        /// TokenInformationClass parameter.
        /// </param>
        /// <param name="TokenInformationLength">
        /// Specifies the size, in bytes, of the buffer pointed to by the TokenInformation parameter.
        /// If TokenInformation is NULL, this parameter must be zero.
        /// </param>
        /// <param name="ReturnLength">
        /// A pointer to a variable that receives the number of bytes needed for the buffer pointed
        /// to by the TokenInformation parameter. If this value is larger than the value specified
        /// in the TokenInformationLength parameter, the function fails and stores no data in the buffer.
        /// If the value of the TokenInformationClass parameter is TokenDefaultDacl and the token has
        /// no default DACL, the function sets the variable pointed to by ReturnLength to
        /// sizeof(TOKEN_DEFAULT_DACL) and sets the DefaultDacl member of the TOKEN_DEFAULT_DACL
        /// structure to NULL.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        /// The calling process must have appropriate access rights to obtain the information.
        /// </remarks>
        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern bool GetTokenInformation(
            [In] IntPtr TokenHandle,
            [In] TOKEN_INFORMATION_CLASS TokenInformationClass,
            [Out] [Optional] IntPtr TokenInformation,
            [In] int TokenInformationLength,
            [Out] out int ReturnLength);

    }
}
