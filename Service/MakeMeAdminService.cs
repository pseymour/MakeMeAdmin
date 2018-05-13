// 
// Copyright © 2010-2018, Sinclair Community College
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

    public partial class MakeMeAdminService : ServiceBase
    {
        private System.Timers.Timer removalTimer;
        private ServiceHost namedPipeServiceHost = null;
        private ServiceHost tcpServiceHost = null;

        public MakeMeAdminService()
        {
            InitializeComponent();

            /*
            this.CanHandleSessionChangeEvent = true;
            */

            this.removalTimer = new System.Timers.Timer()
            {
                Interval = 10000,
                AutoReset = true
            };
            this.removalTimer.Elapsed += RemovalTimerElapsed;
        }

        private void RemovalTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            /*
            string[] expiredSidStrings = PrincipalList.GetExpiredSIDs();
            foreach (string sidString in expiredSidStrings)
            {
                LocalAdministratorGroup.RemovePrincipal(new SecurityIdentifier(sidString), RemovalReason.Timeout);
            }
            */
        
            Principal[] expiredPrincipals = PrincipalList.GetExpiredPrincipals();
            foreach (Principal prin in expiredPrincipals)
            {
#if DEBUG
                ApplicationLog.WriteInformationEvent(string.Format("Expired Principal: {0}", prin.PrincipalSid.Value), EventID.DebugMessage);
#endif
                LocalAdministratorGroup.RemovePrincipal(prin.PrincipalSid, RemovalReason.Timeout);

                if ((Settings.EndRemoteSessionsUponExpiration) && (!string.IsNullOrEmpty(prin.RemoteAddress)))
                {
                    string userName = prin.PrincipalName;
                    while (userName.LastIndexOf("\\") >= 0)
                    {
                        userName = userName.Substring(userName.LastIndexOf("\\") + 1);
                    }
#if DEBUG
                    ApplicationLog.WriteInformationEvent(string.Format("Ending session for \"{0}\" on \"{1}.\"", userName, prin.RemoteAddress), EventID.DebugMessage);
#endif

                    int returnCode = Shared.EndNetworkSession(string.Format(@"\\{0}", prin.RemoteAddress), userName);

#if DEBUG
                    ApplicationLog.WriteInformationEvent(string.Format("Ending session returned error code {0}.", returnCode), EventID.DebugMessage);
#endif
                }
            }

            LocalAdministratorGroup.ValidateAllAddedPrincipals();
        }

        private void OpenNamedPipeServiceHost()
        {
            this.namedPipeServiceHost = new ServiceHost(typeof(AdminGroupManipulator), new Uri(Shared.NamedPipeServiceBaseAddress));
            this.namedPipeServiceHost.Faulted += ServiceHostFaulted;
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport);
            this.namedPipeServiceHost.AddServiceEndpoint(typeof(IAdminGroup), binding, Shared.NamedPipeServiceBaseAddress);
            this.namedPipeServiceHost.Open();
        }

        private void OpenTcpServiceHost()
        {
            this.tcpServiceHost = new ServiceHost(typeof(AdminGroupManipulator), new Uri(Shared.TcpServiceBaseAddress));
            this.tcpServiceHost.Faulted += ServiceHostFaulted;
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            this.tcpServiceHost.AddServiceEndpoint(typeof(IAdminGroup), binding, Shared.TcpServiceBaseAddress);
            this.tcpServiceHost.Open();
        }

        private void ServiceHostFaulted(object sender, EventArgs e)
        {
            // TODO: i18n.
            ApplicationLog.WriteInformationEvent("Service host faulted.", EventID.DebugMessage);
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                base.OnStart(args);
            }
            catch (Exception) { };

            ApplicationLog.CreateSource();

            this.OpenNamedPipeServiceHost();

            if (Settings.AllowRemoteRequests)
            {
                this.OpenTcpServiceHost();
            }

            this.removalTimer.Start();
        }

        protected override void OnStop()
        {
            if (this.namedPipeServiceHost.State == CommunicationState.Opened)
            {
                this.namedPipeServiceHost.Close();
            }

            if ((this.tcpServiceHost != null) && (this.tcpServiceHost.State == CommunicationState.Opened))
            {
                this.tcpServiceHost.Close();
            }

            this.removalTimer.Stop();

            SecurityIdentifier[] sids = PrincipalList.GetSIDs();
            for (int i = 0; i < sids.Length; i++)
            {
                LocalAdministratorGroup.RemovePrincipal(sids[i], RemovalReason.ServiceStopped);
            }

            base.OnStop();
        }

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
                            System.Collections.Generic.List<SecurityIdentifier> sidsToRemove = new System.Collections.Generic.List<SecurityIdentifier>(PrincipalList.GetSIDs());

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
                                    (!(PrincipalList.ContainsSID(sidsToRemove[i]) && PrincipalList.IsRemote(sidsToRemove[i])))
                                    &&
                                    (Settings.RemoveAdminRightsOnLogout || !PrincipalList.GetExpirationTime(sidsToRemove[i]).HasValue)
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
                    // TODO: i18n.
                    ApplicationLog.WriteInformationEvent(string.Format("Session logon. Session ID: {0}", changeDescription.SessionId), EventID.SessionChangeEvent);
#endif

                    WindowsIdentity userIdentity = LsaLogonSessions.LogonSessions.GetWindowsIdentityForSessionId(changeDescription.SessionId);

                    if (userIdentity != null)
                    {
                        /*
#if DEBUG
                        ApplicationLog.WriteInformationEvent("User identity is not null.", EventID.DebugMessage);
                        ApplicationLog.WriteInformationEvent(string.Format("user name: {0}", userIdentity.Name), EventID.DebugMessage);
                        ApplicationLog.WriteInformationEvent(string.Format("user SID: {0}", userIdentity.User), EventID.DebugMessage);
#endif
                        */

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
                        // TODO: i18n.
                        ApplicationLog.WriteWarningEvent("User identity is null.", EventID.DebugMessage);
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
