// 
// Copyright © 2010-2025, Sinclair Community College
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
    using SinclairCC.Collections.MruList;
    using System;
    using System.Reflection;
    using System.ServiceModel;
    using System.Threading.Tasks;
    using System.Windows.Forms;

    /// <summary>
    /// This form allows a user to submit a request for administrator rights
    /// on a remote computer.
    /// </summary>
    public partial class SubmitRequestForm : Form
    {
        /// <summary>
        /// A list of the most-recently used host names.
        /// </summary>
        private MruItemCollection<string> hostNameMruList;

        /// <summary>
        /// Delegate for a function which adds a hostname to the MRU list
        /// and the accompanying combo box.
        /// </summary>
        /// <param name="hostName">
        /// </param>
        private delegate void AddHostNameDelegate(string hostName);

        /// <summary>
        /// Object for the delegate that adds a host name to the MRU.
        /// </summary>
        private AddHostNameDelegate addHostNameToMruDelegate;

        /// <summary>
        /// Initialize a new instance of the SubmitRequestForm class.
        /// </summary>
        public SubmitRequestForm()
        {
            InitializeComponent();

            this.addHostNameToMruDelegate = new AddHostNameDelegate(this.AddHostName);

            this.requestButton.Enabled = !string.IsNullOrEmpty(this.mruComboBox.Text.Trim());

            this.hostNameMruList = new MruItemCollection<string>(10);
            string[] settingsMruList = UserSettings.HostNameMru;
            if (settingsMruList != null)
            {
                for (int i = settingsMruList.Length - 1; i >= 0; i--)
                {
                    AddHostName(settingsMruList[i]);
                }
            }

            this.Icon = Properties.Resources.SecurityLock;

            this.hostNameLabel.Text = Properties.Resources.HostNameLabelText;
            this.requestButton.Text = Properties.Resources.RequestRightsButtonText;
            this.clearHistoryButton.Text = Properties.Resources.ClearHistoryButtonText;

            this.SetFormText();
        }

        /// <summary>
        /// Sets the text at the top of the form.
        /// </summary>
        private void SetFormText()
        {
            System.Text.StringBuilder formText = new System.Text.StringBuilder("Make Me Admin Remote ");

            // Append to the form text the first three parts of the assembly's version number.
            formText.Append(Assembly.GetExecutingAssembly().GetName().Version.ToString(3));

            this.Text = formText.ToString();
        }

        /// <summary>
        /// Adds a hostname to the MRU list and updates the combo box accordingly.
        /// </summary>
        /// <param name="hostName">
        /// The hostname to be added.
        /// </param>
        private void AddHostName(string hostName)
        {
            // Add the hostname to the MRU list.
            this.hostNameMruList.Add(hostName);

            // Update the combo box's auto-complete source list to be the MRU list contents.
            this.mruComboBox.AutoCompleteCustomSource.Clear();
            this.mruComboBox.AutoCompleteCustomSource.AddRange(this.hostNameMruList.ToArray());

            // Set the combo box's contents to be the MRU list contents.
            this.mruComboBox.Items.Clear();
            this.mruComboBox.Items.AddRange(this.hostNameMruList.ToArray());

            this.mruComboBox.SelectionStart = 0;
            this.mruComboBox.SelectionLength = hostName.Length;
        }

        /// <summary>
        /// Submits a request for administrator rights to the specified host.
        /// </summary>
        /// <param name="hostName">
        /// The name of the host to which the request is submitted.
        /// </param>
        private void RequestAdminRights(string hostName)
        {
            // The address of the remote computer's service host.
            string remoteHostAddress = string.Format("net.tcp://{0}:{1}/MakeMeAdmin/Service", hostName, Settings.TCPServicePort);

            // Open a connection to the remote host.
            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            ChannelFactory<IAdminGroup> namedPipeFactory = new ChannelFactory<IAdminGroup>(binding, remoteHostAddress);
            IAdminGroup channel = namedPipeFactory.CreateChannel();

            // Submit a request for administrator rights on the remote host.
            try
            {
                channel.AddUserToAdministratorsGroup();
            }
            catch (System.ServiceModel.EndpointNotFoundException)
            {
                throw;
            }
            catch (Exception)
            {
                throw;
                // TODO: What do we do with this error?
            }
        }

        /// <summary>
        /// Handles the click event for the rights request button.
        /// </summary>
        /// <param name="sender">
        /// The button being clicked.
        /// </param>
        /// <param name="e">
        /// Data specific to the event.
        /// </param>
        private void requestButton_Click(object sender, EventArgs e)
        {
            string hostName = this.mruComboBox.Text.Trim();
            this.mruComboBox.Text = hostName;

            // Add the hostname to the MRU.
            Task.Factory.StartNew(() => this.mruComboBox.Invoke(this.addHostNameToMruDelegate, hostName));

            // Submit a request for administrator rights to the remote computer.
            Task requestRightsTask = Task.Factory
                .StartNew(() => this.RequestAdminRights(hostName))
                .ContinueWith(resultTask =>
                {
                    if (resultTask.IsFaulted)
                    {
                        System.Text.StringBuilder messageBoxText = new System.Text.StringBuilder(Properties.Resources.ErrorRequestingRightsOn);
                        messageBoxText.Append(hostName);
                        messageBoxText.Append(".");
                        messageBoxText.Append(System.Environment.NewLine);
                        messageBoxText.Append(Properties.Resources.ErrorMessage);
                        messageBoxText.Append(": ");
                        messageBoxText.Append(System.Environment.NewLine);
                        messageBoxText.Append("{0}");
                        resultTask.Exception.Flatten().Handle(excep => { MessageBox.Show(string.Format(messageBoxText.ToString(), excep.Message), "Make Me Admin Remote", MessageBoxButtons.OK, MessageBoxIcon.Error); return true; });
                    }
                    else
                    {
                        MessageBox.Show(string.Format(Properties.Resources.AdminRightsRequestedOnHost, hostName), "Make Me Admin Remote", MessageBoxButtons.OK);
                    }
                });
        }

        /// <summary>
        /// Handles the FormClosed event for the form.
        /// </summary>
        /// <param name="sender">
        /// The form being closed.
        /// </param>
        /// <param name="e">
        /// Data specific to the event.
        /// </param>
        private void SubmitRequestForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Save the MRU list.
            UserSettings.HostNameMru = this.hostNameMruList.ToArray();
        }

        /// <summary>
        /// Handles the click event for the Clear History button.
        /// </summary>
        /// <param name="sender">
        /// The button being clicked.
        /// </param>
        /// <param name="e">
        /// Data specific to the event.
        /// </param>
        private void clearHistoryButton_Click(object sender, EventArgs e)
        {
            this.hostNameMruList.Clear();

            this.mruComboBox.AutoCompleteCustomSource.Clear();

            this.mruComboBox.Items.Clear();

            this.mruComboBox.Text = string.Empty;
            this.mruComboBox.SelectionStart = 0;
            this.mruComboBox.SelectionLength = 0;

        }

        /// <summary>
        /// Handles the TextChanged event for the combo box.
        /// </summary>
        /// <param name="sender">
        /// The combo box whose text is being changed.
        /// </param>
        /// <param name="e">
        /// Data specific to the event.
        /// </param>
        private void mruComboBox_TextChanged(object sender, EventArgs e)
        {
            this.requestButton.Enabled = !string.IsNullOrEmpty(this.mruComboBox.Text.Trim());
        }
    }
}
