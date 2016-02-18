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
            principals = new System.Collections.Generic.Dictionary<string, DateTime>();

            // Retrieve the stored SID list from the settings.
            string[] storedSIDs = Settings.SIDs;
            if (storedSIDs != null)
            {
                for (int i = 0; i < storedSIDs.Length; i++)
                {
                    PrincipalList.AddSID(storedSIDs[i], DateTime.Now.AddMinutes(Settings.AdminRightsTimeout * -1));
                }
            }

            Settings.SIDs = null;
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
            if (!principals.ContainsKey(sid))
            {
                principals.Add(sid, expirationTime);
                Settings.SIDs = GetSIDs();
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
                principals.Remove(sid);
                Settings.SIDs = GetSIDs();
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
    }
}
