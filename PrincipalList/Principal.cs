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
    using System.Security.Principal;
    using System.Xml;
    using System.Xml.Serialization;

    [Serializable]
    [XmlRoot(ElementName = "principal")]
    public class Principal
    {
        /*
        private WindowsIdentity userIdentity;
        */
        
        private SecurityIdentifier principalSecurityIdentifier;
        private string principalName;
        private DateTime? expirationDateTime;
        private string remoteHostAddress;

        private Principal() { }

        private Principal(/* WindowsIdentity userIdentity */ SecurityIdentifier principalSecurityIdentifier, DateTime? expirationDateTime)
            : this(/* userIdentity */ principalSecurityIdentifier, expirationDateTime, null)
        {
        }

        public Principal(WindowsIdentity userIdentity, DateTime? expirationDateTime, string remoteHostAddress)
            : this(userIdentity.User, userIdentity.Name, expirationDateTime, remoteHostAddress)
        {
        }

        public Principal(/* WindowsIdentity userIdentity */ SecurityIdentifier principalSecurityIdentifier, DateTime? expirationDateTime, string remoteHostAddress)
            : this(principalSecurityIdentifier, string.Empty, expirationDateTime, remoteHostAddress)
        {
        }

        public Principal(/* WindowsIdentity userIdentity */ SecurityIdentifier principalSecurityIdentifier, string principalName, DateTime? expirationDateTime, string remoteHostAddress)
        {
            /*
            this.userIdentity = userIdentity;
            */
            this.principalSecurityIdentifier = principalSecurityIdentifier;
            this.principalName = principalName;            
            this.expirationDateTime = expirationDateTime;
            this.remoteHostAddress = remoteHostAddress;
        }

        [XmlIgnore]
        public SecurityIdentifier PrincipalSid
        {
            get { return /* this.userIdentity.User */ this.principalSecurityIdentifier; }
        }

        [XmlAttribute("sid")]
        public string SidString
        {
            get { return this.principalSecurityIdentifier.Value; }
            set { this.principalSecurityIdentifier = new SecurityIdentifier(value); }
        }

        [XmlIgnore]
        public string PrincipalName
        {
            get { return this.principalName; }
        }

        [XmlElement(ElementName = "expirationDateTime")]
        public DateTime? ExpirationTime
        {
            get { return this.expirationDateTime; }
            set { this.expirationDateTime = value; }
        }

        [XmlElement(ElementName = "remoteAddress")]
        public string RemoteAddress
        {
            get { return this.remoteHostAddress; }
            set { this.remoteHostAddress = value; }
        }

    }
}
