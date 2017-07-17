// <copyright file="Settings.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using Microsoft.Win32;
    
    /// <summary>
    /// This class manages application settings.
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// The top-level registry key in which the settings will be stored.
        /// </summary>
        private static RegistryKey rootRegistryKey = Registry.LocalMachine;

        public static string[] AllowedEntities
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

        public static string[] DeniedEntities
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

        //private static System.Collections.Generic.Dictionary<string, string> GetKeyValuePairs(string keyPath, string subkeyName)

        public static string[] SIDs
        {
            get
            {
                return GetMultiString(PreferenceRegistryKeyPath, null, "Added SIDs");
            }
            set
            {
                SetMultiString(PreferenceRegistryKeyPath, null, "Added SIDs", value);
            }
        }

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
                { // Neither the policy nor the preference registry entries had a value. Return a default timeout value of 2 minutes.
                    return 2;
                }
            }
            set
            {
                SetDWord(PreferenceRegistryKeyPath, null, "Admin Rights Timeout", value);
            }
        }

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
                { // Neither the policy nor the preference registry entries had a value. Return a default value of true.
                    return false;
                }
            }
            set
            {
                SetDWord(PreferenceRegistryKeyPath, null, "Remove Admin Rights On Logout", Convert.ToInt32(value));
            }
        }

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
            /* RegistryKey settingsKey = rootRegistryKey.OpenSubKey(keyPath, false); */
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
        /// Gets the path of the registry key in which all of the settings are stored.
        /// </summary>
        private static string PreferenceRegistryKeyPath
        {
            get { return string.Format(System.Globalization.CultureInfo.InvariantCulture, @"Software\{0}\{1}", CompanyName, ProductName); }
        }

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

        private static int? GetDWord(string keyPath, string subkeyName, string valueName)
        {
            int? returnValue = null;

            if (!string.IsNullOrEmpty(subkeyName))
            {
                keyPath = System.IO.Path.Combine(keyPath, subkeyName);
            }

            System.Security.AccessControl.RegistryRights rights = System.Security.AccessControl.RegistryRights.QueryValues;
            RegistryKey settingsKey = rootRegistryKey.OpenSubKey(keyPath, RegistryKeyPermissionCheck.ReadSubTree, rights);
            /* RegistryKey settingsKey = rootRegistryKey.OpenSubKey(keyPath, false); */
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
