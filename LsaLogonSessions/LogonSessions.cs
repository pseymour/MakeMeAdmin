// <copyright file="LogonSessions.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

namespace LsaLogonSessions
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// This class allows enumeration of Windows logon sessions.
    /// </summary>
    public class LogonSessions
    {
        /// <summary>
        /// Prevents a default instance of the LogonSessions class from being created.
        /// </summary>
        private LogonSessions()
        {
        }

        public static string[] GetLoggedOnUserNames(string serverName)
        {
            IntPtr serverHandle = NativeMethods.WTSOpenServer(serverName);
            Console.WriteLine("WTSOpenServer last error: {0}", Marshal.GetLastWin32Error());
            string[] userNames = GetLoggedOnUserNames(serverHandle);
            NativeMethods.WTSCloseServer(serverHandle);
            Console.WriteLine("WTSCloseServer last error: {0}", Marshal.GetLastWin32Error());
            return userNames;
        }

        public static System.Security.Principal.SecurityIdentifier GetSidForSessionId(int sessionId)
        {
            System.Security.Principal.SecurityIdentifier returnSid = null;
            IntPtr tokenPointer = IntPtr.Zero;
            int returnValue = NativeMethods.WTSQueryUserToken(sessionId, out tokenPointer);
            int lastWin32Error = Marshal.GetLastWin32Error();
            if (returnValue != 0)
            {
                System.Security.Principal.SecurityIdentifier sid = GetSidFromToken(tokenPointer);
                if (sid != null)
                {
                    returnSid = sid;
                }
                NativeMethods.CloseHandle(tokenPointer);
            }
            return returnSid;
        }

        public static string[] GetLoggedOnUserNames()
        {
            return GetLoggedOnUserNames(NativeMethods.WTS_CURRENT_SERVER_HANDLE);
        }

        /// <summary>
        /// Gets a list of the currently logged-on users.
        /// </summary>
        /// <returns>
        /// Returns an array of strings, which contains the names of the currently logged-on users.
        /// Returns null if no users are logged on.
        /// </returns>
        private static string[] GetLoggedOnUserNames(IntPtr serverHandle)
        {
            string[] returnArray = null;
            IntPtr sessionInfoPointer = IntPtr.Zero;
            int sessionCount = 0;
            int returnValue = NativeMethods.WTSEnumerateSessions(serverHandle, 0, 1, ref sessionInfoPointer, ref sessionCount);
            
            long current = sessionInfoPointer.ToInt64();
            int sessionInfoSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
            if (returnValue != 0)
            {
                System.Collections.Generic.List<string> userNames = new System.Collections.Generic.List<string>(sessionCount);
                for (int i = 0; i < sessionCount; i++)
                {
                    string domainName = null;
                    string userName = null;
                    ushort protocolType = ushort.MinValue;

                    WTS_SESSION_INFO si = (WTS_SESSION_INFO)Marshal.PtrToStructure((System.IntPtr)current, typeof(WTS_SESSION_INFO));
                    current += sessionInfoSize;

                    IntPtr bufferPointer = IntPtr.Zero;
                    int bytesReturned = 0;

                    // Get the logged-on user's domain name.
                    returnValue = NativeMethods.WTSQuerySessionInformation(serverHandle, si.SessionID, WTS_INFO_CLASS.WTSDomainName, out bufferPointer, out bytesReturned);
                    if (returnValue != 0)
                    {
                        domainName = Marshal.PtrToStringAnsi(bufferPointer, bytesReturned - 1);
                        NativeMethods.WTSFreeMemory(bufferPointer);
                    }

                    // Get the logged-on user's name.
                    returnValue = NativeMethods.WTSQuerySessionInformation(serverHandle, si.SessionID, WTS_INFO_CLASS.WTSUserName, out bufferPointer, out bytesReturned);
                    if (returnValue != 0)
                    {
                        userName = Marshal.PtrToStringAnsi(bufferPointer, bytesReturned - 1);
                        NativeMethods.WTSFreeMemory(bufferPointer);
                    }

                    returnValue = NativeMethods.WTSQuerySessionInformation(serverHandle, si.SessionID, WTS_INFO_CLASS.WTSClientProtocolType, out bufferPointer, out bytesReturned);
                    if (returnValue != 0)
                    {
                        protocolType = (ushort)Marshal.PtrToStructure(bufferPointer, typeof(ushort));
                        NativeMethods.WTSFreeMemory(bufferPointer);
                    }

                    if (((domainName != null) || (userName != null)) && (domainName != string.Empty) && (userName != string.Empty))
                    {
                        string fullUserName = string.Format("{0}{1}{2}", domainName == null ? string.Empty : string.Format("{0}\\", domainName.ToUpper(System.Globalization.CultureInfo.CurrentCulture)), userName.ToLower(System.Globalization.CultureInfo.CurrentCulture), protocolType == 2 ? " (RDP)" : string.Empty);
                        userNames.Add(fullUserName);
                    }
                }

                NativeMethods.WTSFreeMemory(sessionInfoPointer);
                userNames.TrimExcess();
                returnArray = new string[userNames.Count];
                userNames.CopyTo(returnArray);
            }
            else
            {
                Console.WriteLine("WTSEnumerateSessions returned zero (0), a failure code.");
            }

            return returnArray;
        }

        public static int[] GetLoggedOnUserSessionIds()
        {
            return GetLoggedOnUserSessionIds(NativeMethods.WTS_CURRENT_SERVER_HANDLE);
        }

        /// <summary>
        /// Gets a list of the currently logged-on users.
        /// </summary>
        /// <returns>
        /// Returns an array of strings, which contains the names of the currently logged-on users.
        /// Returns null if no users are logged on.
        /// </returns>
        private static int[] GetLoggedOnUserSessionIds(IntPtr serverHandle)
        {
            int[] returnArray = null;
            IntPtr sessionInfoPointer = IntPtr.Zero;
            int sessionCount = 0;
            int returnValue = NativeMethods.WTSEnumerateSessions(serverHandle, 0, 1, ref sessionInfoPointer, ref sessionCount);

            long current = sessionInfoPointer.ToInt64();
            int sessionInfoSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));
            if (returnValue != 0)
            {
                System.Collections.Generic.List<int> sessionIds = new System.Collections.Generic.List<int>(sessionCount);
                for (int i = 0; i < sessionCount; i++)
                {
                    /*
                    System.UInt32 domainName = System.UInt32.MaxValue;
                    */

                    WTS_SESSION_INFO si = (WTS_SESSION_INFO)Marshal.PtrToStructure((System.IntPtr)current, typeof(WTS_SESSION_INFO));
                    current += sessionInfoSize;

                    sessionIds.Add(si.SessionID);

                    /*
                    IntPtr bufferPointer = IntPtr.Zero;
                    int bytesReturned = 0;

                    // Get the logged-on user's domain name.
                    returnValue = NativeMethods.WTSQuerySessionInformation(serverHandle, si.SessionID, WTS_INFO_CLASS.WTSSessionId, out bufferPointer, out bytesReturned);
                    if (returnValue != 0)
                    {
                        domainName = Marshal.ptr  Marshal.PtrToStringAnsi(bufferPointer, bytesReturned - 1);
                        NativeMethods.WTSFreeMemory(bufferPointer);
                    }

                    if (((domainName != null)) && (domainName != string.Empty))
                    {
                        string fullUserName = string.Format("{0}{1}{2}", domainName == null ? string.Empty : string.Format("{0}\\", domainName.ToUpper(System.Globalization.CultureInfo.CurrentCulture)), userName.ToLower(System.Globalization.CultureInfo.CurrentCulture), protocolType == 2 ? " (RDP)" : string.Empty);
                        userNames.Add(fullUserName);
                    }
                    */
                }

                NativeMethods.WTSFreeMemory(sessionInfoPointer);
                sessionIds.TrimExcess();
                returnArray = new int[sessionIds.Count];
                sessionIds.CopyTo(returnArray);
            }
            else
            {
                Console.WriteLine("WTSEnumerateSessions returned zero (0), a failure code.");
            }

            return returnArray;
        }

        public static System.Security.Principal.SecurityIdentifier GetSidFromToken(IntPtr tokenHandle)
        {
            int returnLength = 0;
            System.Security.Principal.SecurityIdentifier returnSid = null;
            bool nativeReturnValue = NativeMethods.GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenUser, IntPtr.Zero, returnLength, out returnLength);
            int lastError = Marshal.GetLastWin32Error();

            if (returnLength > 0)
            {
                IntPtr tokenInfo = Marshal.AllocHGlobal(returnLength);
                nativeReturnValue = NativeMethods.GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenUser, tokenInfo, returnLength, out returnLength);
                lastError = Marshal.GetLastWin32Error();

                if (nativeReturnValue)
                {
                    TOKEN_USER tokenUser = (TOKEN_USER)Marshal.PtrToStructure(tokenInfo, typeof(TOKEN_USER));
                    System.Security.Principal.SecurityIdentifier sid = new System.Security.Principal.SecurityIdentifier(tokenUser.User.Sid);
                    returnSid = sid;
                }

                Marshal.FreeHGlobal(tokenInfo);
            }
            return returnSid;
        }

    }
}
