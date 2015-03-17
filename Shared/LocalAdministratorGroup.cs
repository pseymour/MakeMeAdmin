// <copyright file="LocalAdministratorGroup.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices.AccountManagement;
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

        public static void AddPrincipal(ContextType contextType, string contextName, string principalSID)
        {
            PrincipalContext userContext = new PrincipalContext(contextType, contextName);
            UserPrincipal user = UserPrincipal.FindByIdentity(userContext, IdentityType.Sid, principalSID);
            if ((localAdminGroup != null) && (user != null) && (!user.IsMemberOf(localAdminGroup)))
            {
                localAdminGroup.Members.Add(user);
                localAdminGroup.Save();
                PrincipalList.AddSID(principalSID);
                ApplicationLog.WriteInformationEvent(string.Format("User \"{0}\" added to the Administrators group.", user.SamAccountName), EventID.UserAddedToAdminsSuccess);                
            }
        }
        
        /*
        public static void AddUser(UserPrincipal user)
        {
            if ((localAdminGroup != null) && (user != null) && (!user.IsMemberOf(localAdminGroup)))
            {
                localAdminGroup.Members.Add(user);
                localAdminGroup.Save();
            }
        }
        */

        /*
        public static void RemovePrincipal(ContextType contextType, string contextName, string principalSID)
        {
            PrincipalContext userContext = new PrincipalContext(contextType, contextName);
            UserPrincipal user = UserPrincipal.FindByIdentity(userContext, IdentityType.Sid, principalSID);
            if ((localAdminGroup != null) && (user != null) && (user.IsMemberOf(localAdminGroup)))
            {
                localAdminGroup.Members.Remove(user);
                localAdminGroup.Save();
            }
        }
        */

        public static void RemovePrincipal(string principalSID)
        {
            bool groupModified = false;
            /*
            bool principalWasInGroupInitially = false;
            bool principalIsInGroupNow = false;
            */
            if (localAdminGroup != null)
            {
                PrincipalSearchResult<Principal> administrators = localAdminGroup.GetMembers(false);
                foreach (Principal p in administrators)
                {
                    if (string.Compare(p.Sid.Value, principalSID, true) == 0)
                    {
                        /*
                        principalWasInGroupInitially = true;
                        */
                        localAdminGroup.Members.Remove(p);
                        groupModified = true;
                    }
                }
                if (groupModified)
                {
                    ApplicationLog.WriteInformationEvent(string.Format("Principal \"{0}\" removed from the Administrators group.", principalSID), EventID.UserRemovedFromAdminsSuccess);
                    localAdminGroup.Save();
                    PrincipalList.RemoveSID(principalSID);
                    administrators = localAdminGroup.GetMembers(false);
                    foreach (Principal p in administrators)
                    {
                        if (string.Compare(p.Sid.Value, principalSID, true) == 0)
                        {
                            /*
                            principalIsInGroupNow = true;
                            */
                            break;
                        }
                    }
                }
            }

            /*return (!principalWasInGroupInitially) | (!principalIsInGroupNow);*/
        }

        public static bool CurrentUserIsMember()
        {
            bool isMember = false;

            List<Principal> authGroups = Shared.GetAuthorizationGroups(UserPrincipal.Current) /*.GetAuthorizationGroups()*/;
            foreach (Principal p in authGroups)
            {
                isMember = p.Sid.Equals(localAdminsGroupSid);
                if (isMember) { break; }
            }
            
            /*
            authGroups.Dispose();
            */

            return isMember;

            /*
            return UserPrincipal.Current.GetAuthorizationGroups().Where(p => p.Sid.Equals(localAdminsGroupSid)).Count<Principal>() >= 1;
            */
        }

        // TODO: This function could probably be made faster.
        public static bool CurrentUserIsMemberOfAdministratorsDirectly()
        {
            bool returnValue = false;

            if (UserPrincipal.Current != null)
            {
                returnValue = UserPrincipal.Current.IsMemberOf(localAdminGroup);
            }

            return returnValue;
        }
    }
}
