﻿// 
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
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading.Tasks;

    public class Desktop
    {
        private const int NORMAL_PRIORITY_CLASS = 0x00000020;

        private const long DF_ALLOWOTHERACCOUNTHOOK = 0x0001L;

        private const uint DELETE = 0x00010000;
        private const uint READ_CONTROL = 0x00020000;
        private const uint WRITE_DAC = 0x00040000;
        private const uint WRITE_OWNER = 0x00080000;

        private const uint STANDARD_RIGHTS_REQUIRED = DELETE | READ_CONTROL | WRITE_DAC | WRITE_OWNER;

        private const long DESKTOP_CREATEWINDOW = 0x0002L;
        private const long DESKTOP_ENUMERATE = 0x0040L;
        private const long DESKTOP_WRITEOBJECTS = 0x0080L;
        private const long DESKTOP_SWITCHDESKTOP = 0x0100L;
        private const long DESKTOP_CREATEMENU = 0x0004L;
        private const long DESKTOP_HOOKCONTROL = 0x0008L;
        private const long DESKTOP_READOBJECTS = 0x0001L;
        private const long DESKTOP_JOURNALRECORD = 0x0010L;
        private const long DESKTOP_JOURNALPLAYBACK = 0x0020L;
        private const uint AccessRights = (uint)DESKTOP_JOURNALRECORD |
            (uint)DESKTOP_JOURNALPLAYBACK |
            (uint)DESKTOP_CREATEWINDOW |
            (uint)DESKTOP_ENUMERATE |
            (uint)DESKTOP_WRITEOBJECTS |
            (uint)DESKTOP_SWITCHDESKTOP |
            (uint)DESKTOP_CREATEMENU |
            (uint)DESKTOP_HOOKCONTROL |
            (uint)DESKTOP_READOBJECTS |
            STANDARD_RIGHTS_REQUIRED;

        private delegate bool EnumDesktopProc(string lpszDesktop, IntPtr lParam);
        private delegate bool EnumWinStaProc(string lpszWinSta, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern IntPtr GetProcessWindowStation();

        [DllImport("user32.dll")]
        private static extern bool EnumDesktops(IntPtr hwinsta, EnumDesktopProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern IntPtr CreateDesktop(string desktopName, string device, string deviceMode, int flags, uint accessMask, [In] ref SECURITY_ATTRIBUTES attributes);

        [DllImport("user32.dll")]
        private static extern IntPtr OpenDesktop(string lpszDesktop, uint dwFlags, bool fInherit, uint dwDesiredAccess);

        /*
        [DllImport("user32.dll")]
        public static extern IntPtr OpenDesktop(string lpszDesktop, uint dwFlags, bool fInherit, uint dwDesiredAccess);
        */

        [DllImport("user32.dll")]
        private static extern bool SwitchDesktop(IntPtr hDesktop);

        /*
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SwitchDesktop(IntPtr hDesktop);
        */

        [DllImport("kernel32.dll")]
        private static extern bool CreateProcess(string lpApplicationName, string lpCommandLine, IntPtr lpProcessAttributes, IntPtr lpThreadAttributes, bool bInheritHandles, int dwCreationFlags, IntPtr lpEnvironment, string lpCurrentDirectory, ref STARTUPINFO lpStartupInfo, ref PROCESS_INFORMATION lpProcessInformation);
        
        [DllImport("user32.dll")]
        private static extern bool EnumWindowStations(EnumWinStaProc lpEnumFunc, IntPtr lParam);



        // TODO: All of this stuff was added to try to detect the Winlogon desktop.

        /// <summary>
        /// The WTSGetActiveConsoleSessionId function retrieves the Remote Desktop Services session that
        /// is currently attached to the physical console. The physical console is the monitor, keyboard, and mouse.
        /// Note that it is not necessary that Remote Desktop Services be running for this function to succeed.
        /// </summary>
        /// <returns>The session identifier of the session that is attached to the physical console. If there is no
        /// session attached to the physical console, (for example, if the physical console session is in the process
        /// of being attached or detached), this function returns 0xFFFFFFFF.</returns>
        [DllImport("kernel32.dll")]
        public static extern uint WTSGetActiveConsoleSessionId();

        [DllImport("user32.dll", SetLastError = false)]
        public static extern IntPtr GetDesktopWindow();


        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr OpenInputDesktop(uint dwFlags, bool fInherit,
            uint dwDesiredAccess);

        [DllImport("user32.dll", SetLastError = true)]
        public static extern IntPtr CloseDesktop(IntPtr hDesktop);




        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetThreadDesktop(IntPtr hDesktop);


        /*
        BOOL WINAPI EnumDesktops(
  _In_opt_ HWINSTA         hwinsta,
  _In_ DESKTOPENUMPROC lpEnumFunc,
  _In_ LPARAM          lParam
);

        BOOL WINAPI EnumWindowStations(
          _In_ WINSTAENUMPROC lpEnumFunc,
          _In_ LPARAM         lParam
        );
        */

        [StructLayout(LayoutKind.Sequential)]
        public struct SECURITY_ATTRIBUTES
        {
            public int nLength;
            public IntPtr lpSecurityDescriptor;
            public int bInheritHandle;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct STARTUPINFO
        {
            public int cb;
            public string lpReserved;
            public string lpDesktop;
            public string lpTitle;
            public int dwX;
            public int dwY;
            public int dwXSize;
            public int dwYSize;
            public int dwXCountChars;
            public int dwYCountChars;
            public int dwFillAttribute;
            public int dwFlags;
            public short wShowWindow;
            public short cbReserved2;
            public IntPtr lpReserved2;
            public IntPtr hStdInput;
            public IntPtr hStdOutput;
            public IntPtr hStdError;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PROCESS_INFORMATION
        {
            public IntPtr hProcess;
            public IntPtr hThread;
            public int dwProcessId;
            public int dwThreadId;
        }

        private static List<string> _desktops = new List<string>();
        private static List<string> _windowStations = new List<string>();

        public static IntPtr Create(string name)
        {
            SECURITY_ATTRIBUTES securityAttributes = new SECURITY_ATTRIBUTES();
            securityAttributes.nLength = Marshal.SizeOf(securityAttributes);

            return CreateDesktop(name, null, null, (int)DF_ALLOWOTHERACCOUNTHOOK, AccessRights, ref securityAttributes);
        }

        public static IntPtr Open(string name)
        {
            return OpenDesktop(name, 0, false, AccessRights);
        }

        public static bool Show(IntPtr desktopPrt)
        {
            if (desktopPrt == IntPtr.Zero) return false;
            return SwitchDesktop(desktopPrt);
        }

        public static void Prepare(string desktopName)
        {
            CreateProcess("C:\\Windows\\explorer.exe", desktopName);
        }

        public static bool Exists(string name)
        {
            return GetDesktops().Any(s => s == name);
        }

        public static string[] Get()
        {
            return GetDesktops();
        }

        private static void CreateProcess(string path, string desktopName)
        {
            STARTUPINFO si = new STARTUPINFO();
            si.cb = Marshal.SizeOf(si);
            si.lpDesktop = desktopName;

            PROCESS_INFORMATION pi = new PROCESS_INFORMATION();

            string lpCurrentDirectory = System.IO.Path.GetDirectoryName(path);
            CreateProcess(path, null, IntPtr.Zero, IntPtr.Zero, true, NORMAL_PRIORITY_CLASS, IntPtr.Zero, lpCurrentDirectory, ref si, ref pi);
        }

        private static string[] GetDesktops()
        {
            IntPtr windowStation = GetProcessWindowStation();
            if (windowStation == IntPtr.Zero) return new string[0];

            _desktops.Clear();

            bool result = EnumDesktops(windowStation, DesktopEnumProc, IntPtr.Zero);
            if (!result) return new string[0];

            return _desktops.ToArray();
        }

        private static bool DesktopEnumProc(string lpszDesktop, IntPtr lParam)
        {
            _desktops.Add(lpszDesktop);
            return true;
        }

        public static string[] GetWindowStations()
        {
            /*
            IntPtr windowStation = GetProcessWindowStation();
            if (windowStation == IntPtr.Zero) return new string[0];
            */
            _windowStations.Clear();

            bool result = EnumWindowStations(WinStaEnumProc, IntPtr.Zero);
            if (!result) return new string[0];

            return _windowStations.ToArray();
        }

        private static bool WinStaEnumProc(string lpszWinSta, IntPtr lParam)
        {
            _windowStations.Add(lpszWinSta);
            return true;
        }

    }
}
