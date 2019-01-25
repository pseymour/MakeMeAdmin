namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Security.Principal;
    using System.Xml;
    using System.Xml.Serialization;


    /// <summary>
    /// This class stores application data in an encrypted file.
    /// </summary>
    [Serializable]
    [XmlRootAttribute("applicationSettings")]
    public class EncryptedSettings
    {
        /// <summary>
        /// The path of the file containing the settings.
        /// </summary>
        private string filePath;

        /// <summary>
        /// The list of users that have been added to the
        /// local Administrators group.
        /// </summary>
        [XmlArray("addedUsers")]
        [XmlArrayItem("user")]
        public UserList AddedUsers { get; set; }
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// This constructor is used by the serialization process.
        /// </remarks>
        public EncryptedSettings()
        {
            this.filePath = EncryptedSettings.SettingsFilePath;
            if (this.AddedUsers == null)
            {
                this.AddedUsers = new UserList();
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="filePath">
        /// The path of a file containing an XML-serialized EncryptedSettings object.
        /// </param>
        public EncryptedSettings(string filePath)
        {
            if (this.AddedUsers == null)
            {
                this.AddedUsers = new UserList();
            }
            this.filePath = filePath;
            if (System.IO.File.Exists(filePath))
            {
                this.Load(filePath);
            }
            else
            {
                if (this.AddedUsers == null)
                {
                    this.AddedUsers = new UserList();
                }
                this.Save(filePath);
            }
        }

        /// <summary>
        /// Gets the path of the file in which settings are stored.
        /// </summary>
        public static string SettingsFilePath
        {
            get
            {
                const string EncryptedSettingsFile = "users.xml";
                string filePath = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Make Me Admin");
                System.IO.Directory.CreateDirectory(filePath);
                filePath = System.IO.Path.Combine(filePath, EncryptedSettingsFile);
                return filePath;
            }
        }

        /// <summary>
        /// Adds a user to the list.
        /// </summary>
        /// <param name="userIdentity">
        /// 
        /// </param>
        /// <param name="expirationTime">
        /// 
        /// </param>
        /// <param name="remoteAddress">
        /// 
        /// </param>
        public void AddUser(WindowsIdentity userIdentity, DateTime? expirationTime, string remoteAddress)
        {
            if (this.AddedUsers.Contains(userIdentity.User))
            {
                if (this.AddedUsers[userIdentity.User].ExpirationTime.HasValue)
                {
                    if (expirationTime.HasValue)
                    {
                        // Choose the earlier of the two expiration times.
                        this.AddedUsers[userIdentity.User].ExpirationTime = DateTime.FromFileTime(Math.Min(expirationTime.Value.ToFileTime(), this.AddedUsers[userIdentity.User].ExpirationTime.Value.ToFileTime()));
                    }
                    else
                    { // expirationTime doesn't have a value, but the currently-stored user does.
                        // Err on the side of safety and keep the currently-stored value.
                    }
                }
                else if (expirationTime.HasValue)
                { // The currently-stored user does not have an expiration value, but expirationTime does.
                    this.AddedUsers[userIdentity.User].ExpirationTime = expirationTime;
                }
            }
            else
            {
                /* this.AddedUsers.Add(new User(userSecurityIdentifier, expirationTime, remoteAddress)); */
                this.AddedUsers.Add(new User(userIdentity, expirationTime, remoteAddress));
            }
            this.Save();
        }
        

        public void RemoveUser(SecurityIdentifier userSecurityIdentifier)
        {
            if (this.AddedUsers.Contains(userSecurityIdentifier))
            {
                this.AddedUsers.Remove(userSecurityIdentifier);
                this.Save();
            }
        }

        public DateTime? GetExpirationTime(SecurityIdentifier sid)
        {
            if (this.AddedUsers.Contains(sid))
            {
                return this.AddedUsers[sid].ExpirationTime;
            }
            else
            {
                return null;
            }
        }

        [XmlIgnore]
        public SecurityIdentifier[] AddedUserSIDs
        {
            get
            {
                return this.AddedUsers.GetSIDs();
            }
        }

        public bool ContainsSID(SecurityIdentifier sid)
        {
            return this.AddedUsers.Contains(sid);
        }

        public User[] GetExpiredUsers()
        {
            if (this.AddedUsers == null)
            {
                return null;
            }
            else
            {
                return this.AddedUsers.GetExpiredUsers();
            }
        }

        public bool IsRemote(SecurityIdentifier sid)
        {
            return this.AddedUsers.IsRemote(sid);
        }

        /*
        public Dictionary<SecurityIdentifier, DateTime?> GetAddedUsers()
        {
            EncryptedSettings encryptedSettings = new EncryptedSettings();
            return encryptedSettings.AddedUsers;
        }
        */


        /// <summary>
        /// Gets an XML serializer for the AppSettings class.
        /// </summary>
        public static XmlSerializer Serializer
        {
            get
            {
                XmlSerializer serializer = new XmlSerializer(typeof(EncryptedSettings));
                return serializer;
            }
        }


        /// <summary>
        /// Saves the settings.
        /// </summary>
        private void Save()
        {
            this.Save(EncryptedSettings.SettingsFilePath);
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
                System.Xml.XmlTextWriter plaintextWriter = new System.Xml.XmlTextWriter(plaintextStream, System.Text.Encoding.Unicode);
                XmlSerializer serializer = EncryptedSettings.Serializer;
                lock (serializer)
                {
                    plaintextWriter.Indentation = 0;
                    // plaintextWriter.IndentChar = '\t';
                    plaintextWriter.Formatting = System.Xml.Formatting.None;
                    serializer.Serialize(plaintextWriter, this);
                }

                byte[] plaintextBytes = plaintextStream.ToArray();

                byte[] ciphertextBytes = ProtectedData.Protect(plaintextBytes, null, DataProtectionScope.LocalMachine);

                plaintextWriter.Close();
                plaintextWriter.Dispose();

                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));
                System.IO.FileStream ciphertextStream = new System.IO.FileStream(filePath, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                ciphertextStream.Write(ciphertextBytes, 0, ciphertextBytes.Length);
                ciphertextStream.Close();
            }
            catch (System.InvalidOperationException)
            {
                throw;
            }
            
            /*
            // This is the unencrypted version.
            try
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));
                System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(filePath, System.Text.Encoding.Unicode);
                XmlSerializer serializer = EncryptedSettings.Serializer;
                lock (serializer)
                {
                    writer.Indentation = 1;
                    writer.IndentChar = '\t';
                    writer.Formatting = System.Xml.Formatting.Indented;
                    serializer.Serialize(writer, this);
                }
                writer.Close();
            }
            catch (System.InvalidOperationException)
            {
                throw;
            }
            */
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

                EncryptedSettings deserializedSettings = null;
                System.IO.MemoryStream plaintextStream = new System.IO.MemoryStream(plaintextBytes);
                System.Xml.XmlTextReader reader = new XmlTextReader(plaintextStream);
                XmlSerializer serializer = EncryptedSettings.Serializer;
                lock (serializer)
                {
                    deserializedSettings = (EncryptedSettings)serializer.Deserialize(reader);
                }
                reader.Close();
                plaintextStream.Close();

                /*
                // This is the unencrypted version.
                EncryptedSettings deserializedSettings = null;
                System.Xml.XmlTextReader reader = new XmlTextReader(filePath);
                XmlSerializer serializer = EncryptedSettings.Serializer;
                lock (serializer)
                {
                    deserializedSettings = (EncryptedSettings)serializer.Deserialize(reader);
                }
                reader.Close();
                */

                this.AddedUsers = deserializedSettings.AddedUsers;
            }
            else
            {
                this.AddedUsers = new UserList();
                this.Save(filePath);
            }
        }
    }
}
