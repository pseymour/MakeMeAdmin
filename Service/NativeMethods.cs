// <copyright file="NativeMethods.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>
// <author>Patrick S. Seymour</author>

namespace SinclairCC.Interop.Win32
{
    using System;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.InteropServices;

    /// <summary>
    /// This class contains functions for shutting down a Windows computer.
    /// </summary>
    internal class NativeMethods
    {
        /// <summary>
        /// Tells AdjustTokenPrivileges to enable the specified privilege.
        /// </summary>
        private const int SE_PRIVILEGE_ENABLED = 0x00000002;

        /// <summary>
        /// Required to query an access token.
        /// </summary>
        private const int TOKEN_QUERY = 0x00000008;

        /// <summary>
        /// Required to enable or disable the privileges in an access token.
        /// </summary>
        private const int TOKEN_ADJUST_PRIVILEGES = 0x00000020;

        /// <summary>
        /// The name of the privilege required to shut down the local system.
        /// </summary>
        private const string SE_SHUTDOWN_NAME = "SeShutdownPrivilege";

        /// <summary>
        /// This flag has no effect if terminal services is enabled. Otherwise, the system
        /// does not send the WM_QUERYENDSESSION and WM_ENDSESSION messages. This can cause
        /// applications to lose data.
        /// </summary>
        /// <remarks>
        /// You should only use this flag in an emergency.
        /// </remarks>
        private const int EWX_FORCE = 0x00000004;

        /// <summary>
        /// Forces processes to terminate if they do not respond to the WM_QUERYENDSESSION
        /// or WM_ENDSESSION message within the timeout interval.
        /// </summary>
        private const int EWX_FORCEIFHUNG = 0x00000010;

        /// <summary>
        /// Defines flags that are used to control the Windows shutdown process.
        /// </summary>
        public enum ShutdownFlag : int
        {
            /// <summary>
            /// Shuts down all processes running in the logon session of the process
            /// that called the ExitWindowsEx function. Then it logs the user off.
            /// </summary>
            /// <remarks>
            /// This flag can be used only by processes running in an interactive
            /// user's logon session.
            /// </remarks>
            LogOff = 0x00000000,

            /// <summary>
            /// Shuts down the system to a point at which it is safe to turn off the
            /// power. All file buffers have been flushed to disk, and all running
            /// processes have stopped.
            /// </summary>
            Shutdown = 0x00000001,

            /// <summary>
            /// Shuts down the system and then restarts it, as well as any applications
            /// that have been registered for restart using the RegisterApplicationRestart
            /// function.
            /// </summary>
            RestartApps = 0x00000040,

            /// <summary>
            /// Shuts down the system and then restarts the system.
            /// </summary>
            /// <remarks>
            /// The calling process must have the SE_SHUTDOWN_NAME privilege.
            /// </remarks>
            Reboot = 0x00000002,

            /// <summary>
            /// Shuts down the system and turns off the power. The system must support
            /// the power-off feature.
            /// </summary>
            /// <remarks>
            /// The calling process must have the SE_SHUTDOWN_NAME privilege.
            /// </remarks>
            PowerOff = 0x00000008
        }

        /// <summary>
        /// Performs the requested Windows shutdown action (reboot, shutdown, etc.).
        /// </summary>
        /// <param name="flag">
        /// The desired shutdown action.
        /// </param>
        /// <param name="force">
        /// Whether the shutdown action should be forced.
        /// </param>
        public static void PerformShutdownAction(ShutdownFlag flag, bool force)
        {
            int flagValue = (int)flag;
            
            // If the value of the Force parameter does not match the force flag's
            // status in the Flag parameter, alter the flag value to match the Force
            // parameter. For example, if Force is false but Flag contains the force
            // flag, the Flag value should be altered not to include the force flag.
            if (force != ((flagValue & EWX_FORCEIFHUNG) == EWX_FORCEIFHUNG))
            {
                flagValue ^= EWX_FORCEIFHUNG;
            }

            // Get the local shutdown privilege.
            GetShutdownPrivilege();

            // Tell Windows to perform the desired shutdown action.
            // TODO: This flag value doesn't seem to get us a string-value shutdown reason in the event log.
            ExitWindowsEx(flagValue, 64607);
        }

        /// <summary>
        /// Retrieves a pseudo handle for the current process.
        /// </summary>
        /// <returns>
        /// The return value is a pseudo handle to the current process.
        /// </returns>
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetCurrentProcess();

        /// <summary>
        /// Opens the access token associated with a process.
        /// </summary>
        /// <param name="h">
        /// A handle to the process whose access token is opened. The process must have the
        /// PROCESS_QUERY_INFORMATION access permission.
        /// </param>
        /// <param name="acc">
        /// Specifies an access mask that specifies the requested types of access to the
        /// access token. These requested access types are compared with the discretionary
        /// access control list (DACL) of the token to determine which accesses are granted
        /// or denied.
        /// </param>
        /// <param name="phtok">
        /// A pointer to a handle that identifies the newly opened access token when the
        /// function returns.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is non-zero. If it fails, the return
        /// value is zero.
        /// </returns>
        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool OpenProcessToken(IntPtr h, int acc, ref IntPtr phtok);

        /// <summary>
        /// Retrieves the locally unique identifier (LUID) used on a specified system to locally
        /// represent the specified privilege name.
        /// </summary>
        /// <param name="host">
        /// Specifies the name of the system on which the privilege name is retrieved. If a
        /// null string is specified, the function attempts to find the privilege name on
        /// the local system.
        /// </param>
        /// <param name="name">
        /// Specifies the name of the privilege, as defined in the Winnt.h header file. For
        /// example, this parameter could specify the constant, SE_SECURITY_NAME, or its
        /// corresponding string, "SeSecurityPrivilege".
        /// </param>
        /// <param name="pluid">
        /// Receives the LUID by which the privilege is known on the system specified by
        /// the host parameter.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is non-zero. If it fails, the return
        /// value is zero.
        /// </returns>
        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool LookupPrivilegeValue(string host, string name, ref long pluid);

        /// <summary>
        /// Enables or disables privileges in the specified access token. Enabling or disabling
        /// privileges in an access token requires TOKEN_ADJUST_PRIVILEGES access.
        /// </summary>
        /// <param name="htok">
        /// A handle to the access token that contains the privileges to be modified. The handle
        /// must have TOKEN_ADJUST_PRIVILEGES access to the token. If the PreviousState
        /// parameter is not NULL, the handle must also have TOKEN_QUERY access.
        /// </param>
        /// <param name="disall">
        /// Specifies whether the function disables all of the token's privileges. If this value
        /// is TRUE, the function disables all privileges and ignores the NewState parameter.
        /// If it is FALSE, the function modifies privileges based on the information pointed to
        /// by the NewState parameter.
        /// </param>
        /// <param name="newst">
        /// Structure that specifies an array of privileges and their attributes.
        /// </param>
        /// <param name="len">
        /// Specifies the size, in bytes, of the buffer pointed to by the PreviousState parameter.
        /// This parameter can be zero if the PreviousState parameter is NULL.
        /// </param>
        /// <param name="prev">
        /// A pointer to a buffer that the function fills with a TOKEN_PRIVILEGES structure that
        /// contains the previous state of any privileges that the function modifies. That is, if
        /// a privilege has been modified by this function, the privilege and its previous state
        /// are contained in the TOKEN_PRIVILEGES structure referenced by PreviousState. If the
        /// PrivilegeCount member of TOKEN_PRIVILEGES is zero, then no privileges have been changed
        /// by this function. This parameter can be NULL.
        /// </param>
        /// <param name="relen">
        /// A pointer to a variable that receives the required size, in bytes, of the buffer pointed
        /// to by the PreviousState parameter. This parameter can be NULL if PreviousState is NULL.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is nonzero. If the function fails, the
        /// return value is zero.
        /// </returns>
        [DllImport("advapi32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool AdjustTokenPrivileges(IntPtr htok, bool disall, ref TokPriv1Luid newst, int len, IntPtr prev, IntPtr relen);

        /// <summary>
        /// Logs off the interactive user, shuts down the system, or shuts down and restarts the
        /// system. It sends the WM_QUERYENDSESSION message to all applications to determine if
        /// they can be terminated.
        /// </summary>
        /// <param name="flg">
        /// The shutdown type. It should be one of the EWX_ values. You should include at most one
        /// of the EWX_FORCE* values, if any.
        /// </param>
        /// <param name="rea">
        /// The reason for initiating the shutdown.
        /// </param>
        /// <remarks>
        /// If the reason parameter is zero, the SHTDN_REASON_FLAG_PLANNED reason code will not
        /// be set and therefore the default action is an undefined shutdown that is logged as
        /// "No title for this reason could be found". By default, it is also an unplanned shutdown.
        /// Depending on how the system is configured, an unplanned shutdown triggers the creation
        /// of a file that contains the system state information, which can delay shutdown. Therefore,
        /// do not use zero for this parameter.
        /// </remarks>
        /// <returns>
        /// If the function succeeds, the return value is nonzero. If the function fails, the return value is zero.
        /// </returns>
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        private static extern bool ExitWindowsEx(int flg, int rea);

        /// <summary>
        /// Adjusts the privileges of the current process so that it has SeShutdownPrivilege.
        /// </summary>
        private static void GetShutdownPrivilege()
        {
            bool ok;
            TokPriv1Luid tp;
            IntPtr hproc = GetCurrentProcess();
            IntPtr htok = IntPtr.Zero;
            ok = OpenProcessToken(hproc, TOKEN_ADJUST_PRIVILEGES | TOKEN_QUERY, ref htok);
            tp.Count = 1;
            tp.Luid = 0;
            tp.Attr = SE_PRIVILEGE_ENABLED;
            ok = LookupPrivilegeValue(null, SE_SHUTDOWN_NAME, ref tp.Luid);
            ok = AdjustTokenPrivileges(htok, false, ref tp, 0, IntPtr.Zero, IntPtr.Zero);
        }

        /// <summary>
        /// Contains information about a set of privileges for an access token.
        /// </summary>
        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        private struct TokPriv1Luid
        {
            /// <summary>
            /// This must be set to the number of entries in the Privileges array.
            /// </summary>
            public int Count;

            /// <summary>
            /// Specifies a locally unique identifier (LUID) value.
            /// </summary>
            public long Luid;
            
            /// <summary>
            /// Specifies attributes of the LUID.
            /// </summary>
            public int Attr;
        }
    }
}