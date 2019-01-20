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
