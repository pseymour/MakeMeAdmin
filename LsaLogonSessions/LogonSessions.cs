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
        /// Gets the security identifier (SID) associated with a particular session ID.
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
        /// Creates a WindowsIdentity object for the user session with the given ID.
        /// </summary>
        /// <param name="sessionId">
        /// The session ID for which the WindowsIdentity object is to be constructed.
        /// </param>
        /// <returns>
        /// Returns a WindowsIdentity object for the given session ID, if the session ID can be found.
        /// If the session ID cannot be found, null is returned.
        /// </returns>
        public static WindowsIdentity GetWindowsIdentityForSessionId(int sessionId)
        {
            WindowsIdentity returnIdentity = null;
            IntPtr tokenPointer = IntPtr.Zero;
            int returnValue = NativeMethods.WTSQueryUserToken(sessionId, out tokenPointer);
            int lastWin32Error = Marshal.GetLastWin32Error();
            if (returnValue != 0)
            {
                returnIdentity = new WindowsIdentity(tokenPointer);
                NativeMethods.CloseHandle(tokenPointer);
            }
            return returnIdentity;
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
        /// Gets the group memberships for the user with the specified token.
        /// </summary>
        /// <param name="tokenHandle">
        /// The handle to the user's token.
        /// </param>
        /// <returns>
        /// Returns an array of security identifiers (SIDs), one for each
        /// group of which the given user is a member.
        /// </returns>
        public static SecurityIdentifier[] GetGroupMemberships(IntPtr tokenHandle)
        {
            int returnLength = 0;
            SecurityIdentifier[] returnArray = null;

            // Get the size of the memory chunk that we need in order to retrieve the TOKEN_GROUPS structure.
            bool nativeReturnValue = NativeMethods.GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenGroups, IntPtr.Zero, returnLength, out returnLength);

            int lastError = Marshal.GetLastWin32Error();

            if (returnLength > 0)
            {
                // Allocate enough memory to store a TOKEN_GROUPS structure.
                IntPtr tokenInfo = Marshal.AllocHGlobal(returnLength);

                // Get the TOKEN_GROUPS structure for the token.
                nativeReturnValue = NativeMethods.GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenGroups, tokenInfo, returnLength, out returnLength);

                lastError = Marshal.GetLastWin32Error();

                if (nativeReturnValue)
                {
                    TOKEN_GROUPS tokenGroups = (TOKEN_GROUPS)Marshal.PtrToStructure(tokenInfo, typeof(TOKEN_GROUPS));
                    returnArray = new SecurityIdentifier[tokenGroups.GroupCount];
                    uint groupCount = tokenGroups.GroupCount;

                    IntPtr currentItem = new IntPtr(tokenInfo.ToInt64() + Marshal.SizeOf(typeof(TOKEN_GROUPS)) - Marshal.SizeOf(typeof(IntPtr)));
                    int sidAndAttrSize = Marshal.SizeOf(new SID_AND_ATTRIBUTES());
                    for (int i = 0; i < groupCount; i++)
                    {
                        SID_AND_ATTRIBUTES sidAndAttributes = (SID_AND_ATTRIBUTES)Marshal.PtrToStructure(new IntPtr(tokenInfo.ToInt64() + i * sidAndAttrSize + IntPtr.Size), typeof(SID_AND_ATTRIBUTES));

                        IntPtr pstr = IntPtr.Zero;
                        NativeMethods.ConvertSidToStringSid(sidAndAttributes.Sid, out pstr);
                        string sidString = Marshal.PtrToStringAuto(pstr);
                        NativeMethods.LocalFree(pstr);

                        returnArray[i] = new SecurityIdentifier(sidString);
                        currentItem = new IntPtr(currentItem.ToInt64() + Marshal.SizeOf(typeof(SID_AND_ATTRIBUTES)));
                    }

                }

                // Free the memory that we allocated earlier for the TOKEN_GROUPS structure.
                Marshal.FreeHGlobal(tokenInfo);
            }

            return returnArray;
        }

        /// <summary>
        /// Gets an array containing the session IDs of the currently logged-on users for the
        /// server on which the application is running.
        /// </summary>
        /// <returns>
        /// Returns an array of integers, which contains the sessions IDs of currently logged-on users.
        /// Returns null if no users are logged on or the list of session IDs cannot be retrieved.
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

        public static int SendMessageToSession(int sessionId, string message, int messageStyle, int timeout, out int response)
        {
            // TODO: These should be customizable.
            string messageTitle = "Make Me Admin";

            int MB_SYSTEMMODAL = 0x00001000;
            int MB_SETFOREGROUND = 0x00010000; // The message box becomes the foreground window.
            /* int MB_SERVICE_NOTIFICATION = 0x00200000; */
            messageStyle += MB_SYSTEMMODAL + MB_SETFOREGROUND;

            // TODO: Change the style parameter to use MessageBoxButtons + MessageBoxIcons data types?
            // TODO: Change the response to be DialogResult data type?
            return NativeMethods.WTSSendMessage(NativeMethods.WTS_CURRENT_SERVER_HANDLE, sessionId, messageTitle, messageTitle.Length, message, message.Length, messageStyle, timeout, out response, true);
        }

        public static int LogoffSession(int sessionId)
        {
            return NativeMethods.WTSLogoffSession(NativeMethods.WTS_CURRENT_SERVER_HANDLE, sessionId, false);
        }

    }
}
