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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace SinclairCC.MakeMeAdmin
{
    internal class NativeMethods
    {
        [DllImport("ole32.dll")]
        private static extern void CoTaskMemFree(IntPtr ptr);

        [DllImport("Kernel32.dll", EntryPoint = "RtlSecureZeroMemory", SetLastError = false)]
        private static extern void SecureZeroMemory(IntPtr dest, UIntPtr size);

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        private struct CREDUI_INFO
        {
            /// <summary>
            /// The size of the struct.
            /// </summary>
            public int cbSize;

            /// <summary>
            /// Specifies the handle to the parent window of the dialog box. The dialog box is modal with respect to the parent window. If this member is NULL, the desktop is the parent window of the dialog box.
            /// </summary>
            public IntPtr hwndParent;

            /// <summary>
            /// Pointer to a string containing a brief message to display in the dialog box. The length of this string should not exceed CREDUI_MAX_MESSAGE_LENGTH.
            /// </summary>
            public string pszMessageText;

            /// <summary>
            /// Pointer to a string containing the title for the dialog box. The length of this string should not exceed CREDUI_MAX_CAPTION_LENGTH.
            /// </summary>
            public string pszCaptionText;

            /// <summary>
            /// Bitmap to display in the dialog box. If this member is NULL, a default bitmap is used. The bitmap size is limited to 320x60 pixels.
            /// </summary>
            public IntPtr hbmBanner;
        }

        [DllImport("credui.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern bool CredPackAuthenticationBuffer(int dwFlags, string pszUserName, string pszPassword, IntPtr pPackedCredentials, ref int pcbPackedCredentials);


        [DllImport("credui.dll", CharSet = CharSet.Auto)]
        private static extern bool CredUnPackAuthenticationBuffer(int dwFlags,
                                                                   IntPtr pAuthBuffer,
                                                                   uint cbAuthBuffer,
                                                                   StringBuilder pszUserName,
                                                                   ref int pcchMaxUserName,
                                                                   StringBuilder pszDomainName,
                                                                   ref int pcchMaxDomainame,
                                                                   StringBuilder pszPassword,
                                                                   ref int pcchMaxPassword);

        [DllImport("credui.dll", CharSet = CharSet.Auto)]
        private static extern int CredUIPromptForWindowsCredentials(ref CREDUI_INFO notUsedHere,
                                                                     int authError,
                                                                     ref uint authPackage,
                                                                     IntPtr InAuthBuffer,
                                                                     int InAuthBufferSize,
                                                                     out IntPtr refOutAuthBuffer,
                                                                     out uint refOutAuthBufferSize,
                                                                     ref bool fSave,
                                                                     int flags);


        // Define the Windows LogonUser and CloseHandle functions.
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
        private static extern bool LogonUser(String username, String domain, IntPtr password, int logonType, int logonProvider, ref IntPtr token);

        /*
        [DllImport("advapi32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool LogonUser(
            [MarshalAs(UnmanagedType.LPWStr)] string username,
            [MarshalAs(UnmanagedType.LPWStr)] string domain,
            [MarshalAs(UnmanagedType.LPWStr)] string password,
            int logonType,
            int logonProvider,
            ref IntPtr token);
        */

        [DllImport("kernel32.dll", CharSet = CharSet.Auto)]
        private extern static bool CloseHandle(IntPtr handle);

        // Define the required LogonUser enumerations.
        private const int LOGON32_PROVIDER_DEFAULT = 0;

        /*
        private const int LOGON32_LOGON_INTERACTIVE = 2;
        */
        private const int LOGON32_LOGON_NETWORK = 3;
        /*
        private const int LOGON32_LOGON_BATCH = 4;
        private const int LOGON32_LOGON_SERVICE = 5;
        private const int LOGON32_LOGON_UNLOCK = 7;
        */


        internal static System.Net.NetworkCredential GetCredentials(IntPtr parentWindow, string userName = null, int errorCode = 0)
        {
            CREDUI_INFO credui = new CREDUI_INFO();
            credui.hwndParent = parentWindow;
            credui.pszCaptionText = Properties.Resources.CredentialsPromptCaption;
            credui.pszMessageText = Properties.Resources.CredentialsPromptMessage;
            credui.cbSize = Marshal.SizeOf(credui);
            uint authPackage = 0;
            IntPtr outCredBuffer = new IntPtr();
            uint outCredSize;
            bool save = false;
            /*int errorCode = 0;*/

            GetInputBuffer(userName, out var inCredBuffer, out var inCredSize);

            int result = CredUIPromptForWindowsCredentials(ref credui,
                                                           errorCode,
                                                           ref authPackage,
                                                           inCredBuffer,
                                                           inCredSize,
                                                           out outCredBuffer,
                                                           out outCredSize,
                                                           ref save,
                                                           0x200);

            if (inCredBuffer != IntPtr.Zero)
            {
                Marshal.FreeCoTaskMem(inCredBuffer);
            }

            StringBuilder usernameBuf = new StringBuilder(1);
            StringBuilder passwordBuf = new StringBuilder(1);
            StringBuilder domainBuf = new StringBuilder(1);
            int maxUserName = usernameBuf.Length;
            int maxPassword = passwordBuf.Length;
            int maxDomain = domainBuf.Length;

            if (!CredUnPackAuthenticationBuffer(0, outCredBuffer, outCredSize, usernameBuf, ref maxUserName, domainBuf, ref maxDomain, passwordBuf, ref maxPassword))
            {
                usernameBuf = new StringBuilder(maxUserName);
                passwordBuf = new StringBuilder(maxPassword);
                domainBuf = new StringBuilder(maxDomain);
            }


            if (result == 0)
            {
                if (CredUnPackAuthenticationBuffer(0, outCredBuffer, outCredSize, usernameBuf, ref maxUserName,
                                                   domainBuf, ref maxDomain, passwordBuf, ref maxPassword))
                {
                    Marshal.Copy(new byte[outCredSize], 0, outCredBuffer, (int)outCredSize);

                    // Clear the memory allocated by CredUIPromptForWindowsCredentials.
                    CoTaskMemFree(outCredBuffer);

                    System.Net.NetworkCredential returnCreds = new System.Net.NetworkCredential()
                    {
                        UserName = usernameBuf.ToString(),
                        Password = passwordBuf.ToString(),
                        Domain = domainBuf.ToString()
                    };

                    return returnCreds;
                }
            }

            return null;
        }

        // TODO: Change user to string? when moving to C# 8.
        private static void GetInputBuffer(string user, out IntPtr inCredBuffer, out int inCredSize)
        {
            if (!string.IsNullOrEmpty(user))
            {
                inCredSize = 1024;
                inCredBuffer = Marshal.AllocCoTaskMem(inCredSize);
                if (CredPackAuthenticationBuffer(0, user, pszPassword: "", inCredBuffer, ref inCredSize))
                {
                    return;
                }
            }

            inCredBuffer = IntPtr.Zero;
            inCredSize = 0;
        }


        internal static int ValidateCredentials(System.Net.NetworkCredential credentials)
        {
            if (null == credentials) { return 0x0000000D; }

            string userName = credentials.UserName;
            string domain = credentials.Domain;
            if ((string.IsNullOrEmpty(domain)) && (userName.IndexOf('\\') >= 0))
            {
                int slashIndex = userName.IndexOf('\\');
                domain = userName.Substring(0, slashIndex);
                userName = userName.Substring(slashIndex + 1);
            }


            IntPtr tokenHandle = IntPtr.Zero;
            IntPtr passwordPtr = IntPtr.Zero;
            bool returnValue = false;
            int error = 0;

            // Marshal the SecureString to unmanaged memory.
            passwordPtr = Marshal.SecureStringToGlobalAllocUnicode(credentials.SecurePassword);

            // Pass LogonUser the unmanaged (and decrypted) copy of the password.
            returnValue = LogonUser(userName, domain, passwordPtr,
                                    LOGON32_LOGON_NETWORK, LOGON32_PROVIDER_DEFAULT,
                                    ref tokenHandle);

            if (!returnValue && tokenHandle == IntPtr.Zero)
            {
                error = Marshal.GetLastWin32Error();
            }

            // Perform cleanup whether or not the call succeeded.
            // Zero-out and free the unmanaged string reference.
            Marshal.ZeroFreeGlobalAllocUnicode(passwordPtr);

            // Close the token handle.
            CloseHandle(tokenHandle);

            return error;

            /*
            // Throw an exception if an error occurred.
            if (error != 0)
            {
                throw new System.ComponentModel.Win32Exception(error);
            }

            return returnValue;
            */
        }
    }
}
