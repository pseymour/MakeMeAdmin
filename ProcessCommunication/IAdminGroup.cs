﻿// 
// Copyright © 2010-2025, Sinclair Community College
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
    using System.Security.Principal;
    using System.ServiceModel;

    /// <summary>
    /// This interface defines the WCF service contract.
    /// </summary>
    [ServiceContract(Namespace = "http://apps.sinclair.edu/makemeadmin/2017/10/")]
    public interface IAdminGroup
    {
        /// <summary>
        /// Adds a user to the Administrators group.
        /// </summary>
        [OperationContract]
        void AddUserToAdministratorsGroup();

        /// <summary>
        /// Removes a user from the Administrators group.
        /// </summary>
        /// <param name="reason">
        /// The reason that the user is being removed.
        /// </param>
        [OperationContract]
        void RemoveUserFromAdministratorsGroup(RemovalReason reason);

        /// <summary>
        /// Returns a value indicating whether a user is 
        /// already in the list of added users.
        /// </summary>
        /// <returns>
        /// Returns true if the users is already in the list
        /// of added users.
        /// </returns>
        [OperationContract]
        bool UserIsInList();

        [OperationContract]
        bool UserSessionIsInList(int sessionID);

        [OperationContract]
        bool UserIsAuthorized(WindowsIdentity userIdentity, string[] allowedSidsList, string[] deniedSidsList);
    }
}
