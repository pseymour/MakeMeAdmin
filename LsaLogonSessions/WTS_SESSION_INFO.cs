// <copyright file="WTS_SESSION_INFO.cs" company="Sinclair Community College">
// Copyright (c) 2010-2017, Sinclair Community College. All rights reserved.
// </copyright>

namespace LsaLogonSessions
{
    using System.Runtime.InteropServices;

    /// <summary>
    /// Contains information about a client session on a Remote Desktop Session Host (RD Session Host) server.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    internal struct WTS_SESSION_INFO
    {
        /// <summary>
        /// Session identifier of the session.
        /// </summary>
        public int SessionID;

        /// <summary>
        /// Contains the WinStation name of the session.
        /// </summary>
        /// <remarks>
        /// The WinStation name is a name that Windows associates with the session.
        /// </remarks>
        /// <example>
        /// "Services," "Console," or "RDP-Tcp#0."
        /// </example>
        [MarshalAs(UnmanagedType.LPStr)]
        public string WinStationName;

        /// <summary>
        /// Indicates the session's current connection state.
        /// </summary>
        public WTS_CONNECTSTATE_CLASS State;
    }
}
