// <copyright file="TOKEN_GROUPS.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

namespace LsaLogonSessions
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Contains information about the group security identifiers (SIDs) in an access token.
    /// </summary>
    internal struct TOKEN_GROUPS
    {
        /// <summary>
        /// The number of groups in the access token.
        /// </summary>
        public uint GroupCount;

        /// <summary>
        /// An array of SID_AND_ATTRIBUTES structures that contain a set of SIDs and corresponding attributes.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValArray)]
        public SID_AND_ATTRIBUTES[] Groups;
    }
}
