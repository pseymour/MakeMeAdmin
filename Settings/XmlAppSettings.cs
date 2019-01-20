namespace EncryptedStorage
{
    using System;
    /* using System.Collections.Specialized; */
    using System.Collections.Generic;
    using System.Security.Cryptography;
    using System.Security.Principal;
    using System.Xml;
    using System.Xml.Serialization;


    /// <summary>
    /// This class allows simplified management of application settings.
    /// </summary>
    [Serializable]
    [XmlRootAttribute("applicationSettings")]
    public class XmlAppSettings
    {
        /// <summary>
        /// The path of the file containing the settings.
        /// </summary>
        private string filePath;

        /* [XmlElement("addedPrincipals")] */
        [XmlArray("addedPrincipals")]
        [XmlArrayItem("principal")]
        public PrincipalList AddedPrincipals { get; set; }


        /// <summary>
        /// Initializes a new instance of the XmlAppSettings class.
        /// </summary>
        public XmlAppSettings()
        {
            this.filePath = XmlAppSettings.SettingsFilePath;

            if (this.AddedPrincipals == null)
            {
                this.AddedPrincipals = new PrincipalList();
            }
        }

        /// <summary>
        /// Initializes a new instance of the XmlAppSettings class.
        /// </summary>
        /// <param name="filePath">
        /// The path of a file containing an XML-serialized XmlAppSettings object.
        /// </param>
        public XmlAppSettings(string filePath)
        {
            this.filePath = filePath;
            if (System.IO.File.Exists(filePath))
            {
                this.Load(filePath);
            }
            else
            {
                if (this.AddedPrincipals == null)
                {
                    this.AddedPrincipals = new PrincipalList();
                }
                this.Save(filePath);
            }
        }

        /// <summary>
        /// Gets the path of the file in which settings are stored.
        /// </summary>
        private static string SettingsFilePath
        {
            get
            {
                const string XmlAppSettingsFile = "settings.xml";
                string filePath = System.IO.Path.Combine(System.Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Poop Crapson");
                /*
                filePath = System.IO.Path.Combine(filePath, "Schedule Data Viewer");
                filePath = System.IO.Path.Combine(filePath, "0.1.0");
                */
                System.IO.Directory.CreateDirectory(filePath);
                filePath = System.IO.Path.Combine(filePath, XmlAppSettingsFile);
                return filePath;
            }
        }

        /*
        public System.Collections.Generic.SortedList<string, DateTime?> SerializablePrincipals
        {
            get
            {
                SortedList<string, DateTime?> returnDictionary = new SortedList<string, DateTime?>();
                foreach (SecurityIdentifier sid in AddedPrincipals.Keys)
                {
                    returnDictionary.Add(sid.Value, AddedPrincipals[sid]);
                }
                return returnDictionary;
            }
            set
            {
            }
        }
        */


        
        public void AddPrincipal(SecurityIdentifier principalSecurityIdentifier, DateTime? expirationTime)
        {
            if (this.AddedPrincipals.Contains(principalSecurityIdentifier))
            {
                if (this.AddedPrincipals[principalSecurityIdentifier].ExpirationTime.HasValue)
                {
                    if (expirationTime.HasValue)
                    {
                        // Choose the earlier of the two expiration times.
                        this.AddedPrincipals[principalSecurityIdentifier].ExpirationTime = DateTime.FromFileTime(Math.Min(expirationTime.Value.ToFileTime(), this.AddedPrincipals[principalSecurityIdentifier].ExpirationTime.Value.ToFileTime()));
                    }
                    else
                    { // expirationTime doesn't have a value, but the currently-stored principal does.
                        // Err on the side of safety and keep the currently-stored value.
                    }
                }
                else if (expirationTime.HasValue)
                { // The currently-stored principal does not have an expiration value, but expirationTime does.
                    this.AddedPrincipals[principalSecurityIdentifier].ExpirationTime = expirationTime;
                }
            }
            else
            {
                this.AddedPrincipals.Add(new Principal(principalSecurityIdentifier, expirationTime, null));
            }
            this.Save();
        }

        /*
        public void RemovePrincipal(SecurityIdentifier principalSecurityIdentifier)
        {
            XmlAppSettings encryptedSettings = new XmlAppSettings();
            if (encryptedSettings.AddedPrincipals.ContainsKey(principalSecurityIdentifier))
            {
                encryptedSettings.AddedPrincipals.Remove(principalSecurityIdentifier);
                encryptedSettings.Save();
            }
        }
        */

        /*
        public Dictionary<SecurityIdentifier, DateTime?> GetAddedPrincipals()
        {
            XmlAppSettings encryptedSettings = new XmlAppSettings();
            return encryptedSettings.AddedPrincipals;
        }
        */


        /// <summary>
        /// Gets an XML serializer for the AppSettings class.
        /// </summary>
        public static XmlSerializer Serializer
        {
            get
            {
                XmlSerializer serializer = new XmlSerializer(typeof(XmlAppSettings));
                return serializer;
            }
        }


        /// <summary>
        /// Saves the settings.
        /// </summary>
        private void Save()
        {
            this.Save(XmlAppSettings.SettingsFilePath);
        }

        /// <summary>
        /// Saves the application settings to the specified file.
        /// </summary>
        /// <param name="filePath">The path of the file to which settings are to be saved.</param>
        private void Save(string filePath)
        {
            /*
            try
            {

                System.IO.MemoryStream plaintextStream = new System.IO.MemoryStream();
                System.Xml.XmlTextWriter plaintextWriter = new System.Xml.XmlTextWriter(plaintextStream, System.Text.Encoding.Unicode);
                XmlSerializer serializer = XmlAppSettings.Serializer;
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
            */
            try
            {
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(filePath));
                System.Xml.XmlTextWriter writer = new System.Xml.XmlTextWriter(filePath, System.Text.Encoding.Unicode);
                XmlSerializer serializer = XmlAppSettings.Serializer;
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

                XmlAppSettings deserializedSettings = null;
                System.IO.MemoryStream plaintextStream = new System.IO.MemoryStream(plaintextBytes);
                System.Xml.XmlTextReader reader = new XmlTextReader(plaintextStream);
                XmlSerializer serializer = XmlAppSettings.Serializer;
                lock (serializer)
                {
                    deserializedSettings = (XmlAppSettings)serializer.Deserialize(reader);
                }
                reader.Close();
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
