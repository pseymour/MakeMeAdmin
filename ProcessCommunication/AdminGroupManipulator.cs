// <copyright file="AdminGroupManipulator.cs" company="Sinclair Community College">
// Copyright (c) 2010-2017, Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Security.Principal;

    /* IncludeExceptionDetailInFaults = true */
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class AdminGroupManipulator : IAdminGroup
    {
        public void AddPrincipalToAdministratorsGroup()
        {
            string remoteAddress = null;

            WindowsIdentity userIdentity = null;

            if (ServiceSecurityContext.Current != null)
            {
                userIdentity = ServiceSecurityContext.Current.WindowsIdentity;
            }
            
#if DEBUG
            else
            {
                ApplicationLog.WriteWarningEvent("Current service security context is null.", EventID.DebugMessage);
            }
#endif


            if (OperationContext.Current != null)
            {
                if (OperationContext.Current.IncomingMessageProperties != null)
                {
                    if (OperationContext.Current.IncomingMessageProperties.ContainsKey(RemoteEndpointMessageProperty.Name))
                    {
                        remoteAddress = ((RemoteEndpointMessageProperty)OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name]).Address;
                    }
                }
            }

            
#if DEBUG
            if (remoteAddress != null)
            {
                string message = string.Format("Administrator rights request came from [{0}].", remoteAddress);
                ApplicationLog.WriteInformationEvent(message, EventID.DebugMessage);
            }
#endif
            
            if (userIdentity != null)
            {
                int timeoutMinutes = Shared.GetTimeoutForUser(userIdentity);
                DateTime expirationTime = DateTime.Now.AddMinutes(timeoutMinutes);
                LocalAdministratorGroup.AddPrincipal(userIdentity, expirationTime, remoteAddress);
            }
        }

        public void RemovePrincipalFromAdministratorsGroup(RemovalReason reason)
        {
            WindowsIdentity userIdentity = null;

            if (ServiceSecurityContext.Current != null)
            {
                userIdentity = ServiceSecurityContext.Current.WindowsIdentity;
            }

            if (userIdentity != null)
            {
                LocalAdministratorGroup.RemovePrincipal(userIdentity.User, reason);
            }
        }

        public bool PrincipalIsInList()
        {
            WindowsIdentity userIdentity = null;

            if (ServiceSecurityContext.Current != null)
            {
                userIdentity = ServiceSecurityContext.Current.WindowsIdentity;
            }

            if (userIdentity != null)
            {
                return PrincipalList.ContainsSID(userIdentity.User);
            }
            else
            {
                return false;
            }
        }
    }
}
