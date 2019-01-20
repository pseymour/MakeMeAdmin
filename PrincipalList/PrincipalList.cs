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
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    //using System.Security.Cryptography;
    using System.Security.Principal;
    using System.Xml;
    using System.Xml.Serialization;

    // TODO: How do we know we are being passed a valid SID?

    /// <summary>
    /// Maintains a collection of security principals which have been added to the Administrators group.
    /// </summary>
    [Serializable]
    public class PrincipalList : KeyedCollection<SecurityIdentifier, Principal>
    {
        public PrincipalList() : base()
        {

#if DEBUG
            try
            {
#endif
                /*
                // Retrieve the stored SID list from the settings.
                string[] storedSIDs = Settings.SIDs;
                if (storedSIDs != null)
                {
                    for (int i = 0; i < storedSIDs.Length; i++)
                    {
                        PrincipalList.AddSID(storedSIDs[i], DateTime.Now.AddMinutes(Settings.AdminRightsTimeout * -1), null);
                    }
                }
                */

#if DEBUG
            }
            catch (Exception excep)
            {
                ApplicationLog.WriteInformationEvent("error in PrincipalList.ReadSidsFromSettings(), getting SIDs from settings.", EventID.DebugMessage);
                ApplicationLog.WriteInformationEvent(excep.Message, EventID.DebugMessage);
            }
#endif
            

            /*
            try
            {
                Settings.SIDs = null;
            }
            catch (Exception excep)
            {
                ApplicationLog.WriteInformationEvent("clearing SIDs list in Settings", EventID.DebugMessage);
                ApplicationLog.WriteInformationEvent(excep.Message, EventID.DebugMessage);
            }
            */
        }

        protected override SecurityIdentifier GetKeyForItem(Principal item)
        {
            return item.PrincipalSid;
        }


        /// <summary>
        /// Adds a principal's security ID (SID) to the collection.
        /// </summary>
        /// <param name="sid">
        /// The SID to be added to the collection, in SDDL form.
        /// </param>
        /// <param name="expirationTime">
        /// The date and time at which the principal's administrator rights expire.
        /// </param>
        internal void AddSID(/* WindowsIdentity userIdentity */ SecurityIdentifier principalSecurityIdentifier, DateTime? expirationTime, string remoteAddress)
        {
            if (this.Contains(principalSecurityIdentifier))
            //if (principals.ContainsKey(/* userIdentity.User */ principalSecurityIdentifier))
            {
                // Set the expiration time for the given SID to the maximum of
                // its current value or the specified expiration time.
#if DEBUG
                ApplicationLog.WriteInformationEvent(string.Format("Setting expiration for SID {0} to {1}.", principalSecurityIdentifier.Value /* userIdentity.User.Value */, new[] { this[/* userIdentity.User */ principalSecurityIdentifier].ExpirationTime, expirationTime }.Max()), EventID.DebugMessage);
#endif
                /*
                principals[userIdentity.User].ExpirationTime = new[] { principals[userIdentity.User].ExpirationTime, expirationTime }.Max();
                */
                //principals[principalSecurityIdentifier /* userIdentity.User */].RemoteAddress = remoteAddress;
                this[principalSecurityIdentifier].RemoteAddress = remoteAddress;
#if DEBUG
                ApplicationLog.WriteInformationEvent(string.Format("SID list contains {0:N0} items.", this.Count), EventID.DebugMessage);
#endif
            }
            else
            {
#if DEBUG
                System.Text.StringBuilder message = new System.Text.StringBuilder("Adding SID ");
                message.Append(/* userIdentity.User.Value */ principalSecurityIdentifier.Value);
                message.Append(" to list with an expiration of ");
                if (expirationTime.HasValue)
                {
                    message.Append(expirationTime.Value);
                }
                else
                {
                    message.Append("never");
                }
                message.Append(".");
                ApplicationLog.WriteInformationEvent(message.ToString(), EventID.DebugMessage);
#endif

                this.Add(new Principal(principalSecurityIdentifier, expirationTime, remoteAddress));

                //principals.Add(principalSecurityIdentifier /* userIdentity.User */, new Principal(principalSecurityIdentifier, expirationTime, remoteAddress));

                /*
                Settings.SIDs = GetSIDs().Select(p => p.Value).ToArray<string>();
                */
            }
        }

        /*
        internal bool ContainsSID(SecurityIdentifier sid)
        {
            return this.Contains(sid);
        }
        */
        
        /// <summary>
        /// Removes a principal's security ID (SID) from the collection.
        /// </summary>
        /// <param name="sid">
        /// The SID to be removed from the collection, in SDDL form.
        /// </param>
        internal void RemoveSID(SecurityIdentifier sid)
        {
            if (this.Contains(sid))
            {
                this.Remove(sid);
                /*
                Settings.SIDs = GetSIDs().Select(p => p.Value).ToArray<string>();
                */
            }
        }

        internal SecurityIdentifier[] GetSIDs()
        {
            return this.Select(p => p.PrincipalSid).ToArray<SecurityIdentifier>();
        }

        internal Principal[] GetExpiredPrincipals()
        {
            /*
            This is what was returned when the principal list held the time at which admin rights were granted.
            Now, it holds the expiration time.

            return principals.Where(p => p.Value <= DateTime.Now.AddMinutes(-1 * Settings.AdminRightsTimeout)).Select(p => p.Key).ToArray<SecurityIdentifier>();
            */

            //return this.Where(p => (p.ExpirationTime.HasValue && (p.ExpirationTime <= DateTime.Now))).Select(p => p.Value).ToArray<Principal>();

            return this.Where(p => (p.ExpirationTime.HasValue && (p.ExpirationTime <= DateTime.Now))).ToArray<Principal>();
        }

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
