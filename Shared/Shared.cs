// 
// Copyright © 2010-2018, Sinclair Community College
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
    using System.Security.Principal;

    public class Shared
    {
        public static string TcpServiceBaseAddress
        {
            get
            {
                return string.Format("net.tcp://{0}/MakeMeAdmin/Service", FullyQualifiedHostName);
            }
        }

        public static string NamedPipeServiceBaseAddress
        {
            get
            {
                return string.Format("net.pipe://{0}/MakeMeAdmin/Service", FullyQualifiedHostName);
            }
        }

        private static string FullyQualifiedHostName
        {
            get
            {
                string hostName = System.Environment.MachineName;
                try
                {
                    hostName = System.Net.Dns.GetHostEntry(System.Net.Dns.GetHostName()).HostName.ToLowerInvariant();
                }
                catch (System.Net.Sockets.SocketException) { hostName = System.Environment.MachineName; }
                catch (System.ArgumentNullException) { hostName = System.Environment.MachineName; }
                catch (System.ArgumentOutOfRangeException) { hostName = System.Environment.MachineName; }
                catch (System.ArgumentException) { hostName = System.Environment.MachineName; }
                return hostName;
            }
        }


        private static bool ArrayContainsString(string[] stringArray, string targetString)
        {
            /*
#if DEBUG
            ApplicationLog.WriteInformationEvent("In ArrayContainsString().", EventID.DebugMessage);
            ApplicationLog.WriteInformationEvent(string.Format("targetString = \"{0}\"", targetString), EventID.DebugMessage);
#endif
            */

            if ((stringArray != null) && (stringArray.Length > 0))
            {
                for (int i = 0; i < stringArray.Length; i++)
                {
                    /*
#if DEBUG
                    ApplicationLog.WriteInformationEvent(string.Format("stringArray[i] = \"{0}\"", stringArray[i]), EventID.DebugMessage);
                    ApplicationLog.WriteInformationEvent(string.Format("stringArray[i] (expanded) = \"{0}\"", System.Environment.ExpandEnvironmentVariables(stringArray[i])), EventID.DebugMessage);
#endif
                    */

                    if (string.Compare(System.Environment.ExpandEnvironmentVariables(stringArray[i]), targetString, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.CompareOptions.IgnoreCase) == 0)
                    {
                        /*
#if DEBUG
                        ApplicationLog.WriteInformationEvent("Leaving ArrayContainsString(). Returning true.", EventID.DebugMessage);
#endif
                        */
                        return true;
                    }
                }
            }

            /*
#if DEBUG
            ApplicationLog.WriteInformationEvent("Leaving ArrayContainsString(). Returning false.", EventID.DebugMessage);
#endif
            */
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

            foreach (SecurityIdentifier sid in userIdentity.Groups)
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


        /*
        public static bool UserIsAuthorized(IdentityReferenceCollection authorizationGroups, string[] allowedSidsList, string[] deniedSidsList)
        */
        public static bool UserIsAuthorized(WindowsIdentity userIdentity, string[] allowedSidsList, string[] deniedSidsList)
        {
            /*
#if DEBUG
            ApplicationLog.WriteInformationEvent("In Shared.Shared.UserIsAuthorized.", EventID.DebugMessage);
            ApplicationLog.WriteInformationEvent(string.Format("Authorization group count: {0:N0}", userIdentity.Groups.Count), EventID.DebugMessage);
#endif
            */

#if DEBUG

            // TODO: i18n.
            if (deniedSidsList != null)
            {
                ApplicationLog.WriteInformationEvent(string.Format("Denied list contains {0:N0} entries.", deniedSidsList.Length), EventID.DebugMessage);
            }
            // TODO: i18n.
            if (allowedSidsList != null)
            {
                ApplicationLog.WriteInformationEvent(string.Format("Allowed list contains {0:N0} entries.", allowedSidsList.Length), EventID.DebugMessage);
            }
            
#endif

            /*
#if DEBUG
            ApplicationLog.WriteInformationEvent("Checking security principal against allowed and denied list.", EventID.DebugMessage);
#endif
            */            

            if ((deniedSidsList != null) && (deniedSidsList.Length > 0))
            { // The denied list contains entries. Check the user against that list first.

                /*
#if DEBUG
                ApplicationLog.WriteInformationEvent("Denied list contains entries.", EventID.DebugMessage);
#endif
                */                

                // If the user's SID or name is in the denied list, the user is not authorized.
                if ((ArrayContainsString(deniedSidsList, userIdentity.User.Value)) || (ArrayContainsString(deniedSidsList, userIdentity.Name)))
                {
                    /*
#if DEBUG
                    ApplicationLog.WriteInformationEvent("Principal's SID is in the denied list. Permission denied.", EventID.DebugMessage);
#endif
                    */
                    return false;
                }

                // If any of the user's authorization groups are in the denied list, the user is not authorized.
                foreach (SecurityIdentifier sid in userIdentity.Groups)
                {
                    if (ArrayContainsString(deniedSidsList, sid.Value))
                    {
                        /*
#if DEBUG
                        ApplicationLog.WriteInformationEvent("One of the principal's groups is in the denied list. Permission denied.", EventID.DebugMessage);
#endif
                        */                        
                        return false;
                    }

                    if (sid.IsValidTargetType(typeof(NTAccount)))
                    {
                        NTAccount account = (NTAccount)sid.Translate(typeof(NTAccount));
                        if (ArrayContainsString(deniedSidsList, account.Value))
                        {
                            /*
    #if DEBUG
                            ApplicationLog.WriteInformationEvent("One of the principal's groups is in the denied list. Permission denied.", EventID.DebugMessage);
    #endif
                            */
                            return false;
                        }
                    }
                }
            }

            // The user hasn't been denied yet, so now we check the authorization list.

            if (allowedSidsList == null)
            { // The allowed list is null, meaning everyone is allowed administrator rights.
                /*
#if DEBUG
                ApplicationLog.WriteInformationEvent("Allowed list is null. Everyone is allowed to request administrator rights.", EventID.DebugMessage);
#endif
                */
                return true;
            }
            else if (allowedSidsList.Length == 0)
            { // The allowed list is empty, meaning no one is allowed administrator rights.
                /*
#if DEBUG
                ApplicationLog.WriteInformationEvent("Allowed list is empty, but not null. No one is allowed to request administrator rights.", EventID.DebugMessage);
#endif
                */
                return false;
            }
            else
            { // The allowed list has entries.

                // If the user's SID is in the allowed list, the user is authorized.
                if ((ArrayContainsString(allowedSidsList, userIdentity.User.Value))  || (ArrayContainsString(allowedSidsList, userIdentity.Name)))
                {
                    /*
#if DEBUG
                    ApplicationLog.WriteInformationEvent("Principal's SID is in the allowed list. Permission granted.", EventID.DebugMessage);
#endif
                    */
                    return true;
                }

                // If any of the user's authorization groups are in the allowed list, the user is authorized.
                foreach (SecurityIdentifier sid in userIdentity.Groups)
                {
                    if (ArrayContainsString(allowedSidsList, sid.Value))
                    {
                        /*
#if DEBUG
                        ApplicationLog.WriteInformationEvent("One of the principal's groups is in the allowed list. Permission granted.", EventID.DebugMessage);
#endif
                        */
                        return true;
                    }

                    if (sid.IsValidTargetType(typeof(NTAccount)))
                    {
                        NTAccount account = (NTAccount)sid.Translate(typeof(NTAccount));
                        if (ArrayContainsString(allowedSidsList, account.Value))
                        {
                            /*
#if DEBUG
                            ApplicationLog.WriteInformationEvent("One of the principal's groups is in the allowed list. Permission granted.", EventID.DebugMessage);
#endif
                            */
                            return true;
                        }
                    }
                }


                // The user was not found in the allowed list, so the user is not authorized.
                /*
#if DEBUG
                ApplicationLog.WriteInformationEvent("Principal was not found in the allowed list. Permission denied.", EventID.DebugMessage);
#endif
                */
                return false;
            }
        }

        

        public static int EndNetworkSession(string remoteHost, string userName)
        {
            return NativeMethods.NetSessionDel(null, remoteHost, userName);
        }
    }
}
