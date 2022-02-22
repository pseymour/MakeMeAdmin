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
    using System;

    /// <summary>
    /// This interface defines the WCF service contract.
    /// </summary>

    // JDM: I feel like since I'm changing the definition of the interface that I should change the ServiceContract namespace, to avoid confusion.
    [ServiceContract(Namespace = "https://github.com/misartg/MakeMeAdmin/2022/02/21/")]
    //[ServiceContract(Namespace = "http://apps.sinclair.edu/makemeadmin/2017/10/")]

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
        bool UserIsAuthorized(string[] allowedSidsList, string[] deniedSidsList);

        // JDM: We're adding this alternative to "UserIsAuthorized" where the user's identity is sent to the function for evaluation.
        //
        //      This is because on my systems/environmment, the identity obtained from inside the implementation UserIsAuthorized(2) function
        //      will refer to the SYSTEM user, rather than the logged in user. I think it's getting the user account of the user running the
        //      MakeMeAdmin service instead of the user that's logged in to the computer. 
        //
        //      I'm hoping by implementing a version that takes an identity that it will evaluate properly on my computers.
        //
        //      NOTE: I'm using the IntPtr for the Windows Identity "Token" because I had trouble sending a full Windows Identity object; I think
        //      it might be too complex to be easily serialized. Sending the token and building the object on this side seemed the simplest way
        //      out for a non-expert like me. 
        [OperationContract]
        bool UserIsAuthorizedWithIdentityToken(IntPtr userIdentityToken, string[] allowedSidsList, string[] deniedSidsList);
    }
}
