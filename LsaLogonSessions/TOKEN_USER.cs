// <copyright file="TOKEN_USER.cs" company="Sinclair Community College">
// Copyright (c) 2010-2017, Sinclair Community College. All rights reserved.
// </copyright>

namespace LsaLogonSessions
{
    /// <summary>
    /// Identifies the user associated with an access token.
    /// </summary>
    internal struct TOKEN_USER
    {
        /// <summary>
        /// Represents the user associated with the access token.
        /// </summary>
        public SID_AND_ATTRIBUTES User;
    }
}
