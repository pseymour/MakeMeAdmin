// <copyright file="LocalAdministratorGroup.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices.AccountManagement;
    using System.Security.Principal;
    using System.Linq;

    public static class LocalAdministratorGroup
    {
        private static PrincipalContext localMachineContext = null;
        private static System.Security.Principal.SecurityIdentifier localAdminsGroupSid = null;
        private static GroupPrincipal localAdminGroup = null;

        static LocalAdministratorGroup()
        {
            localMachineContext = new PrincipalContext(ContextType.Machine);
            localAdminsGroupSid = new System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.BuiltinAdministratorsSid, null);
            localAdminGroup = GroupPrincipal.FindByIdentity(localMachineContext, IdentityType.Sid, localAdminsGroupSid.Value);
        }

        public static void AddPrincipal(string principalSID, DateTime expirationTime)
        {
            // TODO: Only do this if the principal is not a member of the group?

            if ((localAdminGroup != null) && (!string.IsNullOrEmpty(principalSID)))
            {
                string accountName = GetAccountNameFromSID(principalSID);
                int result = AddLocalGroupMembers(null, localAdminGroup.SamAccountName, new SecurityIdentifier(principalSID));
                if (result == 0)
                {
                    PrincipalList.AddSID(principalSID, expirationTime);
                    ApplicationLog.WriteInformationEvent(string.Format("Principal \"{0}\" ({1}) added to the Administrators group.", principalSID, string.IsNullOrEmpty(accountName) ? "unknown account" : accountName), EventID.UserAddedToAdminsSuccess);
                }
                else
                {
                    ApplicationLog.WriteWarningEvent(string.Format("Adding principal \"{0}\" ({1}) to the Administrators group returned error code {2}.", principalSID, string.IsNullOrEmpty(accountName) ? "unknown account" : accountName, result), EventID.UserAddedToAdminsFailure);
                }
            }
        }

        public static void RemovePrincipal(string principalSID)
        {
            // TODO: Only do this if the principal is a member of the group?

            if ((localAdminGroup != null) && (!string.IsNullOrEmpty(principalSID)))
            {
                System.Security.Principal.SecurityIdentifier[] localAdminSids = GetLocalGroupMembers(null, localAdminGroup.SamAccountName);

                foreach (SecurityIdentifier sid in localAdminSids)
                {
                    if (string.Compare(sid.Value, principalSID, true) == 0)
                    {
                        string accountName = GetAccountNameFromSID(principalSID);
                        int result = RemoveLocalGroupMembers(null, localAdminGroup.SamAccountName, new SecurityIdentifier(principalSID));
                        if (result == 0)
                        {
                            PrincipalList.RemoveSID(principalSID);
                            ApplicationLog.WriteInformationEvent(string.Format("Principal \"{0}\" ({1}) removed from the Administrators group.", principalSID, string.IsNullOrEmpty(accountName) ? "unknown account" : accountName), EventID.UserRemovedFromAdminsSuccess);
                        }
                        else
                        {
                            ApplicationLog.WriteWarningEvent(string.Format("Removing principal \"{0}\" ({1}) from the Administrators group returned error code {1}.", principalSID, string.IsNullOrEmpty(accountName) ? "unknown account" : accountName, result), EventID.UserRemovedFromAdminsFailure);
                        }
                    }
                }
            }
        }

        public static void ValidateAllAddedPrincipals()
        {
            System.Security.Principal.SecurityIdentifier[] localAdminSids = null;
            if (localAdminGroup != null)
            {
                localAdminSids = GetLocalGroupMembers(null, localAdminGroup.SamAccountName);
            }

            string[] allSids = PrincipalList.GetSIDs();

            for (int i = 0; i < allSids.Length; i++)
            {
                bool sidFoundInAdminsGroup = false;
                if ((!string.IsNullOrEmpty(allSids[i])) && (localAdminSids != null))
                {
                    foreach (SecurityIdentifier sid in localAdminSids)
                    {
                        if (string.Compare(sid.Value, allSids[i], true) == 0)
                        {
                            sidFoundInAdminsGroup = true;
                            break;
                        }
                    }

                    if (!sidFoundInAdminsGroup)
                    {
                        string accountName = GetAccountNameFromSID(allSids[i]);
                        PrincipalList.RemoveSID(allSids[i]);
                        ApplicationLog.WriteInformationEvent(string.Format("Principal \"{0}\" ({1}) has been removed from the Administrators group by an outside process. Removing the principal from Make Me Admin's list.", allSids[i], string.IsNullOrEmpty(accountName) ? "unknown account" : accountName), EventID.PrincipalRemovedByExternalProcess);
                    }
                }
            }
        }

        private static string GetAccountNameFromSID(string sddlSid)
        {
            System.Security.Principal.SecurityIdentifier sid = new SecurityIdentifier(sddlSid);
            if (sid.IsAccountSid() && sid.IsValidTargetType(typeof(System.Security.Principal.NTAccount)))
            {
                try
                {
                    System.Security.Principal.NTAccount account = (System.Security.Principal.NTAccount)sid.Translate(typeof(System.Security.Principal.NTAccount));
                    return account.Value;
                }
                catch (System.Security.Principal.IdentityNotMappedException)
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

        private static int AddLocalGroupMembers(string ServerName, string GroupName, System.Security.Principal.SecurityIdentifier memberSid)
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

        private static int RemoveLocalGroupMembers(string ServerName, string GroupName, System.Security.Principal.SecurityIdentifier memberSid)
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

        private static System.Security.Principal.SecurityIdentifier[] GetLocalGroupMembers(string ServerName, string GroupName)
        {
            System.Security.Principal.SecurityIdentifier[] returnValue = null;

            int EntriesRead;
            int TotalEntries;
            IntPtr buffer = IntPtr.Zero;
            IntPtr Resume = IntPtr.Zero;

            int val = NativeMethods.NetLocalGroupGetMembers(ServerName, GroupName, 0, out buffer, -1, out EntriesRead, out TotalEntries, Resume);
            if (EntriesRead > 0)
            {
                returnValue = new System.Security.Principal.SecurityIdentifier[EntriesRead];
                NativeMethods.LOCALGROUP_MEMBERS_INFO_0[] Members = new NativeMethods.LOCALGROUP_MEMBERS_INFO_0[EntriesRead];
                IntPtr iter = buffer;
                for (int i = 0; i < EntriesRead; i++)
                {
                    Members[i] = (NativeMethods.LOCALGROUP_MEMBERS_INFO_0)System.Runtime.InteropServices.Marshal.PtrToStructure(iter, typeof(NativeMethods.LOCALGROUP_MEMBERS_INFO_0));
                    iter = (IntPtr)((long)iter + System.Runtime.InteropServices.Marshal.SizeOf(typeof(NativeMethods.LOCALGROUP_MEMBERS_INFO_0)));
                    if (Members[i].lgrmi0_sid != IntPtr.Zero)
                    {
                        System.Security.Principal.SecurityIdentifier sid = new System.Security.Principal.SecurityIdentifier(Members[i].lgrmi0_sid);
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
            System.Security.Principal.SecurityIdentifier[] localAdminSids = GetLocalGroupMembers(null, localAdminGroup.SamAccountName);
            System.Security.Principal.WindowsIdentity currentIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();

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
