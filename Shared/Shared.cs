// <copyright file="Shared.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices.AccountManagement;

    public class Shared
    {
        // TODO: This might not be an accurate way of getting the hostname.
        public static string GetFullyQualifiedHostName()
        {
            return System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).HostName.ToLowerInvariant();
        }

        public static List<Principal> GetAuthorizationGroups(UserPrincipal user)
        {
            PrincipalSearchResult<Principal> groups = user.GetAuthorizationGroups();
            List<Principal> ret = new List<Principal>();
            var iterGroup = groups.GetEnumerator();
            using (iterGroup)
            {
                while (iterGroup.MoveNext())
                {
                    try
                    {
                        Principal p = iterGroup.Current;
                        ret.Add(p);
                    }
                    catch (NoMatchingPrincipalException)
                    {
                        continue;
                    }
                }
            }
            groups.Dispose();
            return ret;
        }

        private static bool ArrayContainsString(string[] stringArray, string targetString)
        {
            if ((stringArray != null) && (stringArray.Length > 0))
            {
                for (int i = 0; i < stringArray.Length; i++)
                {
                    if (string.Compare(stringArray[i], targetString, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.CompareOptions.IgnoreCase) == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public static bool UserIsAuthorized(UserPrincipal user)
        {
            // Get a list of the user's authorization groups.
            List<Principal> authGroups = Shared.GetAuthorizationGroups(user); /* user.GetAuthorizationGroups(); */

            string[] deniedEntities = Settings.DeniedEntities;
            string[] allowedEntities = Settings.AllowedEntities;

            if ((deniedEntities != null) && (deniedEntities.Length > 0))
            { // The denied list contains entries. Check the user against that list first.

                // If the user's SID is in the denied list, the user is not authorized.
                if (ArrayContainsString(deniedEntities, user.Sid.Value)) { return false; }

                // If any of the user's authorization groups are in the denied list, the user is not authorized.
                foreach (Principal prin in authGroups)
                {
                    if (ArrayContainsString(deniedEntities, prin.Sid.Value)) { return false; }
                }
            }

            // The user hasn't been denied yet, so now we check the authorization list.

            if (allowedEntities == null)
            { // The allowed list is null, meaning everyone is allowed administrator rights.
                return true;
            }
            else if (allowedEntities.Length == 0)
            { // The allowed list is empty, meaning no one is allowed administrator rights.
                return false;
            }
            else
            { // The allowed list has entries.

                // If the user's SID is in the allowed list, the user is authorized.
                if (ArrayContainsString(allowedEntities, user.Sid.Value))
                {
                    return true;
                }

                // If any of the user's authorization groups are in the allowed list, the user is authorized.
                foreach (Principal prin in authGroups)
                {
                    if (ArrayContainsString(allowedEntities, prin.Sid.Value))
                    {
                        return true;
                    }
                }

                // The user was not found in the allowed list, so the user is not authorized.
                return false;
            }
        }


        /*
        public static bool UserHasAdminToken(out bool isDenyOnly)
        {
            isDenyOnly = false;
            return NativeMethods.UserHasAdminToken(out isDenyOnly);
        }
        */
    }
}
