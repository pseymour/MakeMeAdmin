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

/*
 * http://www.diranieh.com/NETSerialization/BinarySerialization.htm 
*/

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Security.Principal;

    /// <summary>
    /// This class allows simplified management of application settings.
    /// </summary>
    [Serializable]
    public class PersistedEncryptedSettings
    {
        /// <summary>
        /// Initializes a new instance of the AppSettings class.
        /// </summary>
        private PersistedEncryptedSettings() : this(PersistedEncryptedSettings.SettingsFilePath)
        {
        }


        /// <summary>
        /// Initializes a new instance of the AppSettings class.
        /// </summary>
        /// <param name="filePath">
        /// The path of a file containing a serialized AppSettings object.
        /// </param>
        private PersistedEncryptedSettings(string filePath)
        {
            this.Load(filePath);

            if (this.AddedPrincipals == null)
            {
                this.AddedPrincipals = new Dictionary<SecurityIdentifier, DateTime?>();
            }
        }


        /// <summary>
        /// Gets the path of the file in which settings are stored.
        /// </summary>
        private static string SettingsFilePath
        {
            get
            {
                const string AppSettingsFile = "encrypted settings.data";
                string filePath = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Make Me Admin");
                /*
                filePath = System.IO.Path.Combine(filePath, "Schedule Data Viewer");
                filePath = System.IO.Path.Combine(filePath, "0.1.0");
                */
                System.IO.Directory.CreateDirectory(filePath);
                filePath = System.IO.Path.Combine(filePath, AppSettingsFile);
                return filePath;
            }
        }


        private Dictionary<SecurityIdentifier, DateTime?> AddedPrincipals { get; set; }



        public static void AddPrincipal(SecurityIdentifier principalSecurityIdentifier, DateTime? expirationTime)
        {
            PersistedEncryptedSettings encryptedSettings = new PersistedEncryptedSettings();
            if (encryptedSettings.AddedPrincipals.ContainsKey(principalSecurityIdentifier))
            {
                if (encryptedSettings.AddedPrincipals[principalSecurityIdentifier].HasValue)
                {
                    if (expirationTime.HasValue)
                    {
                        // Choose the earlier of the two expiration times.
                        encryptedSettings.AddedPrincipals[principalSecurityIdentifier] = DateTime.FromFileTime(Math.Min(expirationTime.Value.ToFileTime(), encryptedSettings.AddedPrincipals[principalSecurityIdentifier].Value.ToFileTime()));
                    }
                    else
                    { // expirationTime doesn't have a value, but the currently-stored principal does.
                        // Err on the side of safety and keep the currently-stored value.
                    }
                }
                else if (expirationTime.HasValue)
                { // The currently-stored principal does not have an expiration value, but expirationTime does.
                    encryptedSettings.AddedPrincipals[principalSecurityIdentifier] = expirationTime;
                }
            }
            else
            {
                encryptedSettings.AddedPrincipals.Add(principalSecurityIdentifier, expirationTime);
            }
            encryptedSettings.Save();
        }


        public static void RemovePrincipal(SecurityIdentifier principalSecurityIdentifier)
        {
            PersistedEncryptedSettings encryptedSettings = new PersistedEncryptedSettings();
            if (encryptedSettings.AddedPrincipals.ContainsKey(principalSecurityIdentifier))
            {
                encryptedSettings.AddedPrincipals.Remove(principalSecurityIdentifier);
                encryptedSettings.Save();
            }
        }


        public static Dictionary<SecurityIdentifier, DateTime?> GetAddedPrincipals()
        {
            PersistedEncryptedSettings encryptedSettings = new PersistedEncryptedSettings();
            return encryptedSettings.AddedPrincipals;
        }

        /// <summary>
        /// Saves the settings.
        /// </summary>
        private void Save()
        {
            this.Save(PersistedEncryptedSettings.SettingsFilePath);
        }

        /// <summary>
        /// Saves the application settings to the specified file.
        /// </summary>
        /// <param name="filePath">The path of the file to which settings are to be saved.</param>
        private void Save(string filePath)
        {
            try
            {
                System.IO.MemoryStream plaintextStream = new System.IO.MemoryStream();

                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                formatter.Serialize(plaintextStream, this);

                byte[] plaintextBytes = plaintextStream.ToArray();

                plaintextStream.Close();

                byte[] ciphertextBytes = System.Security.Cryptography.ProtectedData.Protect(plaintextBytes, null, DataProtectionScope.LocalMachine);

                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));
                System.IO.FileStream ciphertextStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                ciphertextStream.Write(ciphertextBytes, 0, ciphertextBytes.Length);
                ciphertextStream.Close();
            }
            catch (System.InvalidOperationException)
            {
                throw;
            }
        }


        /// <summary>
        /// Loads application settings from the specified file.
        /// </summary>
        /// <param name="filePath">The path of the file from which settings should be loaded.</param>
        private void Load(string filePath)
        {
            if (System.IO.File.Exists(filePath))
            {
                byte[] buffer = new byte[128];
                int bytesRead = int.MinValue;
                System.IO.MemoryStream ciphertextMemoryStream = new System.IO.MemoryStream();
                System.IO.FileStream ciphertextFileStream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);
                while ((bytesRead = ciphertextFileStream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ciphertextMemoryStream.Write(buffer, 0, bytesRead);
                }
                ciphertextFileStream.Close();
                
                byte[] ciphertextBytes = ciphertextMemoryStream.ToArray();
                byte[] plaintextBytes = System.Security.Cryptography.ProtectedData.Unprotect(ciphertextBytes, null, DataProtectionScope.LocalMachine);
                ciphertextMemoryStream.Close();

                System.IO.MemoryStream plaintextStream = new System.IO.MemoryStream(plaintextBytes);

                System.Runtime.Serialization.Formatters.Binary.BinaryFormatter formatter = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();

                PersistedEncryptedSettings deserializedSettings = (PersistedEncryptedSettings)formatter.Deserialize(plaintextStream);

                plaintextStream.Close();

                this.AddedPrincipals = deserializedSettings.AddedPrincipals;
            }
            else
            {
                this.Save(filePath);
            }
        }
    }
}
