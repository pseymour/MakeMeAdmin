// <copyright file="SidAttributes.cs" company="Sinclair Community College">
// Copyright (c) 2010-2017, Sinclair Community College. All rights reserved.
// </copyright>

namespace LsaLogonSessions
{
    /// <summary>
    /// Defines the SID attributes that can be specified in a SID_AND_ATTRIBUTES structure.
    /// </summary>
    [System.Flags]
    internal enum SidAttributes : uint
    {
        /// <summary>
        /// The SID is enabled for access checks. When the system performs an access check,
        /// it checks for access-allowed and access-denied access control entries (ACEs) that apply to the SID.
        /// </summary>
        /// <remarks>
        /// A SID without this attribute is ignored during an access check unless the
        /// SE_GROUP_USE_FOR_DENY_ONLY attribute is set.
        /// </remarks>
        SE_GROUP_ENABLED = 0x00000004,

        /// <summary>
        /// The SID is enabled by default.
        /// </summary>
        SE_GROUP_ENABLED_BY_DEFAULT = 0x00000002,

        /// <summary>
        /// The SID is a mandatory integrity SID.
        /// </summary>
        SE_GROUP_INTEGRITY = 0x00000020,

        /// <summary>
        /// The SID is enabled for mandatory integrity checks.
        /// </summary>
        SE_GROUP_INTEGRITY_ENABLED = 0x00000040,

        /// <summary>
        /// The SID is a logon SID that identifies the logon session associated with an access token.
        /// </summary>
        SE_GROUP_LOGON_ID = 0xC0000000,

        /// <summary>
        /// The SID cannot have the SE_GROUP_ENABLED attribute cleared by a call to the
        /// AdjustTokenGroups function. However, you can use the CreateRestrictedToken function to
        /// convert a mandatory SID to a deny-only SID.
        /// </summary>
        SE_GROUP_MANDATORY = 0x00000001,

        /// <summary>
        /// The SID identifies a group account for which the user of the token is the owner of the
        /// group, or the SID can be assigned as the owner of the token or objects.
        /// </summary>
        SE_GROUP_OWNER = 0x00000008,

        /// <summary>
        /// The SID identifies a domain-local group.
        /// </summary>
        SE_GROUP_RESOURCE = 0x20000000,

        /// <summary>
        /// The SID is a deny-only SID in a restricted token. When the system performs an access check,
        /// it checks for access-denied ACEs that apply to the SID; it ignores access-allowed ACEs for the SID.
        /// </summary>
        /// <remarks>
        /// If this attribute is set, SE_GROUP_ENABLED is not set, and the SID cannot be reenabled.
        /// </remarks>
        SE_GROUP_USE_FOR_DENY_ONLY = 0x00000010
    }
}
