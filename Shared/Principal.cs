// <copyright file="Principal.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.Security.Principal;

    public class Principal
    {
        private WindowsIdentity userIdentity;
        private DateTime expirationDateTime;
        private string remoteHostAddress;

        public Principal(WindowsIdentity userIdentity, DateTime expirationDateTime) : this(userIdentity, expirationDateTime, null) { }

        public Principal(WindowsIdentity userIdentity, DateTime expirationDateTime, string remoteHostAddress)
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

        public DateTime ExpirationTime
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
