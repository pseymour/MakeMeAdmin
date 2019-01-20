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

            Principal[] expiredPrincipals = encryptedSettings.GetExpiredPrincipals();

            if (expiredPrincipals != null)
            {
                foreach (Principal prin in expiredPrincipals)
                {
                    LocalAdministratorGroup.RemovePrincipal(prin.PrincipalSid, RemovalReason.Timeout);

                    if ((Settings.EndRemoteSessionsUponExpiration) && (!string.IsNullOrEmpty(prin.RemoteAddress)))
                    {
                        string userName = prin.PrincipalName;
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

            LocalAdministratorGroup.ValidateAllAddedPrincipals();
        }


        /// <summary>
        /// Creates the WCF Service Host which is accessible via named pipes.
        /// </summary>
        private void OpenNamedPipeServiceHost()
        {
            this.namedPipeServiceHost = new ServiceHost(typeof(AdminGroupManipulator), new Uri(Shared.NamedPipeServiceBaseAddress));
            this.namedPipeServiceHost.Faulted += ServiceHostFaulted;
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport);
            this.namedPipeServiceHost.AddServiceEndpoint(typeof(IAdminGroup), binding, Shared.NamedPipeServiceBaseAddress);
            this.namedPipeServiceHost.Open();
        }


        /// <summary>
        /// Creates the WCF Service Host which is accessible via TCP.
        /// </summary>
        private void OpenTcpServiceHost()
        {
            this.tcpServiceHost = new ServiceHost(typeof(AdminGroupManipulator), new Uri(Shared.TcpServiceBaseAddress));
            this.tcpServiceHost.Faulted += ServiceHostFaulted;
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            this.tcpServiceHost.AddServiceEndpoint(typeof(IAdminGroup), binding, Shared.TcpServiceBaseAddress);
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
            ApplicationLog.WriteWarningEvent(Properties.Resources.ServiceHostFaulted, EventID.DebugMessage);
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

            // TODO: Does this do anything? It may never need to because of the SessionChange event.
            // If a user is added and we stop the service, are they removed?
            // If a user is added and we reboot, are they removed?            
            EncryptedSettings encryptedSettings = new EncryptedSettings(EncryptedSettings.SettingsFilePath);
            SecurityIdentifier[] sids = encryptedSettings.AddedPrincipalSIDs;
            for (int i = 0; i < sids.Length; i++)
            {
                LocalAdministratorGroup.RemovePrincipal(sids[i], RemovalReason.ServiceStopped);
            }

            base.OnStop();
        }


        // TODO: This function needs to be commented, after the testing of it is finished.

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
#if DEBUG
                    ApplicationLog.WriteInformationEvent(string.Format("Session {0} has logged off.", changeDescription.SessionId), EventID.DebugMessage);
#endif
                    //if (Settings.RemoveAdminRightsOnLogout)
                    //{
                    EncryptedSettings encryptedSettings = new EncryptedSettings(EncryptedSettings.SettingsFilePath);
                    System.Collections.Generic.List<SecurityIdentifier> sidsToRemove = new System.Collections.Generic.List<SecurityIdentifier>(encryptedSettings.AddedPrincipalSIDs);

                            /*
#if DEBUG
                            ApplicationLog.WriteInformationEvent("SID to remove list has been retrieved.", EventID.DebugMessage);
                            for (int i = 0; i < sidsToRemove.Count; i++)
                            {
                                ApplicationLog.WriteInformationEvent(string.Format("SID to remove: {0}", sidsToRemove[i]), EventID.DebugMessage);
                            }
#endif
                            */

                            int[] sessionIds = LsaLogonSessions.LogonSessions.GetLoggedOnUserSessionIds();
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

                    /*
#if DEBUG
                    ApplicationLog.WriteInformationEvent("SID to remove list has been updated.", EventID.DebugMessage);
                    for (int i = 0; i < sidsToRemove.Count; i++)
                    {
                        ApplicationLog.WriteInformationEvent(string.Format("SID to remove: {0}", sidsToRemove[i]), EventID.DebugMessage);
                    }
#endif
                    */

                    for (int i = 0; i < sidsToRemove.Count; i++)
                    {
                        if (
                            (!(encryptedSettings.ContainsSID(sidsToRemove[i]) && encryptedSettings.IsRemote(sidsToRemove[i])))
                            &&
                            (Settings.RemoveAdminRightsOnLogout || !encryptedSettings.GetExpirationTime(sidsToRemove[i]).HasValue)
                            )
                        {
                            LocalAdministratorGroup.RemovePrincipal(sidsToRemove[i], RemovalReason.UserLogoff);
                        }
                    }

                            /*
                             * In theory, this code should remove the user associated with the logoff, but it doesn't work.
                            SecurityIdentifier sid = LsaLogonSessions.LogonSessions.GetSidForSessionId(changeDescription.SessionId);
                            if (!(PrincipalList.ContainsSID(sid) && PrincipalList.IsRemote(sid)))
                            {
                                LocalAdministratorGroup.RemovePrincipal(sid, RemovalReason.UserLogoff);
                            }
                            */
                        //}
                        /*
                        else
                        {
#if DEBUG
                            ApplicationLog.WriteInformationEvent("Removing admin rights on log off is disabled.", EventID.DebugMessage);
#endif
                    }
                    */

                    break;

                // The user has logged on to a session, either locally or remotely.
                case SessionChangeReason.SessionLogon:

#if DEBUG
                    ApplicationLog.WriteInformationEvent(string.Format("Session {0} has logged on.", changeDescription.SessionId), EventID.DebugMessage);
#endif

                    WindowsIdentity userIdentity = LsaLogonSessions.LogonSessions.GetWindowsIdentityForSessionId(changeDescription.SessionId);

                    if (userIdentity != null)
                    {
                        if (
                            (Settings.AutomaticAddAllowed != null) &&
                            (Settings.AutomaticAddAllowed.Length > 0) &&
                            (Shared.UserIsAuthorized(userIdentity, Settings.AutomaticAddAllowed, Settings.AutomaticAddDenied))
                           )
                        {
#if DEBUG
                            ApplicationLog.WriteInformationEvent("User is allowed to be automatically added!", EventID.DebugMessage);
#endif
                            LocalAdministratorGroup.AddPrincipal(userIdentity, null, null);
                        }
                    }
                    else
                    {
                        ApplicationLog.WriteWarningEvent(Properties.Resources.UserIdentifyIsNull, EventID.DebugMessage);
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
