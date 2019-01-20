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
    using System.Security.Principal;

    public class Shared
    {
        /// <summary>
        /// Gets the base address for the service host that is available via TCP.
        /// </summary>
        public static string TcpServiceBaseAddress
        {
            get
            {
                return string.Format("net.tcp://{0}/MakeMeAdmin/Service", FullyQualifiedHostName);
            }
        }


        /// <summary>
        /// Gets the base address for the service host that is available via named pipes.
        /// </summary>
        public static string NamedPipeServiceBaseAddress
        {
            get
            {
                return string.Format("net.pipe://{0}/MakeMeAdmin/Service", FullyQualifiedHostName);
            }
        }


        /// <summary>
        /// Returns the fully-qualified host name of the local computer.
        /// </summary>
        /// <remarks>
        /// If there is an error determining the fully-qualified name, the NetBIOS name is returned.
        /// </remarks>
        public static string FullyQualifiedHostName
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


        /// <summary>
        /// Determines whether the given array of strings contains the given target string.
        /// </summary>
        /// <param name="stringArray">
        /// An array to be searched for the target string.
        /// </param>
        /// <param name="targetString">
        /// The string to be searched for in the array.
        /// </param>
        /// <returns>
        /// Returns true if the given target string is present in the array.
        /// If the array is null or empty, false is returned.
        /// </returns>
        /// <remarks>
        /// String comparisons are case-insensitive.
        /// </remarks>
        private static bool ArrayContainsString(string[] stringArray, string targetString)
        {
            if ((stringArray != null) && (stringArray.Length > 0))
            {
                for (int i = 0; i < stringArray.Length; i++)
                {
                    if (string.Compare(System.Environment.ExpandEnvironmentVariables(stringArray[i]), targetString, System.Globalization.CultureInfo.CurrentCulture, System.Globalization.CompareOptions.IgnoreCase) == 0)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Gets the administrator rights timeout value for the given user.
        /// </summary>
        /// <param name="userIdentity">
        /// An identity object representing the user whose authorization is to be checked.
        /// </param>
        /// <returns>
        /// Returns the number of minutes at which the user's administrator rights should expire.
        /// </returns>
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


        /// <summary>
        /// Determines whether the given user is authorized to obtain administrator rights.
        /// </summary>
        /// <param name="userIdentity">
        /// An identity object representing the user whose authorization is to be checked.
        /// </param>
        /// <param name="allowedSidsList">
        /// The list of allowed SIDs and principal names against which the user's identity is checked.
        /// </param>
        /// <param name="deniedSidsList">
        /// The list of denied SIDs and principal names against which the user's identity is checked.
        /// </param>
        /// <returns>
        /// Returns true if the user is authorized to obtain administrator rights.
        /// </returns>
        public static bool UserIsAuthorized(WindowsIdentity userIdentity, string[] allowedSidsList, string[] deniedSidsList)
        {
            if ((deniedSidsList != null) && (deniedSidsList.Length > 0))
            { // The denied list contains entries. Check the user against that list first.

                // If the user's SID or name is in the denied list, the user is not authorized.
                if ((ArrayContainsString(deniedSidsList, userIdentity.User.Value)) || (ArrayContainsString(deniedSidsList, userIdentity.Name)))
                {
                    return false;
                }

                // If any of the user's authorization groups are in the denied list, the user is not authorized.
                foreach (SecurityIdentifier sid in userIdentity.Groups)
                {
                    // Check the SID values.
                    if (ArrayContainsString(deniedSidsList, sid.Value))
                    {
                        return false;
                    }

                    // Translate the SID to an NT Account (Domain\User), and check the resulting values.
                    if (sid.IsValidTargetType(typeof(NTAccount)))
                    {
                        NTAccount account = (NTAccount)sid.Translate(typeof(NTAccount));
                        if (ArrayContainsString(deniedSidsList, account.Value))
                        {
                            return false;
                        }
                    }
                }
            }

            // The user hasn't been denied yet, so now we check the authorization list.

            if (allowedSidsList == null)
            { // The allowed list is null, meaning everyone is allowed administrator rights.
                return true;
            }
            else if (allowedSidsList.Length == 0)
            { // The allowed list is empty, meaning no one is allowed administrator rights.
                return false;
            }
            else
            { // The allowed list has entries.

                // If the user's SID is in the allowed list, the user is authorized.
                if ((ArrayContainsString(allowedSidsList, userIdentity.User.Value))  || (ArrayContainsString(allowedSidsList, userIdentity.Name)))
                {
                    return true;
                }

                // If any of the user's authorization groups are in the allowed list, the user is authorized.
                foreach (SecurityIdentifier sid in userIdentity.Groups)
                {
                    // Check the SID values.
                    if (ArrayContainsString(allowedSidsList, sid.Value))
                    {
                        return true;
                    }

                    // Translate the SID to an NT Account (Domain\User), and check the resulting values.
                    if (sid.IsValidTargetType(typeof(NTAccount)))
                    {
                        NTAccount account = (NTAccount)sid.Translate(typeof(NTAccount));
                        if (ArrayContainsString(allowedSidsList, account.Value))
                        {
                            return true;
                        }
                    }
                }

                // The user was not found in the allowed list, so the user is not authorized.
                return false;
            }
        }
    }
}
