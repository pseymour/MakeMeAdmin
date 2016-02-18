// <copyright file="ServiceContract.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Security.Principal;

    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class ServiceContract : IServiceContract
    {
        public void AddPrincipalToAdministratorsGroup(string principalSID, DateTime expirationTime)
        {
#if DEBUG
            ApplicationLog.WriteInformationEvent("In Service.ServiceContract.AddPrincipalToAdministratorsGroup.", EventID.DebugMessage);
#endif
            if (!string.IsNullOrEmpty(principalSID) /*&& Shared.UserIsAuthorized(WindowsIdentity.GetCurrent())*/)
            {
#if DEBUG
                ApplicationLog.WriteInformationEvent("SID is not null or empty, and user is authorized.", EventID.DebugMessage);
#endif
                LocalAdministratorGroup.AddPrincipal(principalSID, expirationTime);
            }

#if DEBUG
            ApplicationLog.WriteInformationEvent("Leaving Service.ServiceContract.AddPrincipalToAdministratorsGroup.", EventID.DebugMessage);
#endif
        }

        public void RemovePrincipalFromAdministratorsGroup(string principalSID)
        {
#if DEBUG
            ApplicationLog.WriteInformationEvent("In Service.ServiceContract.RemovePrincipalFromAdministratorsGroup", EventID.DebugMessage);
#endif
            LocalAdministratorGroup.RemovePrincipal(principalSID);
#if DEBUG
            ApplicationLog.WriteInformationEvent("Leaving Service.ServiceContract.RemovePrincipalFromAdministratorsGroup", EventID.DebugMessage);
#endif
        }
    }
}
