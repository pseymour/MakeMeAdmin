// <copyright file="WTS_CONNECTSTATE_CLASS.cs" company="Sinclair Community College">
// Copyright (c) 2010-2017, Sinclair Community College. All rights reserved.
// </copyright>

namespace LsaLogonSessions
{
    /// <summary>
    /// Specifies the connection state of a Remote Desktop Services session.
    /// </summary>
    internal enum WTS_CONNECTSTATE_CLASS
    {
        /// <summary>
        /// A user is logged on to the WinStation.
        /// </summary>
        WTSActive,

        /// <summary>
        /// The WinStation is connected to the client.
        /// </summary>
        WTSConnected,

        /// <summary>
        /// The WinStation is in the process of connecting to the client.
        /// </summary>
        WTSConnectQuery,

        /// <summary>
        /// The WinStation is shadowing another WinStation.
        /// </summary>
        WTSShadow,

        /// <summary>
        /// The WinStation is active but the client is disconnected.
        /// </summary>
        WTSDisconnected,

        /// <summary>
        /// The WinStation is waiting for a client to connect.
        /// </summary>
        WTSIdle,

        /// <summary>
        /// The WinStation is listening for a connection.
        /// </summary>
        /// <remarks>
        /// A listener session waits for requests for new client connections. No user is logged on to a listener session.
        /// A listener session cannot be reset, shadowed, or changed to a regular client session.
        /// </remarks>
        WTSListen,

        /// <summary>
        /// The WinStation is being reset.
        /// </summary>
        WTSReset,

        /// <summary>
        /// The WinStation is down due to an error.
        /// </summary>
        WTSDown,

        /// <summary>
        /// The WinStation is initializing.
        /// </summary>
        WTSInit
    }
}
