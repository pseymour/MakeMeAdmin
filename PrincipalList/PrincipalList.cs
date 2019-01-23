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
    using System;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Security.Principal;

    // TODO: How do we know we are being passed a valid SID?

    /// <summary>
    /// Maintains a collection of security principals which have been added to the Administrators group.
    /// </summary>
    [Serializable]
    public class PrincipalList : KeyedCollection<SecurityIdentifier, Principal>
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public PrincipalList() : base()
        {
        }

        /// <summary>
        /// Gets the key (security identifier) for the specified principal.
        /// </summary>
        /// <param name="item">
        /// The principal whose SID is to be returned.
        /// </param>
        /// <returns>
        /// Returns the Security Identifier (SID) of the given principal.
        /// </returns>
        protected override SecurityIdentifier GetKeyForItem(Principal item)
        {
            return item.PrincipalSid;
        }

        /// <summary>
        /// Adds a principal's security ID (SID) to the collection.
        /// </summary>
        /// <param name="principalSecurityIdentifier">
        /// The SID to be added to the collection.
        /// </param>
        /// <param name="expirationTime">
        /// The date and time at which the principal's administrator rights expire.
        /// </param>
        /// <param name="remoteAddress">
        /// The address of the remote computer from which a request for administrator rights came.
        /// </param>
        internal void AddSID(SecurityIdentifier principalSecurityIdentifier, DateTime? expirationTime, string remoteAddress)
        {
            if (this.Contains(principalSecurityIdentifier))
            {
                this[principalSecurityIdentifier].RemoteAddress = remoteAddress;
            }
            else
            {
                this.Add(new Principal(principalSecurityIdentifier, expirationTime, remoteAddress));
            }
        }
                
        /// <summary>
        /// Removes a principal's security ID (SID) from the collection.
        /// </summary>
        /// <param name="sid">
        /// The SID to be removed from the collection.
        /// </param>
        internal void RemoveSID(SecurityIdentifier sid)
        {
            if (this.Contains(sid))
            {
                this.Remove(sid);
            }
        }

        /// <summary>
        /// Gets a list of the SIDs for all principals in the list.
        /// </summary>
        /// <returns>
        /// Returns an array of Security Identifiers (SIDs), one for each principal in the list.
        /// </returns>
        internal SecurityIdentifier[] GetSIDs()
        {
            return this.Select(p => p.PrincipalSid).ToArray<SecurityIdentifier>();
        }

        /// <summary>
        /// Gets a list of the principals whose administrator rights have expired.
        /// </summary>
        /// <returns>
        /// Returns an array containing the list of expired principals.
        /// </returns>
        internal Principal[] GetExpiredPrincipals()
        {
            return this.Where(p => (p.ExpirationTime.HasValue && (p.ExpirationTime <= DateTime.Now))).ToArray<Principal>();
        }

        /// <summary>
        /// Gets the expiration date and time for the administrator rights of the given principal.
        /// </summary>
        /// <param name="sid">
        /// The Security Identifier (SID) of the principal to be checked.
        /// </param>
        /// <returns>
        /// Returns the date and time at which the principal's administrator rights expire.
        /// If the principal is not in the list, null is returned.
        /// </returns>
        internal DateTime? GetExpirationTime(SecurityIdentifier sid)
        {
            if (this.Contains(sid))
            {
                return this[sid].ExpirationTime;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Determines whether the given principal has been added as a result of a remote request.
        /// </summary>
        /// <param name="sid">
        /// The Security Identifier (SID) of the principal to be checked.
        /// </param>
        /// <returns>
        /// Returns true if the given principal was added due to a remote request.
        /// Otherwise, false is returned.
        /// </returns>
        internal bool IsRemote(SecurityIdentifier sid)
        {
            if (this.Contains(sid))
            {
                return !string.IsNullOrEmpty(this[sid].RemoteAddress);
            }
            else
            {
                return false;
            }
        }
    }
}
