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

    /// <summary>
    /// This class stores information about a security principal that has
    /// been added to the Administrators group.
    /// </summary>
    /// <remarks>
    /// This data is stored on disk so that the service may take action on
    /// those added principals after recovery from a potential crash.
    /// </remarks>
    [Serializable]
    [XmlRoot(ElementName = "principal")]
    public class Principal
    {
        /// <summary>
        /// The security identifier (SID) of the principal.
        /// </summary>
        private SecurityIdentifier principalSecurityIdentifier;

        /// <summary>
        /// The name of the principal (e.g., DOMAIN\UserName).
        /// </summary>
        private string principalName;

        /// <summary>
        /// The date and time at which the principal's administrator
        /// rights should expire.
        /// </summary>
        private DateTime? expirationDateTime;

        /// <summary>
        /// The address of the remote computer from which a request for
        /// admin rights came.
        /// </summary>
        private string remoteHostAddress;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <remarks>
        /// This constructor only exists to support serialization.
        /// </remarks>
        private Principal()
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="userIdentity">
        /// The identity of the principal.
        /// </param>
        /// <param name="expirationDateTime">
        /// The date and time at which the principal's administrator rights expire.
        /// </param>
        /// <param name="remoteHostAddress">
        /// The address from which the remote administrator rights request came.
        /// </param>
        public Principal(WindowsIdentity userIdentity, DateTime? expirationDateTime, string remoteHostAddress)
        {
            this.principalSecurityIdentifier = userIdentity.User;
            this.principalName = userIdentity.Name;
            this.expirationDateTime = expirationDateTime;
            this.remoteHostAddress = remoteHostAddress;
        }

        /// <summary>
        /// Gets the security identifier (SID) of the principal.
        /// </summary>
        /// <remarks>
        /// This member is not stored on disk, because SecurityIdentifier
        /// objects are not serializable.
        /// </remarks>
        [XmlIgnore]
        public SecurityIdentifier PrincipalSid
        {
            get { return this.principalSecurityIdentifier; }
        }

        /// <summary>
        /// Gets or sets the security identifier (SID) of the principal, in SDDL format.
        /// </summary>
        /// <remarks>
        /// This really only exists to support XML serialization and should
        /// not normally be used by code.
        /// </remarks>
        [XmlAttribute("sid")]
        public string SidString
        {
            get { return this.principalSecurityIdentifier.Value; }
            set { this.principalSecurityIdentifier = new SecurityIdentifier(value); }
        }

        /// <summary>
        /// Gets the name of the principal (e.g., DOMAIN\UserName).
        /// </summary>
        /// <remarks>
        /// This value is not serialized, because it is unnecessary.
        /// </remarks>
        [XmlIgnore]
        public string PrincipalName
        {
            get { return this.principalName; }
        }

        /// <summary>
        /// Gets or sets the date and time at which the principal's
        /// administrator rights should expire.
        /// </summary>
        [XmlElement(ElementName = "expirationDateTime")]
        public DateTime? ExpirationTime
        {
            get { return this.expirationDateTime; }
            set { this.expirationDateTime = value; }
        }

        /// <summary>
        /// Gets or sets the address from which a remote request for
        /// administrator rights came.
        /// </summary>
        [XmlElement(ElementName = "remoteAddress")]
        public string RemoteAddress
        {
            get { return this.remoteHostAddress; }
            set { this.remoteHostAddress = value; }
        }
    }
}
