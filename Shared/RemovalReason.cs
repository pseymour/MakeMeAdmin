// <copyright file="RemovalReason.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    /// <summary>
    /// This enumeration contains all of the reasons that a principal is removed
    /// from the Administrators group.
    /// </summary>
    public enum RemovalReason : int
    {
        /// <summary>
        /// The principal's rights expired.
        /// </summary>
        Timeout,

        ServiceStopped,

        UserLogoff,

        /// <summary>
        /// A user requested to be removed from the Administrators group.
        /// </summary>
        UserRequest
    }
}
