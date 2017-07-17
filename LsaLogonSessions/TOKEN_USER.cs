// <copyright file="TOKEN_USER.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

namespace LsaLogonSessions
{
    using System.Runtime.InteropServices;

    internal struct TOKEN_USER
    {
        public SID_AND_ATTRIBUTES User;
    }
}
