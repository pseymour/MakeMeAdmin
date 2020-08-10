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
    using System.Linq;
    using System.Runtime.Serialization.Formatters.Binary;
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


        private string portSharingServiceName = "NetTcpPortSharing";



        /// <summary>
        /// Instantiate a new instance of the Make Me Admin Windows service.
        /// </summary>
        public MakeMeAdminService()
        {
            InitializeComponent();

            /*
            this.CanHandleSessionChangeEvent = true;
            */

            this.EventLog.Source = "Make Me Admin";
            this.AutoLog = false;

            this.removalTimer = new System.Timers.Timer()
            {
                Interval = 10000,   // Raise the Elapsed event every ten seconds.
                AutoReset = true    // Raise the Elapsed event repeatedly.
            };
            this.removalTimer.Elapsed += RemovalTimerElapsed;
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
        }


        /// <summary>
        /// Creates the WCF Service Host which is accessible via named pipes.
        /// </summary>
        private void OpenNamedPipeServiceHost()
        {
            if (null != this.namedPipeServiceHost)
            {
                this.namedPipeServiceHost.Close();
            }
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
            if (null != this.tcpServiceHost)
            {
                this.tcpServiceHost.Close();
            }

            this.tcpServiceHost = new ServiceHost(typeof(AdminGroupManipulator), new Uri(Settings.TcpServiceBaseAddress));
            this.tcpServiceHost.Faulted += ServiceHostFaulted;
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            if (TcpPortInUse)
            {
                binding.PortSharingEnabled = true;
            }

            // If port sharing is enabled, then the Net.Tcp Port Sharing Service must be available as well.
            if (binding.PortSharingEnabled)
            {
                if (PortSharingServiceExists)
                {
                    ServiceController controller = new ServiceController(portSharingServiceName);
                    controller.Close();
                }
                else
                {
                    //ApplicationLog.WriteEvent("The Net.Tcp Port Sharing service does not exist. Unable to enable remote access.", 
                    return;
                }
            }

            this.tcpServiceHost.AddServiceEndpoint(typeof(IAdminGroup), binding, Settings.TcpServiceBaseAddress);
            this.tcpServiceHost.Open();
        }

        private bool PortSharingServiceExists
        {
            get
            {
                bool serviceExists = false;
                ServiceController[] services = ServiceController.GetServices();
                for (int i = 0; (i < services.Length) && (!serviceExists); i++)
                {
                    serviceExists |= (string.Compare(services[i].ServiceName, portSharingServiceName, true) == 0);
                }
                return serviceExists;
            }
        }

        private bool TcpPortInUse
        {
            get
            {
                System.Net.NetworkInformation.IPGlobalProperties globalIPProps = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties();
                return globalIPProps.GetActiveTcpListeners().Where(n => n.Port == Settings.TCPServicePort).Count() > 0;
            }
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
        /// Executes when a stop command is sent to the service by the Service Control Manager (SCM).
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
