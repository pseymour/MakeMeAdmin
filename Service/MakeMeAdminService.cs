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
        private System.Timers.Timer removalTimer;

        public MakeMeAdminService()
        {
            InitializeComponent();
            this.removalTimer = new System.Timers.Timer(5000);
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
    }
}
