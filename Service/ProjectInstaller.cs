// <copyright file="ProjectInstaller.cs" company="Sinclair Community College">
// Copyright (c) 2010-2017, Sinclair Community College. All rights reserved.
// </copyright>

namespace SinclairCC.MakeMeAdmin
{
    using System;
    using System.ComponentModel;
    using System.Configuration.Install;

    /// <summary>
    /// This class installs and configures the service.
    /// </summary>
    [RunInstaller(true)]
    public partial class ProjectInstaller : System.Configuration.Install.Installer
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public ProjectInstaller()
        {
            // This call is required by the Designer.
            InitializeComponent();
        }

        /// <summary>
        /// This event fires before the installers perform their uninstall operations.
        /// </summary>
        /// <param name="sender">
        /// The service installer that is performing the installation.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        void serviceInstaller_BeforeUninstall(object sender, InstallEventArgs e)
        {
            ApplicationLog.RemoveSource();

            try
            {
                System.ServiceProcess.ServiceController controller = new System.ServiceProcess.ServiceController(this.serviceInstaller.ServiceName);
                controller.Stop();
            }

            // An error occurred when accessing a system API.
            catch (System.ComponentModel.Win32Exception) { }

            // The service cannot be stopped.
            catch (InvalidOperationException) { }

            // Remove all of the service settings stored on the computer.
            Settings.RemoveAllSettings();

            // Raise the BeforeUninstall event on the base class.
            base.OnBeforeUninstall(e.SavedState);
        }

        /// <summary>
        /// This event fires after the Install methods of all the installers in the
        /// Installers property have run.
        /// </summary>
        /// <param name="sender">
        /// The service installer that is performing the installation.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        void serviceInstaller_AfterInstall(object sender, InstallEventArgs e)
        {
            ApplicationLog.CreateSource();

            // Create a ServiceInstaller object from the sender object.
            System.ServiceProcess.ServiceInstaller installer = sender as System.ServiceProcess.ServiceInstaller;

            // If the ServiceInstaller object was created successfully, attempt to
            // start the service.
            if (installer != null)
            {
                // Attempt to start the service.
                try
                {
                    System.ServiceProcess.ServiceController controller = new System.ServiceProcess.ServiceController(installer.ServiceName);
                    controller.Start();
                }

                // An error occurred when accessing a system API.
                catch (System.ComponentModel.Win32Exception) { }

                // The service cannot be started.
                catch (InvalidOperationException) { }
            }

            base.OnAfterInstall(e.SavedState);
        }
    }
}
