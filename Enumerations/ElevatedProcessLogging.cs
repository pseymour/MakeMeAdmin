// 
// Copyright © 2010-2025, Sinclair Community College
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
    /// This enumeration specifies options for logging of elevated processes.
    /// </summary>
    public enum ElevatedProcessLogging : int
    {
        /// <summary>
        /// Never log elevated processes.
        /// </summary>
        Never,

        /// <summary>
        /// Only log elevated processes when the owner of the process has administrator rights.
        /// </summary>
        OnlyWhenAdmin,

        /// <summary>
        /// Always log elevated processes.
        /// </summary>
        Always
    }
}
