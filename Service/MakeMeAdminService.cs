// <copyright file="MakeMeAdminService.cs" company="Sinclair Community College">
// Copyright (c) 2010-2017, Sinclair Community College. All rights reserved.
// </copyright>

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

            this.removalTimer = new System.Timers.Timer(10000);
            this.removalTimer.AutoReset = true;
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
            ApplicationLog.WriteInformationEvent("Service host faulted.", EventID.DebugMessage);
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                base.OnStart(args);
            }
            catch (Exception) { };

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
                /*
                case SessionChangeReason.ConsoleDisconnect:
                case SessionChangeReason.RemoteDisconnect:
                */
                case SessionChangeReason.SessionLogoff:
#if DEBUG
                    ApplicationLog.WriteInformationEvent(string.Format("Session {0} has logged off.", changeDescription.SessionId), EventID.DebugMessage);
#endif

                    if (Settings.RemoveAdminRightsOnLogout)
                    {
                        System.Collections.Generic.List<SecurityIdentifier> sidsToRemove = new System.Collections.Generic.List<SecurityIdentifier>(PrincipalList.GetSIDs());

                        int[] sessionIds = LsaLogonSessions.LogonSessions.GetLoggedOnUserSessionIds();
                        foreach (int id in sessionIds)
                        {
                            System.Security.Principal.SecurityIdentifier sid = LsaLogonSessions.LogonSessions.GetSidForSessionId(id);
                            if (sid != null)
                            {
                                if (sidsToRemove.Contains(sid))
                                {
                                    sidsToRemove.Remove(sid);
                                }
                            }
                        }

                        for (int i = 0; i < sidsToRemove.Count; i++)
                        {
                            if (!(PrincipalList.ContainsSID(sidsToRemove[i]) && PrincipalList.IsRemote(sidsToRemove[i])))
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
                    }

                    break;
            }

            base.OnSessionChange(changeDescription);
        }
    }
}
