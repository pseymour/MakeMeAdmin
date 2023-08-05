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

using System;
using System.Runtime.InteropServices;

namespace SinclairCC.MakeMeAdmin
{
    internal class NativeMethods
    {
        /// <summary>
        /// Contains the security identifier (SID) associated with a local group member.
        /// </summary>
        internal struct LOCALGROUP_MEMBERS_INFO_0
        {
            /// <summary>
            /// Pointer to a SID structure that contains the security identifier
            /// (SID) of the local group member.
            /// </summary>
            public IntPtr lgrmi0_sid;
        }

        /// <summary>
        /// Frees the memory that the NetApiBufferAllocate function allocates.
        /// </summary>
        /// <param name="Buffer">
        /// A pointer to a buffer returned previously by another network management
        /// function or memory allocated by calling the NetApiBufferAllocate function.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is NERR_Success.
        /// If the function fails, the return value is a system error code.
        /// </returns>
        [DllImport("Netapi32.dll", SetLastError = true)]
        internal static extern int NetApiBufferFree(IntPtr Buffer);

        /// <summary>
        /// Adds membership of one or more existing user accounts or global group accounts to an existing local group. 
        /// </summary>
        /// <param name="servername">
        /// The DNS or NetBIOS name of the remote server on which the function is to execute.
        /// If this parameter is NULL, the local computer is used.
        /// </param>
        /// <param name="localgroupname">
        /// The name of the local group to which the specified users or global groups will be added.
        /// </param>
        /// <param name="level">
        /// Specifies the information level of the data.
        /// </param>
        /// <param name="newMembers">
        /// The data for the new local group members.
        /// </param>
        /// <param name="totalentries">
        /// Specifies the number of entries in the newMembers parameter.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is NERR_Success.
        /// If the function fails, the return value can be one of a few error codes (see Microsoft documentation).
        /// </returns>
        [DllImport("NetApi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern Int32 NetLocalGroupAddMembers(
            [MarshalAs(UnmanagedType.LPWStr)] string servername,
            [MarshalAs(UnmanagedType.LPWStr)] string localgroupname,
            int level,
            ref LOCALGROUP_MEMBERS_INFO_0 newMembers,
            int totalentries
            );

        /// <summary>
        /// Removes one or more members from an existing local group. Local group members can be users or global groups.
        /// </summary>
        /// <param name="servername">
        /// The DNS or NetBIOS name of the remote server on which the function is to execute.
        /// If this parameter is NULL, the local computer is used.
        /// </param>
        /// <param name="localgroupname">
        /// The name of the local group from which the specified users or global groups will be removed. 
        /// </param>
        /// <param name="level">
        /// Specifies the information level of the data.
        /// </param>
        /// <param name="newMembers">
        /// The data for the group members to be removed.
        /// </param>
        /// <param name="totalentries">
        /// Specifies the number of entries in the newMembers parameter.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is NERR_Success.
        /// If the function fails, the return value can be one of a few error codes (see Microsoft documentation).
        /// </returns>
        [DllImport("NetApi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern Int32 NetLocalGroupDelMembers(
            [MarshalAs(UnmanagedType.LPWStr)] string servername,
            [MarshalAs(UnmanagedType.LPWStr)] string localgroupname,
            int level,
            ref LOCALGROUP_MEMBERS_INFO_0 newMembers,
            int totalentries
            );

        /// <summary>
        /// The NetLocalGroupGetMembers function retrieves a list of the members of a particular local group in 
        /// the security database, which is the security accounts manager (SAM) database or, in the case 
        /// of domain controllers, the Active Directory. Local group members can be users or global groups.
        /// </summary>
        /// <param name="servername"></param>
        /// <param name="localgroupname"></param>
        /// <param name="level"></param>
        /// <param name="bufptr"></param>
        /// <param name="prefmaxlen"></param>
        /// <param name="entriesread"></param>
        /// <param name="totalentries"></param>
        /// <param name="resume_handle"></param>
        /// <returns></returns>
        [DllImport("NetAPI32.dll", CharSet = CharSet.Unicode)]
        internal extern static int NetLocalGroupGetMembers(
            [MarshalAs(UnmanagedType.LPWStr)] string servername,
            [MarshalAs(UnmanagedType.LPWStr)] string localgroupname,
            int level,
            out IntPtr bufptr,
            int prefmaxlen,
            out int entriesread,
            out int totalentries,
            IntPtr resume_handle);

        /// <summary>
        /// Ends a network session between a server and a workstation.
        /// </summary>
        /// <param name="ServerName">
        /// The DNS or NetBIOS name of the remote server on which the function is to execute. If this parameter is NULL, the local computer is used.
        /// </param>
        /// <param name="UncClientName">
        /// The computer name of the client to disconnect. If the UncClientName parameter is NULL, then all the sessions of the user identified by the username parameter will be deleted on the server specified by the servername parameter.
        /// </param>
        /// <param name="UserName">
        /// The name of the user whose session is to be terminated. If this parameter is NULL, all users' sessions from the client specified by the UncClientName parameter are to be terminated.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is zero (0).
        /// </returns>
        [DllImport("netapi32.dll", CharSet = CharSet.Unicode)]
        internal static extern int NetSessionDel(
                     [In, MarshalAs(UnmanagedType.LPWStr)] string ServerName,
                     [In, MarshalAs(UnmanagedType.LPWStr)] string UncClientName,
                     [In, MarshalAs(UnmanagedType.LPWStr)] string UserName);

    }
}
