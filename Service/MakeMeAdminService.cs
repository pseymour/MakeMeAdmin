// <copyright file="MakeMeAdminService.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.ServiceModel;
    using System.ServiceProcess;

    public partial class MakeMeAdminService : ServiceBase
    {
        private System.Timers.Timer removalTimer;

        public MakeMeAdminService()
        {
            InitializeComponent();

            this.CanHandleSessionChangeEvent = true;

            this.removalTimer = new System.Timers.Timer(10000);
            this.removalTimer.AutoReset = true;
            this.removalTimer.Elapsed += RemovalTimerElapsed;
        }

        private void RemovalTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string[] expiredSids = PrincipalList.GetExpiredSIDs();
            foreach (string sid in expiredSids)
            {
                LocalAdministratorGroup.RemovePrincipal(sid);
            }

            LocalAdministratorGroup.ValidateAllAddedPrincipals();
        }

        private string EndPointAddress
        {
            get
            {
                return string.Format("net.pipe://{0}/MakeMeAdmin/Service", Shared.GetFullyQualifiedHostName());
            }
        }

        private void OpenServiceHost()
        {
            ServiceHost serviceHost = new ServiceHost(typeof(ServiceContract));
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            serviceHost.AddServiceEndpoint(typeof(IServiceContract), binding, EndPointAddress);
            serviceHost.Open();
        }
                
        protected override void OnStart(string[] args)
        {
            try
            {
                base.OnStart(args);
            }
            catch (Exception) { };

            this.OpenServiceHost();
            this.removalTimer.Start();
        }

        protected override void OnStop()
        {
            this.removalTimer.Stop();

            string[] sids = PrincipalList.GetSIDs();

            for (int i = 0; i < sids.Length; i++)
            {
                LocalAdministratorGroup.RemovePrincipal(sids[i]);
            }

            base.OnStop();
        }

        protected override void OnSessionChange(SessionChangeDescription changeDescription)
        {
            /*
            ApplicationLog.WriteInformationEvent("In OnSessionChange().", EventID.DebugMessage);
            */

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

                        System.Collections.Generic.List<string> sidsToRemove = new System.Collections.Generic.List<string>(PrincipalList.GetSIDs());

                        int[] sessionIds = LsaLogonSessions.LogonSessions.GetLoggedOnUserSessionIds();
                        foreach (int id in sessionIds)
                        {
                            System.Security.Principal.SecurityIdentifier sid = LsaLogonSessions.LogonSessions.GetSidForSessionId(id);
                            if (sid != null)
                            {
                                if (sidsToRemove.Contains(sid.Value))
                                {
#if DEBUG
                                    ApplicationLog.WriteInformationEvent(string.Format("session ID: {0}, SID: {1}, can keep their rights", id, sid.Value), EventID.DebugMessage);
#endif
                                    sidsToRemove.Remove(sid.Value);
                                }
                            }
                        }

                        for (int i = 0; i < sidsToRemove.Count; i++)
                        {
#if DEBUG
                            ApplicationLog.WriteInformationEvent(string.Format("SID {0} should be removed.", sidsToRemove[i]), EventID.DebugMessage);
#endif
                            LocalAdministratorGroup.RemovePrincipal(sidsToRemove[i]);
                        }
                    }

                    break;
            }


            base.OnSessionChange(changeDescription);
        }
    }
}
