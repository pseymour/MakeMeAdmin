// 
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
    using Microsoft.Win32;
    
    /// <summary>
    /// This class manages application settings.
    /// </summary>
    public static class UserSettings
    {
        /// <summary>
        /// The top-level registry key in which the settings will be stored.
        /// </summary>
        private static RegistryKey rootRegistryKey = Registry.CurrentUser;

        /// <summary>
        /// Gets or sets the most-recently used (MRU) list of hostnames.
        /// </summary>
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
        /// Gets the path of the registry key in which all of the preferred settings are stored.
        /// </summary>
        private static string PreferenceRegistryKeyPath
        {
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
