// <copyright file="AdminGroupManipulator.cs" company="Sinclair Community College">
// Copyright (c) 2010-2017, Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.Collections.Generic;
    using System.ServiceModel;
    using System.Security.Principal;

    /* IncludeExceptionDetailInFaults = true */
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class AdminGroupManipulator : IAdminGroup
    {
        public void AddPrincipalToAdministratorsGroup(string principalSID, DateTime expirationTime)
        {
            /*
#if DEBUG
            ApplicationLog.WriteInformationEvent("In Service.ServiceContract.AddPrincipalToAdministratorsGroup.", EventID.DebugMessage);
#endif
            */
            if (!string.IsNullOrEmpty(principalSID) /*&& Shared.UserIsAuthorized(WindowsIdentity.GetCurrent())*/)
            {
                /*
#if DEBUG
                ApplicationLog.WriteInformationEvent("SID is not null or empty, and user is authorized.", EventID.DebugMessage);
#endif
                */
                LocalAdministratorGroup.AddPrincipal(principalSID, expirationTime);
            }

            /*
#if DEBUG
            ApplicationLog.WriteInformationEvent("Leaving Service.ServiceContract.AddPrincipalToAdministratorsGroup.", EventID.DebugMessage);
#endif
            */
        }

        public void RemovePrincipalFromAdministratorsGroup(string principalSID, RemovalReason reason)
        {
            /*
#if DEBUG
            ApplicationLog.WriteInformationEvent("In Service.ServiceContract.RemovePrincipalFromAdministratorsGroup", EventID.DebugMessage);
#endif
            */
            LocalAdministratorGroup.RemovePrincipal(principalSID, reason);
            /*
#if DEBUG
            ApplicationLog.WriteInformationEvent("Leaving Service.ServiceContract.RemovePrincipalFromAdministratorsGroup", EventID.DebugMessage);
#endif
            */
        }

        public bool PrincipalIsInList(string principalSid)
        {
            return PrincipalList.ContainsSID(principalSid);
        }
    }
}
