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
    using System.Security.Principal;
    using System.ServiceModel;
    using System.ServiceProcess;

    /// <summary>
    /// This class is the Windows Service, which does privileged work
    /// on behalf of the an unprivileged user.
    /// </summary>
    public partial class MakeMeAdminService : ServiceBase
    {
        /// <summary>
        /// A timer to monitor when administrator rights should be removed.
        /// </summary>
        private System.Timers.Timer removalTimer;

        /// <summary>
        /// A Windows Communication Foundation (WCF) service host which communicates over named pipes.
        /// </summary>
        /// <remarks>
        /// This service host exists for communication on the local computer. It is not accessible
        /// from remote computers, and it is therefore always enabled.
        /// </remarks>
        private ServiceHost namedPipeServiceHost = null;

        /// <summary>
        /// A Windows Communication Foundation (WCF) service host which communicates over TCP.
        /// </summary>
        /// <remarks>
        /// This service host exists for communication from remote computers. It is only
        /// created if the remote administrator rights setting is enabled (true).
        /// </remarks>
        private ServiceHost tcpServiceHost = null;

        private ProcessWatcher processWatcher = null;

        /// <summary>
        /// Instantiate a new instance of the Make Me Admin Windows service.
        /// </summary>
        public MakeMeAdminService()
        {
            InitializeComponent();

            /*
            this.CanHandleSessionChangeEvent = true;
            */

            this.removalTimer = new System.Timers.Timer()
            {
                Interval = 10000,   // Raise the Elapsed event every ten seconds.
                AutoReset = true    // Raise the Elapsed event repeatedly.
            };
            this.removalTimer.Elapsed += RemovalTimerElapsed;


#if DEBUG
            ApplicationLog.WriteEvent(string.Format("process logging setting: {0:N0}", (int)Settings.LogElevatedProcesses), EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Information);
#endif
            if ((Settings.LogElevatedProcesses != ElevatedProcessLogging.Never) && (this.processWatcher == null))
            {
                processWatcher = new ProcessWatcher();
                processWatcher.ProcessCreated += ProcessCreatedHandler;
                processWatcher.Start();
            }

        }


        private void ProcessCreatedHandler(WMI.Win32.Process proc)
        {
            bool? processIsElevated = ProcessIsElevated(proc.ProcessId);
            if (processIsElevated.HasValue && processIsElevated.Value)
            { // Process is elevated, so we need to log it if the settings say so.

#if DEBUG
                ApplicationLog.WriteEvent(string.Format("elevated process detected: \"{0}\"", proc.CommandLine.Trim()), EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Information);
#endif

                System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById((int)proc.ProcessId);
                WindowsIdentity processOwner = NativeMethods.GetProcessOwner(process.Handle);
                process.Dispose();

#if DEBUG
                if (processOwner == null)
                {
                    ApplicationLog.WriteEvent("Process owner is null.", EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Information);
                }
                else
                {
                    ApplicationLog.WriteEvent(string.Format("process user SID : \"{0}\"", processOwner.User), EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Information);
                }
#endif

                bool logEvent = false;

                if (Settings.LogElevatedProcesses == ElevatedProcessLogging.OnlyWhenAdmin)
                { // The settings say only log if the user is an admin, so figure that out.
                    if (processOwner != null)
                    {
                        EncryptedSettings encryptedSettings = new EncryptedSettings(EncryptedSettings.SettingsFilePath);
                        logEvent = encryptedSettings.ContainsSID(processOwner.User);
                    }
                }
                else if (Settings.LogElevatedProcesses == ElevatedProcessLogging.Always)
                {
                    logEvent = true;
                }

                if (logEvent)
                {
                    System.Text.StringBuilder eventLogMessage = new System.Text.StringBuilder("Process ");
                    eventLogMessage.Append(proc.Name);
                    eventLogMessage.Append(" (ID: ");
                    eventLogMessage.Append(proc.ProcessId);
                    eventLogMessage.Append(") created at ");
                    eventLogMessage.Append(DateTime.Now);
                    eventLogMessage.Append(". Path is \"");
                    eventLogMessage.Append(proc.ExecutablePath);
                    eventLogMessage.Append(".\"");
                    if (processOwner != null)
                    {
                        eventLogMessage.Append(" User SID is \"");
                        eventLogMessage.Append(processOwner.User.Value);
                        eventLogMessage.Append(".\"");
                    }
                    ApplicationLog.WriteEvent(eventLogMessage.ToString(), EventID.ElevatedProcess, System.Diagnostics.EventLogEntryType.Information);
                }
            }
        }

        private bool? ProcessIsElevated(uint processId)
        {
            bool? returnValue = null;
            try
            {
                System.Diagnostics.Process process = System.Diagnostics.Process.GetProcessById((int)processId);
                returnValue = NativeMethods.IsProcessElevated2(process.Handle);
                process.Dispose();
            }
            catch (System.ComponentModel.Win32Exception)
            {
            }
            catch (System.InvalidOperationException)
            {
            }
            catch (System.ArgumentException)
            {
            }
            return returnValue;
        }

        /// <summary>
        /// Handles the Elapsed event for the rights removal timer.
        /// </summary>
        /// <param name="sender">
        /// The timer whose Elapsed event is firing.
        /// </param>
        /// <param name="e">
        /// Data related to the event.
        /// </param>
        private void RemovalTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            EncryptedSettings encryptedSettings = new EncryptedSettings(EncryptedSettings.SettingsFilePath);

            User[] expiredUsers = encryptedSettings.GetExpiredUsers();

            if (expiredUsers != null)
            {
                foreach (User prin in expiredUsers)
                {
                    LocalAdministratorGroup.RemoveUser(prin.Sid, RemovalReason.Timeout);

                    if ((Settings.EndRemoteSessionsUponExpiration) && (!string.IsNullOrEmpty(prin.RemoteAddress)))
                    {
                        string userName = prin.Name;
                        while (userName.LastIndexOf("\\") >= 0)
                        {
                            userName = userName.Substring(userName.LastIndexOf("\\") + 1);
                        }

                        int returnCode = 0;
                        if (!string.IsNullOrEmpty(userName))
                        {
                            returnCode = LocalAdministratorGroup.EndNetworkSession(string.Format(@"\\{0}", prin.RemoteAddress), userName);
                        }
                    }
                }
            }

            LocalAdministratorGroup.ValidateAllAddedUsers();

            if ((Settings.LogElevatedProcesses != ElevatedProcessLogging.Never) && (this.processWatcher == null))
            {
                processWatcher = new ProcessWatcher();
                processWatcher.ProcessCreated += ProcessCreatedHandler;
                processWatcher.Start();
            }

        }


        /// <summary>
        /// Creates the WCF Service Host which is accessible via named pipes.
        /// </summary>
        private void OpenNamedPipeServiceHost()
        {
            this.namedPipeServiceHost = new ServiceHost(typeof(AdminGroupManipulator), new Uri(Settings.NamedPipeServiceBaseAddress));
            this.namedPipeServiceHost.Faulted += ServiceHostFaulted;
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport);
            this.namedPipeServiceHost.AddServiceEndpoint(typeof(IAdminGroup), binding, Settings.NamedPipeServiceBaseAddress);
            this.namedPipeServiceHost.Open();
        }


        /// <summary>
        /// Creates the WCF Service Host which is accessible via TCP.
        /// </summary>
        private void OpenTcpServiceHost()
        {
            this.tcpServiceHost = new ServiceHost(typeof(AdminGroupManipulator), new Uri(Settings.TcpServiceBaseAddress));
            this.tcpServiceHost.Faulted += ServiceHostFaulted;
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            this.tcpServiceHost.AddServiceEndpoint(typeof(IAdminGroup), binding, Settings.TcpServiceBaseAddress);
            this.tcpServiceHost.Open();
        }


        /// <summary>
        /// Handles the faulted event for a WCF service host.
        /// </summary>
        /// <param name="sender">
        /// The service host that has entered the faulted state.
        /// </param>
        /// <param name="e">
        /// Data related to the event.
        /// </param>
        private void ServiceHostFaulted(object sender, EventArgs e)
        {
            ApplicationLog.WriteEvent(Properties.Resources.ServiceHostFaulted, EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Warning);
        }


        /// <summary>
        /// Handles the startup of the service. 
        /// </summary>
        /// <param name="args">
        /// Data passed the start command.
        /// </param>
        /// <remarks>
        /// This function executes when a Start command is sent to the service by the
        /// Service Control Manager (SCM) or when the operating system starts
        /// (for a service that starts automatically).
        /// </remarks>
        protected override void OnStart(string[] args)
        {
            try
            {
                base.OnStart(args);
            }
            catch (Exception) { };

            // Create the Windows Event Log source for this application.
            ApplicationLog.CreateSource();

            // Open the service host which is accessible via named pipes.
            this.OpenNamedPipeServiceHost();

            // If remote requests are allowed, open the service host which
            // is accessible via TCP.
            if (Settings.AllowRemoteRequests)
            {
                this.OpenTcpServiceHost();
            }

            // Start the timer that watches for expired administrator rights.
            this.removalTimer.Start();
        }


        /// <summary>
        /// Handles the stopping of the service.
        /// </summary>
        /// <remarks>
        /// Executes when a stop command is sent to the service by the Serivce Control Manager (SCM).
        /// </remarks>
        protected override void OnStop()
        {
            if ((this.namedPipeServiceHost != null) && (this.namedPipeServiceHost.State == CommunicationState.Opened))
            {
                this.namedPipeServiceHost.Close();
            }

            if ((this.tcpServiceHost != null) && (this.tcpServiceHost.State == CommunicationState.Opened))
            {
                this.tcpServiceHost.Close();
            }

            this.removalTimer.Stop();

            EncryptedSettings encryptedSettings = new EncryptedSettings(EncryptedSettings.SettingsFilePath);
            SecurityIdentifier[] sids = encryptedSettings.AddedUserSIDs;
            for (int i = 0; i < sids.Length; i++)
            {
                LocalAdministratorGroup.RemoveUser(sids[i], RemovalReason.ServiceStopped);
            }

            base.OnStop();
        }


        /// <summary>
        /// Executes when a change event is received from a Terminal Server session.
        /// </summary>
        /// <param name="changeDescription">
        /// Identifies the type of session change and the session to which it applies.
        /// </param>
        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            switch (changeDescription.Reason)
            {
                // The user has logged off from a session, either locally or remotely.
                case SessionChangeReason.SessionLogoff:

                    EncryptedSettings encryptedSettings = new EncryptedSettings(EncryptedSettings.SettingsFilePath);
                    System.Collections.Generic.List<SecurityIdentifier> sidsToRemove = new System.Collections.Generic.List<SecurityIdentifier>(encryptedSettings.AddedUserSIDs);

                    int[] sessionIds = LsaLogonSessions.LogonSessions.GetLoggedOnUserSessionIds();

                    // For any user that is still logged on, remove their SID from the list of
                    // SIDs to be removed from Administrators. That is, let the users who are still
                    // logged on stay in the Administrators group.
                    foreach (int id in sessionIds)
                    {
                        SecurityIdentifier sid = LsaLogonSessions.LogonSessions.GetSidForSessionId(id);
                        if (sid != null)
                        {
                            if (sidsToRemove.Contains(sid))
                            {
                                sidsToRemove.Remove(sid);
                            }
                        }
                    }

                    // Process the list of SIDs to be removed from Administrators.
                    for (int i = 0; i < sidsToRemove.Count; i++)
                    {
                        if (
                            // If the user is not remote.
                            (!(encryptedSettings.ContainsSID(sidsToRemove[i]) && encryptedSettings.IsRemote(sidsToRemove[i])))
                            &&
                            // If admin rights are to be removed on logoff, or the user's rights do not expire.
                            (Settings.RemoveAdminRightsOnLogout || !encryptedSettings.GetExpirationTime(sidsToRemove[i]).HasValue)
                            )
                        {
                            LocalAdministratorGroup.RemoveUser(sidsToRemove[i], RemovalReason.UserLogoff);
                        }
                    }

                    /*
                     * In theory, this code should remove the user associated with the logoff, but it doesn't work.
                    SecurityIdentifier sid = LsaLogonSessions.LogonSessions.GetSidForSessionId(changeDescription.SessionId);
                    if (!(UserList.ContainsSID(sid) && UserList.IsRemote(sid)))
                    {
                        LocalAdministratorGroup.RemoveUser(sid, RemovalReason.UserLogoff);
                    }
                    */

                    break;

                // The user has logged on to a session, either locally or remotely.
                case SessionChangeReason.SessionLogon:

                    WindowsIdentity userIdentity = LsaLogonSessions.LogonSessions.GetWindowsIdentityForSessionId(changeDescription.SessionId);

                    if (userIdentity != null)
                    {

                        NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport);
                        ChannelFactory<IAdminGroup> namedPipeFactory = new ChannelFactory<IAdminGroup>(binding, Settings.NamedPipeServiceBaseAddress);
                        IAdminGroup channel = namedPipeFactory.CreateChannel();
                        bool userIsAuthorizedForAutoAdd = channel.UserIsAuthorized(Settings.AutomaticAddAllowed, Settings.AutomaticAddDenied);
                        namedPipeFactory.Close();

                        // If the user is in the automatic add list, then add them to the Administrators group.
                        if (
                            (Settings.AutomaticAddAllowed != null) &&
                            (Settings.AutomaticAddAllowed.Length > 0) &&
                            (userIsAuthorizedForAutoAdd /*UserIsAuthorized(userIdentity, Settings.AutomaticAddAllowed, Settings.AutomaticAddDenied)*/)
                           )
                        {
                            LocalAdministratorGroup.AddUser(userIdentity, null, null);
                        }
                    }
                    else
                    {
                        ApplicationLog.WriteEvent(Properties.Resources.UserIdentifyIsNull, EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Warning);
                    }

                    break;

                /*
                // The user has reconnected or logged on to a remote session.
                case SessionChangeReason.RemoteConnect:
                    ApplicationLog.WriteInformationEvent(string.Format("Remote connect. Session ID: {0}", changeDescription.SessionId), EventID.SessionChangeEvent);
                    break;
                */

                /*
                // The user has disconnected or logged off from a remote session.
                case SessionChangeReason.RemoteDisconnect:
                    ApplicationLog.WriteInformationEvent(string.Format("Remote disconnect. Session ID: {0}", changeDescription.SessionId), EventID.SessionChangeEvent);
                    break;
                */

                /*
                // The user has locked their session.
                case SessionChangeReason.SessionLock:
                    ApplicationLog.WriteInformationEvent(string.Format("Session lock. Session ID: {0}", changeDescription.SessionId), EventID.SessionChangeEvent);
                    break;
                */

                /*
                // The user has unlocked their session.
                case SessionChangeReason.SessionUnlock:
                    ApplicationLog.WriteInformationEvent(string.Format("Session unlock. Session ID: {0}", changeDescription.SessionId), EventID.SessionChangeEvent);
                    break;
                */

            }

            base.OnSessionChange(changeDescription);
        }
    }
}
