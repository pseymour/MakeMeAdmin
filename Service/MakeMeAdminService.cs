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
            string[] expiredSids = PrincipalList.GetExpiredSIDs();
            foreach (string sid in expiredSids)
            {
                LocalAdministratorGroup.RemovePrincipal(sid, RemovalReason.Timeout);
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
                    }

                    break;
            }

            base.OnSessionChange(changeDescription);
        }
    }
}
