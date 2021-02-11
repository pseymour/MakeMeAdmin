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

namespace SinclairCC.MakeMeAdmin
{
    /// <summary>
    /// This enumeration contains all of the event IDs that are written to the log.
    /// </summary>
    public enum EventID : int
    {
        /// <summary>
        /// A user was added to the Administrators group.
        /// </summary>
        UserAddedToAdminsSuccess,

        /// <summary>
        /// A user was removed from the Administrators group.
        /// </summary>
        UserRemovedFromAdminsSuccess,

        /// <summary>
        /// The application failed to add a user to the Administrators group.
        /// </summary>
        UserAddedToAdminsFailure,

        /// <summary>
        /// The application failed to remove a user from the Administrators group.
        /// </summary>
        UserRemovedFromAdminsFailure,

        /// <summary>
        /// A user has been removed from the Administrators group by some
        /// external (to Make Me Admin) process.
        /// </summary>
        UserRemovedByExternalProcess,

        /// <summary>
        /// Information about a request for administrator rights that was received
        /// from a remote host.
        /// </summary>
        RemoteRequestInformation,

        /// <summary>
        /// A user session has changed.
        /// </summary>
        SessionChangeEvent,

        /// <summary>
        /// Syslog server info, stored in the registry, is invalid in some way.
        /// </summary>
        RejectedSyslogServerInfo,

        /// <summary>
        /// An elevated process was detected.
        /// </summary>
        ElevatedProcess = 101,

        /// <summary>
        /// Event ID for debug messages.
        /// </summary>
        DebugMessage = 9000
    }
}
