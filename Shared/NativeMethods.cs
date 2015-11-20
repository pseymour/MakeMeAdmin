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
    }
}
