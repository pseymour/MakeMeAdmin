// <copyright file="Settings.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
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
                return GetMultiString(null, "Allowed Entities");
            }
            set
            {
                SetMultiString(null, "Allowed Entities", value);
            }
        }

        public static string[] DeniedEntities
        {
            get
            {
                return GetMultiString(null, "Denied Entities");
            }
            set
            {
                SetMultiString(null, "Denied Entities", value);
            }
        }

        public static string[] SIDs
        {
            get
            {
                return GetMultiString(null, "Added SIDs");
            }
            set
            {
                SetMultiString(null, "Added SIDs", value);
            }
        }

        public static int AdminRightsTimeout
        {
            get
            {
                int? timeout = GetDWord(null, "Admin Rights Timeout");
                if (timeout.HasValue)
                {
                    return timeout.Value;
                }
                else
                {
                    return 2;
                }
            }
            set
            {
                SetDWord(null, "Admin Rights Timeout", value);
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
                Microsoft.Win32.Registry.LocalMachine.DeleteSubKeyTree(RegistryKeyPath);
            }

            // The specified subkey is a null reference.
            catch (System.ArgumentNullException) { }

            // Deletion of a root hive is attempted, or the subkey does not specify a valid registry subkey.
            catch (System.ArgumentException) { }

            // The user does not have the permissions required to delete the key.
            catch (System.Security.SecurityException) { }

            // The RegistryKey being manipulated is closed (closed keys cannot be accessed).
            catch (System.ObjectDisposedException) { }

            // The user does not have the necessary registry rights.
            catch (System.UnauthorizedAccessException) { }

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
        private static string[] GetMultiString(string subkeyName, string valueName)
        {
            string[] returnValue = null;

            string keyPath = RegistryKeyPath;
            if (!string.IsNullOrEmpty(subkeyName))
            {
                keyPath = System.IO.Path.Combine(keyPath, subkeyName);
            }

            RegistryKey settingsKey = rootRegistryKey.OpenSubKey(keyPath, false);
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
        private static void SetMultiString(string subkeyName, string valueName, string[] value)
        {
            string keyPath = RegistryKeyPath;
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


        /// <summary>
        /// Gets the path of the registry key in which all of the settings are stored.
        /// </summary>
        private static string RegistryKeyPath
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

        private static string[] GetValueNames(string subkeyName)
        {
            string[] returnArray = null;

            string keyPath = RegistryKeyPath;
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
        private static string GetString(string subkeyName, string valueName)
        {
            string returnValue = null;

            string keyPath = RegistryKeyPath;
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
        private static void SetString(string subkeyName, string valueName, string value)
        {
            string keyPath = RegistryKeyPath;
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

        private static int? GetDWord(string subkeyName, string valueName)
        {
            int? returnValue = null;

            string keyPath = RegistryKeyPath;
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
                    returnValue = System.Convert.ToInt32(regValue);
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
        private static void SetDWord(string subkeyName, string valueName, int? value)
        {
            string keyPath = RegistryKeyPath;
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
