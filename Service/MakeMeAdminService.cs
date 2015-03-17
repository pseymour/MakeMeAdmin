// <copyright file="MakeMeAdminService.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.Linq;
    using System.ServiceModel;
    using System.ServiceProcess;

    public partial class MakeMeAdminService : ServiceBase
    {
        private const string EndPointAddress = "net.pipe://localhost/MakeMeAdmin/Service";
        private System.Timers.Timer removalTimer;

        public MakeMeAdminService()
        {
            InitializeComponent();
            this.removalTimer = new System.Timers.Timer(5000);
            this.removalTimer.AutoReset = true;
            this.removalTimer.Elapsed += RemovalTimerElapsed;
        }

        void RemovalTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            string[] expiredSids = PrincipalList.GetExpiredSIDs();
            foreach (string sid in expiredSids)
            {
                /*bool removedSuccessfully = */ LocalAdministratorGroup.RemovePrincipal(sid);
                /*
                if (removedSuccessfully)
                {
                    PrincipalList.RemoveSID(sid);
                }
                */
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

            /*
            // We do this immediately, just in case we can't do any removals.
            // Save the added user list to the settings. We'll try to remove these again at service startup.
            Settings.SIDs = sids;
            */

            for (int i = 0; i < sids.Length; i++)
            {
                LocalAdministratorGroup.RemovePrincipal(sids[i]);
                /*PrincipalList.RemoveSID(sids[i]);*/
            }
            /*
            while (this.addedUsers.Count > 0)
            {
                string[] sids = this.addedUsers.Where(p => DateTime.Now.Subtract(p.Value).TotalMinutes >= Settings.AdminRightsTimeout).Select(p => p.Key).ToArray<string>();
                for (int i = 0; i < sids.Length; i++)
                {
                    LocalAdministratorGroup.RemovePrincipal(sids[i]);
                    addedUsers.Remove(sids[i]);
                }
            }
            */

            /*
            // Save the added user list to the settings. We'll try to remove these again at service startup.
            Settings.SIDs = PrincipalList.GetSIDs();
            */

            base.OnStop();
        }
    }
}
