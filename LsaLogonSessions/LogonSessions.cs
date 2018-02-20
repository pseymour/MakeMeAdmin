// <copyright file="LogonSessions.cs" company="Sinclair Community College">
// Copyright (c) 2010-2017, Sinclair Community College. All rights reserved.
// </copyright>

namespace LsaLogonSessions
{
    using System;
    using System.Runtime.InteropServices;
    using System.Security.Principal;

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
        
        /// <summary>
        /// Gets the security identifier (SID) associated with a particular
        /// session ID.
        /// </summary>
        /// <param name="sessionId">
        /// The session ID for which the SID should be retrieved.
        /// </param>
        /// <returns>
        /// Returns the security identifier (SID) associated with the specified
        /// session ID, or null if the function is unable to determine the SID.
        /// </returns>
        public static SecurityIdentifier GetSidForSessionId(int sessionId)
        {
            SecurityIdentifier returnSid = null;
            IntPtr tokenPointer = IntPtr.Zero;
            int returnValue = NativeMethods.WTSQueryUserToken(sessionId, out tokenPointer);
            int lastWin32Error = Marshal.GetLastWin32Error();
            if (returnValue != 0)
            {
                SecurityIdentifier sid = GetSidFromToken(tokenPointer);
                if (sid != null)
                {
                    returnSid = sid;
                }
                NativeMethods.CloseHandle(tokenPointer);
            }
            return returnSid;
        }

        /// <summary>
        /// Gets the security identifier (SID) for a token.
        /// </summary>
        /// <param name="tokenHandle">
        /// A handle to the primary access token of a user.
        /// </param>
        /// <returns>
        /// Returns the security identifier (SID) for a token, or null if the SID cannot
        /// be obtained.
        /// </returns>
        private static SecurityIdentifier GetSidFromToken(IntPtr tokenHandle)
        {
            int returnLength = 0;
            SecurityIdentifier returnSid = null;

            // Get the size of the memory chunk that we need in order to retrieve the TOKEN_USER structure.
            bool nativeReturnValue = NativeMethods.GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenUser, IntPtr.Zero, returnLength, out returnLength);

            int lastError = Marshal.GetLastWin32Error();

            if (returnLength > 0)
            {
                // Allocate enough memory to store a TOKEN_USER structure.
                IntPtr tokenInfo = Marshal.AllocHGlobal(returnLength);

                // Get the TOKEN_USER structure for the token.
                nativeReturnValue = NativeMethods.GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenUser, tokenInfo, returnLength, out returnLength);

                lastError = Marshal.GetLastWin32Error();

                if (nativeReturnValue)
                { // Get the TOKEN_USER structure, so we can retrieve the SID from it.
                    TOKEN_USER tokenUser = (TOKEN_USER)Marshal.PtrToStructure(tokenInfo, typeof(TOKEN_USER));
                    SecurityIdentifier sid = new SecurityIdentifier(tokenUser.User.Sid);
                    returnSid = sid;
                }

                // Free the memory that we allocated earlier for the TOKEN_USER structure.
                Marshal.FreeHGlobal(tokenInfo);
            }

            return returnSid;
        }


        /// <summary>
        /// Gets an array containing the session IDs of the currently logged-on users for the
        /// server on which the application is running.
        /// </summary>
        /// <returns>
        /// Returns an array of integers, which contains the sessions IDs of currently logged-on users.
        /// Returns null if no users are logged on or the list of session IDs cannot be retrievd.
        /// </returns>
        public static int[] GetLoggedOnUserSessionIds()
        {
            return GetLoggedOnUserSessionIds(NativeMethods.WTS_CURRENT_SERVER_HANDLE);
        }


        /// <summary>
        /// Gets an array containing the session IDs of the currently logged-on users for the
        /// server pointed to by the specified handle.
        /// </summary>
        /// <returns>
        /// Returns an array of integers, which contains the sessions IDs of currently logged-on users.
        /// Returns null if no users are logged on or the list of session IDs cannot be retrieved.
        /// </returns>
        private static int[] GetLoggedOnUserSessionIds(IntPtr serverHandle)
        {
            int[] returnArray = null;
            IntPtr sessionInfoPointer = IntPtr.Zero;
            int sessionCount = 0;

            // Get a list of the sessions on a server.
            int returnValue = NativeMethods.WTSEnumerateSessions(serverHandle, 0, 1, ref sessionInfoPointer, ref sessionCount);

            long current = sessionInfoPointer.ToInt64();
            int sessionInfoSize = Marshal.SizeOf(typeof(WTS_SESSION_INFO));

            if (returnValue != 0)
            {
                System.Collections.Generic.List<int> sessionIds = new System.Collections.Generic.List<int>(sessionCount);

                // Retrieve the session ID for each session.
                for (int i = 0; i < sessionCount; i++)
                {
                    WTS_SESSION_INFO si = (WTS_SESSION_INFO)Marshal.PtrToStructure((System.IntPtr)current, typeof(WTS_SESSION_INFO));
                    current += sessionInfoSize;
                    sessionIds.Add(si.SessionID);
                }

                NativeMethods.WTSFreeMemory(sessionInfoPointer);

                // Copy session IDs into the return array.
                sessionIds.TrimExcess();
                returnArray = new int[sessionIds.Count];
                sessionIds.CopyTo(returnArray);
            }

            return returnArray;
        }
    }
}
