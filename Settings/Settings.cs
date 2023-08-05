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
    using Microsoft.Win32;
    using System;
    using System.Collections.Generic;
    /*
    using System.Security.Cryptography;
    */

    /// <summary>
    /// This class manages application settings.
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// The top-level registry key in which the settings will be stored.
        /// </summary>
        private readonly static RegistryKey rootRegistryKey = Registry.LocalMachine;

        /// <summary>
        /// Gets the base address for the service host that is available via TCP.
        /// </summary>
        public static string TcpServiceBaseAddress
        {
            get
            {
                return string.Format("net.tcp://{0}:{1}/MakeMeAdmin/Service", FullyQualifiedHostName, Settings.TCPServicePort);
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

        // TODO: i18n.
        public static string[] LocalAllowedEntities
        {
            get
            {
                string[] policyAllowedEntities = GetMultiString(PolicyRegistryKeyPath, null, "Allowed Entities");
                string[] preferenceAllowedEntities = GetMultiString(PreferenceRegistryKeyPath, null, "Allowed Entities");
                if (policyAllowedEntities != null)
                { // The policy setting has a value. Go with whatever it says.
                    return policyAllowedEntities;
                }
                else
                { // The preference setting has a value. Go with whatever it says.
                    return preferenceAllowedEntities;
                }
            }
            set
            {
                SetMultiString(PreferenceRegistryKeyPath, null, "Allowed Entities", value);
            }
        }

        // TODO: i18n.
        public static string[] LocalDeniedEntities
        {
            get
            {
                string[] policyDeniedEntities = GetMultiString(PolicyRegistryKeyPath, null, "Denied Entities");
                string[] preferenceDeniedEntities = GetMultiString(PreferenceRegistryKeyPath, null, "Denied Entities");
                if (policyDeniedEntities != null)
                { // The policy setting has a value. Go with whatever it says.
                    return policyDeniedEntities;
                }
                else
                { // The preference setting has a value. Go with whatever it says.
                    return preferenceDeniedEntities;
                }
            }
            set
            {
                SetMultiString(PreferenceRegistryKeyPath, null, "Denied Entities", value);
            }
        }

        // TODO: i18n.
        public static string[] AutomaticAddAllowed
        {
            get
            {
                string[] policyAllowedEntities = GetMultiString(PolicyRegistryKeyPath, null, "Automatic Add Allowed");
                string[] preferenceAllowedEntities = GetMultiString(PreferenceRegistryKeyPath, null, "Automatic Add Allowed");
                if (policyAllowedEntities != null)
                { // The policy setting has a value. Go with whatever it says.
                    return policyAllowedEntities;
                }
                else
                { // The preference setting has a value. Go with whatever it says.
                    return preferenceAllowedEntities;
                }
            }
            set
            {
                SetMultiString(PreferenceRegistryKeyPath, null, "Automatic Add Allowed", value);
            }
        }

        // TODO: i18n.
        public static string[] AutomaticAddDenied
        {
            get
            {
                string[] policyDeniedEntities = GetMultiString(PolicyRegistryKeyPath, null, "Automatic Add Denied");
                string[] preferenceDeniedEntities = GetMultiString(PreferenceRegistryKeyPath, null, "Automatic Add Denied");
                if (policyDeniedEntities != null)
                { // The policy setting has a value. Go with whatever it says.
                    return policyDeniedEntities;
                }
                else
                { // The preference setting has a value. Go with whatever it says.
                    return preferenceDeniedEntities;
                }
            }
            set
            {
                SetMultiString(PreferenceRegistryKeyPath, null, "Automatic Add Denied", value);
            }
        }

        // TODO: i18n.
        public static string[] RemoteAllowedEntities
        {
            get
            {
                string[] policyAllowedEntities = GetMultiString(PolicyRegistryKeyPath, null, "Remote Allowed Entities");
                string[] preferenceAllowedEntities = GetMultiString(PreferenceRegistryKeyPath, null, "Remote Allowed Entities");
                if (policyAllowedEntities != null)
                { // The policy setting has a value. Go with whatever it says.
                    return policyAllowedEntities;
                }
                else
                { // The preference setting has a value. Go with whatever it says.
                    return preferenceAllowedEntities;
                }
            }
            set
            {
                SetMultiString(PreferenceRegistryKeyPath, null, "Remote Allowed Entities", value);
            }
        }

        // TODO: i18n.
        public static string[] RemoteDeniedEntities
        {
            get
            {
                string[] policyDeniedEntities = GetMultiString(PolicyRegistryKeyPath, null, "Remote Denied Entities");
                string[] preferenceDeniedEntities = GetMultiString(PreferenceRegistryKeyPath, null, "Remote Denied Entities");
                if (policyDeniedEntities != null)
                { // The policy setting has a value. Go with whatever it says.
                    return policyDeniedEntities;
                }
                else
                { // The preference setting has a value. Go with whatever it says.
                    return preferenceDeniedEntities;
                }
            }
            set
            {
                SetMultiString(PreferenceRegistryKeyPath, null, "Remote Denied Entities", value);
            }
        }

        // TODO: i18n.
        public static List<SyslogServerInfo> SyslogServers
        {
            get
            {
                string[] policySyslogEntries = GetMultiString(PolicyRegistryKeyPath, null, "syslog servers");
                string[] preferenceSyslogEntries = GetMultiString(PreferenceRegistryKeyPath, null, "syslog servers");
                if (policySyslogEntries != null)
                { // The policy setting has a value. Go with whatever it says.
                    return ProcessSyslogServerStrings(policySyslogEntries);
                }
                else
                { // The preference setting has a value. Go with whatever it says.
                    return ProcessSyslogServerStrings(preferenceSyslogEntries);
                }
            }
            // TODO: Implement set for this setting.
            /*
            set
            {
                SetMultiString(PreferenceRegistryKeyPath, null, "syslog servers", value);
            }
            */
        }

        /// <summary>
        /// Processes the array of strings, retrieved from the registry, containing
        /// the syslog server information.
        /// </summary>
        /// <param name="serverStrings">
        /// The array of strings retrieved from the registry.
        /// </param>
        /// <returns>
        /// Returns a collection of SyslogServerInfo objects, one object for each
        /// valid entry in the registry.
        /// </returns>
        private static List<SyslogServerInfo> ProcessSyslogServerStrings(string[] serverStrings)
        {
            List<SyslogServerInfo> returnList = new List<SyslogServerInfo>();

            if (serverStrings != null)
            {
                for (int i = 0; i < serverStrings.Length; i++)
                {
                    string[] stringParts = serverStrings[i].Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries);

                    string hostname = (stringParts.Length >= 1) ? stringParts[0] : string.Empty;

                    int portNumber = 0;
                    string protocol = string.Empty;
                    if (stringParts.Length >= 2)
                    {
                        if (!int.TryParse(stringParts[1], out portNumber))
                        {
                            protocol = stringParts[1];
                        }
                    }

                    if (string.IsNullOrEmpty(protocol))
                    {
                        protocol = (stringParts.Length >= 3) ? stringParts[2] : "udp";
                    }

                    string version = (stringParts.Length >= 4) ? stringParts[3] : "3164";

                    SyslogServerInfo serverInfo = new SyslogServerInfo(hostname, portNumber, protocol, version);

                    if (serverInfo.IsValid)
                    {
                        returnList.Add(serverInfo);
                    }
                    else
                    {
                        // TODO: Should this be logged?
                        /*
                        ApplicationLog.WriteWarningEvent(string.Format("rejected invalid syslog server information string \"{0}\"", serverStrings[i]), EventID.RejectedSyslogServerInfo);
                        */
                    }
                }
            }
            return returnList;
        }

        // TODO: i18n.
        public static System.Collections.Generic.Dictionary<string, string> TimeoutOverrides
        {
            get
            {
                System.Collections.Generic.Dictionary<string, string> policyTimeoutOverrides = GetKeyValuePairs(PolicyRegistryKeyPath, "Timeout Overrides");
                System.Collections.Generic.Dictionary<string, string> preferenceTimeoutOverrides = GetKeyValuePairs(PreferenceRegistryKeyPath, "Timeout Overrides");
                if (policyTimeoutOverrides.Count > 0)
                { // The policy setting has a value. Go with whatever it says.
                    return policyTimeoutOverrides;
                }
                else
                { // The preference setting has a value. Go with whatever it says.
                    return preferenceTimeoutOverrides;
                }
            }
            set
            {
                SetKeyValuePairs(PreferenceRegistryKeyPath, "Timeout Overrides", value);
            }
        }

        // TODO: Remove this registry value from PCs.
        /*
        public static string[] SIDs
        {
            get
            {
                return GetMultiStringEncrypted(PreferenceRegistryKeyPath, null, "Added SIDs");
            }
            set
            {
                SetMultiStringEncrypted(PreferenceRegistryKeyPath, null, "Added SIDs", value);
            }
        }
        */

        // TODO: i18n.
        public static int AdminRightsTimeout
        {
            get
            {
                int? policyTimeoutSetting = GetDWord(PolicyRegistryKeyPath, null, "Admin Rights Timeout");
                int? preferenceTimeoutSetting = GetDWord(PreferenceRegistryKeyPath, null, "Admin Rights Timeout");
                if (policyTimeoutSetting.HasValue)
                { // The policy setting has a value. Go with whatever it says.
                    return policyTimeoutSetting.Value;
                }
                else if (preferenceTimeoutSetting.HasValue)
                { // The preference setting has a value. Go with whatever it says.
                    return preferenceTimeoutSetting.Value;
                }
                else
                { // Neither the policy nor the preference registry entries had a value. Return a default timeout value of 10 minutes.
                    return 10;
                }
            }
            set
            {
                SetDWord(PreferenceRegistryKeyPath, null, "Admin Rights Timeout", value);
            }
        }

        // TODO: i18n.
        public static int LogOffAfterExpiration
        {
            get
            {
                int? policyLogoffSetting = GetDWord(PolicyRegistryKeyPath, null, "Log Off After Expiration");
                int? preferenceLogoffSetting = GetDWord(PreferenceRegistryKeyPath, null, "Log Off After Expiration");
                if (policyLogoffSetting.HasValue)
                { // The policy setting has a value. Go with whatever it says.
                    return policyLogoffSetting.Value;
                }
                else if (preferenceLogoffSetting.HasValue)
                { // The preference setting has a value. Go with whatever it says.
                    return preferenceLogoffSetting.Value;
                }
                else
                { // Neither the policy nor the preference registry entries had a value. Return a default value 0 (no logoff).
                    return 0;
                }
            }
            set
            {
                SetDWord(PreferenceRegistryKeyPath, null, "Log Off After Expiration", value);
            }
        }

        // TODO: i18n.
        public static string[] LogOffMessage
        {
            get
            {
                string[] policyLogoffSetting = GetMultiString(PolicyRegistryKeyPath, null, "Log Off Message");
                string[] preferenceLogoffSetting = GetMultiString(PreferenceRegistryKeyPath, null, "Log Off Message");
                if (null != policyLogoffSetting)
                { // The policy setting has a value. Go with whatever it says.
                    return policyLogoffSetting;
                }
                else if (null != preferenceLogoffSetting)
                { // The preference setting has a value. Go with whatever it says.
                    return preferenceLogoffSetting;
                }
                else
                { // Neither the policy nor the preference registry entries had a value. Return a default value.
                    return new string[] { Properties.Resources.DefaultLogOffMessage };
                }
            }
            set
            {
                SetMultiString(PreferenceRegistryKeyPath, null, "Log Off Message", value);
            }
        }

        // TODO: i18n.
        public static bool RemoveAdminRightsOnLogout
        {
            get
            {
                int? policyRemovalSetting = GetDWord(PolicyRegistryKeyPath, null, "Remove Admin Rights On Logout");
                int? preferenceRemovalSetting = GetDWord(PreferenceRegistryKeyPath, null, "Remove Admin Rights On Logout");
                if (policyRemovalSetting.HasValue)
                { // The policy setting has a value. Go with whatever it says.

                    return Convert.ToBoolean(policyRemovalSetting.Value);
                }
                else if (preferenceRemovalSetting.HasValue)
                { // The preference setting has a value. Go with whatever it says.
                    return Convert.ToBoolean(preferenceRemovalSetting.Value);
                }
                else
                { // Neither the policy nor the preference registry entries had a value. Return a default value of false.
                    return false;
                }
            }
            set
            {
                SetDWord(PreferenceRegistryKeyPath, null, "Remove Admin Rights On Logout", Convert.ToInt32(value));
            }
        }

        // TODO: i18n.
        public static bool OverrideRemovalByOutsideProcess
        {
            get
            {
                int? policyOverrideSetting = GetDWord(PolicyRegistryKeyPath, null, "Override Removal By Outside Process");
                int? preferenceOverrideSetting = GetDWord(PreferenceRegistryKeyPath, null, "Override Removal By Outside Process");
                if (policyOverrideSetting.HasValue)
                { // The policy setting has a value. Go with whatever it says.

                    return Convert.ToBoolean(policyOverrideSetting.Value);
                }
                else if (preferenceOverrideSetting.HasValue)
                { // The preference setting has a value. Go with whatever it says.
                    return Convert.ToBoolean(preferenceOverrideSetting.Value);
                }
                else
                { // Neither the policy nor the preference registry entries had a value. Return a default value of false.
                    return false;
                }
            }
            set
            {
                SetDWord(PreferenceRegistryKeyPath, null, "Override Removal By Outside Process", Convert.ToInt32(value));
            }
        }

        // TODO: i18n.
        public static bool RequireAuthenticationForPrivileges
        {
            get
            {
                int? policyOverrideSetting = GetDWord(PolicyRegistryKeyPath, null, "Require Authentication For Privileges");
                int? preferenceOverrideSetting = GetDWord(PreferenceRegistryKeyPath, null, "Require Authentication For Privileges");
                if (policyOverrideSetting.HasValue)
                { // The policy setting has a value. Go with whatever it says.
                    return Convert.ToBoolean(policyOverrideSetting.Value);
                }
                else if (preferenceOverrideSetting.HasValue)
                { // The preference setting has a value. Go with whatever it says.
                    return Convert.ToBoolean(preferenceOverrideSetting.Value);
                }
                else
                { // Neither the policy nor the preference registry entries had a value. Return a default value of false.
                    return false;
                }
            }
            set
            {
                SetDWord(PreferenceRegistryKeyPath, null, "Require Authentication For Privileges", Convert.ToInt32(value));
            }
        }

        // TODO: i18n.
        public static bool AllowRemoteRequests
        {
            get
            {
                int? policyAllowRemoteSetting = GetDWord(PolicyRegistryKeyPath, null, "Allow Remote Requests");
                int? preferenceAllowRemoteSetting = GetDWord(PreferenceRegistryKeyPath, null, "Allow Remote Requests");
                if (policyAllowRemoteSetting.HasValue)
                { // The policy setting has a value. Go with whatever it says.
                    return Convert.ToBoolean(policyAllowRemoteSetting.Value);
                }
                else if (preferenceAllowRemoteSetting.HasValue)
                { // The preference setting has a value. Go with whatever it says.
                    return Convert.ToBoolean(preferenceAllowRemoteSetting.Value);
                }
                else
                { // Neither the policy nor the preference registry entries had a value. Return a default value of false.
                    return false;
                }
            }
            set
            {
                SetDWord(PreferenceRegistryKeyPath, null, "Allow Remote Requests", Convert.ToInt32(value));
            }
        }

        // TODO: i18n.
        public static bool EndRemoteSessionsUponExpiration
        {
            get
            {
                int? policyEndRemoteSessionSetting = GetDWord(PolicyRegistryKeyPath, null, "End Remote Sessions Upon Expiration");
                int? preferenceEndRemoteSessionSetting = GetDWord(PreferenceRegistryKeyPath, null, "End Remote Sessions Upon Expiration");
                if (policyEndRemoteSessionSetting.HasValue)
                { // The policy setting has a value. Go with whatever it says.
                    return Convert.ToBoolean(policyEndRemoteSessionSetting.Value);
                }
                else if (preferenceEndRemoteSessionSetting.HasValue)
                { // The preference setting has a value. Go with whatever it says.
                    return Convert.ToBoolean(preferenceEndRemoteSessionSetting.Value);
                }
                else
                { // Neither the policy nor the preference registry entries had a value. Return a default value of true.
                    return true;
                }
            }
            set
            {
                SetDWord(PreferenceRegistryKeyPath, null, "End Remote Sessions Upon Expiration", Convert.ToInt32(value));
            }
        }

        // TODO: i18n.
        public static bool CloseApplicationOnExpiration
        {
            get
            {
                int? policyCloseApplicationSetting = GetDWord(PolicyRegistryKeyPath, null, "Close Application Upon Expiration");
                int? preferenceCloseApplicationSetting = GetDWord(PreferenceRegistryKeyPath, null, "Close Application Upon Expiration");
                if (policyCloseApplicationSetting.HasValue)
                { // The policy setting has a value. Go with whatever it says.
                    return Convert.ToBoolean(policyCloseApplicationSetting.Value);
                }
                else if (preferenceCloseApplicationSetting.HasValue)
                { // The preference setting has a value. Go with whatever it says.
                    return Convert.ToBoolean(preferenceCloseApplicationSetting.Value);
                }
                else
                { // Neither the policy nor the preference registry entries had a value. Return a default value of true.
                    return true;
                }
            }
            set
            {
                SetDWord(PreferenceRegistryKeyPath, null, "Close Application Upon Expiration", Convert.ToInt32(value));
            }
        }

        public static ReasonPrompt PromptForReason
        {
            get
            {

                int? policySetting = GetDWord(PolicyRegistryKeyPath, null, "Prompt For Reason");
                int? preferenceSetting = GetDWord(PreferenceRegistryKeyPath, null, "Prompt For Reason");
                if (policySetting.HasValue)
                { // The policy setting has a value. Go with whatever it says.
                    ReasonPrompt convertedValue = ReasonPrompt.None;
                    if (Enum.TryParse<ReasonPrompt>(Convert.ToString(policySetting.Value), true, out convertedValue))
                    {
                        if (!Enum.IsDefined(typeof(ReasonPrompt), convertedValue))
                        {
                            convertedValue = ReasonPrompt.None;
                        }
                    }
                    else
                    {
                        convertedValue = ReasonPrompt.None;
                    }
                    return convertedValue;
                }
                else if (preferenceSetting.HasValue)
                { // The preference setting has a value. Go with whatever it says.
                    ReasonPrompt convertedValue = ReasonPrompt.None;
                    if (Enum.TryParse<ReasonPrompt>(Convert.ToString(preferenceSetting.Value), true, out convertedValue))
                    {
                        if (!Enum.IsDefined(typeof(ReasonPrompt), convertedValue))
                        {
                            convertedValue = ReasonPrompt.None;
                        }
                    }
                    else
                    {
                        convertedValue = ReasonPrompt.None;
                    }
                    return convertedValue;
                }
                else
                { // Neither the policy nor the preference registry entries had a value. Return a default value of None.
                    return ReasonPrompt.None;
                }
            }
            set
            {
                SetDWord(PreferenceRegistryKeyPath, null, "Prompt For Reason", Convert.ToInt32(value));
            }
        }


        public static bool AllowFreeFormReason
        {
            get
            {

                int? policySetting = GetDWord(PolicyRegistryKeyPath, null, "Allow Free-Form Reason");
                int? preferenceSetting = GetDWord(PreferenceRegistryKeyPath, null, "Allow Free-Form Reason");
                if (policySetting.HasValue)
                { // The policy setting has a value. Go with whatever it says.
                    return Convert.ToBoolean(policySetting.Value);
                }
                else if (preferenceSetting.HasValue)
                { // The preference setting has a value. Go with whatever it says.
                    return Convert.ToBoolean(preferenceSetting.Value);
                }
                else
                { // Neither the policy nor the preference registry entries had a value. Return a default value of true.
                    return true;
                }
            }
            set
            {
                SetDWord(PreferenceRegistryKeyPath, null, "Allow Free-Form Reason", Convert.ToInt32(value));
            }
        }


        // TODO: i18n.
        public static string[] CannedReasons
        {
            get
            {
                string[] policySetting = GetMultiString(PolicyRegistryKeyPath, null, "Canned Reasons");
                string[] preferenceSetting = GetMultiString(PreferenceRegistryKeyPath, null, "Canned Reasons");
                if (policySetting != null)
                { // The policy setting has a value. Go with whatever it says.
                    return policySetting;
                }
                else
                { // Go with whatever the preference setting says, even if it is null.
                    return preferenceSetting;
                }
            }
            set
            {
                SetMultiString(PreferenceRegistryKeyPath, null, "Canned Reasons", value);
            }
        }

        // TODO: i18n.
        public static ElevatedProcessLogging LogElevatedProcesses
        {
            get
            {
                int? policyTimeoutSetting = GetDWord(PolicyRegistryKeyPath, null, "Log Elevated Processes");
                int? preferenceTimeoutSetting = GetDWord(PreferenceRegistryKeyPath, null, "Log Elevated Processes");
                if (policyTimeoutSetting.HasValue)
                { // The policy setting has a value. Go with whatever it says.
                    if (Enum.IsDefined(typeof(ElevatedProcessLogging), policyTimeoutSetting.Value))
                    {
                        return (ElevatedProcessLogging)policyTimeoutSetting.Value;
                    }
                    else
                    {
                        return ElevatedProcessLogging.OnlyWhenAdmin;
                    }
                }
                else if (preferenceTimeoutSetting.HasValue)
                { // The preference setting has a value. Go with whatever it says.
                    if (Enum.IsDefined(typeof(ElevatedProcessLogging), preferenceTimeoutSetting.Value))
                    {
                        return (ElevatedProcessLogging)preferenceTimeoutSetting.Value;
                    }
                    else
                    {
                        return ElevatedProcessLogging.OnlyWhenAdmin;
                    }
                }
                else
                { // Neither the policy nor the preference registry entries had a value.
                  // Return a default value indicating that logging should not be done.
                    return ElevatedProcessLogging.Never;
                }
            }
            set
            {
                SetDWord(PreferenceRegistryKeyPath, null, "Log Elevated Processes", (int)value);
            }
        }

        // TODO: i18n.
        public static int MaximumReasonLength
        {
            get
            {
                int? policyTimeoutSetting = GetDWord(PolicyRegistryKeyPath, null, "Maximum Reason Length");
                int? preferenceTimeoutSetting = GetDWord(PreferenceRegistryKeyPath, null, "Maximum Reason Length");
                if (policyTimeoutSetting.HasValue)
                { // The policy setting has a value. Go with whatever it says.
                    return policyTimeoutSetting.Value;
                }
                else if (preferenceTimeoutSetting.HasValue)
                { // The preference setting has a value. Go with whatever it says.
                    return preferenceTimeoutSetting.Value;
                }
                else
                { // Neither the policy nor the preference registry entries had a value. Return a default timeout value of 333.
                    return 333;
                }
            }
            set
            {
                SetDWord(PreferenceRegistryKeyPath, null, "Maximum Reason Length", value);
            }
        }

        // TODO: i18n.
        public static int TCPServicePort
        {
            get
            {
                int? policyPortSetting = GetDWord(PolicyRegistryKeyPath, null, "TCP Service Port");
                int? preferencePortSetting = GetDWord(PreferenceRegistryKeyPath, null, "TCP Service Port");
                if (policyPortSetting.HasValue)
                { // The policy setting has a value. Go with whatever it says.
                    return policyPortSetting.Value;
                }
                else if (preferencePortSetting.HasValue)
                { // The preference setting has a value. Go with whatever it says.
                    return preferencePortSetting.Value;
                }
                else
                { // Neither the policy nor the preference registry entries had a value. Return a default timeout value of 808.
                    return 808;
                }
            }
            set
            {
                SetDWord(PreferenceRegistryKeyPath, null, "TCP Service Port", value);
            }
        }

        /// <summary>
        /// Removes from the computer all of the settings related to this application.
        /// </summary>
        public static void RemoveAllSettings()
        {
            // Attempt to delete the settings registry key.
            try
            {
                Microsoft.Win32.Registry.LocalMachine.DeleteSubKeyTree(PreferenceRegistryKeyPath);
            }

            // The specified subkey is a null reference.
            catch (ArgumentNullException) { }

            // Deletion of a root hive is attempted, or the subkey does not specify a valid registry subkey.
            catch (ArgumentException) { }

            // The user does not have the permissions required to delete the key.
            catch (System.Security.SecurityException) { }

            // The RegistryKey being manipulated is closed (closed keys cannot be accessed).
            catch (ObjectDisposedException) { }

            // The user does not have the necessary registry rights.
            catch (UnauthorizedAccessException) { }

        }

        /// <summary>
        /// Gets the path of the registry key in which all of the preferred settings are stored.
        /// </summary>
        private static string PreferenceRegistryKeyPath
        {
            get { return string.Format(System.Globalization.CultureInfo.InvariantCulture, @"Software\{0}\{1}", CompanyName, ProductName); }
        }

        /// <summary>
        /// Gets the path of the registry key in which all of the policy-enforced settings are stored.
        /// </summary>
        private static string PolicyRegistryKeyPath
        {
            get { return string.Format(System.Globalization.CultureInfo.InvariantCulture, @"Software\Policies\{0}\{1}", CompanyName, ProductName); }
        }

        /// <summary>
        /// Gets the company name from this class's assembly.
        /// </summary>
        /// <remarks>
        /// If a company name is not specified in the assembly, "My Company" is returned.
        /// </remarks>
        private static string CompanyName
        {
            get
            {
                string returnValue = Properties.Resources.MyCompany;
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                object[] attributes = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyCompanyAttribute), false);
                foreach (object o in attributes)
                {
                    if (o != null)
                    {
                        returnValue = ((System.Reflection.AssemblyCompanyAttribute)o).Company;
                    }
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets the product name from this class's assembly.
        /// </summary>
        /// <remarks>
        /// If a product name is not specified in the assembly, "My Product" is returned.
        /// </remarks>
        private static string ProductName
        {
            get
            {
                string returnValue = Properties.Resources.MyProduct;
                System.Reflection.Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
                object[] attributes = assembly.GetCustomAttributes(typeof(System.Reflection.AssemblyProductAttribute), false);
                foreach (object o in attributes)
                {
                    if (o != null)
                    {
                        returnValue = ((System.Reflection.AssemblyProductAttribute)o).Product;
                    }
                }

                return returnValue;
            }
        }

        /// <summary>
        /// Gets a list of the value names in the specified registry key.
        /// </summary>
        /// <param name="keyPath">
        /// The path of the registry key that contains the data to be read.
        /// </param>
        /// <param name="subkeyName">
        /// An optional relative path of a subkey in which the data is stored.
        /// </param>
        /// <returns>
        /// Returns an array containing the names of the registry values in
        /// the specified key.
        /// If there is a problem retrieving the value names from the registry, a null array is returned.
        /// </returns>
        private static string[] GetValueNames(string keyPath, string subkeyName)
        {
            string[] returnArray = null;

            if (!string.IsNullOrEmpty(subkeyName))
            {
                keyPath = System.IO.Path.Combine(keyPath, subkeyName);
            }

            RegistryKey settingsKey = rootRegistryKey.OpenSubKey(keyPath, false);
            if (settingsKey != null)
            {
                returnArray = settingsKey.GetValueNames();
                settingsKey.Close();
            }

            return returnArray;
        }

        /// <summary>
        /// Gets the value of a string stored in the registry.
        /// </summary>
        /// <param name="keyPath">
        /// The path of the registry key that contains the data to be read.
        /// </param>
        /// <param name="subkeyName">
        /// An optional relative path of a subkey in which the data is stored.
        /// </param>
        /// <param name="valueName">
        /// The name of the value to be retrieved from the registry.
        /// </param>
        /// <returns>
        /// Returns the value of the string from the registry.
        /// If there is a problem retrieving the specified value from the registry, null is returned.
        /// </returns>
        private static string GetString(string keyPath, string subkeyName, string valueName)
        {
            string returnValue = null;

            if (!string.IsNullOrEmpty(subkeyName))
            {
                keyPath = System.IO.Path.Combine(keyPath, subkeyName);
            }

            RegistryKey settingsKey = rootRegistryKey.OpenSubKey(keyPath, false);
            if (settingsKey != null)
            {
                object regValue = settingsKey.GetValue(valueName);
                if (regValue != null)
                {
                    returnValue = System.Convert.ToString(regValue);
                }

                settingsKey.Close();
            }

            return returnValue;
        }

        /// <summary>
        /// Stores a string in the registry.
        /// </summary>
        /// <param name="keyPath">
        /// The path of the registry key that contains the data to be read.
        /// </param>
        /// <param name="subkeyName">
        /// An optional relative path of a subkey in which the data is stored.
        /// </param>
        /// <param name="valueName">
        /// The name of the registry value in which the data will be stored.
        /// </param>
        /// <param name="value">
        /// The value to be stored in the registry.
        /// </param>
        private static void SetString(string keyPath, string subkeyName, string valueName, string value)
        {
            if (!string.IsNullOrEmpty(subkeyName))
            {
                keyPath = System.IO.Path.Combine(keyPath, subkeyName);
            }
            RegistryKey settingsKey = rootRegistryKey.CreateSubKey(keyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
            if (settingsKey != null)
            {
                settingsKey.SetValue(valueName, value, RegistryValueKind.String);
                settingsKey.Flush();
                settingsKey.Close();
            }
        }

        /// <summary>
        /// Gets a double-word value from the registry.
        /// </summary>
        /// <param name="keyPath">
        /// The path of the registry key that contains the data to be read.
        /// </param>
        /// <param name="subkeyName">
        /// An optional relative path of a subkey in which the data is stored.
        /// </param>
        /// <param name="valueName">
        /// The name of the value to be retrieved from the registry.
        /// </param>
        /// <returns>
        /// Returns the value of the specified registry value.
        /// If there is a problem retrieving the specified value from the registry, null is returned.
        /// </returns>
        private static int? GetDWord(string keyPath, string subkeyName, string valueName)
        {
            int? returnValue = null;

            if (!string.IsNullOrEmpty(subkeyName))
            {
                keyPath = System.IO.Path.Combine(keyPath, subkeyName);
            }

            System.Security.AccessControl.RegistryRights rights = System.Security.AccessControl.RegistryRights.QueryValues;
            RegistryKey settingsKey = rootRegistryKey.OpenSubKey(keyPath, RegistryKeyPermissionCheck.ReadSubTree, rights);
            if (settingsKey != null)
            {
                object regValue = settingsKey.GetValue(valueName);
                if (regValue != null)
                {
                    returnValue = Convert.ToInt32(regValue);
                }

                settingsKey.Close();
            }

            return returnValue;
        }

        /// <summary>
        /// Stores a string in the registry.
        /// </summary>
        /// <param name="keyPath">
        /// The path of the registry key that contains the data to be read.
        /// </param>
        /// <param name="subkeyName">
        /// An optional relative path of a subkey in which the data is stored.
        /// </param>
        /// <param name="valueName">
        /// The name of the registry value in which the data will be stored.
        /// </param>
        /// <param name="value">
        /// The value to be stored in the registry.
        /// </param>
        private static void SetDWord(string keyPath, string subkeyName, string valueName, int? value)
        {
            if (!string.IsNullOrEmpty(subkeyName))
            {
                keyPath = System.IO.Path.Combine(keyPath, subkeyName);
            }
            RegistryKey settingsKey = rootRegistryKey.CreateSubKey(keyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
            if (settingsKey != null)
            {
                settingsKey.SetValue(valueName, value, RegistryValueKind.DWord);
                settingsKey.Flush();
                settingsKey.Close();
            }
        }

        /// <summary>
        /// Gets a collection of key-value pairs from the registry.
        /// </summary>
        /// <param name="keyPath">
        /// The path of the registry key that contains the data to be read.
        /// </param>
        /// <param name="subkeyName">
        /// An optional relative path of a subkey in which the data is stored.
        /// </param>
        /// <returns>
        /// Returns a collection of the key-value pairs retrieved from the registry.
        /// If there is a problem retrieving the values from the specified key, an empty collection is returned.
        /// </returns>
        private static System.Collections.Generic.Dictionary<string, string> GetKeyValuePairs(string keyPath, string subkeyName)
        {
            System.Collections.Generic.Dictionary<string, string> returnData = new System.Collections.Generic.Dictionary<string, string>();

            if (!string.IsNullOrEmpty(subkeyName))
            {
                keyPath = System.IO.Path.Combine(keyPath, subkeyName);
            }

            RegistryKey settingsKey = rootRegistryKey.OpenSubKey(keyPath, false);
            if (settingsKey != null)
            {
                string valueData = string.Empty;
                string[] valueNames = settingsKey.GetValueNames();
                foreach (string valueName in valueNames)
                {
                    object regValue = settingsKey.GetValue(valueName, null);
                    if (regValue != null)
                    {
                        valueData = System.Convert.ToString(regValue);
                    }
                    else
                    {
                        valueData = null;
                    }
                    returnData.Add(valueName, valueData);
                }

                settingsKey.Close();
            }

            return returnData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="keyPath">
        /// The path of the registry key that contains the data to be read.
        /// </param>
        /// <param name="subkeyName">
        /// An optional relative path of a subkey in which the data is stored.
        /// </param>
        /// <param name="values">
        /// The collection of keys and values to be stored in the registry.
        /// </param>
        private static void SetKeyValuePairs(string keyPath, string subkeyName, System.Collections.Generic.Dictionary<string, string> values)
        {
            if (!string.IsNullOrEmpty(subkeyName))
            {
                keyPath = System.IO.Path.Combine(keyPath, subkeyName);
            }

            RegistryKey settingsKey = rootRegistryKey.CreateSubKey(keyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
            if (settingsKey != null)
            {
                foreach (string valueName in values.Keys)
                {
                    settingsKey.SetValue(valueName, values[valueName], RegistryValueKind.String);
                }
                settingsKey.Flush();
                settingsKey.Close();
            }
        }

        /// <summary>
        /// Gets the value of a multi-string stored in the registry.
        /// </summary>
        /// <param name="keyPath">
        /// The path of the registry key that contains the data to be read.
        /// </param>
        /// <param name="subkeyName">
        /// An optional relative path of a subkey in which the data is stored.
        /// </param>
        /// <param name="valueName">
        /// The name of the value to be retrieved from the registry.
        /// </param>
        /// <returns>
        /// Returns the value of the multi-string from the registry.
        /// If there is a problem retrieving the specified value from the registry, null is returned.
        /// </returns>
        private static string[] GetMultiString(string keyPath, string subkeyName, string valueName)
        {
            string[] returnValue = null;

            if (!string.IsNullOrEmpty(subkeyName))
            {
                keyPath = System.IO.Path.Combine(keyPath, subkeyName);
            }

            System.Security.AccessControl.RegistryRights rights = System.Security.AccessControl.RegistryRights.QueryValues;
            RegistryKey settingsKey = rootRegistryKey.OpenSubKey(keyPath, RegistryKeyPermissionCheck.ReadSubTree, rights);
            if (settingsKey != null)
            {
                object regValue = settingsKey.GetValue(valueName, null);
                if (regValue != null)
                {
                    returnValue = (string[])regValue;
                }

                settingsKey.Close();
            }

            return returnValue;
        }

        /// <summary>
        /// Stores a multi-string in the registry.
        /// </summary>
        /// <param name="keyPath">
        /// The path of the registry key that contains the data to be read.
        /// </param>
        /// <param name="subkeyName">
        /// An optional relative path of a subkey in which the data is stored.
        /// </param>
        /// <param name="valueName">
        /// The name of the registry value in which the data will be stored.
        /// </param>
        /// <param name="value">
        /// The array of strings to be stored in the registry.
        /// </param>
        private static void SetMultiString(string keyPath, string subkeyName, string valueName, string[] value)
        {
            if (!string.IsNullOrEmpty(subkeyName))
            {
                keyPath = System.IO.Path.Combine(keyPath, subkeyName);
            }
            RegistryKey settingsKey = rootRegistryKey.CreateSubKey(keyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
            if (settingsKey != null)
            {
                if (value == null)
                {
                    value = new string[0];
                }
                settingsKey.SetValue(valueName, value, RegistryValueKind.MultiString);
                settingsKey.Flush();
                settingsKey.Close();
            }
        }
    }
}
