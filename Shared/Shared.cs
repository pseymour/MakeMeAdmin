// <copyright file="Shared.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.Collections.Generic;
    using System.DirectoryServices.AccountManagement;
    using System.Security.Principal;

    public class Shared
    {
        public static string GetFullyQualifiedHostName()
        {
            return System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).HostName.ToLowerInvariant();
        }

        public static List<SecurityIdentifier> GetAuthorizationGroups(WindowsIdentity identity)
        {
#if DEBUG
            ApplicationLog.WriteInformationEvent("In Shared.Shared.GetAuthorizationGroups.", EventID.DebugMessage);
#endif

            List<SecurityIdentifier> returnList = new List<SecurityIdentifier>();

#if DEBUG
            ApplicationLog.WriteInformationEvent(string.Format("Retrieving the group memberships for {0}.", identity.Name), EventID.DebugMessage);
#endif

            foreach (System.Security.Principal.IdentityReference reference in identity.Groups)
            {
                /*
#if DEBUG
                ApplicationLog.WriteInformationEvent(string.Format("Found identity reference {0}.", reference.Value), EventID.DebugMessage);
#endif
                */

                SecurityIdentifier sid = reference as SecurityIdentifier;
                if (sid != null)
                {
                    returnList.Add(sid);
                }
            }

#if DEBUG
            ApplicationLog.WriteInformationEvent("Finished retrieving group memberships for the Windows identity.", EventID.DebugMessage);
#endif

#if DEBUG
            ApplicationLog.WriteInformationEvent("Leaving Shared.Shared.GetAuthorizationGroups.", EventID.DebugMessage);
#endif

            return returnList;
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

        public static int GetTimeoutForUser(WindowsIdentity userIdentity)
        {
            Dictionary<string, string> overrides = Settings.TimeoutOverrides;

            int overrideMinutes = int.MinValue;
            int timeoutMinutes = Settings.AdminRightsTimeout;

            if (overrides.ContainsKey(userIdentity.User.Value))
            {
                overrideMinutes = int.MinValue;
                if (int.TryParse(overrides[userIdentity.User.Value], out overrideMinutes))
                {
                    timeoutMinutes = Math.Max(timeoutMinutes, overrideMinutes);
                }
            }

            List<SecurityIdentifier> authGroups = Shared.GetAuthorizationGroups(userIdentity);
            foreach (SecurityIdentifier sid in authGroups)
            {
                if (overrides.ContainsKey(sid.Value))
                {
                    overrideMinutes = int.MinValue;
                    if (int.TryParse(overrides[sid.Value], out overrideMinutes))
                    {
                        timeoutMinutes = Math.Max(timeoutMinutes, overrideMinutes);
                    }
                }
            }

            return timeoutMinutes;
        }

        public static bool UserIsAuthorized(/*SecurityIdentifier principalSid*/ WindowsIdentity userIdentity )
        {
            // Get a list of the user's authorization groups.
            /*List<Principal> authGroups = Shared.GetAuthorizationGroups(user);*/ /* user.GetAuthorizationGroups(); */
#if DEBUG
            ApplicationLog.WriteInformationEvent("In Shared.Shared.UserIsAuthorized.", EventID.DebugMessage);
#endif
            List<SecurityIdentifier> userSids = Shared.GetAuthorizationGroups(userIdentity);

#if DEBUG
            ApplicationLog.WriteInformationEvent("Retrieving denied and allowed lists.", EventID.DebugMessage);
#endif
            string[] deniedEntities = Settings.DeniedEntities;
            string[] allowedEntities = Settings.AllowedEntities;

#if DEBUG
            if (deniedEntities != null)
            {
                ApplicationLog.WriteInformationEvent(string.Format("Denied list contains {0:N0} entries.", deniedEntities.Length), EventID.DebugMessage);
            }
            if (allowedEntities != null)
            {
                ApplicationLog.WriteInformationEvent(string.Format("Allowed list contains {0:N0} entries.", allowedEntities.Length), EventID.DebugMessage);
            }
#endif

#if DEBUG
            ApplicationLog.WriteInformationEvent("Checking security principal against allowed and denied list.", EventID.DebugMessage);
#endif

            if ((deniedEntities != null) && (deniedEntities.Length > 0))
            { // The denied list contains entries. Check the user against that list first.

#if DEBUG
                ApplicationLog.WriteInformationEvent("Denied list contains entries.", EventID.DebugMessage);
#endif

                // If the user's SID is in the denied list, the user is not authorized.
                /* if (ArrayContainsString(deniedEntities, user.Sid.Value)) { return false; } */
                if (ArrayContainsString(deniedEntities, userIdentity.User.Value))
                {
#if DEBUG
                    ApplicationLog.WriteInformationEvent("Principal's SID is in the denied list. Permission denied.", EventID.DebugMessage);
#endif
                    return false;
                }

                // If any of the user's authorization groups are in the denied list, the user is not authorized.
                /*
                foreach (Principal prin in authGroups)
                {
                    if (ArrayContainsString(deniedEntities, prin.Sid.Value)) { return false; }
                }
                */
                foreach (SecurityIdentifier sid in userSids)
                {
                    if (ArrayContainsString(deniedEntities, sid.Value))
                    {
#if DEBUG
                        ApplicationLog.WriteInformationEvent("One of the principal's groups is in the denied list. Permission denied.", EventID.DebugMessage);
#endif
                        return false;
                    }
                }
            }

            // The user hasn't been denied yet, so now we check the authorization list.

            if (allowedEntities == null)
            { // The allowed list is null, meaning everyone is allowed administrator rights.
#if DEBUG
                ApplicationLog.WriteInformationEvent("Allowed list is null. Everyone is allowed to request administrator rights.", EventID.DebugMessage);
#endif
                return true;
            }
            else if (allowedEntities.Length == 0)
            { // The allowed list is empty, meaning no one is allowed administrator rights.
#if DEBUG
                ApplicationLog.WriteInformationEvent("Allowed list is empty, but not null. No one is allowed to request administrator rights.", EventID.DebugMessage);
#endif
                return false;
            }
            else
            { // The allowed list has entries.

                // If the user's SID is in the allowed list, the user is authorized.
                /*
                if (ArrayContainsString(allowedEntities, user.Sid.Value))
                {
                    return true;
                }
                */
                if (ArrayContainsString(allowedEntities, userIdentity.User.Value))
                {
#if DEBUG
                    ApplicationLog.WriteInformationEvent("Principal's SID is in the allowed list. Permission granted.", EventID.DebugMessage);
#endif
                    return true;
                }

                // If any of the user's authorization groups are in the allowed list, the user is authorized.
                /*
                foreach (Principal prin in authGroups)
                {
                    if (ArrayContainsString(allowedEntities, prin.Sid.Value))
                    {
                        return true;
                    }
                }
                */
                foreach (SecurityIdentifier sid in userSids)
                {
                    if (ArrayContainsString(allowedEntities, sid.Value))
                    {
#if DEBUG
                        ApplicationLog.WriteInformationEvent("One of the principal's groups is in the allowed list. Permission granted.", EventID.DebugMessage);
#endif
                        return true;
                    }
                }


                // The user was not found in the allowed list, so the user is not authorized.
#if DEBUG
                ApplicationLog.WriteInformationEvent("Principal was not found in the allowed list. Permission denied.", EventID.DebugMessage);
#endif
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
