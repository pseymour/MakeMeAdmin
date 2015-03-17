// <copyright file="ServiceContract.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices.AccountManagement;
    using System.ServiceModel;

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceContract : IServiceContract
    {
        public void AddPrincipalToAdministratorsGroup(ContextType contextType, string contextName, string principalSID)
        {
            PrincipalContext userContext = new PrincipalContext(contextType, contextName);
            UserPrincipal user = UserPrincipal.FindByIdentity(userContext, IdentityType.Sid, principalSID);
            if ((user != null) && Shared.UserIsAuthorized(user))
            {
                LocalAdministratorGroup.AddPrincipal(contextType, contextName, principalSID);
            }
            user.Dispose();
            userContext.Dispose();
        }

        public void RemoveUserFromAdministratorsGroup(string principalSID)
        {
            LocalAdministratorGroup.RemovePrincipal(principalSID);
        }
    }
}
