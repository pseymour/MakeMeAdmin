// <copyright file="NativeMethods.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SinclairCC.MakeMeAdmin
{
    internal class NativeMethods
    {
        //[StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal struct LOCALGROUP_MEMBERS_INFO_0
        {
            public IntPtr lgrmi0_sid;
        }

        [DllImport("Netapi32.dll", SetLastError = true)]
        internal static extern int NetApiBufferFree(IntPtr Buffer);

        [DllImport("NetApi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern Int32 NetLocalGroupAddMembers(
            [MarshalAs(UnmanagedType.LPWStr)] string servername,
            [MarshalAs(UnmanagedType.LPWStr)] string localgroupname,
            int level, //info level 
            ref LOCALGROUP_MEMBERS_INFO_0 newMembers,
            int totalentries //number of entries 
            );

        [DllImport("NetApi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        internal static extern Int32 NetLocalGroupDelMembers(
            [MarshalAs(UnmanagedType.LPWStr)] string servername,
            [MarshalAs(UnmanagedType.LPWStr)] string localgroupname,
            int level, //info level 
            ref LOCALGROUP_MEMBERS_INFO_0 newMembers,
            int totalentries //number of entries 
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
