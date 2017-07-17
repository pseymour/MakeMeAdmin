// <copyright file="SID_AND_ATTRIBUTES.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

namespace LsaLogonSessions
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Represents a security identifier (SID) and its attributes.
    /// </summary>
    /// <remarks>
    /// SIDs are used to uniquely identify users or groups.
    /// </remarks>
    [StructLayout(LayoutKind.Sequential)]
    internal struct SID_AND_ATTRIBUTES
    {
        /// <summary>
        /// A pointer to a SID structure.
        /// </summary>
        public IntPtr Sid;

        /// <summary>
        /// Specifies attributes of the SID.
        /// </summary>
        /// <remarks>
        /// This value contains up to 32 one-bit flags. Its meaning depends on the definition and use of the SID.
        /// </remarks>
        public SidAttributes Attributes;
    }
}
