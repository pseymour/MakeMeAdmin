// 
// Copyright © 2010-2018, Sinclair Community College
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
