﻿// 
// Copyright © 2010-2025, Sinclair Community College
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
    using System.ServiceModel;
    using System.ServiceModel.Channels;

    /// <summary>
    /// This class implements the WCF service contract.
    /// </summary>
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = false)]
    public class AdminGroupManipulator : IAdminGroup
    {
        /// <summary>
        /// Adds a user to the local Administrators group.
        /// </summary>
        public void AddUserToAdministratorsGroup()
        {
            string remoteAddress = null;

            WindowsIdentity userIdentity = null;

            if (ServiceSecurityContext.Current != null)
            {
                userIdentity = ServiceSecurityContext.Current.WindowsIdentity;
            }

            if (OperationContext.Current != null)
            {
                if (OperationContext.Current.IncomingMessageProperties != null)
                {
                    if (OperationContext.Current.IncomingMessageProperties.ContainsKey(RemoteEndpointMessageProperty.Name))
                    {
                        remoteAddress = ((RemoteEndpointMessageProperty)OperationContext.Current.IncomingMessageProperties[RemoteEndpointMessageProperty.Name]).Address;
                        if (remoteAddress != null)
                        {
                            ApplicationLog.WriteEvent(string.Format(Properties.Resources.RequestSentFromHost, remoteAddress), EventID.RemoteRequestInformation, System.Diagnostics.EventLogEntryType.Information);
                        }

                    }
                }
            }

            if (userIdentity != null)
            {
                int timeoutMinutes = GetTimeoutForUser(userIdentity);
                DateTime expirationTime = DateTime.Now.AddMinutes(timeoutMinutes);
                LocalAdministratorGroup.AddUser(userIdentity, expirationTime, remoteAddress);
            }
        }

        /// <summary>
        /// Removes a user from the local Administrators group.
        /// </summary>
        /// <param name="reason">
        /// The reason that the rights are being removed.
        /// </param>
        public void RemoveUserFromAdministratorsGroup(RemovalReason reason)
        {
            WindowsIdentity userIdentity = null;

            if (ServiceSecurityContext.Current != null)
            {
                userIdentity = ServiceSecurityContext.Current.WindowsIdentity;
            }

            if (userIdentity != null)
            {
                LocalAdministratorGroup.RemoveUser(userIdentity.User, reason);
            }
        }

        /// <summary>
        /// Returns a value indicating whether a user is in the
        /// list of added users.
        /// </summary>
        /// <returns>
        /// Returns true if the given user is already in the list of added
        /// users. Otherwise, false is returned.
        /// </returns>
        public bool UserIsInList()
        {
            WindowsIdentity userIdentity = null;

            if (ServiceSecurityContext.Current != null)
            {
                userIdentity = ServiceSecurityContext.Current.WindowsIdentity;
            }

            if (userIdentity != null)
            {
                EncryptedSettings encryptedSettings = new EncryptedSettings(EncryptedSettings.SettingsFilePath);
                return encryptedSettings.ContainsSID(userIdentity.User);
            }
            else
            {
                return false;
            }
        }

        
        public bool UserSessionIsInList(int sessionID)
        {
            SecurityIdentifier sid = LsaLogonSessions.LogonSessions.GetSidForSessionId(sessionID);
            if (sid != null)
            {
                EncryptedSettings encryptedSettings = new EncryptedSettings(EncryptedSettings.SettingsFilePath);
                return encryptedSettings.ContainsSID(sid);
            }
            else
            {
                return false;
            }
        }
        

        /// <summary>
        /// Determines whether the given array of SIDs/Identities contains the given target user identity.
        /// </summary>
        /// <param name="accountList">
        /// An array to be searched for the target identity.
        /// </param>
        /// <param name="userIdentity">
        /// The user identity to check to see if they or one of the groups they belong to are in the list of identities.
        /// </param>
        /// <returns>
        /// Returns true if the given target identity is present in the array of SIDs/Identities.
        /// If the array is null or empty, false is returned.
        /// </returns>
        private static bool AccountListContainsIdentity(string[] accountList, WindowsIdentity userIdentity)
        {
            if ((accountList != null) && (accountList.Length > 0))
            {
                foreach (string account in accountList)
                {
                    SecurityIdentifier sid = LocalAdministratorGroup.GetSIDFromAccountName(account);
                    /*
#if DEBUG
                    ApplicationLog.WriteEvent(string.Format("Checking user against SID {0}.", sid.Value), EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Information);
#endif
                    */


                    // If the user's SID or name is in the list, return true
                    if (userIdentity.User == sid)
                    {
                        /*
#if DEBUG
                        ApplicationLog.WriteEvent(string.Format("User SID matches {0}.", sid.Value), EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Information);
#endif
                        */
                        return true;
                    }

                    // If any of the user's authorization groups are in the denied list, the user is not authorized.
                    foreach (IdentityReference groupsid in userIdentity.Groups)
                    {
                        // Translate the NT Account (Domain\User) to SID if needed, and check the resulting values.
                        if (sid == (SecurityIdentifier)groupsid.Translate(typeof(SecurityIdentifier)))
                        {
                            /*
#if DEBUG
                            ApplicationLog.WriteEvent("SIDs match!", EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Information);
#endif
                            */
                            return true;
                        }
                    }
                }
            }
#if DEBUG
            ApplicationLog.WriteEvent("Was not able to find a matching SID.", EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Information);
#endif
            return false;
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
        public bool UserIsAuthorized(WindowsIdentity userIdentity, string[] allowedSidsList, string[] deniedSidsList)
        {
            /*
            WindowsIdentity userIdentity = null;

            if (ServiceSecurityContext.Current != null)
            {
                userIdentity = ServiceSecurityContext.Current.WindowsIdentity;
            }
            */

            if (userIdentity == null)
            {
#if DEBUG
                ApplicationLog.WriteEvent("User identity is null.", EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Information);
#endif
                return false;
            }

            if (((deniedSidsList != null) && (deniedSidsList.Length > 0)) && AccountListContainsIdentity(deniedSidsList, userIdentity))
            { // The denied list contains entries. Check the user against that list first.
#if DEBUG
                ApplicationLog.WriteEvent("User is denied.", EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Information);
#endif
                return false;
            }

            // The user hasn't been denied yet, so now we check for authorization.

            // Check the authorization list.
            if (allowedSidsList == null)
            { // The allowed list is null, meaning everyone is allowed administrator rights.
                /*
#if DEBUG
                ApplicationLog.WriteEvent("Allowed SIDs list is null.", EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Information);
#endif
                */

                return true;
            }
            else if (allowedSidsList.Length == 0)
            { // The allowed list is empty, meaning no one is allowed administrator rights.
                /*
#if DEBUG
                ApplicationLog.WriteEvent("Allowed SIDs list is empty.", EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Information);
#endif
                */

                return false;
            }
            else
            { // The allowed list has entries.
#if DEBUG
                ApplicationLog.WriteEvent("Made it to the allowed list check. That's good.", EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Information);
#endif
                if (AccountListContainsIdentity(allowedSidsList, userIdentity))
                {
                    return true;
                }

                // The user was not found in the allowed list, so the user is not authorized.
                return false;
            }
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
    }
}
