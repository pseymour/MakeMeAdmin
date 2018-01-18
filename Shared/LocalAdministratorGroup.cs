// <copyright file="LocalAdministratorGroup.cs" company="Sinclair Community College">
// Copyright (c) 2010-2017, Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices.AccountManagement;
    using System.IdentityModel.Claims;
    using System.Linq;
    using System.Security.Principal;
    using System.ServiceModel;

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
                        ApplicationLog.WriteErrorEvent(string.Format("InvalidEnumArgumentException: {0}", invalidEnumException.Message), EventID.DebugMessage);
                        throw;
                    }
                    catch (ArgumentException argException)
                    {
                        ApplicationLog.WriteErrorEvent(string.Format("ArgumentException: {0}", argException.Message), EventID.DebugMessage);
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

        private static GroupPrincipal LocalAdminGroup
        {
            get
            {
                if (LocalMachineContext == null)
                {
                    ApplicationLog.WriteErrorEvent("Local machine context is null.", EventID.DebugMessage);
                }
                else
                {
                    if (LocalAdminsGroupSid == null)
                    {
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
                                ApplicationLog.WriteErrorEvent(string.Format("Exception: {0}", exception.Message), EventID.DebugMessage);
                                throw;
                            }
                        }
                    }
                }
                return localAdminGroup;
            }
        }

        public static void AddPrincipal(WindowsIdentity userIdentity, DateTime expirationTime, string remoteAddress)
        {
            // TODO: Only do this if the principal is not a member of the group?

            /*
#if DEBUG
            ApplicationLog.WriteInformationEvent(string.Format("User is a member of {0:N0} groups.", userIdentity.Groups.Count), EventID.DebugMessage);
#endif

#if DEBUG
            ApplicationLog.WriteInformationEvent("Checking local allowed/denied list.", EventID.DebugMessage);
#endif
            */
            bool userIsAuthorized = Shared.UserIsAuthorized(userIdentity, Settings.LocalAllowedEntities, Settings.LocalDeniedEntities);
#if DEBUG
            ApplicationLog.WriteInformationEvent(string.Format("User is authorized: {0}", userIsAuthorized), EventID.DebugMessage);
#endif

            if (!string.IsNullOrEmpty(remoteAddress))
            { // Request is from a remote computer. Check the remote authorization list.
                /*
#if DEBUG
                ApplicationLog.WriteInformationEvent("Checking remote allowed/denied list.", EventID.DebugMessage);
#endif
                */
                userIsAuthorized &= Shared.UserIsAuthorized(userIdentity, Settings.RemoteAllowedEntities, Settings.RemoteDeniedEntities);
                /*
#if DEBUG
                ApplicationLog.WriteInformationEvent(string.Format("User is authorized: {0}", userIsAuthorized), EventID.DebugMessage);
#endif 
                */
            }

            if (
                (LocalAdminGroup != null) &&
                (userIdentity.User != null) && 
                (userIdentity.Groups != null) && 
                (userIsAuthorized)
               )
            {
                PrincipalList.AddSID(userIdentity, expirationTime, remoteAddress);
                AddPrincipalToAdministrators(userIdentity.User, /* expirationTime, */ remoteAddress);
            }
        }

        private static void AddPrincipalToAdministrators(SecurityIdentifier userSid, /* DateTime expirationTime, */ string remoteAddress)
        {
            int result = AddLocalGroupMembers(null, LocalAdminGroup.SamAccountName, userSid);
            if (result == 0)
            {
                /* PrincipalList.AddSID(userSid, expirationTime, remoteAddress); */
                ApplicationLog.WriteInformationEvent(string.Format("Principal {0} ({1}) added to the Administrators group.", userSid, GetAccountNameFromSID(userSid.Value)), EventID.UserAddedToAdminsSuccess);
                if (remoteAddress != null)
                {
                    ApplicationLog.WriteInformationEvent(string.Format("Request was sent from host {0}.", remoteAddress), EventID.RemoteRequestInformation);
                }
                Settings.SIDs = PrincipalList.GetSIDs().Select(p => p.Value).ToArray<string>();
            }
            else
            {
                ApplicationLog.WriteWarningEvent(string.Format("Adding principal {0} ({1}) to the Administrators group returned error code {2}.", userSid, GetAccountNameFromSID(userSid.Value), result), EventID.UserAddedToAdminsFailure);
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
                        string accountName = GetAccountNameFromSID(userSid.Value);
                        int result = RemoveLocalGroupMembers(null, LocalAdminGroup.SamAccountName, userSid);
                        if (result == 0)
                        {
                            PrincipalList.RemoveSID(userSid);
                            Settings.SIDs = PrincipalList.GetSIDs().Select(p => p.Value).ToArray<string>();
                            string reasonString = "Unknown";
                            switch (reason)
                            {
                                case RemovalReason.ServiceStopped:
                                    reasonString = "Make Me Admin service stopped";
                                    break;
                                case RemovalReason.Timeout:
                                    reasonString = "timeout elapsed";
                                    break;
                                case RemovalReason.UserLogoff:
                                    reasonString = "user logged off";
                                    break;
                                case RemovalReason.UserRequest:
                                    reasonString = "user requested removal";
                                    break;
                            }
                            string message = string.Format("Principal {0} ({1}) removed from the Administrators group. Reason: {2}.", userSid, accountName, reasonString);
                            ApplicationLog.WriteInformationEvent(message, EventID.UserRemovedFromAdminsSuccess);
                        }
                        else
                        {
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
            SecurityIdentifier[] addedSids = PrincipalList.GetSIDs();

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
                        if (PrincipalList.GetExpirationTime(addedSids[i]).HasValue)
                        { // The principal's rights expire at some point.
                            if (PrincipalList.GetExpirationTime(addedSids[i]).Value > DateTime.Now)
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
                        else
                        { // The principal's rights never expire. This should never happen.
                          // Remove the principal from the administrator group.
#if DEBUG
                            string accountName = GetAccountNameFromSID(addedSids[i]);
                            ApplicationLog.WriteInformationEvent(string.Format("Principal {0} ({1}) has been removed from the Administrators group by an outside process. Removing the principal from Make Me Admin's list.", addedSids[i], string.IsNullOrEmpty(accountName) ? "unknown account" : accountName), EventID.DebugMessage);
#endif
                            LocalAdministratorGroup.RemovePrincipal(addedSids[i], RemovalReason.Timeout);
                        }
                    }
                    else
                    { // Principal's SID was not found in the local administrators group.
                        if (PrincipalList.GetExpirationTime(addedSids[i]).HasValue)
                        { // The principal's rights expire at some point.
                            if (PrincipalList.GetExpirationTime(addedSids[i]).Value > DateTime.Now)
                            { // The principal's administrator rights expire in the future.
                                string accountName = GetAccountNameFromSID(addedSids[i]);
                                if (Settings.OverrideRemovalByOutsideProcess)
                                {
                                    ApplicationLog.WriteInformationEvent(string.Format("Principal {0} ({1}) has been removed from the Administrators group by an outside process. Adding the principal back to the Administrators group.", addedSids[i], string.IsNullOrEmpty(accountName) ? "unknown account" : accountName), EventID.PrincipalRemovedByExternalProcess);
                                    /* AddPrincipal(addedSids[i], DateTime.MinValue, null); */
                                    AddPrincipalToAdministrators(addedSids[i], /* DateTime.MinValue, */ null);
                                }
                                else
                                {
                                    ApplicationLog.WriteInformationEvent(string.Format("Principal {0} ({1}) has been removed from the Administrators group by an outside process. Removing the principal from Make Me Admin's list.", addedSids[i], string.IsNullOrEmpty(accountName) ? "unknown account" : accountName), EventID.PrincipalRemovedByExternalProcess);
                                    PrincipalList.RemoveSID(addedSids[i]);
                                    Settings.SIDs = PrincipalList.GetSIDs().Select(p => p.Value).ToArray<string>();
                                }
                            }
                            else
                            { // The principal's administrator rights have expired.
                              // No need to remove from the administrators group, as we already know the SID
                              // is not present in the group.
#if DEBUG
                                ApplicationLog.WriteInformationEvent(string.Format("Removing SID \"{0}\" from the principal list.", addedSids[i]), EventID.DebugMessage);
#endif
                                PrincipalList.RemoveSID(addedSids[i]);
                                Settings.SIDs = PrincipalList.GetSIDs().Select(p => p.Value).ToArray<string>();
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

            int EntriesRead;
            int TotalEntries;
            IntPtr buffer = IntPtr.Zero;
            IntPtr Resume = IntPtr.Zero;

            int val = NativeMethods.NetLocalGroupGetMembers(ServerName, GroupName, 0, out buffer, -1, out EntriesRead, out TotalEntries, Resume);
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
    }
}
