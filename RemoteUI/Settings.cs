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

namespace SinclairCC.MakeMeAdmin.RemoteUI
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
        private static RegistryKey rootRegistryKey = Registry.CurrentUser;

        public static string[] HostNameMru
        {
            // TODO: i18n.
            get
            {
                string[] preferenceAllowedEntities = GetMultiString(PreferenceRegistryKeyPath, null, "Host Name MRU");
                return preferenceAllowedEntities;
            }
            set
            {
                SetMultiString(PreferenceRegistryKeyPath, null, "Host Name MRU", value);
            }
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
            // TODO: i18n.
            get { return string.Format(System.Globalization.CultureInfo.InvariantCulture, @"Software\{0}\{1}", CompanyName, ProductName); }
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
