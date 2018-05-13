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
    using System.Reflection;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.Threading.Tasks;
    using System.Windows.Forms;
    using SinclairCC.Collections.MruList;

    public partial class SubmitRequestForm : Form
    {
        private MruItemCollection<string> hostNameMruList;

        private delegate void AddHostNameDelegate(string hostName);
        private AddHostNameDelegate addHostNameToMruDelegate;

        public SubmitRequestForm()
        {
            InitializeComponent();

            this.addHostNameToMruDelegate = new AddHostNameDelegate(this.AddHostName);
            this.requestButton.Enabled = !string.IsNullOrEmpty(this.mruComboBox.Text.Trim());

            this.hostNameMruList = new MruItemCollection<string>(10);
            string[] settingsMruList = RemoteUI.Settings.HostNameMru;
            if (settingsMruList != null)
            {
                for (int i = settingsMruList.Length - 1; i >= 0; i--)
                {
                    AddHostName(settingsMruList[i]);
                }
            }

            this.Icon = Properties.Resources.SecurityLock;

            this.SetFormText();
        }

        private void SetFormText()
        {
            System.Text.StringBuilder formText = new System.Text.StringBuilder();
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            //if (attributes.Length == 0)
            //{
            // TODO: i18n.
            formText.Append("Make Me Admin Remote");
            //}
            //else
            //{
            //    formText.Append(((AssemblyProductAttribute)attributes[0]).Product);
            //}
            formText.Append(' ');
            formText.Append(Assembly.GetExecutingAssembly().GetName().Version.ToString(3));

            this.Text = formText.ToString();
        }

        private void AddHostName(string hostName)
        {
            this.hostNameMruList.Add(hostName);

            this.mruComboBox.AutoCompleteCustomSource.Clear();
            this.mruComboBox.AutoCompleteCustomSource.AddRange(this.hostNameMruList.ToArray());

            this.mruComboBox.Items.Clear();
            this.mruComboBox.Items.AddRange(this.hostNameMruList.ToArray());

            this.mruComboBox.SelectionStart = 0;
            this.mruComboBox.SelectionLength = hostName.Length;
        }

        private void RequestAdminRights(string hostName)
        {
            string remoteHostAddress = string.Format("net.tcp://{0}/MakeMeAdmin/Service", hostName);

            NetTcpBinding binding = new NetTcpBinding(SecurityMode.Transport);
            ChannelFactory<IAdminGroup> namedPipeFactory = new ChannelFactory<IAdminGroup>(binding, remoteHostAddress);
            IAdminGroup channel = namedPipeFactory.CreateChannel();

            try
            {
                channel.AddPrincipalToAdministratorsGroup();
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

        private void requestButton_Click(object sender, EventArgs e)
        {
            string hostName = this.mruComboBox.Text.Trim();
            this.mruComboBox.Text = hostName;

            Task.Factory.StartNew(() => this.mruComboBox.Invoke(this.addHostNameToMruDelegate, hostName));

            Task requestRightsTask = Task.Factory
                .StartNew(() => this.RequestAdminRights(hostName))
                .ContinueWith(resultTask =>
                {
                    // TODO: i18n.
                    if (resultTask.IsFaulted)
                    {
                        System.Text.StringBuilder messageBoxText = new System.Text.StringBuilder("An error occurred while requesting administrator rights on ");
                        messageBoxText.Append(hostName);
                        messageBoxText.Append(".");
                        messageBoxText.Append(System.Environment.NewLine);
                        messageBoxText.Append("Message: ");
                        messageBoxText.Append(System.Environment.NewLine);
                        messageBoxText.Append("{0}");
                        resultTask.Exception.Flatten().Handle(excep => { MessageBox.Show(string.Format(messageBoxText.ToString(), excep.Message), "Make Me Admin Remote", MessageBoxButtons.OK, MessageBoxIcon.Error); return true; });
                    }
                    else
                    {
                        MessageBox.Show(string.Format("Administrator rights requested on {0}.", hostName), "Make Me Admin Remote", MessageBoxButtons.OK);
                    }                    
                });
        }

        private void SubmitRequestForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            RemoteUI.Settings.HostNameMru = this.hostNameMruList.ToArray();
        }

        private void clearHistoryButton_Click(object sender, EventArgs e)
        {
            this.hostNameMruList.Clear();

            this.mruComboBox.AutoCompleteCustomSource.Clear();

            this.mruComboBox.Items.Clear();

            this.mruComboBox.Text = string.Empty;
            this.mruComboBox.SelectionStart = 0;
            this.mruComboBox.SelectionLength = 0;

        }

        private void mruComboBox_TextChanged(object sender, EventArgs e)
        {
            this.requestButton.Enabled = !string.IsNullOrEmpty(this.mruComboBox.Text.Trim());
        }
    }
}
