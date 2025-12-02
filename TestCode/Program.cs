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
    using System;

    /// <summary>
    /// This class defines the main entry point for the application.
    /// </summary>
    internal class Program
    {
        /*
        [StructLayout(LayoutKind.Sequential)]
        internal struct LSA_UNICODE_STRING
        {
            public ushort Length;
            public ushort MaximumLength;
            public IntPtr Buffer;   
        }
        */

        /*
        [StructLayout(LayoutKind.Sequential)]
        private struct LSA_UNICODE_STRING
        {
            public UInt16 Length;
            public UInt16 MaximumLength;
            public IntPtr Buffer;
        }
        */

        /*
        [StructLayout(LayoutKind.Sequential)]
        internal struct LSA_OBJECT_ATTRIBUTES
        {
            public uint Length;
            public IntPtr RootDirectory;
            public LSA_UNICODE_STRING ObjectName;
            public uint Attributes;
            public IntPtr SecurityDescriptor;
            public IntPtr SecurityQualityOfService;
        }

        [Flags]
        public enum ACCESS_MASK : uint
        {
            DELETE = 0x00010000,
            READ_CONTROL = 0x00020000,
            WRITE_DAC = 0x00040000,
            WRITE_OWNER = 0x00080000,
            SYNCHRONIZE = 0x00100000,

            STANDARD_RIGHTS_REQUIRED = 0x000F0000,

            STANDARD_RIGHTS_READ = 0x00020000,
            STANDARD_RIGHTS_WRITE = 0x00020000,
            STANDARD_RIGHTS_EXECUTE = 0x00020000,

            STANDARD_RIGHTS_ALL = 0x001F0000,

            SPECIFIC_RIGHTS_ALL = 0x0000FFFF,

            ACCESS_SYSTEM_SECURITY = 0x01000000,

            MAXIMUM_ALLOWED = 0x02000000,

            GENERIC_READ = 0x80000000,
            GENERIC_WRITE = 0x40000000,
            GENERIC_EXECUTE = 0x20000000,
            GENERIC_ALL = 0x10000000,

            DESKTOP_READOBJECTS = 0x00000001,
            DESKTOP_CREATEWINDOW = 0x00000002,
            DESKTOP_CREATEMENU = 0x00000004,
            DESKTOP_HOOKCONTROL = 0x00000008,
            DESKTOP_JOURNALRECORD = 0x00000010,
            DESKTOP_JOURNALPLAYBACK = 0x00000020,
            DESKTOP_ENUMERATE = 0x00000040,
            DESKTOP_WRITEOBJECTS = 0x00000080,
            DESKTOP_SWITCHDESKTOP = 0x00000100,

            WINSTA_ENUMDESKTOPS = 0x00000001,
            WINSTA_READATTRIBUTES = 0x00000002,
            WINSTA_ACCESSCLIPBOARD = 0x00000004,
            WINSTA_CREATEDESKTOP = 0x00000008,
            WINSTA_WRITEATTRIBUTES = 0x00000010,
            WINSTA_ACCESSGLOBALATOMS = 0x00000020,
            WINSTA_EXITWINDOWS = 0x00000040,
            WINSTA_ENUMERATE = 0x00000100,
            WINSTA_READSCREEN = 0x00000200,

            WINSTA_ALL_ACCESS = 0x0000037F
        }


        static long DELETE = 0x00010000L;
        static long READ_CONTROL = 0x00020000L;
        static long WRITE_DAC = 0x00040000L;
        static long WRITE_OWNER = 0x00080000L;
        static long SYNCHRONIZE = 0x00100000L;

        static long STANDARD_RIGHTS_REQUIRED = 0x000F0000L;

        static long STANDARD_RIGHTS_READ = READ_CONTROL;
        static long STANDARD_RIGHTS_WRITE = READ_CONTROL;
        static long STANDARD_RIGHTS_EXECUTE = READ_CONTROL;

        static long STANDARD_RIGHTS_ALL = 0x001F0000L;

        static long SPECIFIC_RIGHTS_ALL = 0x0000FFFFL;
        */

        /*

        //
        // Access types for the Policy object
        //

        static long POLICY_VIEW_LOCAL_INFORMATION    =          0x00000001L;
        static long POLICY_VIEW_AUDIT_INFORMATION     =         0x00000002L;
        static long POLICY_GET_PRIVATE_INFORMATION     =        0x00000004L;
        static long POLICY_TRUST_ADMIN                 =        0x00000008L;
        static long POLICY_CREATE_ACCOUNT              =        0x00000010L;
        static long POLICY_CREATE_SECRET               =        0x00000020L;
        static long POLICY_CREATE_PRIVILEGE            =        0x00000040L;
        static long POLICY_SET_DEFAULT_QUOTA_LIMITS    =        0x00000080L;
        static long POLICY_SET_AUDIT_REQUIREMENTS      =        0x00000100L;
        static long POLICY_AUDIT_LOG_ADMIN             =        0x00000200L;
        static long POLICY_SERVER_ADMIN                =        0x00000400L;
        static long POLICY_LOOKUP_NAMES                =        0x00000800L;
        static long POLICY_NOTIFICATION                =        0x00001000L;

        static long POLICY_ALL_ACCESS = (STANDARD_RIGHTS_REQUIRED |
                                  POLICY_VIEW_LOCAL_INFORMATION |
                                  POLICY_VIEW_AUDIT_INFORMATION |
                                  POLICY_GET_PRIVATE_INFORMATION |
                                  POLICY_TRUST_ADMIN |
                                  POLICY_CREATE_ACCOUNT |
                                  POLICY_CREATE_SECRET |
                                  POLICY_CREATE_PRIVILEGE |
                                  POLICY_SET_DEFAULT_QUOTA_LIMITS |
                                  POLICY_SET_AUDIT_REQUIREMENTS |
                                  POLICY_AUDIT_LOG_ADMIN |
                                  POLICY_SERVER_ADMIN |
                                  POLICY_LOOKUP_NAMES);

        */
        
        /*
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true, PreserveSig = true)]
        internal static extern bool LookupAccountName(
            string lpSystemName,
            string lpAccountName,
            IntPtr psid,
            ref int cbsid,
            System.Text.StringBuilder domainName,
            ref int cbdomainLength,
            ref int use);
        */

        /*


static long POLICY_READ     =      (STANDARD_RIGHTS_READ             |\
                               POLICY_VIEW_AUDIT_INFORMATION    |\
                               POLICY_GET_PRIVATE_INFORMATION)

static long POLICY_WRITE   =       (STANDARD_RIGHTS_WRITE            |\
                               POLICY_TRUST_ADMIN               |\
                               POLICY_CREATE_ACCOUNT            |\
                               POLICY_CREATE_SECRET             |\
                               POLICY_CREATE_PRIVILEGE          |\
                               POLICY_SET_DEFAULT_QUOTA_LIMITS  |\
                               POLICY_SET_AUDIT_REQUIREMENTS    |\
                               POLICY_AUDIT_LOG_ADMIN           |\
                               POLICY_SERVER_ADMIN)

static long POLICY_EXECUTE    =    (STANDARD_RIGHTS_EXECUTE          |\
                               POLICY_VIEW_LOCAL_INFORMATION    |\
                               POLICY_LOOKUP_NAMES)
            */

        /*
        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern uint LsaOpenPolicy(
            [In] [Optional] ref LSA_UNICODE_STRING SystemName,
            [In] ref LSA_OBJECT_ATTRIBUTES ObjectAttributes,
            [In] int DesiredAccess,
            [Out] out IntPtr PolicyHandle        
            );
        */

        /*
        [DllImport("advapi32.dll", PreserveSig = true)]
        private static extern UInt32 LsaOpenPolicy(
            ref LSA_UNICODE_STRING SystemName,
            ref LSA_OBJECT_ATTRIBUTES ObjectAttributes,
            Int32 DesiredAccess,
            out IntPtr PolicyHandle);
        */

            /*
        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        private static extern int LsaAddAccountRights(
            IntPtr PolicyHandle,
            // IntPtr AccountSid,
            [In] [MarshalAs(UnmanagedType.LPArray)] byte[] AccountSid,
            LSA_UNICODE_STRING[] UserRights,
            int CountOfRights);
        */

        /*
        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        private static extern int LsaRemoveAccountRights(
            IntPtr PolicyHandle,
            // IntPtr AccountSid,
            [In] [MarshalAs(UnmanagedType.LPArray)] byte[] AccountSid,
            [In] int AllRights,
            LSA_UNICODE_STRING[] UserRights,
            int CountOfRights);
        */

        /*
        NTSTATUS LsaAddAccountRights(
          _In_ LSA_HANDLE          PolicyHandle,
          _In_ PSID                AccountSid,
          _In_ PLSA_UNICODE_STRING UserRights,
          _In_ ULONG               CountOfRights
        );

        NTSTATUS LsaRemoveAccountRights(
          _In_ LSA_HANDLE          PolicyHandle,
          _In_ PSID                AccountSid,
          _In_ BOOLEAN             AllRights,
          _In_ PLSA_UNICODE_STRING UserRights,
          _In_ ULONG               CountOfRights
        );

        NTSTATUS LsaEnumerateAccountRights(
          _In_ LSA_HANDLE          PolicyHandle,
          _In_ PSID                AccountSid,
          _Out_ PLSA_UNICODE_STRING *UserRights,
          _Out_ PULONG              CountOfRights
        );
        */

        //[DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        //internal static extern long LsaEnumerateAccountRights(
        //    [In] IntPtr PolicyHandle,
        //    [In] IntPtr AccountSid,
        //    // [MarshalAs(UnmanagedType.LPArray)] byte[] AccountSid,
        //    [Out] out LSA_UNICODE_STRING[] /* IntPtr */ UserRights,
        //    [Out] out long CountOfRights);

        /*
        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        internal static extern int LsaEnumerateAccountRights(
            [In] IntPtr PolicyHandle,
            // [In] IntPtr AccountSid,
            [In] [MarshalAs(UnmanagedType.LPArray)] byte[] AccountSid,
            [Out] out IntPtr UserRightsPtr,
            [Out] out int CountOfRights);
        */

        /*
        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern uint LsaClose(
          [In] IntPtr ObjectHandle
        );
        */

        /*
        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern long LsaNtStatusToWinError(long status);
        */

        /*
        [DllImport("kernel32.dll")]
        private static extern int GetLastError();
        */

        /*
        [DllImport("advapi32.dll")]
        public static extern void FreeSid(IntPtr pSid);
        */

        /*
        private static System.Collections.Specialized.StringCollection GetRights(WindowsIdentity identity)
        {
            System.Collections.Specialized.StringCollection rights = new System.Collections.Specialized.StringCollection();

            LSA_OBJECT_ATTRIBUTES objectAttributes = CreateLSAObjectAttributes();
            IntPtr policyHandle = IntPtr.Zero;

            LSA_UNICODE_STRING SystemName = new LSA_UNICODE_STRING();

            //SystemName.Length = (ushort)(System.Environment.MachineName.Length * System.Text.UnicodeEncoding.CharSize);
            //SystemName.MaximumLength = (ushort)((System.Environment.MachineName.Length + 1) * System.Text.UnicodeEncoding.CharSize);
            //SystemName.Buffer = System.Environment.MachineName; // Marshal.StringToHGlobalUni(System.Environment.MachineName);


            // Open a policy handle on the remote PC.
            uint ret = LsaOpenPolicy(ref SystemName, ref objectAttributes, (int)POLICY_ALL_ACCESS, out policyHandle);
            if (ret == 0)
            {

                //long winErrorCode = 0;
                //IntPtr sid = IntPtr.Zero;
                //int sidSize = 0;
                //System.Text.StringBuilder domainName = new System.Text.StringBuilder();
                //int nameSize = 0;
                //int accountType = 0;
                long winErrorCode = 0;

                IntPtr userRightsPtr = IntPtr.Zero;
                int countOfRights = 0;

                byte[] accountSidBytes = new byte[identity.User.BinaryLength];
                identity.User.GetBinaryForm(accountSidBytes, 0);
                int result = LsaEnumerateAccountRights(policyHandle, accountSidBytes, out userRightsPtr, out countOfRights);

                winErrorCode = LsaNtStatusToWinError(result);
                if (winErrorCode == 0)
                {
                    long current = userRightsPtr.ToInt64();
                    LSA_UNICODE_STRING userRight;

                    for (int i = 0; i < countOfRights; i++)
                    {
                        userRight = (LSA_UNICODE_STRING)Marshal.PtrToStructure(new IntPtr(current), typeof(LSA_UNICODE_STRING));
                        string userRightStr = Marshal.PtrToStringAuto(userRight.Buffer);
                        rights.Add(userRightStr);
                        current += Marshal.SizeOf(userRight);
                    }
                }

                LsaClose(policyHandle);
            }

            return rights;
        }
        */

        /*
        private static void SetRight(WindowsIdentity identity, string privilegeName)
        {
            LSA_OBJECT_ATTRIBUTES objectAttributes = CreateLSAObjectAttributes();
            IntPtr policyHandle = IntPtr.Zero;

            LSA_UNICODE_STRING SystemName = new LSA_UNICODE_STRING();

            
            //SystemName.Length = (ushort)(System.Environment.MachineName.Length * System.Text.UnicodeEncoding.CharSize);
            //SystemName.MaximumLength = (ushort)((System.Environment.MachineName.Length + 1) * System.Text.UnicodeEncoding.CharSize);
            //SystemName.Buffer = System.Environment.MachineName; // Marshal.StringToHGlobalUni(System.Environment.MachineName);
            


            // Open a policy handle on the remote PC.
            uint ret = LsaOpenPolicy(ref SystemName, ref objectAttributes, (int)POLICY_ALL_ACCESS, out policyHandle);
            if (ret != 0)
            {
                Console.WriteLine(LsaNtStatusToWinError(ret));
            }
            else
            {
                // Console.WriteLine("LsaOpenPolicy succeeded!");

                //long winErrorCode = 0;
                //IntPtr sid = IntPtr.Zero;
                //int sidSize = 0;
                //System.Text.StringBuilder domainName = new System.Text.StringBuilder();
                //int nameSize = 0;
                //int accountType = 0;
                long winErrorCode = 0;

                
                //LookupAccountName(string.Empty, currentIdentity.Name, sid, ref sidSize, domainName, ref nameSize, ref accountType);
                

                //domainName = new System.Text.StringBuilder(nameSize);
                //sid = Marshal.AllocHGlobal(sidSize);

                //if (!LookupAccountName(string.Empty, currentIdentity.Name, sid, ref sidSize, domainName, ref nameSize, ref accountType))
                //{
                //    winErrorCode = GetLastError();
                //    Console.WriteLine("LookupAccountName failed: " + winErrorCode);
                //}
                //else
                //{
                IntPtr userRightsPtr = IntPtr.Zero;
                int countOfRights = 0;

                // int result = LsaEnumerateAccountRights(policyHandle, sid, out userRightsPtr, out countOfRights);

                LSA_UNICODE_STRING[] userRights = new LSA_UNICODE_STRING[1];
                userRights[0] = new LSA_UNICODE_STRING();
                userRights[0].Buffer = Marshal.StringToHGlobalUni(privilegeName);
                userRights[0].Length = (UInt16)(privilegeName.Length * System.Text.UnicodeEncoding.CharSize);
                userRights[0].MaximumLength = (UInt16)((privilegeName.Length + 1) * System.Text.UnicodeEncoding.CharSize);

                byte[] accountSidBytes = new byte[identity.User.BinaryLength];
                identity.User.GetBinaryForm(accountSidBytes, 0);
                int result = LsaAddAccountRights(policyHandle, accountSidBytes, userRights, userRights.Length);

                winErrorCode = LsaNtStatusToWinError(result);
                if (winErrorCode != 0)
                {
                    Console.WriteLine("LsaAddAccountRights failed: {0}", winErrorCode);
                }

                LsaClose(policyHandle);

            }
        }
        */

        /*
        private static void RemoveRight(WindowsIdentity identity, string privilegeName)
        {
            LSA_OBJECT_ATTRIBUTES objectAttributes = CreateLSAObjectAttributes();
            IntPtr policyHandle = IntPtr.Zero;

            LSA_UNICODE_STRING SystemName = new LSA_UNICODE_STRING();

            //SystemName.Length = (ushort)(System.Environment.MachineName.Length * System.Text.UnicodeEncoding.CharSize);
            //SystemName.MaximumLength = (ushort)((System.Environment.MachineName.Length + 1) * System.Text.UnicodeEncoding.CharSize);
            //SystemName.Buffer = System.Environment.MachineName; // Marshal.StringToHGlobalUni(System.Environment.MachineName);


            // Open a policy handle on the remote PC.
            uint ret = LsaOpenPolicy(ref SystemName, ref objectAttributes, (int)POLICY_ALL_ACCESS, out policyHandle);
            if (ret != 0)
            {
                Console.WriteLine(LsaNtStatusToWinError(ret));
            }
            else
            {
                // Console.WriteLine("LsaOpenPolicy succeeded!");

                //long winErrorCode = 0;
                //IntPtr sid = IntPtr.Zero;
                //int sidSize = 0;
                //System.Text.StringBuilder domainName = new System.Text.StringBuilder();
                //int nameSize = 0;
                //int accountType = 0;
                long winErrorCode = 0;

                //LookupAccountName(string.Empty, currentIdentity.Name, sid, ref sidSize, domainName, ref nameSize, ref accountType);

                //domainName = new System.Text.StringBuilder(nameSize);
                //sid = Marshal.AllocHGlobal(sidSize);

                //if (!LookupAccountName(string.Empty, currentIdentity.Name, sid, ref sidSize, domainName, ref nameSize, ref accountType))
                //{
                //    winErrorCode = GetLastError();
                //    Console.WriteLine("LookupAccountName failed: " + winErrorCode);
                //}
                //else
                //{
                IntPtr userRightsPtr = IntPtr.Zero;
                int countOfRights = 0;

                // int result = LsaEnumerateAccountRights(policyHandle, sid, out userRightsPtr, out countOfRights);

                LSA_UNICODE_STRING[] userRights = new LSA_UNICODE_STRING[1];
                userRights[0] = new LSA_UNICODE_STRING();
                userRights[0].Buffer = Marshal.StringToHGlobalUni(privilegeName);
                userRights[0].Length = (UInt16)(privilegeName.Length * System.Text.UnicodeEncoding.CharSize);
                userRights[0].MaximumLength = (UInt16)((privilegeName.Length + 1) * System.Text.UnicodeEncoding.CharSize);

                byte[] accountSidBytes = new byte[identity.User.BinaryLength];
                identity.User.GetBinaryForm(accountSidBytes, 0);
                int result = LsaRemoveAccountRights(policyHandle, accountSidBytes, 0, userRights, userRights.Length);

                winErrorCode = LsaNtStatusToWinError(result);
                if (winErrorCode != 0)
                {
                    Console.WriteLine("LsaRemoveAccountRights failed: {0}", winErrorCode);
                }

                LsaClose(policyHandle);

            }
        }
        */

        /*
        // Returns the Local Security Authority rights granted to the account
        public static System.Collections.Generic.IList<string> GetRights(string accountName)
        {
            System.Collections.Generic.IList<string> rights = new System.Collections.Generic.List<string>();
            string errorMessage = string.Empty;

            long winErrorCode = 0;
            IntPtr sid = IntPtr.Zero;
            int sidSize = 0;
            System.Text.StringBuilder domainName = new System.Text.StringBuilder();
            int nameSize = 0;
            int accountType = 0;

            LookupAccountName(string.Empty, accountName, sid, ref sidSize, domainName, ref nameSize, ref accountType);

            domainName = new System.Text.StringBuilder(nameSize);
            sid = Marshal.AllocHGlobal(sidSize);

            if (!LookupAccountName(string.Empty, accountName, sid, ref sidSize, domainName, ref nameSize, ref accountType))
            {
                winErrorCode = GetLastError();
                errorMessage = ("LookupAccountName failed: " + winErrorCode);
            }
            else
            {
                LSA_UNICODE_STRING systemName = new LSA_UNICODE_STRING();

                IntPtr policyHandle = IntPtr.Zero;
                IntPtr userRightsPtr = IntPtr.Zero;
                int countOfRights = 0;

                LSA_OBJECT_ATTRIBUTES objectAttributes = CreateLSAObject();

                uint policyStatus = LsaOpenPolicy(ref systemName, ref objectAttributes, (int)POLICY_ALL_ACCESS, out policyHandle);
                winErrorCode = LsaNtStatusToWinError(Convert.ToInt32(policyStatus));

                if (winErrorCode != 0)
                {
                    errorMessage = string.Format("OpenPolicy failed: {0}.", winErrorCode);
                }
                else
                {
                    int result = LsaEnumerateAccountRights(policyHandle, sid, out userRightsPtr, out countOfRights);
                    winErrorCode = LsaNtStatusToWinError(result);
                    if (winErrorCode != 0)
                    {
                        errorMessage = string.Format("LsaEnumerateAccountRights failed: {0}", winErrorCode);
                    }

                    Int32 ptr = userRightsPtr.ToInt32();
                    LSA_UNICODE_STRING userRight;

                    for (int i = 0; i < countOfRights; i++)
                    {
                        userRight = (LSA_UNICODE_STRING)Marshal.PtrToStructure(new IntPtr(ptr), typeof(LSA_UNICODE_STRING));
                        string userRightStr = Marshal.PtrToStringAuto(userRight.Buffer);
                        rights.Add(userRightStr);
                        ptr += Marshal.SizeOf(userRight);
                    }
                    LsaClose(policyHandle);
                }
                FreeSid(sid);
            }
            if (winErrorCode > 0)
            {
                throw new ApplicationException(string.Format("Error occurred in LSA, error code {0}, detail: {1}", winErrorCode, errorMessage));
            }
            return rights;
        }
        */

        /*
        private static LSA_OBJECT_ATTRIBUTES CreateLSAObjectAttributes()
        {
            LSA_OBJECT_ATTRIBUTES objectAttributes = new LSA_OBJECT_ATTRIBUTES();

            objectAttributes.Length = (uint)Marshal.SizeOf(objectAttributes);
            objectAttributes.RootDirectory = IntPtr.Zero;
            objectAttributes.ObjectName = new LSA_UNICODE_STRING();
            objectAttributes.Attributes = 0;
            objectAttributes.SecurityDescriptor = IntPtr.Zero;
            objectAttributes.SecurityQualityOfService = IntPtr.Zero;

            return objectAttributes;
        }
        */

        /*
        public static void GetLocalAdminNames()
        {
            // LDAP://servername/<SID=XXXXX>
            string adminsSID = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null).ToString();
            DirectoryEntry localAdmins = new DirectoryEntry(string.Format("WinNT://{0}/{1}", System.Environment.MachineName, LocalAdministratorGroup.LocalAdminGroupName));
            Console.WriteLine(localAdmins.Name);
            localAdmins.UsePropertyCache = true;
            object members = localAdmins.Invoke("members", null);
            localAdmins.Dispose();
        }
        */

        /*
        public static List<string> GetLocalAdministratorsNames()
        {
            List<string> admins = new List<string>();
            DirectoryEntry localMachine = new DirectoryEntry("WinNT://" + Environment.MachineName);
            string adminsSID = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null).ToString();

            string localizedAdmin = new SecurityIdentifier(adminsSID).Translate(typeof(NTAccount)).ToString();

            localizedAdmin = localizedAdmin.Replace(@"BUILTIN\", "");

            DirectoryEntry admGroup = localMachine.Children.Find(localizedAdmin, "group");
            object adminmembers = admGroup.Invoke("members", null);

            //DirectoryEntry userGroup = localMachine.Children.Find("users", "group");
            ///object usermembers = userGroup.Invoke("members", null);

            //Retrieve each user name.
            foreach (object groupMember in (IEnumerable)adminmembers)
            {
                DirectoryEntry member = new DirectoryEntry(groupMember);

                string sidAsText = member.Path; // GetTextualSID(member);
                admins.Add(member.Name);
            }
            return admins;
        }
        */

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        internal static void Main()
        {
            Console.WriteLine("Main() starting at {0}.", DateTime.Now);

#if DEBUG
            Console.WriteLine("\n\nMain() finished at {0}.", DateTime.Now);
            Console.Write("Press <ENTER> to close this program.");
            Console.ReadLine();
#endif
        }

    }
}
