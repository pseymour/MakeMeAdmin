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
    using System.ServiceModel;
    using System.ServiceModel.Channels;
    using System.Security.Principal;

    /// <summary>
    /// This class implements the WCF service contract.
    /// </summary>
    //JDM: Setting the IncludeExceptionDetailInFaults to true will allow us to see verbose service output.
    //     This is useful while debugging, especially if you're a bad programmer, as I am. 
    //[ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = false)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single, IncludeExceptionDetailInFaults = true)]

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
                //JDM: I worry that we'll need a version of this function that takes a WindowsIdentity token as well, as this lookup might pick up the wrong account...
                userIdentity = ServiceSecurityContext.Current.WindowsIdentity;
            }

            //ApplicationLog.WriteEvent("In AGM::AUTAG(0). userIdentity: " + userIdentity.Name, EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Information);

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
                //ApplicationLog.WriteEvent("In AGM::AUTAG(0). About to call LAG::AU(" + userIdentity.Name + ", " + expirationTime + ", " + remoteAddress + ").", EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Information);
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
            if (accountList != null)
            {
                foreach (string account in accountList)
                {
                    SecurityIdentifier sid = LocalAdministratorGroup.GetSIDFromAccountName(account);

                    // If the user's SID or name is in the list, return true
                    if (sid == userIdentity.User)
                    {
                        return true;
                    }

                    // If any of the user's authorization groups are in the denied list, the user is not authorized.
                    foreach (IdentityReference groupsid in userIdentity.Groups)
                    {
                        // Translate the NT Account (Domain\User) to SID if needed, and check the resulting values.
                        if (sid == (SecurityIdentifier)groupsid.Translate(typeof(SecurityIdentifier)))
                        {
                            return true;
                        }
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// Determines whether the given user is authorized to obtain administrator rights.
        /// </summary>
        /// <param name="allowedSidsList">
        /// The list of allowed SIDs and principal names against which the user's identity is checked.
        /// </param>
        /// <param name="deniedSidsList">
        /// The list of denied SIDs and principal names against which the user's identity is checked.
        /// </param>
        /// <returns>
        /// Returns true if the user is authorized to obtain administrator rights.
        /// </returns>
        public bool UserIsAuthorized(string[] allowedSidsList, string[] deniedSidsList)
        {
            WindowsIdentity userIdentity = null;

            if (ServiceSecurityContext.Current != null)
            {
                userIdentity = ServiceSecurityContext.Current.WindowsIdentity;
                //ApplicationLog.WriteEvent("In AGM::UIAWIT(2). About to call this.UIAWIT(3) for userIdentity: " + userIdentity.Name + ".", EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Information);
                return this.UserIsAuthorizedWithIdentityToken(userIdentity.Token, allowedSidsList, deniedSidsList);
            }
            
            return false; 

            // JDM: Original code commented out as we'll call the UserIsAuthorizedWithIdentityToken() function instead. 
            /*if (userIdentity == null)
            {
                return false;
            }

            // The denied list contains entries. Check the user against that list first.
            if ((deniedSidsList != null) && AccountListContainsIdentity(deniedSidsList, userIdentity))
            { 
                return false;
            }

            // The user hasn't been denied yet, so now we check for authorization.

            // Check the authorization list.
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
                if (AccountListContainsIdentity(allowedSidsList, userIdentity))
                {
                    return true;
                }

                // The user was not found in the allowed list, so the user is not authorized.
                return false;
            }*/
        }

        // JDM: Adding this implementation, largely a copy/paste of the 2-param UserIsAuthorized version to call this one. 
        //      Going to refactor the 2-param UserIsAuthorized to call this function, to make things a little simpler to understand. 

        /// <summary>
        /// Determines whether the given user is authorized to obtain administrator rights.
        /// </summary>
        /// <param name="userIdentityToken">
        /// A token that can be used to re-create an identity object representing the user whose authorization is to be checked.
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
        public bool UserIsAuthorizedWithIdentityToken(IntPtr userIdentityToken, string[] allowedSidsList, string[] deniedSidsList)
        {
            // JDM: Reconstitute our userIdentity since we had trouble sending it over directly.  
            //      Our errors kind of looked like this:
            /*
             * Failed to process session change. System.ServiceModel.CommunicationException: There was an error while trying to serialize parameter http://apps.sinclair.edu/makemeadmin/2017/10/:userIdentity. The InnerException message was 'Type 'System.IntPtr' with data contract name 'IntPtr:http://schemas.datacontract.org/2004/07/System' is not expected. Consider using a DataContractResolver if you are using DataContractSerializer or add any types not known statically to the list of known types - for example, by using the KnownTypeAttribute attribute or by adding them to the list of known types passed to the serializer.'.  Please see InnerException for more details. ---> System.Runtime.Serialization.SerializationException: Type 'System.IntPtr' with data contract name 'IntPtr:http://schemas.datacontract.org/2004/07/System' is not expected. Consider using a DataContractResolver if you are using DataContractSerializer or add any types not known statically to the list of known types - for example, by using the KnownTypeAttribute attribute or by adding them
             */
            //      But going to the IntPtr token instead seems to work. 
            WindowsIdentity userIdentity = new WindowsIdentity(userIdentityToken);
            //ApplicationLog.WriteEvent("In AGM::UIAWIT(3). userIdentity: " + userIdentity.Name + ".", EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Information);

            if (userIdentity == null)
            {
                return false;
            }

            // The denied list contains entries. Check the user against that list first.
            if ((deniedSidsList != null) && AccountListContainsIdentity(deniedSidsList, userIdentity))
            {
                return false;
            }

            // The user hasn't been denied yet, so now we check for authorization.

            // Check the authorization list.
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
        private static int GetTimeoutForUser(WindowsIdentity userIdentity)
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
