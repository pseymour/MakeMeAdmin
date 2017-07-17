// <copyright file="PrincipalList.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.Linq;

    // TODO: How do we know we are being passed a valid SID?

    /// <summary>
    /// Maintains a collection of security principals which have been added to the Administrators group.
    /// </summary>
    public class PrincipalList
    {
        /// <summary>
        /// A collection of security principals which have been added to the Administrators group,
        /// along with the time at which they were added.
        /// </summary>
        private static System.Collections.Generic.Dictionary<string, DateTime> principals = null;

        /// <summary>
        /// Initializes static members of the <see cref="PrincipalList"/> class.
        /// </summary>
        static PrincipalList()
        {
#if DEBUG
            try
            {
#endif
                principals = new System.Collections.Generic.Dictionary<string, DateTime>();
#if DEBUG
        }
            catch (Exception excep)
            {
                ApplicationLog.WriteInformationEvent("error in PrincipalList constructor, creating Dictionary object.", EventID.DebugMessage);
                ApplicationLog.WriteInformationEvent(excep.Message, EventID.DebugMessage);
        }
#endif

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
        public static void AddSID(string sid)
        {
            AddSID(sid, DateTime.Now.AddMinutes(Settings.AdminRightsTimeout));
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
        public static void AddSID(string sid, DateTime expirationTime)
        {
            if (principals.ContainsKey(sid))
            {
                // Set the expiration time for the given SID to the maximum of
                // its current value or the specified expiration time.
#if DEBUG
                ApplicationLog.WriteInformationEvent(string.Format("Setting expiration for SID {0} to {1}.", sid, new[] { principals[sid], expirationTime }.Max()), EventID.DebugMessage);
#endif
                principals[sid] = new[] { principals[sid], expirationTime }.Max();
#if DEBUG
                ApplicationLog.WriteInformationEvent(string.Format("SID list contains {0:N0} items.", principals.Count), EventID.DebugMessage);
                string[] sids = GetSIDs();
                foreach (string s in sids)
                {
                    ApplicationLog.WriteInformationEvent(string.Format("SID {0} expires at {1}.", s, principals[s]), EventID.DebugMessage);
                }
#endif
            }
            else
            {
#if DEBUG
                ApplicationLog.WriteInformationEvent(string.Format("Adding SID {0} to list with an expiration of {1}.", sid, expirationTime), EventID.DebugMessage);
#endif
                principals.Add(sid, expirationTime);

#if DEBUG
                ApplicationLog.WriteInformationEvent(string.Format("SID list contains {0:N0} items.", principals.Count), EventID.DebugMessage);
                string[] sids = GetSIDs();
                foreach (string s in sids)
                {
                    ApplicationLog.WriteInformationEvent(string.Format("SID {0} expires at {1}.", s, principals[s]), EventID.DebugMessage);
                }
#endif

                /*Settings.SIDs = GetSIDs();*/
            }
        }

        public static bool ContainsSID(string sid)
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
        public static void RemoveSID(string sid)
        {
            if (principals.ContainsKey(sid))
            {
#if DEBUG
                ApplicationLog.WriteInformationEvent(string.Format("Removing SID {0} from list.", sid), EventID.DebugMessage);
#endif
                principals.Remove(sid);

#if DEBUG
                ApplicationLog.WriteInformationEvent(string.Format("SID list contains {0:N0} items.", principals.Count), EventID.DebugMessage);
                string[] sids = GetSIDs();
                foreach (string s in sids)
                {
                    ApplicationLog.WriteInformationEvent(string.Format("SID {0} expires at {1}.", s, principals[s]), EventID.DebugMessage);
                }
#endif

                /*Settings.SIDs = GetSIDs();*/
            }
        }

        /// <summary>
        /// Gets an array of the SIDs in the collection.
        /// </summary>
        /// <returns>
        /// Returns an array of strings, containing SIDs in SDDL form.
        /// </returns>
        public static string[] GetSIDs()
        {
            return principals.Select(p => p.Key).ToArray<string>();
        }

        /// <summary>
        /// Gets an array of the SIDs in the collection which are past their expiration time.
        /// </summary>
        /// <returns>
        /// Returns an array of strings, containing SIDs in SDDL form.
        /// </returns>
        public static string[] GetExpiredSIDs()
        {
            /*
            This is what was returned when the principal list held the time at which admin rights were granted.
            Now, it holds the expiration time.

            return principals.Where(p => p.Value <= DateTime.Now.AddMinutes(-1 * Settings.AdminRightsTimeout)).Select(p => p.Key).ToArray<string>();
            */
            return principals.Where(p => p.Value <= DateTime.Now).Select(p => p.Key).ToArray<string>();
        }

        public static DateTime? GetExpirationTime(string sid)
        {
            if (principals.ContainsKey(sid))
            {
                return principals[sid];
            }
            else
            {
                return null;
            }
        }
    }
}
