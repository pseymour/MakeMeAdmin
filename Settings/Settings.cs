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
    using Microsoft.Win32;
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
        private static RegistryKey rootRegistryKey = Registry.LocalMachine;

        // TODO: Can we provide better entropy here?
        /// <summary>
        /// An additional byte array used to encrypt the data.
        /// </summary>
        private readonly static byte[] optionalEntropy = new byte[] { 8, 6, 7, 5, 3, 0, 9 };

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

        // TODO: Document how this should be setup in the ADMX/ADML files.
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
        
        // TODO: i18n.
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
                { // Neither the policy nor the preference registry entries had a value. Return a default value of true.
                    return false;
                }
            }
            set
            {
                SetDWord(PreferenceRegistryKeyPath, null, "Override Removal By Outside Process", Convert.ToInt32(value));
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
        /// Gets the value of a multi-string stored in the registry.
        /// </summary>
        /// <param name="subkeyName">
        /// The relative path of the subkey in which the value is stored.
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

        /*
        private static string[] GetMultiStringEncrypted(string keyPath, string subkeyName, string valueName)
        {
            string[] returnValue = GetMultiString(keyPath, subkeyName, valueName);

            if (returnValue != null)
            {
                // Decrypt all of the strings in the array.
                for (int i = 0; i < returnValue.Length; i++)
                {
                    byte[] stringBytes = System.Text.Encoding.Default.GetBytes(returnValue[i]);
                    byte[] decryptedBytes = ProtectedData.Unprotect(stringBytes, null, DataProtectionScope.LocalMachine);
                    returnValue[i] = System.Text.Encoding.Default.GetString(decryptedBytes);
                }
            }

            return returnValue;
        }
        */

        /// <summary>
        /// Stores a multi-string in the registry.
        /// </summary>
        /// <param name="valueName">
        /// The name of the registry value in which the string will be stored.
        /// </param>
        /// <param name="value">
        /// The array of strings to stored in the registry.
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

        /*
        private static void SetMultiStringEncrypted(string keyPath, string subkeyName, string valueName, string[] value)
        {
            if (value != null)
            {
                // Encrypt all of the strings in the array.
                for (int i = 0; i < value.Length; i++)
                {
                    byte[] stringBytes = System.Text.Encoding.Default.GetBytes(value[i]);
                    byte[] encryptedData = ProtectedData.Protect(stringBytes, optionalEntropy, DataProtectionScope.LocalMachine);
                    value[i] = System.Text.Encoding.Default.GetString(encryptedData);
                }
            }

            SetMultiString(keyPath, subkeyName, valueName, value);
        }
        */

        /*
        /// <summary>
        /// Encrypts a string and stores it in the registry.
        /// </summary>
        /// <param name="valueName">
        /// The name of the registry value in which the encrypted string will be stored.
        /// </param>
        /// <param name="value">
        /// The decrypted string to be encrypted and stored in the registry.
        /// </param>
        private static void SetEncryptedString(string valueName, string value)
        {
            RegistryKey settingsKey = rootRegistryKey.CreateSubKey(RegistryKeyPath, RegistryKeyPermissionCheck.ReadWriteSubTree);
            if (settingsKey != null)
            {
                System.Text.ASCIIEncoding encoding = new System.Text.ASCIIEncoding();
                settingsKey.Flush();
                settingsKey.Close();
            }
        }
        */

        // TODO: This needs to be commented.
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

        // TODO: This needs to be commented.
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
                // TODO: i18n.
                string returnValue = "My Company";
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
                // TODO: i18n.
                string returnValue = "My Product";
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

        // TODO: This needs to be commented.
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
        /// <param name="valueName">
        /// The name of the registry value in which the string will be stored.
        /// </param>
        /// <param name="value">
        /// The string to stored in the registry.
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

        // TODO: This needs to be commented.
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
        /// <param name="valueName">
        /// The name of the registry value in which the string will be stored.
        /// </param>
        /// <param name="value">
        /// The string to stored in the registry.
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
    }
}
