// <copyright file="Program.cs" company="Sinclair Community College">
// Copyright (c) 2010-2017, Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.ServiceModel;
    using System.Security.Principal;
    using System.Runtime.InteropServices;
    using System.Security.Cryptography;

    /// <summary>
    /// This class defines the main entry point for the application.
    /// </summary>
    internal class Program
    {

        [StructLayout(LayoutKind.Sequential)]
        internal struct LSA_UNICODE_STRING
        {
            public ushort Length;
            public ushort MaximumLength;
            public IntPtr Buffer;   
        }

        /*
        [StructLayout(LayoutKind.Sequential)]
        private struct LSA_UNICODE_STRING
        {
            public UInt16 Length;
            public UInt16 MaximumLength;
            public IntPtr Buffer;
        }
        */

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
        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern uint LsaOpenPolicy(
            [In] [Optional] ref LSA_UNICODE_STRING SystemName,
            [In] ref LSA_OBJECT_ATTRIBUTES ObjectAttributes,
            [In] int DesiredAccess,
            [Out] out IntPtr PolicyHandle        
            );

        /*
        [DllImport("advapi32.dll", PreserveSig = true)]
        private static extern UInt32 LsaOpenPolicy(
            ref LSA_UNICODE_STRING SystemName,
            ref LSA_OBJECT_ATTRIBUTES ObjectAttributes,
            Int32 DesiredAccess,
            out IntPtr PolicyHandle);
        */

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        private static extern int LsaAddAccountRights(
            IntPtr PolicyHandle,
            // IntPtr AccountSid,
            [In] [MarshalAs(UnmanagedType.LPArray)] byte[] AccountSid,
            LSA_UNICODE_STRING[] UserRights,
            int CountOfRights);

        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        private static extern int LsaRemoveAccountRights(
            IntPtr PolicyHandle,
            // IntPtr AccountSid,
            [In] [MarshalAs(UnmanagedType.LPArray)] byte[] AccountSid,
            [In] int AllRights,
            LSA_UNICODE_STRING[] UserRights,
            int CountOfRights);

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


        [DllImport("advapi32.dll", SetLastError = true, PreserveSig = true)]
        internal static extern int LsaEnumerateAccountRights(
            [In] IntPtr PolicyHandle,
            // [In] IntPtr AccountSid,
            [In] [MarshalAs(UnmanagedType.LPArray)] byte[] AccountSid,
            [Out] out IntPtr UserRightsPtr,
            [Out] out int CountOfRights);


        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern uint LsaClose(
          [In] IntPtr ObjectHandle
        );

        [DllImport("advapi32.dll", SetLastError = true)]
        internal static extern long LsaNtStatusToWinError(long status);

        [DllImport("kernel32.dll")]
        private static extern int GetLastError();

        [DllImport("advapi32.dll")]
        public static extern void FreeSid(IntPtr pSid);


        private static System.Collections.Specialized.StringCollection GetRights(System.Security.Principal.WindowsIdentity identity)
        {
            System.Collections.Specialized.StringCollection rights = new System.Collections.Specialized.StringCollection();

            LSA_OBJECT_ATTRIBUTES objectAttributes = CreateLSAObjectAttributes();
            IntPtr policyHandle = IntPtr.Zero;

            LSA_UNICODE_STRING SystemName = new LSA_UNICODE_STRING();

            /*
            SystemName.Length = (ushort)(System.Environment.MachineName.Length * System.Text.UnicodeEncoding.CharSize);
            SystemName.MaximumLength = (ushort)((System.Environment.MachineName.Length + 1) * System.Text.UnicodeEncoding.CharSize);
            SystemName.Buffer = System.Environment.MachineName; // Marshal.StringToHGlobalUni(System.Environment.MachineName);
            */


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


        private static void SetRight(System.Security.Principal.WindowsIdentity identity, string privilegeName)
        {
            LSA_OBJECT_ATTRIBUTES objectAttributes = CreateLSAObjectAttributes();
            IntPtr policyHandle = IntPtr.Zero;

            LSA_UNICODE_STRING SystemName = new LSA_UNICODE_STRING();

            /*
            SystemName.Length = (ushort)(System.Environment.MachineName.Length * System.Text.UnicodeEncoding.CharSize);
            SystemName.MaximumLength = (ushort)((System.Environment.MachineName.Length + 1) * System.Text.UnicodeEncoding.CharSize);
            SystemName.Buffer = System.Environment.MachineName; // Marshal.StringToHGlobalUni(System.Environment.MachineName);
            */


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

                /*
                LookupAccountName(string.Empty, currentIdentity.Name, sid, ref sidSize, domainName, ref nameSize, ref accountType);
                */

                /*
                domainName = new System.Text.StringBuilder(nameSize);
                sid = Marshal.AllocHGlobal(sidSize);
                */

                /*
                if (!LookupAccountName(string.Empty, currentIdentity.Name, sid, ref sidSize, domainName, ref nameSize, ref accountType))
                {
                    winErrorCode = GetLastError();
                    Console.WriteLine("LookupAccountName failed: " + winErrorCode);
                }
                else
                {
                */
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

        private static void RemoveRight(System.Security.Principal.WindowsIdentity identity, string privilegeName)
        {
            LSA_OBJECT_ATTRIBUTES objectAttributes = CreateLSAObjectAttributes();
            IntPtr policyHandle = IntPtr.Zero;

            LSA_UNICODE_STRING SystemName = new LSA_UNICODE_STRING();

            /*
            SystemName.Length = (ushort)(System.Environment.MachineName.Length * System.Text.UnicodeEncoding.CharSize);
            SystemName.MaximumLength = (ushort)((System.Environment.MachineName.Length + 1) * System.Text.UnicodeEncoding.CharSize);
            SystemName.Buffer = System.Environment.MachineName; // Marshal.StringToHGlobalUni(System.Environment.MachineName);
            */


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

                /*
                LookupAccountName(string.Empty, currentIdentity.Name, sid, ref sidSize, domainName, ref nameSize, ref accountType);
                */

                /*
                domainName = new System.Text.StringBuilder(nameSize);
                sid = Marshal.AllocHGlobal(sidSize);
                */

                /*
                if (!LookupAccountName(string.Empty, currentIdentity.Name, sid, ref sidSize, domainName, ref nameSize, ref accountType))
                {
                    winErrorCode = GetLastError();
                    Console.WriteLine("LookupAccountName failed: " + winErrorCode);
                }
                else
                {
                */
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
                throw new ApplicationException(string.Format("Error occured in LSA, error code {0}, detail: {1}", winErrorCode, errorMessage));
            }
            return rights;
        }
        */
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

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        internal static void Main()
        {
            /*

            System.Security.Principal.WindowsIdentity currentIdentity = System.Security.Principal.WindowsIdentity.GetCurrent(TokenAccessLevels.Read);
            //Console.WriteLine(currentIdentity.User.Value);
            System.Security.Principal.SecurityIdentifier userSid = new SecurityIdentifier("S-1-5-21-149779583-363096731-646672791-379352");
            Console.WriteLine((userSid == currentIdentity.User));

            System.Collections.Specialized.StringCollection existingRights = GetRights(currentIdentity);
            Console.WriteLine("Rights for {0}", currentIdentity.Name);
            Console.WriteLine(new string('=', 30));
            foreach (string rightName in existingRights)
            {
                Console.WriteLine(rightName);
            }

            SetRight(currentIdentity, "SeDebugPrivilege");
            SetRight(currentIdentity, "SeBackupPrivilege");

            System.Collections.Specialized.StringCollection newRights = GetRights(currentIdentity);
            Console.WriteLine();
            Console.WriteLine("Rights for {0}", currentIdentity.Name);
            Console.WriteLine(new string('=', 30));
            foreach (string rightName in newRights)
            {
                Console.WriteLine(rightName);
            }

            RemoveRight(currentIdentity, "SeBackupPrivilege");

            newRights = GetRights(currentIdentity);
            Console.WriteLine();
            Console.WriteLine("Rights for {0}", currentIdentity.Name);
            Console.WriteLine(new string('=', 30));
            foreach (string rightName in newRights)
            {
                Console.WriteLine(rightName);
            }
            */


            string SyslogServerHostname = "patrick-syslog";
            //int SyslogServerPort = 514;
            int SyslogServerPort = 1468;

            Console.WriteLine(DateTime.Now);
            /*
            System.Net.Sockets.TcpClient tcpClient = new System.Net.Sockets.TcpClient();
            tcpClient.SendTimeout = 5;
            try
            {
                tcpClient.Connect(SyslogServerHostname, SyslogServerPort);
            }
            catch (Exception) { };
            */


            Console.WriteLine(DateTime.Now);

            /*
            System.Net.Sockets.UdpClient udpClient = new System.Net.Sockets.UdpClient();
            udpClient.Connect(SyslogServerHostname, SyslogServerPort);

            byte[] testBytes = System.Text.UnicodeEncoding.Unicode.GetBytes("TEST");

            int returnValue = udpClient.Send(testBytes, testBytes.Length);
            */

            using (System.Net.Sockets.TcpClient tcp = new System.Net.Sockets.TcpClient())
            {
                IAsyncResult asyncResult = tcp.BeginConnect(SyslogServerHostname, SyslogServerPort, null, null);
                System.Threading.WaitHandle waitHandle = asyncResult.AsyncWaitHandle;
                try
                {
                    if (!asyncResult.AsyncWaitHandle.WaitOne(TimeSpan.FromSeconds(2), false))
                    {
                        tcp.Close();
                        Console.WriteLine("Failed to connect.");
                        //throw new TimeoutException();
                    }

                    if (tcp.Client != null)
                    {
                        if (tcp.Client.Connected)
                        {
                            tcp.EndConnect(asyncResult);
                        }
                        else
                        {
                            Console.WriteLine("TCP client is not connected.");
                        }
                    }
                }
                finally
                {
                    waitHandle.Close();
                }
            }

            int q = 0;

            /*
            for (int x = 0; x < 2; x++)
            {
                syslog.WriteInformationEvent(string.Format("This is informational event {0:N0}.", x), System.Diagnostics.Process.GetCurrentProcess().Id, string.Empty);
            }
            */

            /*
            Console.WriteLine(new string('=', 30));
            Console.WriteLine(new string('=', 30));
            Console.WriteLine(LocalAdministratorGroup.LocalAdminGroupName);
            Console.WriteLine(new string('=', 30));
            Console.WriteLine(new string('=', 30));

            Console.WriteLine(System.Globalization.CultureInfo.CurrentCulture.Name);
            Console.WriteLine(System.Globalization.CultureInfo.CurrentUICulture.Name);

            Console.WriteLine(new string('=', 30));

            Console.WriteLine(System.Threading.Thread.CurrentThread.CurrentCulture.Name);
            Console.WriteLine(System.Threading.Thread.CurrentThread.CurrentUICulture.Name);

            System.Threading.Thread.CurrentThread.CurrentUICulture = System.Threading.Thread.CurrentThread.CurrentCulture;

            Console.WriteLine(new string('=', 30));

            Console.WriteLine(System.Threading.Thread.CurrentThread.CurrentCulture.Name);
            Console.WriteLine(System.Threading.Thread.CurrentThread.CurrentUICulture.Name);
            */

            /*
            }

            FreeSid(sid);
            */


            /*
            //Now that we have the SID an the policy,
            //we can add rights to the account.

            //initialize an unicode-string for the privilege name
            LSA_UNICODE_STRING[] userRights = new LSA_UNICODE_STRING[1];
            userRights[0] = new LSA_UNICODE_STRING();
            userRights[0].Buffer = Marshal.StringToHGlobalUni(privilegeName);
            userRights[0].Length = (UInt16)(privilegeName.Length * UnicodeEncoding.CharSize);
            userRights[0].MaximumLength = (UInt16)((privilegeName.Length + 1) * UnicodeEncoding.CharSize);

            //add the right to the account
            long res = LsaAddAccountRights(policyHandle, sid, userRights, 1);
            winErrorCode = LsaNtStatusToWinError(res);
            if (winErrorCode != 0)
            {
                Console.WriteLine("LsaAddAccountRights failed: " + winErrorCode);
            }
            */



            /*

            int WinWorldSid = 1;
            int POLICY_ALL_ACCESS = 0xF0FFF;
            int SECURITY_MAX_SID_SIZE = 68;
            string SE_DENY_REMOTE_INTERACTIVE_LOGON_NAME = "SeDenyRemoteInteractiveLogonRight";
            uint NT_STATUS_OBJECT_NAME_NOT_FOUND = 0xC0000034;
            uint STATUS_NO_MORE_ENTRIES = 0x8000001A;

    // add the Deny permission
    Public Sub DenyTS(ByVal PC As String)
    Dim ret, Access, sidsize As Integer
    Dim SystemName, DenyTSRights As LSA_UNICODE_STRING
    Dim objectAttributes As LSA_OBJECT_ATTRIBUTES
    Dim policyHandle, EveryoneSID As IntPtr

    // build a well-known SID for "Everyone"
    sidsize = SECURITY_MAX_SID_SIZE
    EveryoneSID = Marshal.AllocHGlobal(sidsize)
    If CreateWellKnownSid(WinWorldSid, IntPtr.Zero, EveryoneSID, sidsize) = False Then
        ret = Marshal.GetLastWin32Error()
        Throw New Win32Exception(ret)
    End If

    // setup the parameters for the LsaOpenPolicy API
    objectAttributes.Length = Marshal.SizeOf(objectAttributes)
    SystemName.Length = PC.Length * UnicodeEncoding.CharSize
    SystemName.MaximumLength = (PC.Length + 1) * UnicodeEncoding.CharSize
    SystemName.Buffer = Marshal.StringToHGlobalUni(PC)
    Access = POLICY_ALL_ACCESS

    // open a policy handle on the remote PC
    ret = LsaOpenPolicy(SystemName, objectAttributes, Access, policyHandle)
    If ret<> 0 Then
       Throw New Win32Exception(LsaNtStatusToWinError(ret))
    End If

    // clean up
    Marshal.FreeHGlobal(SystemName.Buffer)

    // Setup the input parameters for the LsaRemoveAccountRights API
    DenyTSRights.Length = SE_DENY_REMOTE_INTERACTIVE_LOGON_NAME.Length * UnicodeEncoding.CharSize
    DenyTSRights.MaximumLength = (SE_DENY_REMOTE_INTERACTIVE_LOGON_NAME.Length + 1) * UnicodeEncoding.CharSize
    DenyTSRights.Buffer = Marshal.StringToHGlobalUni(SE_DENY_REMOTE_INTERACTIVE_LOGON_NAME)

    // Do it!
    ret = LsaAddAccountRights(policyHandle, EveryoneSID, DenyTSRights, 1)
    If ret<> 0 Then
        Marshal.FreeHGlobal(DenyTSRights.Buffer)
        LsaClose(policyHandle)
        Throw New Win32Exception(LsaNtStatusToWinError(ret))
    End If

    // clean up
    Marshal.FreeHGlobal(DenyTSRights.Buffer)
    LsaClose(policyHandle)
    End Sub

                */
            /*
            for (int i = 0; i < 10; i++)
            {
            */
            /*
            Console.WriteLine(DateTime.Now.ToLongTimeString());
            Console.WriteLine("Desktops Detected");
            Console.WriteLine("=================");

            System.Text.StringBuilder desktopNameString = new System.Text.StringBuilder("Desktops: ");
            string[] desktopNames = Desktop.GetDesktops();
            for (int j = 0; j < desktopNames.Length; j++)
            {
                desktopNameString.Append(desktopNames[j]);
                if (j < (desktopNames.Length - 1))
                {
                    desktopNameString.Append(", ");
                }
            }
            Console.WriteLine(desktopNameString.ToString());
            */

            /*
            foreach (string desktop in Desktop.GetDesktops())
            {
                Console.WriteLine(desktop);
            }
            */

            /*
            Console.WriteLine();
            */

            /*
            Console.WriteLine("Window Stations Detected");
            Console.WriteLine("========================");
            System.Text.StringBuilder winStaNameString = new System.Text.StringBuilder("Window Stations: ");
            string[] winStaNames = Desktop.GetWindowStations();
            for (int j = 0; j < winStaNames.Length; j++)
            {
                winStaNameString.Append(winStaNames[j]);
                if (j < (winStaNames.Length - 1))
                {
                    winStaNameString.Append(", ");
                }
            }
            Console.WriteLine(winStaNameString.ToString());
            */

            /*
            foreach (string winsta in Desktop.GetWindowStations())
            {
                Console.WriteLine(winsta);
            }
            */

            /*
            long DESKTOP_ENUMERATE = 0x0040L;
            long DESKTOP_READOBJECTS = 0x0001L;
            long DESKTOP_WRITEOBJECTS = 0x0080L;
            long READ_CONTROL = 0x00020000L;

            long MAXIMUM_ALLOWED = 0x02000000L;
            */

            /*
            //Console.WriteLine(LsaLogonSessions.LogonSessions.WTSGetActiveConsoleSessionId());
            //Console.WriteLine(LsaLogonSessions.LogonSessions.GetDesktopWindow());
            IntPtr inputDesktopPtr = LsaLogonSessions.LogonSessions.OpenInputDesktop(0, false, 256);

            if (inputDesktopPtr == IntPtr.Zero)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("input desktop pointer was zero!");
                Console.ResetColor();

                inputDesktopPtr = LsaLogonSessions.LogonSessions.OpenDesktop("Winlogon", 0, false, (uint)MAXIMUM_ALLOWED);
                if (inputDesktopPtr == IntPtr.Zero)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("input desktop pointer is still zero!");
                    Console.ResetColor();
                }
                LsaLogonSessions.LogonSessions.SetThreadDesktop(inputDesktopPtr);
                int lastError = Marshal.GetLastWin32Error();
                Console.WriteLine("SetThreadDesktop Error: {0:N0}", lastError);
                LsaLogonSessions.LogonSessions.CloseDesktop(inputDesktopPtr);
                lastError = Marshal.GetLastWin32Error();
                Console.WriteLine("CloseDesktop Error: {0:N0}", lastError);
                */

            /*
            inputDesktopPtr = LsaLogonSessions.LogonSessions.OpenDesktop("Default", 0, false, 256);
            if (inputDesktopPtr == IntPtr.Zero)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("input desktop pointer is still zero!");
                Console.ResetColor();
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("i was able to open the default desktop!");
                Console.ResetColor();
                bool switchResult = LsaLogonSessions.LogonSessions.SwitchDesktop(inputDesktopPtr);
                Console.WriteLine("able to switch desktops: {0}", switchResult);
            }
            */
            /*
            }

            //int lastError = Marshal.GetLastWin32Error();
            Console.WriteLine(inputDesktopPtr);
            //Console.WriteLine("Last Error: {0:N0}", lastError);
            LsaLogonSessions.LogonSessions.CloseDesktop(inputDesktopPtr);
            */

            /*
                System.Threading.Thread.Sleep(2000);
                Console.WriteLine();
            }
            */


            /*currentIdentity.Dispose();*/

#if DEBUG
            Console.Write("\n\nPress <ENTER> to close this program.");
            Console.ReadLine();
#endif
        }
    }
}
