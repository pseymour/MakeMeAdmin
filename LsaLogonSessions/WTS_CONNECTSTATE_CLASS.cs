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
