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

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.Security.Principal;

    public class Principal
    {
        private WindowsIdentity userIdentity;
        private DateTime? expirationDateTime;
        private string remoteHostAddress;

        public Principal(WindowsIdentity userIdentity, DateTime? expirationDateTime) : this(userIdentity, expirationDateTime, null) { }

        public Principal(WindowsIdentity userIdentity, DateTime? expirationDateTime, string remoteHostAddress)
        {
            this.userIdentity = userIdentity;
            this.expirationDateTime = expirationDateTime;
            this.remoteHostAddress = remoteHostAddress;
        }

        public SecurityIdentifier PrincipalSid
        {
            get { return this.userIdentity.User; }
        }

        public string PrincipalName
        {
            get { return this.userIdentity.Name; }
        }

        public DateTime? ExpirationTime
        {
            get { return this.expirationDateTime; }
            set { this.expirationDateTime = value; }
        }

        public string RemoteAddress
        {
            get { return this.remoteHostAddress; }
            set { this.remoteHostAddress = value; }
        }

    }
}
