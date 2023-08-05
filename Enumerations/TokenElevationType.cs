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
    public enum TokenElevationType : int
    {
        /// <summary>
        /// Type 1 is a full token with no privileges removed or groups disabled.
        /// </summary>
        /// <remarks>
        /// A full token is only used if User Account Control is disabled or if the user is the built-in
        /// Administrator account (for which UAC disabled by default), service account or local system account.
        /// </remarks>
        Default = 1,

        /// <summary>
        /// Type 2 is an elevated token with no privileges removed or groups disabled.
        /// </summary>
        /// <remarks>
        /// An elevated token is used when User Account Control is enabled and the user chooses to start
        /// the program using Run as administrator. An elevated token is also used when an application is
        /// configured to always require administrative privilege or to always require maximum privilege,
        /// and the user is a member of the Administrators group.
        /// </remarks>
        Full = 2,

        /// <summary>
        /// Type 3 is a limited token with administrative privileges removed and administrative groups disabled.
        /// </summary>
        /// <remarks>
        /// The limited token is used when User Account Control is enabled, the application does not require
        /// administrative privilege, and the user does not choose to start the program using Run as administrator.
        /// </remarks>
        Limited = 3
    }
}
