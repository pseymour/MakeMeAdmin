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
    using System.Runtime.InteropServices;
    using System.Security.Principal;

    /// <summary>
    /// This class defines various methods that wrap native Windows functions.
    /// </summary>
    public class NativeMethods
    {
        /*
        public static bool IsProcessElevated(IntPtr processHandle)
        {
            System.Security.Principal.SecurityIdentifier adminsSid = new System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.BuiltinAdministratorsSid, null);

            bool returnValue = false;
            int returnLength = 0;

            IntPtr tokenHandle = IntPtr.Zero;

            bool nativeReturnValue = OpenProcessToken(processHandle, (uint)(System.Security.Principal.TokenAccessLevels.QuerySource | TokenAccessLevels.Read), out tokenHandle);

            if (tokenHandle != IntPtr.Zero)
            {
                nativeReturnValue = GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenGroups, IntPtr.Zero, returnLength, out returnLength);
                if (returnLength > 0)
                {
                    IntPtr tokenInfo = Marshal.AllocHGlobal(returnLength);
                    nativeReturnValue = GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenGroups, tokenInfo, returnLength, out returnLength);

                    if (nativeReturnValue)
                    {
                        TOKEN_GROUPS tokenGroups = (TOKEN_GROUPS)Marshal.PtrToStructure(tokenInfo, typeof(TOKEN_GROUPS));

                        // long ptr = tokenInfo.ToInt64() + 4;
                        IntPtr ptr = new IntPtr(tokenInfo.ToInt64() + 4);
                        for (int i = 0; i < tokenGroups.GroupCount; i++)
                        {
                            // Marshal the SID_AND_ATTRIBUTES struct from native to .NET
                            SID_AND_ATTRIBUTES tokenGroup = (SID_AND_ATTRIBUTES)Marshal.PtrToStructure(ptr, typeof(SID_AND_ATTRIBUTES));

                            System.Security.Principal.SecurityIdentifier sid = new System.Security.Principal.SecurityIdentifier(tokenGroup.Sid);
                            
                            if (sid.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid))
                            {
                                if ((tokenGroup.Attributes & SidAttributes.SE_GROUP_ENABLED) == SidAttributes.SE_GROUP_ENABLED)
                                {
                                    returnValue = ((tokenGroup.Attributes & SidAttributes.SE_GROUP_USE_FOR_DENY_ONLY) != SidAttributes.SE_GROUP_USE_FOR_DENY_ONLY);
                                }
                                //if ((tokenGroup.Attributes & SidAttributes.SE_GROUP_MANDATORY) == SidAttributes.SE_GROUP_MANDATORY)
                                //{
                                //    returnValue = true;
                                //    Console.ForegroundColor = ConsoleColor.Red;
                                //    Console.Write(" mandatory!", tokenGroup.Attributes);
                                //}
                            }
                            ptr = new IntPtr(ptr.ToInt64() + Marshal.SizeOf(typeof(SID_AND_ATTRIBUTES)));
                        }
                    }
                    Marshal.FreeHGlobal(tokenInfo);
                }
            }

            CloseHandle(tokenHandle);
            return returnValue;
        }
        */

        public static bool IsProcessElevated2(IntPtr processHandle)
        {
            System.Security.Principal.SecurityIdentifier adminsSid = new System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.BuiltinAdministratorsSid, null);

            bool returnValue = false;
            int returnLength = 0;

            IntPtr tokenHandle = IntPtr.Zero;

            bool nativeReturnValue = OpenProcessToken(processHandle, (uint)(TokenAccessLevels.Read), out tokenHandle);

            /*

            bool success = GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenElevationType, elevationTypePtr, (uint)elevationResultSize, out returnLength);
            if (success)
            {

            }
            else
            {
                throw new ApplicationException("Unable to determine the current elevation.");
            }
            */

            if (tokenHandle != IntPtr.Zero)
            {
                TOKEN_ELEVATION_TYPE elevationResult = TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault;
                returnLength = Marshal.SizeOf((int)elevationResult);
                IntPtr elevationTypePtr = Marshal.AllocHGlobal(returnLength);

                nativeReturnValue = GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenElevationType, elevationTypePtr, returnLength, out returnLength);
                if (nativeReturnValue)
                {
                    elevationResult = (TOKEN_ELEVATION_TYPE)Marshal.ReadInt32(elevationTypePtr);
                    returnValue = elevationResult == TOKEN_ELEVATION_TYPE.TokenElevationTypeFull;
                }
            }

            CloseHandle(tokenHandle);
            return returnValue;
        }


        public static WindowsIdentity GetProcessOwner(IntPtr processHandle)
        {
            IntPtr tokenHandle = IntPtr.Zero;

            try
            {
                bool nativeReturnValue = OpenProcessToken(processHandle, (uint)(TokenAccessLevels.Query), out tokenHandle);
                return new WindowsIdentity(tokenHandle);
            }
            catch
            {
                return null;
            }
            finally
            {
                if (tokenHandle != IntPtr.Zero)
                {
                    CloseHandle(tokenHandle);
                }
            }
        }

        /*
        public static void DisplayProcessTokens(IntPtr processHandle)
        {
            System.Security.Principal.SecurityIdentifier adminsSid = new System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.BuiltinAdministratorsSid, null);

            int returnLength = 0;

            IntPtr tokenHandle = IntPtr.Zero;

            bool nativeReturnValue = OpenProcessToken(processHandle, (uint)(System.Security.Principal.TokenAccessLevels.QuerySource | TokenAccessLevels.Read), out tokenHandle);

            if (tokenHandle != IntPtr.Zero)
            {
                nativeReturnValue = GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenGroups, IntPtr.Zero, returnLength, out returnLength);
                if (returnLength > 0)
                {
                    IntPtr tokenInfo = Marshal.AllocHGlobal(returnLength);
                    nativeReturnValue = GetTokenInformation(tokenHandle, TOKEN_INFORMATION_CLASS.TokenGroups, tokenInfo, returnLength, out returnLength);

                    if (nativeReturnValue)
                    {
                        TOKEN_GROUPS tokenGroups = (TOKEN_GROUPS)Marshal.PtrToStructure(tokenInfo, typeof(TOKEN_GROUPS));

                        // long ptr = tokenInfo.ToInt64() + 4;
                        IntPtr ptr = new IntPtr(tokenInfo.ToInt64() + 4);
                        for (int i = 0; i < tokenGroups.GroupCount; i++)
                        {
                            // Marshal the SID_AND_ATTRIBUTES struct from native to .NET
                            SID_AND_ATTRIBUTES tokenGroup = (SID_AND_ATTRIBUTES)Marshal.PtrToStructure(ptr, typeof(SID_AND_ATTRIBUTES));

                            System.Security.Principal.SecurityIdentifier sid = new System.Security.Principal.SecurityIdentifier(tokenGroup.Sid);
                            Console.Write(sid);

                            System.Security.Principal.NTAccount ntAcc = null;
                            try
                            {
                                ntAcc = (System.Security.Principal.NTAccount)sid.Translate(typeof(System.Security.Principal.NTAccount));
                            }
                            catch (IdentityNotMappedException) { }
                            if (ntAcc != null)
                            {
                                Console.Write(" {0}", ntAcc.Value);
                            }

                            if (sid.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid))
                            {
                                ConsoleColor foreColor = Console.ForegroundColor;
                                Console.ForegroundColor = ConsoleColor.Green;
                                Console.Write(" W00T!");
                                if ((tokenGroup.Attributes & SidAttributes.SE_GROUP_USE_FOR_DENY_ONLY) == SidAttributes.SE_GROUP_USE_FOR_DENY_ONLY)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write(" deny only!", tokenGroup.Attributes);
                                }
                                if ((tokenGroup.Attributes & SidAttributes.SE_GROUP_MANDATORY) == SidAttributes.SE_GROUP_MANDATORY)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.Write(" mandatory!", tokenGroup.Attributes);
                                }
                                Console.ForegroundColor = foreColor;

                                // Match the builtin admin group SID, so the user belongs
                                // to the local administrators group.
                                //fIsAdmin = true;
                                //break;
                            }
                            Console.WriteLine();
                            ptr = new IntPtr(ptr.ToInt64() + Marshal.SizeOf(typeof(SID_AND_ATTRIBUTES)));
                        }
                    }

                    Marshal.FreeHGlobal(tokenInfo);
                }
            }

            CloseHandle(tokenHandle);
        }
        */

        /*
        public static void DisplayTokens()
        {
            System.Security.Principal.WindowsIdentity currentIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.SecurityIdentifier adminsSid = new System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.BuiltinAdministratorsSid, null);

            int returnLength = 0;
            bool nativeReturnValue = GetTokenInformation(currentIdentity.Token, TOKEN_INFORMATION_CLASS.TokenGroups, IntPtr.Zero, returnLength, out returnLength);

            if (returnLength > 0)
            {
                IntPtr tokenInfo = Marshal.AllocHGlobal(returnLength);
                nativeReturnValue = GetTokenInformation(currentIdentity.Token, TOKEN_INFORMATION_CLASS.TokenGroups, tokenInfo, returnLength, out returnLength);

                if (nativeReturnValue)
                {
                    TOKEN_GROUPS tokenGroups = (TOKEN_GROUPS)Marshal.PtrToStructure(tokenInfo, typeof(TOKEN_GROUPS));

                    // long ptr = tokenInfo.ToInt64() + 4;
                    IntPtr ptr = new IntPtr(tokenInfo.ToInt64() + 4);
                    for (int i = 0; i < tokenGroups.GroupCount; i++)
                    {
                        // Marshal the SID_AND_ATTRIBUTES struct from native to .NET
                        SID_AND_ATTRIBUTES tokenGroup = (SID_AND_ATTRIBUTES)Marshal.PtrToStructure(ptr, typeof(SID_AND_ATTRIBUTES));

                        System.Security.Principal.SecurityIdentifier sid = new System.Security.Principal.SecurityIdentifier(tokenGroup.Sid);
                        Console.Write(sid);
                        
                        System.Security.Principal.NTAccount ntAcc = null;
                        try
                        {
                            ntAcc = (System.Security.Principal.NTAccount)sid.Translate(typeof(System.Security.Principal.NTAccount));
                        }
                        catch (IdentityNotMappedException) { }
                        if (ntAcc != null)
                        {
                            Console.Write(" {0}", ntAcc.Value);
                        }

                        if (sid.IsWellKnown(WellKnownSidType.BuiltinAdministratorsSid))
                        {
                            ConsoleColor foreColor = Console.ForegroundColor;
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.Write(" W00T!");
                            if ((tokenGroup.Attributes & SidAttributes.SE_GROUP_USE_FOR_DENY_ONLY) == SidAttributes.SE_GROUP_USE_FOR_DENY_ONLY)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(" deny only!", tokenGroup.Attributes);
                            }
                            if ((tokenGroup.Attributes & SidAttributes.SE_GROUP_MANDATORY) == SidAttributes.SE_GROUP_MANDATORY)
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write(" mandatory!", tokenGroup.Attributes);
                            }
                            Console.ForegroundColor = foreColor;

                            // Match the builtin admin group SID, so the user belongs
                            // to the local administrators group.
                            //fIsAdmin = true;
                            //break;
                        }
                        Console.WriteLine();
                        ptr = new IntPtr(ptr.ToInt64() + Marshal.SizeOf(typeof(SID_AND_ATTRIBUTES)));
                    }
                }

                Marshal.FreeHGlobal(tokenInfo);
            }

            currentIdentity.Dispose();
        }
        */

        /*
        public static void DisplayPrivileges()
        {
            System.Security.Principal.WindowsIdentity currentIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();
            System.Security.Principal.SecurityIdentifier adminsSid = new System.Security.Principal.SecurityIdentifier(System.Security.Principal.WellKnownSidType.BuiltinAdministratorsSid, null);

            int returnLength = 0;
            bool nativeReturnValue = GetTokenInformation(currentIdentity.Token, TOKEN_INFORMATION_CLASS.TokenPrivileges, IntPtr.Zero, returnLength, out returnLength);

            if (returnLength > 0)
            {
                IntPtr tokenInfo = Marshal.AllocHGlobal(returnLength);
                nativeReturnValue = GetTokenInformation(currentIdentity.Token, TOKEN_INFORMATION_CLASS.TokenPrivileges, tokenInfo, returnLength, out returnLength);

                if (nativeReturnValue)
                {
                    TOKEN_GROUPS tokenGroups = (TOKEN_GROUPS)Marshal.PtrToStructure(tokenInfo, typeof(TOKEN_GROUPS));

                    // long ptr = tokenInfo.ToInt64() + 4;
                    IntPtr ptr = new IntPtr(tokenInfo.ToInt64() + 4);
                    for (int i = 0; i < tokenGroups.GroupCount; i++)
                    {
                        // Marshal the LUID_AND_ATTRIBUTES struct from native to .NET
                        LUID_AND_ATTRIBUTES tokenGroup = (LUID_AND_ATTRIBUTES)Marshal.PtrToStructure(ptr, typeof(LUID_AND_ATTRIBUTES));

                        Console.WriteLine("LUID: {0}\t\tAttributes: {1}", tokenGroup.Luid, tokenGroup.Attributes);

                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        int luidNameLen = 0;
                        IntPtr ptrLuid = Marshal.AllocHGlobal(Marshal.SizeOf(tokenGroup.Luid));
                        Marshal.StructureToPtr(tokenGroup.Luid, ptrLuid, true);
                        LookupPrivilegeName(null, ptrLuid, null, ref luidNameLen); // call once to get the name len
                        sb.EnsureCapacity(luidNameLen + 1);
                        if (LookupPrivilegeName(null, ptrLuid, sb, ref luidNameLen)) // call again to get the name
                        {
                            Console.WriteLine(string.Format("{0} ({1})", tokenGroup.Luid.LowPart, sb.ToString()));
                        }
                        Marshal.FreeHGlobal(ptrLuid);
                        
                        Console.WriteLine();
                        ptr = new IntPtr(ptr.ToInt64() + Marshal.SizeOf(typeof(LUID_AND_ATTRIBUTES)));

                    }
                }

                Marshal.FreeHGlobal(tokenInfo);
            }

            currentIdentity.Dispose();
        }
        */

        /*
        public static void DisplayPrivileges2()
        {
            //combine all policies
            uint aAccess = (uint)(
                LSA_AccessPolicy.POLICY_AUDIT_LOG_ADMIN |
                LSA_AccessPolicy.POLICY_CREATE_ACCOUNT |
                LSA_AccessPolicy.POLICY_CREATE_PRIVILEGE |
                LSA_AccessPolicy.POLICY_CREATE_SECRET |
                LSA_AccessPolicy.POLICY_GET_PRIVATE_INFORMATION |
                LSA_AccessPolicy.POLICY_LOOKUP_NAMES |
                LSA_AccessPolicy.POLICY_NOTIFICATION |
                LSA_AccessPolicy.POLICY_SERVER_ADMIN |
                LSA_AccessPolicy.POLICY_SET_AUDIT_REQUIREMENTS |
                LSA_AccessPolicy.POLICY_SET_DEFAULT_QUOTA_LIMITS |
                LSA_AccessPolicy.POLICY_TRUST_ADMIN |
                LSA_AccessPolicy.POLICY_VIEW_AUDIT_INFORMATION |
                LSA_AccessPolicy.POLICY_VIEW_LOCAL_INFORMATION
                );

            //initialize a pointer for the policy handle
            IntPtr aPolicyHandle = IntPtr.Zero;

            //these attributes are not used, but LsaOpenPolicy wants them to exists
            LSA_OBJECT_ATTRIBUTES aObjectAttributes = new LSA_OBJECT_ATTRIBUTES();
            aObjectAttributes.Length = 0;
            aObjectAttributes.RootDirectory = IntPtr.Zero;
            aObjectAttributes.Attributes = 0;
            aObjectAttributes.SecurityDescriptor = IntPtr.Zero;
            aObjectAttributes.SecurityQualityOfService = IntPtr.Zero;

            LSA_UNICODE_STRING systemName = new LSA_UNICODE_STRING();

            uint returnCode = LsaOpenPolicy(ref systemName, ref aObjectAttributes, aAccess, out aPolicyHandle);
            Console.WriteLine("return code from LsaOpenPolicy(): {0:N0}", returnCode);

            System.Security.Principal.WindowsIdentity currentIdentity = System.Security.Principal.WindowsIdentity.GetCurrent();
            byte[] sidBytes = new byte[currentIdentity.User.BinaryLength];
            currentIdentity.User.GetBinaryForm(sidBytes, 0);
            IntPtr userRights = IntPtr.Zero;
            uint rightsCount = 0;
            returnCode = LsaEnumerateAccountRights(aPolicyHandle, sidBytes, out userRights, out rightsCount);
            Console.WriteLine("return code from LsaEnumerateAccountRights(): {0:N0}", returnCode);

            Console.WriteLine("rights count: {0:N0}", rightsCount);
            Console.WriteLine("rights pointer: {0}", userRights);

            LsaClose(aPolicyHandle);
        }
        */

        /// <summary>
        /// Retrieves a specified type of information about an access token.
        /// </summary>
        /// <param name="TokenHandle">
        /// A handle to an access token from which information is retrieved. If TokenInformationClass
        /// specifies TokenSource, the handle must have TOKEN_QUERY_SOURCE access. For all other
        /// TokenInformationClass values, the handle must have TOKEN_QUERY access.
        /// </param>
        /// <param name="TokenInformationClass">
        /// Specifies a value from the TOKEN_INFORMATION_CLASS enumerated type to identify the type
        /// of information the function retrieves.
        /// </param>
        /// <param name="TokenInformation">
        /// A pointer to a buffer the function fills with the requested information. The structure
        /// put into this buffer depends upon the type of information specified by the
        /// TokenInformationClass parameter.
        /// </param>
        /// <param name="TokenInformationLength">
        /// Specifies the size, in bytes, of the buffer pointed to by the TokenInformation parameter.
        /// If TokenInformation is NULL, this parameter must be zero.
        /// </param>
        /// <param name="ReturnLength">
        /// A pointer to a variable that receives the number of bytes needed for the buffer pointed
        /// to by the TokenInformation parameter. If this value is larger than the value specified
        /// in the TokenInformationLength parameter, the function fails and stores no data in the buffer.
        /// If the value of the TokenInformationClass parameter is TokenDefaultDacl and the token has
        /// no default DACL, the function sets the variable pointed to by ReturnLength to
        /// sizeof(TOKEN_DEFAULT_DACL) and sets the DefaultDacl member of the TOKEN_DEFAULT_DACL
        /// structure to NULL.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero.
        /// If the function fails, the return value is zero. To get extended error information, call GetLastError.
        /// </returns>
        /// <remarks>
        /// The calling process must have appropriate access rights to obtain the information.
        /// </remarks>
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool GetTokenInformation(
            [In] IntPtr TokenHandle,
            [In] TOKEN_INFORMATION_CLASS TokenInformationClass,
            [Out] [Optional] IntPtr TokenInformation,
            [In] int TokenInformationLength,
            [Out] out int ReturnLength);


        [DllImport("advapi32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool OpenProcessToken([In] IntPtr ProcessHandle, [In] UInt32 DesiredAccess, [Out] out IntPtr TokenHandle);

        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool CloseHandle(IntPtr hObject);

        /*
        [DllImport("advapi32.dll", SetLastError = true)]
        public static extern uint LsaEnumerateAccountRights(
            IntPtr PolicyHandle,
            [MarshalAs(UnmanagedType.LPArray)] byte[] AccountSid,
            out IntPtr UserRights,
            out uint CountOfRights
            );
        */

        /*
        [DllImport("advapi32.dll", PreserveSig = true)]
        private static extern UInt32 LsaOpenPolicy(
            ref LSA_UNICODE_STRING SystemName,
            ref LSA_OBJECT_ATTRIBUTES ObjectAttributes,
            uint DesiredAccess,
            out IntPtr PolicyHandle
            );
        */

        /*
        [DllImport("advapi32.dll")]
        private static extern long LsaClose(IntPtr ObjectHandle);
        */

        /*
        public enum LSA_AccessPolicy : long
        {
            POLICY_VIEW_LOCAL_INFORMATION = 0x00000001L,
            POLICY_VIEW_AUDIT_INFORMATION = 0x00000002L,
            POLICY_GET_PRIVATE_INFORMATION = 0x00000004L,
            POLICY_TRUST_ADMIN = 0x00000008L,
            POLICY_CREATE_ACCOUNT = 0x00000010L,
            POLICY_CREATE_SECRET = 0x00000020L,
            POLICY_CREATE_PRIVILEGE = 0x00000040L,
            POLICY_SET_DEFAULT_QUOTA_LIMITS = 0x00000080L,
            POLICY_SET_AUDIT_REQUIREMENTS = 0x00000100L,
            POLICY_AUDIT_LOG_ADMIN = 0x00000200L,
            POLICY_SERVER_ADMIN = 0x00000400L,
            POLICY_LOOKUP_NAMES = 0x00000800L,
            POLICY_NOTIFICATION = 0x00001000L
        }
        */

        /*
        [StructLayout(LayoutKind.Sequential)]
        private struct LSA_OBJECT_ATTRIBUTES
        {
            public int Length;
            public IntPtr RootDirectory;
            public string ObjectName;
            public UInt32 Attributes;
            public IntPtr SecurityDescriptor;
            public IntPtr SecurityQualityOfService;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct LSA_UNICODE_STRING
        {
            ushort length;
            ushort maximumLength;
            string buffer;
        }
        */

        /*
        [DllImport("credui.dll", CharSet = CharSet.Unicode)]
        public static extern uint CredUIPromptForWindowsCredentials(ref CREDUI_INFO notUsedHere,
          int authError,
          ref uint authPackage,
          IntPtr InAuthBuffer,
          uint InAuthBufferSize,
          out IntPtr refOutAuthBuffer,
          out uint refOutAuthBufferSize,
          ref bool fSave,
          PromptForWindowsCredentialsFlags flags);
        */

        /*
        public enum PromptForWindowsCredentialsFlags
        {
            /// <summary>
            /// The caller is requesting that the credential provider return the user name and password in plain text.
            /// This value cannot be combined with SECURE_PROMPT.
            /// </summary>
            CREDUIWIN_GENERIC = 0x1,
            /// <summary>
            /// The Save check box is displayed in the dialog box.
            /// </summary>
            CREDUIWIN_CHECKBOX = 0x2,
            /// <summary>
            /// Only credential providers that support the authentication package specified by the authPackage parameter should be enumerated.
            /// This value cannot be combined with CREDUIWIN_IN_CRED_ONLY.
            /// </summary>
            CREDUIWIN_AUTHPACKAGE_ONLY = 0x10,
            /// <summary>
            /// Only the credentials specified by the InAuthBuffer parameter for the authentication package specified by the authPackage parameter should be enumerated.
            /// If this flag is set, and the InAuthBuffer parameter is NULL, the function fails.
            /// This value cannot be combined with CREDUIWIN_AUTHPACKAGE_ONLY.
            /// </summary>
            CREDUIWIN_IN_CRED_ONLY = 0x20,
            /// <summary>
            /// Credential providers should enumerate only administrators. This value is intended for User Account Control (UAC) purposes only. We recommend that external callers not set this flag.
            /// </summary>
            CREDUIWIN_ENUMERATE_ADMINS = 0x100,
            /// <summary>
            /// Only the incoming credentials for the authentication package specified by the authPackage parameter should be enumerated.
            /// </summary>
            CREDUIWIN_ENUMERATE_CURRENT_USER = 0x200,
            /// <summary>
            /// The credential dialog box should be displayed on the secure desktop. This value cannot be combined with CREDUIWIN_GENERIC.
            /// Windows Vista: This value is not supported until Windows Vista with SP1.
            /// </summary>
            CREDUIWIN_SECURE_PROMPT = 0x1000,
            /// <summary>
            /// The credential provider should align the credential BLOB pointed to by the refOutAuthBuffer parameter to a 32-bit boundary, even if the provider is running on a 64-bit system.
            /// </summary>
            CREDUIWIN_PACK_32_WOW = 0x10000000,
        }
        */

        /*
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LookupPrivilegeName(
           string lpSystemName,
           IntPtr lpLuid,
           System.Text.StringBuilder lpName,
           ref int cchName);
        */

        /*
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool CheckTokenMembership(
            [In] [Optional] IntPtr TokenHandle,
            [In] IntPtr SidToCheck,
            [Out] out bool IsMember);
        */
    }
}
