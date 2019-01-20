// 
// Copyright © 2010-2019, Sinclair Community College
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
