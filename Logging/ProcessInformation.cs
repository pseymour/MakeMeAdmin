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
    using System;

    /// <summary>
    /// Contains information about an elevated process.
    /// </summary>
    public class ProcessInformation
    {
        /// <summary>
        /// The full command line used to start the process.
        /// </summary>
        public string CommandLine { get; set; }

        /// <summary>
        /// The file name of the executable file.
        /// </summary>
        public string ImageFileName { get; set; }

        /// <summary>
        /// The ID of the process.
        /// </summary>
        public int ProcessID { get; set; }

        /// <summary>
        /// The ID of the session in which the process is running.
        /// </summary>
        public int SessionID { get; set; }

        /// <summary>
        /// The SID of the user corresponding to the given session ID.
        /// </summary>
        public string UserSIDString { get; set; }

        /// <summary>
        /// The date and time at which the process was created.
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
}
