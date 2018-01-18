// <copyright file="PrincipalList.cs" company="Sinclair Community College">
// Copyright (c) 2010-2017, Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Principal;

    // TODO: How do we know we are being passed a valid SID?

    /// <summary>
    /// Maintains a collection of security principals which have been added to the Administrators group.
    /// </summary>
    public class PrincipalList
    {
        /// <summary>
        /// A collection of security principals which have been added to the Administrators group.
        /// </summary>
        private static Dictionary<SecurityIdentifier, Principal> principals = null;

        /// <summary>
        /// Initializes static members of the <see cref="PrincipalList"/> class.
        /// </summary>
        static PrincipalList()
        {
#if DEBUG
            try
            {
#endif
                principals = new Dictionary<SecurityIdentifier, Principal>();
#if DEBUG
            }
            catch (Exception excep)
            {
                ApplicationLog.WriteInformationEvent("error in PrincipalList constructor, creating Dictionary object.", EventID.DebugMessage);
                ApplicationLog.WriteInformationEvent(excep.Message, EventID.DebugMessage);
            }
#endif


            // TODO: Put this back, to remove SIDs that remain after a reboot?
            /*
#if DEBUG
            try
            {
#endif

                // Retrieve the stored SID list from the settings.
                string[] storedSIDs = Settings.SIDs;
                if (storedSIDs != null)
                {
                    for (int i = 0; i < storedSIDs.Length; i++)
                    {
                        PrincipalList.AddSID(storedSIDs[i], DateTime.Now.AddMinutes(Settings.AdminRightsTimeout * -1));
                    }
                }

#if DEBUG
            }
            catch (Exception excep)
            {
                ApplicationLog.WriteInformationEvent("error in PrincipalList.ReadSidsFromSettings(), getting SIDs from settings.", EventID.DebugMessage);
                ApplicationLog.WriteInformationEvent(excep.Message, EventID.DebugMessage);
            }
#endif
            */

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

        /// <summary>
        /// Prevents a default instance of the <see cref="PrincipalList"/> class from being created.
        /// </summary>
        private PrincipalList()
        {
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
        public static void AddSID(WindowsIdentity userIdentity, DateTime expirationTime, string remoteAddress)
        {
            if (principals.ContainsKey(userIdentity.User))
            {
                // Set the expiration time for the given SID to the maximum of
                // its current value or the specified expiration time.
#if DEBUG
                ApplicationLog.WriteInformationEvent(string.Format("Setting expiration for SID {0} to {1}.", userIdentity.User.Value, new[] { principals[userIdentity.User].ExpirationTime, expirationTime }.Max()), EventID.DebugMessage);
#endif
                /*
                principals[userIdentity.User].ExpirationTime = new[] { principals[userIdentity.User].ExpirationTime, expirationTime }.Max();
                */
                principals[userIdentity.User].RemoteAddress = remoteAddress;

#if DEBUG
                ApplicationLog.WriteInformationEvent(string.Format("SID list contains {0:N0} items.", principals.Count), EventID.DebugMessage);
#endif
            }
            else
            {
#if DEBUG
                ApplicationLog.WriteInformationEvent(string.Format("Adding SID {0} to list with an expiration of {1}.", sid, expirationTime), EventID.DebugMessage);
#endif
                
                principals.Add(userIdentity.User, new Principal(userIdentity, expirationTime, remoteAddress));
                Settings.SIDs = GetSIDs().Select(p => p.Value).ToArray<string>();
            }
        }

        public static bool ContainsSID(SecurityIdentifier sid)
        {
            if (principals == null)
            {
                return false;
            }
            else
            {
                return principals.ContainsKey(sid);
            }
        }
        
        /// <summary>
        /// Removes a principal's security ID (SID) from the collection.
        /// </summary>
        /// <param name="sid">
        /// The SID to be removed from the collection, in SDDL form.
        /// </param>
        public static void RemoveSID(SecurityIdentifier sid)
        {
            if (principals.ContainsKey(sid))
            {
                principals.Remove(sid);
                Settings.SIDs = GetSIDs().Select(p => p.Value).ToArray<string>();
            }
        }

        public static SecurityIdentifier[] GetSIDs()
        {
            return principals.Select(p => p.Key).ToArray<SecurityIdentifier>();
        }

        public static Principal[] GetExpiredPrincipals()
        {
            /*
            This is what was returned when the principal list held the time at which admin rights were granted.
            Now, it holds the expiration time.

            return principals.Where(p => p.Value <= DateTime.Now.AddMinutes(-1 * Settings.AdminRightsTimeout)).Select(p => p.Key).ToArray<SecurityIdentifier>();
            */
            return principals.Where(p => p.Value.ExpirationTime <= DateTime.Now).Select(p => p.Value).ToArray<Principal>();
        }

        public static DateTime? GetExpirationTime(SecurityIdentifier sid /* string sidString */)
        {
            if (principals.ContainsKey(sid))
            {
                return principals[sid].ExpirationTime;
            }
            else
            {
                return null;
            }
        }
    }
}
