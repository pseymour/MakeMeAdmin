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
    using System.Linq;
    using System.Security.Principal;

    public static class LocalAdministratorGroup
    {
        private static PrincipalContext localMachineContext = null;
        private static SecurityIdentifier localAdminsGroupSid = null;
        private static GroupPrincipal localAdminGroup = null;

        static LocalAdministratorGroup()
        {
        }

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
                        ApplicationLog.WriteErrorEvent(invalidEnumException.Message, EventID.DebugMessage);
                        throw;
                    }
                    catch (ArgumentException argException)
                    {
                        ApplicationLog.WriteErrorEvent(argException.Message, EventID.DebugMessage);
                        throw;
                    }
                }
                return localMachineContext;
            }
        }

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

        public static string LocalAdminGroupName
        {
            get { return LocalAdminGroup.Name; }
        }

        private static GroupPrincipal LocalAdminGroup
        {
            get
            {
                if (LocalMachineContext == null)
                {
                    // TODO: i18n.
                    ApplicationLog.WriteErrorEvent("Local machine context is null.", EventID.DebugMessage);
                }
                else
                {
                    if (LocalAdminsGroupSid == null)
                    {
                        // TODO: i18n.
                        ApplicationLog.WriteErrorEvent("Local admins group SID is null.", EventID.DebugMessage);
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
                                ApplicationLog.WriteErrorEvent(string.Format("Exception: {0}", exception.Message), EventID.DebugMessage);
                                throw;
                            }
                        }
                    }
                }
                return localAdminGroup;
            }
        }

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
                EncryptedSettings encryptedSettings = new EncryptedSettings(EncryptedSettings.SettingsFilePath);
                encryptedSettings.AddPrincipal(userIdentity.User, expirationTime, remoteAddress);
                AddPrincipalToAdministrators(userIdentity.User, remoteAddress);
            }
        }

        private static void AddPrincipalToAdministrators(SecurityIdentifier userSid, string remoteAddress)
        {
            int result = AddLocalGroupMembers(null, LocalAdminGroup.SamAccountName, userSid);
            if (result == 0)
            {
                /* PrincipalList.AddSID(userSid, expirationTime, remoteAddress); */
                // TODO: i18n.
                ApplicationLog.WriteInformationEvent(string.Format("Principal {0} ({1}) added to the Administrators group.", userSid, GetAccountNameFromSID(userSid)), EventID.UserAddedToAdminsSuccess);
                if (remoteAddress != null)
                {
                    // TODO: i18n.
                    ApplicationLog.WriteInformationEvent(string.Format("Request was sent from host {0}.", remoteAddress), EventID.RemoteRequestInformation);
                }
                
                /*
                Settings.SIDs = PrincipalList.GetSIDs().Select(p => p.Value).ToArray<string>();
                */

            }
            else
            {
                // TODO: i18n.
                ApplicationLog.WriteWarningEvent(string.Format("Adding principal {0} ({1}) to the Administrators group returned error code {2}.", userSid, GetAccountNameFromSID(userSid), result), EventID.UserAddedToAdminsFailure);
            }
        }

        public static void RemovePrincipal(SecurityIdentifier userSid, RemovalReason reason)
        {
            // TODO: Only do this if the principal is a member of the group?
            if ((LocalAdminGroup != null) && (userSid != null))
            {
                SecurityIdentifier[] localAdminSids = GetLocalGroupMembers(null, LocalAdminGroup.SamAccountName);

                foreach (SecurityIdentifier sid in localAdminSids)
                {
                    if (sid == userSid)
                    /* if (string.Compare(sid.Value, principalSID, true) == 0) */
                    {
                        string accountName = GetAccountNameFromSID(userSid);
                        int result = RemoveLocalGroupMembers(null, LocalAdminGroup.SamAccountName, userSid);
                        if (result == 0)
                        {
                            EncryptedSettings encryptedSettings = new EncryptedSettings(EncryptedSettings.SettingsFilePath);
                            encryptedSettings.RemovePrincipal(userSid);
                            /*
                            Settings.SIDs = PrincipalList.GetSIDs().Select(p => p.Value).ToArray<string>();
                            */

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
                            ApplicationLog.WriteInformationEvent(message, EventID.UserRemovedFromAdminsSuccess);
                        }
                        else
                        {
                            // TODO: i18n.
                            ApplicationLog.WriteWarningEvent(string.Format("Removing principal {0} ({1}) from the Administrators group returned error code {1}.", userSid, accountName, result), EventID.UserRemovedFromAdminsFailure);
                        }
                    }
                }
            }
        }

        public static void ValidateAllAddedPrincipals()
        {
            SecurityIdentifier[] localAdminSids = null;

            /* string[] addedSids = PrincipalList.GetSIDs(); */

            EncryptedSettings encryptedSettings = new EncryptedSettings(EncryptedSettings.SettingsFilePath);
            SecurityIdentifier[] addedSids = encryptedSettings.AddedPrincipalSIDs;

            if ((addedSids.Length > 0) && (LocalAdminGroup != null))
            {
                localAdminSids = GetLocalGroupMembers(null, LocalAdminGroup.SamAccountName);
            }

            for (int i = 0; i < addedSids.Length; i++)
            {
                bool sidFoundInAdminsGroup = false;
                if ((addedSids[i] != null) && (localAdminSids != null))
                {
                    foreach (SecurityIdentifier sid in localAdminSids)
                    {
                        if (sid == addedSids[i])
                        {
                            sidFoundInAdminsGroup = true;
                            break;
                        }
                    }

                    if (sidFoundInAdminsGroup)
                    {  // Principal's SID was found in the local administrators group.

                        DateTime? expirationTime = encryptedSettings.GetExpirationTime(addedSids[i]);

                        if (expirationTime.HasValue)
                        { // The principal's rights expire at some point.
                            if (expirationTime.Value > DateTime.Now)
                            { // The principal's administrator rights expire in the future.
                                // Nothing to do here, since the principal is already in the administrators group.
                            }
                            else
                            { // The principal's administrator rights have expired.
#if DEBUG
                                string accountName = GetAccountNameFromSID(addedSids[i]);
                                ApplicationLog.WriteInformationEvent(string.Format("Principal {0} ({1}) has been removed from the Administrators group by an outside process. Removing the principal from Make Me Admin's list.", addedSids[i], string.IsNullOrEmpty(accountName) ? "unknown account" : accountName), EventID.DebugMessage);
#endif
                                LocalAdministratorGroup.RemovePrincipal(addedSids[i], RemovalReason.Timeout);
                            }
                        }

                        // TODO: This should be put back in, but it needs to account for the fact that
                        // some principals may be added without expiration times.
                        /*
                        else
                        { // The principal's rights never expire. This should never happen.
                          // Remove the principal from the administrator group.
#if DEBUG
                            string accountName = GetAccountNameFromSID(addedSids[i]);
                            ApplicationLog.WriteInformationEvent(string.Format("Principal {0} ({1}) has been removed from the Administrators group by an outside process. Removing the principal from Make Me Admin's list.", addedSids[i], string.IsNullOrEmpty(accountName) ? "unknown account" : accountName), EventID.DebugMessage);
#endif
                            LocalAdministratorGroup.RemovePrincipal(addedSids[i], RemovalReason.Timeout);

                            if (
                                (Settings.AutomaticAddAllowed != null) &&
                                (Settings.AutomaticAddAllowed.Length > 0) &&
                                (Shared.UserIsAuthorized(userIdentity, Settings.AutomaticAddAllowed, Settings.AutomaticAddDenied))
                               )
                            {
#if DEBUG
                                ApplicationLog.WriteInformationEvent("User is allowed to be automatically added!", EventID.DebugMessage);
#endif
                                LocalAdministratorGroup.AddPrincipal(userIdentity, null, null);
                            }
                        }
                        */
                    }
                    else
                    { // Principal's SID was not found in the local administrators group.

                        DateTime? expirationTime = encryptedSettings.GetExpirationTime(addedSids[i]);

                        if (expirationTime.HasValue)
                        { // The principal's rights expire at some point.
                            if (expirationTime.Value > DateTime.Now)
                            { // The principal's administrator rights expire in the future.
                                string accountName = GetAccountNameFromSID(addedSids[i]);
                                if (Settings.OverrideRemovalByOutsideProcess)
                                {
                                    // TODO: i18n.
                                    ApplicationLog.WriteInformationEvent(string.Format("Principal {0} ({1}) has been removed from the Administrators group by an outside process. Adding the principal back to the Administrators group.", addedSids[i], string.IsNullOrEmpty(accountName) ? "unknown account" : accountName), EventID.PrincipalRemovedByExternalProcess);
                                    AddPrincipalToAdministrators(addedSids[i], null);
                                }
                                else
                                {
                                    // TODO: i18n.
                                    ApplicationLog.WriteInformationEvent(string.Format("Principal {0} ({1}) has been removed from the Administrators group by an outside process. Removing the principal from Make Me Admin's list.", addedSids[i], string.IsNullOrEmpty(accountName) ? "unknown account" : accountName), EventID.PrincipalRemovedByExternalProcess);

                                    encryptedSettings.RemovePrincipal(addedSids[i]);
                                    /*
                                    Settings.SIDs = PrincipalList.GetSIDs().Select(p => p.Value).ToArray<string>();
                                    */
                                }
                            }
                            else
                            { // The principal's administrator rights have expired.
                              // No need to remove from the administrators group, as we already know the SID
                              // is not present in the group.
#if DEBUG
                                ApplicationLog.WriteInformationEvent(string.Format("Removing SID \"{0}\" from the principal list.", addedSids[i]), EventID.DebugMessage);
#endif
                                
                                encryptedSettings.RemovePrincipal(addedSids[i]);
                                /*
                                Settings.SIDs = PrincipalList.GetSIDs().Select(p => p.Value).ToArray<string>();
                                */
                            }
                        }
                        /*
                         * Rights shouldn't need to be removed here, as we already know the SID is not
                         * a member of the local administrator group.
                        else
                        { // The principal's rights never expire. This should never happen.
                          // Remove the principal from the administrator. group.
                            LocalAdministratorGroup.RemovePrincipal(addedSids[i], RemovalReason.Timeout);
                        }
                        */
                    }
                }
            }
        }

        private static string GetAccountNameFromSID(SecurityIdentifier sid)
        {
            if (sid.IsAccountSid() && sid.IsValidTargetType(typeof(NTAccount)))
            {
                try
                {
                    NTAccount account = (NTAccount)sid.Translate(typeof(NTAccount));
                    return account.Value;
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
        private static string GetAccountNameFromSID(string sidString)
        {
            SecurityIdentifier sid = new SecurityIdentifier(sidString);
            if (sid.IsAccountSid() && sid.IsValidTargetType(typeof(NTAccount)))
            {
                try
                {
                    NTAccount account = (NTAccount)sid.Translate(typeof(NTAccount));
                    return account.Value;
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
        */

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


        private static int AddLocalGroupMembers(string ServerName, string GroupName, SecurityIdentifier memberSid)
        {
            int returnValue = -1;
            NativeMethods.LOCALGROUP_MEMBERS_INFO_0 newMemberInfo = new NativeMethods.LOCALGROUP_MEMBERS_INFO_0();
            byte[] binarySid = new byte[memberSid.BinaryLength];
            memberSid.GetBinaryForm(binarySid, 0);

            IntPtr unmanagedPointer = System.Runtime.InteropServices.Marshal.AllocHGlobal(binarySid.Length);
            System.Runtime.InteropServices.Marshal.Copy(binarySid, 0, unmanagedPointer, binarySid.Length);
            newMemberInfo.lgrmi0_sid = unmanagedPointer;
            returnValue = NativeMethods.NetLocalGroupAddMembers(ServerName, GroupName, 0, ref newMemberInfo, 1);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(unmanagedPointer);
            return returnValue;
        }

        private static int RemoveLocalGroupMembers(string ServerName, string GroupName, SecurityIdentifier memberSid)
        {
            int returnValue = -1;
            NativeMethods.LOCALGROUP_MEMBERS_INFO_0 memberInfo = new NativeMethods.LOCALGROUP_MEMBERS_INFO_0();
            byte[] binarySid = new byte[memberSid.BinaryLength];
            memberSid.GetBinaryForm(binarySid, 0);

            IntPtr unmanagedPointer = System.Runtime.InteropServices.Marshal.AllocHGlobal(binarySid.Length);
            System.Runtime.InteropServices.Marshal.Copy(binarySid, 0, unmanagedPointer, binarySid.Length);
            memberInfo.lgrmi0_sid = unmanagedPointer;
            returnValue = NativeMethods.NetLocalGroupDelMembers(ServerName, GroupName, 0, ref memberInfo, 1);
            System.Runtime.InteropServices.Marshal.FreeHGlobal(unmanagedPointer);
            return returnValue;
        }

        private static SecurityIdentifier[] GetLocalGroupMembers(string ServerName, string GroupName)
        {
            SecurityIdentifier[] returnValue = null;
            IntPtr buffer = IntPtr.Zero;
            IntPtr Resume = IntPtr.Zero;

            int val = NativeMethods.NetLocalGroupGetMembers(ServerName, GroupName, 0, out buffer, -1, out int EntriesRead, out int TotalEntries, Resume);
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

        /*
        public static bool WindowsIdentityIsMember(WindowsIdentity identity)
        {
            bool isMember = false;
        */

            /*
            List<Principal> authGroups = Shared.GetAuthorizationGroups(UserPrincipal.Current)
            */
            /*.GetAuthorizationGroups();*/

            //foreach (SecurityIdentifier sid in identity.Groups)
            //{
            //    isMember |= (LocalAdminsGroupSid == sid);
            //    if (isMember) { break; }
            //}

            /*
            authGroups.Dispose();
            */

         /*   
            PrincipalSearchResult<System.DirectoryServices.AccountManagement.Principal> adminGroupMembers = LocalAdminGroup.GetMembers(true);
            foreach (System.DirectoryServices.AccountManagement.Principal prin in adminGroupMembers)
            {
                isMember |= (prin.Sid == identity.User);
                if (isMember) { break; }
            }
         */   

            /*
            foreach (SecurityIdentifier userGroupSid in identity.Groups)
            {
                isMember |= (userGroupSid == LocalAdminsGroupSid);
                if (isMember) { break; }
            }
            */

            //LocalAdminGroup.GetMembers(true).Contains(new System.DirectoryServices.AccountManagement.Principal

            /*
            return UserPrincipal.Current.GetAuthorizationGroups().Where(p => p.Sid.Equals(localAdminsGroupSid)).Count<Principal>() >= 1;
            */

        /*
            return isMember;
        }
        */

        /*
        public static bool CurrentUserIsMemberOfAdministratorsDirectly()
        {
            SecurityIdentifier[] localAdminSids = GetLocalGroupMembers(null, LocalAdminGroup.SamAccountName);
            WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();

            bool isMember = false;

            foreach (SecurityIdentifier sid in localAdminSids)
            {
                isMember = sid.Equals(currentIdentity.User);
                if (isMember) { break; }
            }

            return isMember;
        }
        */

        // TODO: This function needs to be commented.
        public static bool IsMemberOfAdministratorsDirectly(WindowsIdentity userIdentity)
        {
            SecurityIdentifier[] localAdminSids = GetLocalGroupMembers(null, LocalAdminGroup.SamAccountName);

            bool isMember = false;

            foreach (SecurityIdentifier sid in localAdminSids)
            {
                isMember = sid.Equals(userIdentity.User);
                if (isMember) { break; }
            }

            return isMember;
        }

        // TODO: This function needs to be commented.
        public static bool IsMemberOfAdministrators(WindowsIdentity userIdentity)
        {
            SecurityIdentifier[] localAdminSids = GetLocalGroupMembers(null, LocalAdminGroup.SamAccountName);

            bool isMember = false;

            foreach (SecurityIdentifier sid in localAdminSids)
            {
                isMember |= sid.Equals(userIdentity.User);
                if (isMember) { break; }
            }

            if (!isMember)
            {
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
        public static int EndNetworkSession(string remoteHost, string userName)
        {
            return NativeMethods.NetSessionDel(null, remoteHost, userName);
        }
    }
}
