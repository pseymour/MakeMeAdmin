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
