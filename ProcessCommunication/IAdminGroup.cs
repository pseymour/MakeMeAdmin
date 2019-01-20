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
    using System.ServiceModel;

    /// <summary>
    /// This interface defines the WCF service contract.
    /// </summary>
    [ServiceContract(Namespace = "http://apps.sinclair.edu/makemeadmin/2017/10/")]
    public interface IAdminGroup
    {
        /// <summary>
        /// Adds a security principal to the Administrators group.
        /// </summary>
        [OperationContract]
        void AddPrincipalToAdministratorsGroup();

        /// <summary>
        /// Removes a security principal from the Administrators group.
        /// </summary>
        /// <param name="reason">
        /// The reason that the principal is being removed.
        /// </param>
        [OperationContract]
        void RemovePrincipalFromAdministratorsGroup(RemovalReason reason);

        /// <summary>
        /// Returns a value indicating whether a security principal is 
        /// already in the list of added security principals.
        /// </summary>
        /// <returns>
        /// Returns true if the security principal is already in the list
        /// of added principals.
        /// </returns>
        [OperationContract]
        bool PrincipalIsInList();
    }
}
