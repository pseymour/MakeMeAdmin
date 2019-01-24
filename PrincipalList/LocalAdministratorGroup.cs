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

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.DirectoryServices.AccountManagement;
    using System.Security.Principal;

    /// <summary>
    /// This class allows management of the local Administrators group.
    /// </summary>
    public static class LocalAdministratorGroup
    {
        /// <summary>
        /// The context against which all operations are performed by this class.
        /// In this case, the context is the local machine.
        /// </summary>
        private static PrincipalContext localMachineContext = null;

        /// <summary>
        /// The security identifier (SID) of the local Administrators group.
        /// </summary>
        private static SecurityIdentifier localAdminsGroupSid = null;

        /// <summary>
        /// Represents the local Administrators group.
        /// </summary>
        private static GroupPrincipal localAdminGroup = null;

        /// <summary>
        /// Constructor.
        /// </summary>
        static LocalAdministratorGroup()
        {
        }

        /// <summary>
        /// Gets a context representing the local machine.
        /// </summary>
        private static PrincipalContext LocalMachineContext
        {
            get
            {
                if (localMachineContext == null)
                {
                    try
                    {
                        localMachineContext = new PrincipalContext(ContextType.Machine);
                    }
                    catch (System.ComponentModel.InvalidEnumArgumentException invalidEnumException)
                    {
                        ApplicationLog.WriteEvent(invalidEnumException.Message, EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Error);
                        throw;
                    }
                    catch (ArgumentException argException)
                    {
                        ApplicationLog.WriteEvent(argException.Message, EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Error);
                        throw;
                    }
                }
                return localMachineContext;
            }
        }

        /// <summary>
        /// Gets the security identifier (SID) of the local Administrators group.
        /// </summary>
        private static SecurityIdentifier LocalAdminsGroupSid
        {
            get
            {
                if (localAdminsGroupSid == null)
                {
                    localAdminsGroupSid = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null);
                }
                return localAdminsGroupSid;
            }
        }

        /// <summary>
        /// Gets the name of the local Administrators group.
        /// </summary>
        public static string LocalAdminGroupName
        {
            get { return LocalAdminGroup.Name; }
        }

        /// <summary>
        /// Gets an object representing the local Administrators group.
        /// </summary>
        private static GroupPrincipal LocalAdminGroup
        {
            get
            {
                if (LocalMachineContext == null)
                {
                    // TODO: i18n.
                    ApplicationLog.WriteEvent("Local machine context is null.", EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Error);
                }
                else
                {
                    if (LocalAdminsGroupSid == null)
                    {
                        // TODO: i18n.
                        ApplicationLog.WriteEvent("Local admins group SID is null.", EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Error);
                    }
                    else
                    {
                        if (localAdminGroup == null)
                        {
                            try
                            {
                                localAdminGroup = GroupPrincipal.FindByIdentity(LocalMachineContext, IdentityType.Sid, LocalAdminsGroupSid.Value);
                            }
                            catch (Exception exception)
                            {
                                // TODO: i18n.
                                ApplicationLog.WriteEvent(string.Format("Exception: {0}", exception.Message), EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Error);
                                throw;
                            }
                        }
                    }
                }
                return localAdminGroup;
            }
        }

        /// <summary>
        /// Adds a security principal to the local Administrators group.
        /// </summary>
        /// <param name="userIdentity">
        /// The identity of the principal to be added to the Administrators group.
        /// </param>
        /// <param name="expirationTime">
        /// The date and time at which the principal's administrator rights should expire.
        /// </param>
        /// <param name="remoteAddress">
        /// The address of the remote host from which a request for administrator rights came, if applicable.
        /// </param>
        public static void AddPrincipal(WindowsIdentity userIdentity, DateTime? expirationTime, string remoteAddress)
        {
            // TODO: Only do this if the principal is not a member of the group?

            bool userIsAuthorized = Shared.UserIsAuthorized(userIdentity, Settings.LocalAllowedEntities, Settings.LocalDeniedEntities);

            if (!string.IsNullOrEmpty(remoteAddress))
            { // Request is from a remote computer. Check the remote authorization list.
                userIsAuthorized &= Shared.UserIsAuthorized(userIdentity, Settings.RemoteAllowedEntities, Settings.RemoteDeniedEntities);
            }

            if (
                (LocalAdminGroup != null) &&
                (userIdentity.User != null) && 
                (userIdentity.Groups != null) && 
                (userIsAuthorized)
               )
            {
                // Save the principal's information to the list of principals.
                EncryptedSettings encryptedSettings = new EncryptedSettings(EncryptedSettings.SettingsFilePath);
                encryptedSettings.AddPrincipal(userIdentity, expirationTime, remoteAddress);

                AddPrincipalToAdministrators(userIdentity.User);
            }
        }

        /// <summary>
        /// Adds the given security identifier (SID) to the local Administrators group.
        /// </summary>
        /// <param name="userSid">
        /// The security identifier (SID) to be added to the local Administrators group.
        /// </param>
        private static bool AddPrincipalToAdministrators(SecurityIdentifier userSid)
        {
            int result = AddLocalGroupMembers(LocalAdminGroup.SamAccountName, userSid);
            if (result == 0)
            {
                // TODO: i18n.
                ApplicationLog.WriteEvent(string.Format("Principal {0} ({1}) added to the Administrators group.", userSid, GetAccountNameFromSID(userSid)), EventID.UserAddedToAdminsSuccess, System.Diagnostics.EventLogEntryType.Information);
                return true;
            }
            else
            {
                // TODO: i18n.
                ApplicationLog.WriteEvent(string.Format("Adding principal {0} ({1}) to the Administrators group returned error code {2}.", userSid, GetAccountNameFromSID(userSid), result), EventID.UserAddedToAdminsFailure, System.Diagnostics.EventLogEntryType.Warning);
                return false;
            }
        }

        /// <summary>
        /// Removes the given security identifier (SID) from the local Administrators group.
        /// </summary>
        /// <param name="userSid">
        /// The security identifier (SID) to be removed from the local Administrators group.
        /// </param>
        /// <param name="reason">
        /// The reason for the removal.
        /// </param>
        public static void RemovePrincipal(SecurityIdentifier userSid, RemovalReason reason)
        {
            // TODO: Only do this if the principal is a member of the group?

            if ((LocalAdminGroup != null) && (userSid != null))
            {
                SecurityIdentifier[] localAdminSids = GetLocalGroupMembers(LocalAdminGroup.SamAccountName);

                foreach (SecurityIdentifier sid in localAdminSids)
                {
                    if (sid == userSid)
                    {
                        string accountName = GetAccountNameFromSID(userSid);
                        int result = RemoveLocalGroupMembers(LocalAdminGroup.SamAccountName, userSid);
                        if (result == 0)
                        {
                            EncryptedSettings encryptedSettings = new EncryptedSettings(EncryptedSettings.SettingsFilePath);
                            encryptedSettings.RemovePrincipal(userSid);

                            string reasonString = Properties.Resources.RemovalReasonUnknown;
                            switch (reason)
                            {
                                case RemovalReason.ServiceStopped:
                                    reasonString = Properties.Resources.RemovalReasonServiceStopped;
                                    break;
                                case RemovalReason.Timeout:
                                    reasonString = Properties.Resources.RemovalReasonTimeout;
                                    break;
                                case RemovalReason.UserLogoff:
                                    reasonString = Properties.Resources.RemovalReasonUserLogoff;
                                    break;
                                case RemovalReason.UserRequest:
                                    reasonString = Properties.Resources.RemovalReasonUserRequest;
                                    break;
                            }
                            // TODO: i18n.
                            string message = string.Format("Principal {0} ({1}) removed from the Administrators group. Reason: {2}.", userSid, accountName, reasonString);
                            ApplicationLog.WriteEvent(message, EventID.UserRemovedFromAdminsSuccess, System.Diagnostics.EventLogEntryType.Information);
                        }
                        else
                        {
                            // TODO: i18n.
                            ApplicationLog.WriteEvent(string.Format("Removing principal {0} ({1}) from the Administrators group returned error code {1}.", userSid, accountName, result), EventID.UserRemovedFromAdminsFailure, System.Diagnostics.EventLogEntryType.Warning);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Validates that all of the principals stored in the on-disk principal list
        /// are in the local Adminstrators group if they're supposed to be, and vice-vera.
        /// </summary>
        public static void ValidateAllAddedPrincipals()
        {
            // Get a list of the principals stored in the on-disk list.
            EncryptedSettings encryptedSettings = new EncryptedSettings(EncryptedSettings.SettingsFilePath);
            SecurityIdentifier[] addedPrincipalList = encryptedSettings.AddedPrincipalSIDs;

            // Get a list of the current members of the Administrators group.
            SecurityIdentifier[] localAdminSids = null;
            if ((addedPrincipalList.Length > 0) && (LocalAdminGroup != null))
            {
                localAdminSids = GetLocalGroupMembers(LocalAdminGroup.SamAccountName);
            }

            for (int i = 0; i < addedPrincipalList.Length; i++)
            {
                bool sidFoundInAdminsGroup = false;
                if ((addedPrincipalList[i] != null) && (localAdminSids != null))
                {
                    foreach (SecurityIdentifier sid in localAdminSids)
                    {
                        if (sid == addedPrincipalList[i])
                        {
                            sidFoundInAdminsGroup = true;
                            break;
                        }
                    }

                    if (sidFoundInAdminsGroup)
                    {  // Principal's SID was found in the local administrators group.

                        DateTime? expirationTime = encryptedSettings.GetExpirationTime(addedPrincipalList[i]);

                        if (expirationTime.HasValue)
                        { // The principal's rights expire at some point.
                            if (expirationTime.Value > DateTime.Now)
                            { // The principal's administrator rights expire in the future.

                                // Nothing to do here, since the principal is already in the administrators group.

                            }
                            else
                            { // The principal's administrator rights have expired.
#if DEBUG
                                string accountName = GetAccountNameFromSID(addedPrincipalList[i]);
                                ApplicationLog.WriteEvent(string.Format("Principal {0} ({1}) has been removed from the Administrators group by an outside process. Removing the principal from Make Me Admin's list.", addedPrincipalList[i], string.IsNullOrEmpty(accountName) ? "unknown account" : accountName), EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Information);
#endif
                                LocalAdministratorGroup.RemovePrincipal(addedPrincipalList[i], RemovalReason.Timeout);
                            }
                        }

                        else
                        { // The principal's rights never expire.

                            // Get a WindowsIdentity object for the user matching the added principal SID.
                            WindowsIdentity sessionIdentity = null;
                            WindowsIdentity userIdentity = null;
                            int[] loggedOnSessionIDs = LsaLogonSessions.LogonSessions.GetLoggedOnUserSessionIds();
                            foreach (int sessionId in loggedOnSessionIDs)
                            {
                                sessionIdentity = LsaLogonSessions.LogonSessions.GetWindowsIdentityForSessionId(sessionId);
                                if ((sessionIdentity != null) && (sessionIdentity.User == addedPrincipalList[i]))
                                {
                                    userIdentity = sessionIdentity;
                                    break;
                                }
                            }

                            if (
                                (Settings.AutomaticAddAllowed != null) &&
                                (Settings.AutomaticAddAllowed.Length > 0) &&
                                (Shared.UserIsAuthorized(userIdentity, Settings.AutomaticAddAllowed, Settings.AutomaticAddDenied))
                               )
                            { // The user is an automatically-added user.

                                // Nothing to do here. The user is an automatically-added one, and their rights don't expire.
                            }
                            else
                            { // The user is not an automatically-added user.

                                // Users who are not automatically added should not have non-expiring rights. Remove this user.
                                LocalAdministratorGroup.RemovePrincipal(addedPrincipalList[i], RemovalReason.Timeout);
                            }
                        }
                    }
                    else
                    { // Principal's SID was not found in the local administrators group.

                        DateTime? expirationTime = encryptedSettings.GetExpirationTime(addedPrincipalList[i]);

                        if (expirationTime.HasValue)
                        { // The principal's rights expire at some point.
                            if (expirationTime.Value > DateTime.Now)
                            { // The principal's administrator rights expire in the future.
                                string accountName = GetAccountNameFromSID(addedPrincipalList[i]);
                                if (Settings.OverrideRemovalByOutsideProcess)
                                {
                                    // TODO: i18n.
                                    ApplicationLog.WriteEvent(string.Format("Principal {0} ({1}) has been removed from the Administrators group by an outside process. Adding the principal back to the Administrators group.", addedPrincipalList[i], string.IsNullOrEmpty(accountName) ? "unknown account" : accountName), EventID.PrincipalRemovedByExternalProcess, System.Diagnostics.EventLogEntryType.Information);
                                    AddPrincipalToAdministrators(addedPrincipalList[i]);
                                }
                                else
                                {
                                    // TODO: i18n.
                                    ApplicationLog.WriteEvent(string.Format("Principal {0} ({1}) has been removed from the Administrators group by an outside process. Removing the principal from Make Me Admin's list.", addedPrincipalList[i], string.IsNullOrEmpty(accountName) ? "unknown account" : accountName), EventID.PrincipalRemovedByExternalProcess, System.Diagnostics.EventLogEntryType.Information);

                                    encryptedSettings.RemovePrincipal(addedPrincipalList[i]);
                                }
                            }
                            else
                            { // The principal's administrator rights have expired.
                              // No need to remove from the administrators group, as we already know the SID
                              // is not present in the group.
                                
                                encryptedSettings.RemovePrincipal(addedPrincipalList[i]);

                            }
                        }
                        else
                        { // The principal's rights never expire.

                            // Get a WindowsIdentity object for the user matching the added principal SID.
                            WindowsIdentity sessionIdentity = null;
                            WindowsIdentity userIdentity = null;
                            int[] loggedOnSessionIDs = LsaLogonSessions.LogonSessions.GetLoggedOnUserSessionIds();
                            foreach (int sessionId in loggedOnSessionIDs)
                            {
                                sessionIdentity = LsaLogonSessions.LogonSessions.GetWindowsIdentityForSessionId(sessionId);
                                if ((sessionIdentity != null) && (sessionIdentity.User == addedPrincipalList[i]))
                                {
                                    userIdentity = sessionIdentity;
                                    break;
                                }
                            }

                            if (
                                (Settings.AutomaticAddAllowed != null) &&
                                (Settings.AutomaticAddAllowed.Length > 0) &&
                                (Shared.UserIsAuthorized(userIdentity, Settings.AutomaticAddAllowed, Settings.AutomaticAddDenied))
                               )
                            { // The user is an automatically-added user.

                                // The users rights do not expire, but they are an automatically-added user and are missing
                                // from the Administrators group. Add the user back in.

                                AddPrincipalToAdministrators(addedPrincipalList[i]);
                            }
                            else
                            { // The user is not an automatically-added user.

                                // The user is not in the Administrators group now, but they are
                                // listed as having non-expiring rights, even though they are not
                                // automatically added. This should never really happen, but
                                // just in case, we'll remove them from the on-disk principal list.
                                encryptedSettings.RemovePrincipal(addedPrincipalList[i]);

                            }

                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the human-friendly account name corresponding to a givem
        /// security identifier (SID).
        /// </summary>
        /// <param name="sid">
        /// The security identifier (SID) for which the account name should be retrieved.
        /// </param>
        /// <returns>
        /// Returns a string containing the name of the account corresponding to the given
        /// SID. If the account name cannot be determined or the SID does not belong to a
        /// valid Windows account, null is returned.
        /// </returns>
        private static string GetAccountNameFromSID(SecurityIdentifier sid)
        {
            if (sid.IsAccountSid() && sid.IsValidTargetType(typeof(NTAccount)))
            {
                try
                {
                    try
                    {
                        NTAccount account = (NTAccount)sid.Translate(typeof(NTAccount));
                        return account.Value;
                    }
                    catch (System.SystemException)
                    {
                        return null;
                    }
                }
                catch (IdentityNotMappedException)
                { // Some or all identity references could not be translated.
                    return null;
                }
                catch (System.ArgumentNullException)
                { // The target translation type is null.
                    return null;
                }
                catch (System.ArgumentException)
                { // The target translation type is not an IdentityReference type.
                    return null;
                }
                catch (System.SystemException)
                { // A Win32 error code was returned.
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /*
        public static bool CurrentUserIsMember()
        {
            bool isMember = false;

            List<SecurityIdentifier> userSids = Shared.GetAuthorizationGroups(WindowsIdentity.GetCurrent(TokenAccessLevels.Read));

            foreach (SecurityIdentifier sid in userSids)
            {
                isMember = sid.Equals(localAdminsGroupSid);
                if (isMember) { break; }
            }

            return isMember;
        }
        */
        
        /// <summary>
        /// Adds a security identifier (SID) to a local security group.
        /// </summary>
        /// <param name="GroupName">
        /// The name of the group to which the SID is to be added.
        /// </param>
        /// <param name="memberSid">
        /// The security identifier (SID) to be added to the local group.
        /// </param>
        /// <returns>
        /// If the addition of the group member is successful, this function returns
        /// zero (0). Otherwise, a non-zero value is returned.
        /// </returns>
        private static int AddLocalGroupMembers(string GroupName, SecurityIdentifier memberSid)
        {
            int returnValue = -1;
            NativeMethods.LOCALGROUP_MEMBERS_INFO_0 newMemberInfo = new NativeMethods.LOCALGROUP_MEMBERS_INFO_0();
            byte[] binarySid = new byte[memberSid.BinaryLength];
            memberSid.GetBinaryForm(binarySid, 0);

            IntPtr unmanagedPointer = System.Runtime.InteropServices.Marshal.AllocHGlobal(binarySid.Length);
            System.Runtime.InteropServices.Marshal.Copy(binarySid, 0, unmanagedPointer, binarySid.Length);
            newMemberInfo.lgrmi0_sid = unmanagedPointer;
            returnValue = NativeMethods.NetLocalGroupAddMembers(null, GroupName, 0, ref newMemberInfo, 1);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(unmanagedPointer);
            return returnValue;
        }

        /// <summary>
        /// Removes a security identifier (SID) from a local security group.
        /// </summary>
        /// <param name="GroupName">
        /// The name of the group from which the SID is to be removed.
        /// </param>
        /// <param name="memberSid">
        /// The security identifier (SID) to be removed from the local group.
        /// </param>
        /// <returns>
        /// If the removal of the group member is successful, this function returns
        /// zero (0). Otherwise, a non-zero value is returned.
        /// </returns>
        private static int RemoveLocalGroupMembers(string GroupName, SecurityIdentifier memberSid)
        {
            int returnValue = -1;
            NativeMethods.LOCALGROUP_MEMBERS_INFO_0 memberInfo = new NativeMethods.LOCALGROUP_MEMBERS_INFO_0();
            byte[] binarySid = new byte[memberSid.BinaryLength];
            memberSid.GetBinaryForm(binarySid, 0);

            IntPtr unmanagedPointer = System.Runtime.InteropServices.Marshal.AllocHGlobal(binarySid.Length);
            System.Runtime.InteropServices.Marshal.Copy(binarySid, 0, unmanagedPointer, binarySid.Length);
            memberInfo.lgrmi0_sid = unmanagedPointer;
            returnValue = NativeMethods.NetLocalGroupDelMembers(null, GroupName, 0, ref memberInfo, 1);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(unmanagedPointer);
            return returnValue;
        }

        /// <summary>
        /// Retrieves a list of the members of a particular local group in the security database.
        /// </summary>
        /// <param name="GroupName">
        /// The name of the local group whose members are to be listed.
        /// </param>
        /// <returns>
        /// Returns an array of security identifiers (SIDs), each representing a group member.
        /// </returns>
        private static SecurityIdentifier[] GetLocalGroupMembers(string GroupName)
        {
            SecurityIdentifier[] returnValue = null;
            IntPtr buffer = IntPtr.Zero;
            IntPtr Resume = IntPtr.Zero;

            int val = NativeMethods.NetLocalGroupGetMembers(null, GroupName, 0, out buffer, -1, out int EntriesRead, out int TotalEntries, Resume);
            if (EntriesRead > 0)
            {
                returnValue = new SecurityIdentifier[EntriesRead];
                NativeMethods.LOCALGROUP_MEMBERS_INFO_0[] Members = new NativeMethods.LOCALGROUP_MEMBERS_INFO_0[EntriesRead];
                IntPtr iter = buffer;
                for (int i = 0; i < EntriesRead; i++)
                {
                    Members[i] = (NativeMethods.LOCALGROUP_MEMBERS_INFO_0)System.Runtime.InteropServices.Marshal.PtrToStructure(iter, typeof(NativeMethods.LOCALGROUP_MEMBERS_INFO_0));
                    iter = (IntPtr)((long)iter + System.Runtime.InteropServices.Marshal.SizeOf(typeof(NativeMethods.LOCALGROUP_MEMBERS_INFO_0)));
                    if (Members[i].lgrmi0_sid != IntPtr.Zero)
                    {
                        SecurityIdentifier sid = new SecurityIdentifier(Members[i].lgrmi0_sid);
                        returnValue[i] = sid;
                    }
                    else
                    {
                        returnValue[i] = null;
                    }
                }
                NativeMethods.NetApiBufferFree(buffer);
            }
            return returnValue;
        }

        /// <summary>
        /// Determines whether the given WindowsIdentity is directly a member
        /// of the local Administrators group.
        /// </summary>
        /// <param name="userIdentity">
        /// The WindowsIdentity whose group membership is to be checked.
        /// </param>
        /// <returns>
        /// Returns true if the given WindowsIdentity is specifically listed in
        /// the group membership of the local Administrators group.
        /// </returns>
        /// <remarks>
        /// A given security principal is considered a direct member of the
        /// Administrators group if they are specifically listed in the group's
        /// membership.
        /// </remarks>
        public static bool IsMemberOfAdministratorsDirectly(WindowsIdentity userIdentity)
        {
            SecurityIdentifier[] localAdminSids = GetLocalGroupMembers(LocalAdminGroup.SamAccountName);

            bool isMember = false;

            foreach (SecurityIdentifier sid in localAdminSids)
            {
                isMember = sid.Equals(userIdentity.User);
                if (isMember) { break; }
            }

            return isMember;
        }

        /// <summary>
        /// Determines whether the given WindowsIdentity is a member of the local
        /// Administrators group, either directly or via nested group memberships.
        /// </summary>
        /// <param name="userIdentity">
        /// The WindowsIdentity whose group membership is to be checked.
        /// </param>
        /// <returns>
        /// Returns true if the given WindowsIdentity is a member of the local
        /// Administrators group in any way (specifically listed or via nested groups).
        /// </returns>
        public static bool IsMemberOfAdministrators(WindowsIdentity userIdentity)
        {
            bool isMember = IsMemberOfAdministratorsDirectly(userIdentity);

            if (!isMember)
            {
                SecurityIdentifier[] localAdminSids = GetLocalGroupMembers(LocalAdminGroup.SamAccountName);

                // We can't use userIdentity.Groups here, because it does not include group memberships
                // that are used for deny-only.
                SecurityIdentifier[] groupSids = LsaLogonSessions.LogonSessions.GetGroupMemberships(userIdentity.Token);

                foreach (SecurityIdentifier userGroupSid in groupSids)
                {
                    foreach (SecurityIdentifier adminSid in localAdminSids)
                    {
                        isMember |= adminSid.Equals(userGroupSid);
                        if (isMember) { break; }
                    }
                    if (isMember) { break; }
                }
            }

            return isMember;
        }


        // TODO: This function doesn't really belong here.
        /// <summary>
        /// Ends a network session between this computer and a remote host.
        /// </summary>
        /// <param name="remoteHost">
        /// The name of the computer of the client to disconnect.
        /// </param>
        /// <param name="userName">
        /// The name of the user whose session is to be terminated.
        /// </param>
        /// <returns>
        /// If this function succeeds, the return value is zero (0).
        /// </returns>
        public static int EndNetworkSession(string remoteHost, string userName)
        {
            return NativeMethods.NetSessionDel(null, remoteHost, userName);
        }
    }
}
