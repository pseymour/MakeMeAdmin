// 
// Copyright © 2010-2019, Sinclair Community College
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
    using System.Drawing;
    using System.Reflection;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.Windows.Forms;

    /// <summary>
    /// This form allows the user to submit a request for administrator-level rights.
    /// </summary>
    internal partial class SubmitRequestForm : Form
    {
        /// <summary>
        /// Whether the user is a direct member of the Administrator's group.
        /// </summary>
        /// <remarks>
        /// This is stored in a variable because it is a rather expensive operation to check.
        /// </remarks>
        private bool userIsDirectAdmin = false;

        /// <summary>
        /// Whether the user is a member of the Administrator's group, either directly or
        /// via nested group memberships.
        /// </summary>
        /// <remarks>
        /// This is stored in a variable because it is a rather expensive operation to check.
        /// </remarks>
        private bool userIsAdmin = false;

        /// <summary>
        /// Whether the user had administrator rights the last time the check was performed.
        /// </summary>
        /// <remarks>
        /// This is used to determine when the user's administrator status changes.
        /// </remarks>
        private bool userWasAdminOnLastCheck = false;

        /// <summary>
        /// Timer to notify users when their administrator rights expire.
        /// </summary>
        private System.Timers.Timer notifyIconTimer;


        /// <summary>
        /// Initializes a new instance of the SubmitRequestForm class.
        /// </summary>
        public SubmitRequestForm()
        {
            this.InitializeComponent();

            this.Icon = Properties.Resources.SecurityLock;
            this.notifyIcon.Icon = Properties.Resources.SecurityLock;

            this.SetFormText();
            this.SetInstructions();

            // Configure the notification timer.
            this.notifyIconTimer = new System.Timers.Timer()
            {
                Interval = 5000
            };
            this.notifyIconTimer.AutoReset = true;
            this.notifyIconTimer.Elapsed += NotifyIconTimerElapsed;
        }


        /// <summary>
        /// Handles the Elapsed event for the notification area icon.
        /// </summary>
        /// <param name="sender">
        /// The timer whose Elapsed event is firing.
        /// </param>
        /// <param name="e">
        /// Data related to the event.
        /// </param>
        void NotifyIconTimerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            this.UpdateUserAdministratorStatus();

            if (this.userIsAdmin != this.userWasAdminOnLastCheck)
            { // The user's administrator status has changed.

                NetNamedPipeBinding namedPipeBinding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport);
                ChannelFactory<IAdminGroup> namedPipeFactory = new ChannelFactory<IAdminGroup>(namedPipeBinding, Settings.NamedPipeServiceBaseAddress);
                IAdminGroup namedPipeChannel = namedPipeFactory.CreateChannel();

                this.userWasAdminOnLastCheck = this.userIsAdmin;

                if ((!this.userIsAdmin) && (!namedPipeChannel.UserIsInList()))
                {
                    this.notifyIconTimer.Stop();
                    notifyIcon.ShowBalloonTip(5000, Properties.Resources.ApplicationName, string.Format(Properties.Resources.UIMessageRemovedFromGroup, LocalAdministratorGroup.LocalAdminGroupName), ToolTipIcon.Info);
                }

                namedPipeFactory.Close();
            }
        }


        /// <summary>
        /// Sets the form's text to the name of the application plus a partial version number.
        /// </summary>
        private void SetFormText()
        {
            System.Text.StringBuilder formText = new System.Text.StringBuilder();
            object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
            if (attributes.Length == 0)
            {
                formText.Append(Properties.Resources.ApplicationName);
            }
            else
            {
                formText.Append(((AssemblyProductAttribute)attributes[0]).Product);
            }
            formText.Append(' ');
            formText.Append(Assembly.GetExecutingAssembly().GetName().Version.ToString(3));

            this.Text = formText.ToString();
            this.notifyIcon.Text = formText.ToString();
        }


        /// <summary>
        /// Sets the instructions label to the value specified in Settings.
        /// </summary>
        private void SetInstructions()
        {
            this.instructionsLabel.Text = string.Join(Environment.NewLine, Settings.Instructions);

            // Calculate the preferred label height based on the content and maximum width
            int preferredHeight = TextRenderer.MeasureText(instructionsLabel.Text, instructionsLabel.Font, new Size(instructionsLabel.Width, int.MaxValue), TextFormatFlags.WordBreak).Height;

            // Set the calculated height for the label
            instructionsLabel.Height = preferredHeight;

            // Set the calculated height for the form
            this.Height = this.Height + preferredHeight;
        }


        /// <summary>
        /// This function handles the Click event for the Submit button.
        /// </summary>
        /// <param name="sender">
        /// The button being clicked.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        private void ClickSubmitButton(object sender, EventArgs e)
        {
            if (AuthenticationSuccessful && ReasonDialogSatisfied)
            {
                this.DisableButtons();
                this.appStatus.Text = string.Format(Properties.Resources.UIMessageAddingToGroup, LocalAdministratorGroup.LocalAdminGroupName);
                addUserBackgroundWorker.RunWorkerAsync();
            }
        }

        private bool AuthenticationSuccessful
        {
            get
            {
                bool authenticationSuccessful = true;
                if (Settings.RequireAuthenticationForPrivileges)
                {
                    authenticationSuccessful = false;

                    System.Net.NetworkCredential credentials = null;
                    int authenticationReturnCode = 0;
                    WindowsIdentity currentIdentity = WindowsIdentity.GetCurrent();
                    try
                    {
                        do
                        {
                            do
                            {
                                credentials = NativeMethods.GetCredentials(this.Handle, currentIdentity.Name, authenticationReturnCode);
                            } while ((null != credentials) && (string.Compare(credentials.UserName, currentIdentity.Name, true) != 0));

                            if (null != credentials)
                            {
                                authenticationReturnCode = NativeMethods.ValidateCredentials(credentials);
                            }
                        } while ((null != credentials) && (authenticationReturnCode != 0));
                    }
                    catch (ArgumentException excep)
                    {
                        MessageBox.Show(this, string.Format("{0}: {1}", excep.GetType().Name, excep.Message), Properties.Resources.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, 0);
                    }
                    catch (System.ComponentModel.Win32Exception excep)
                    {
                        MessageBox.Show(this, string.Format("{0}: {1}", excep.GetType().Name, excep.Message), Properties.Resources.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, 0);
                    }
                    catch (Exception excep)
                    {
                        MessageBox.Show(this, string.Format("{0}: {1}", excep.GetType().Name, excep.Message), Properties.Resources.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, 0);
                    }

                    authenticationSuccessful = (null != credentials);
                    authenticationSuccessful &= (authenticationReturnCode == 0);
                }
                return authenticationSuccessful;
            }
        }


        private bool ReasonDialogSatisfied
        {
            get
            {
                bool dialogSatisfied = false;

                switch (Settings.PromptForReason)
                {
                    case ReasonPrompt.None:
                        // No reason dialog box required.
                        dialogSatisfied = true;
                        break;

                    case ReasonPrompt.Optional:

                        // The reason dialog is optional, so rights are always allowed.
                        dialogSatisfied = true;

                        if ((Settings.AllowFreeFormReason) || ((Settings.CannedReasons != null) && (Settings.CannedReasons.Length > 0)))
                        {
                            using (RequestReasonDialog reasonDialog = new RequestReasonDialog())
                            {
                                switch (reasonDialog.ShowDialog(this))
                                {
                                    case DialogResult.Cancel:
                                        // User did not provide a reason, but is not obligated to do so.
                                        break;
                                    case DialogResult.OK:
                                        ApplicationLog.WriteEvent(string.Format(Properties.Resources.ReasonProvidedByUser, reasonDialog.Reason), EventID.ReasonProvidedByUser, System.Diagnostics.EventLogEntryType.Information);
                                        break;
                                    default:
                                        // Not sure how we got to this point, because it should never happen.
                                        break;
                                }
                            }
                        }
                        else
                        {
                            ApplicationLog.WriteEvent(Properties.Resources.ReasonDialogEmpty, EventID.ReasonDialogEmpty, System.Diagnostics.EventLogEntryType.Warning);
                        }

                        break;

                    case ReasonPrompt.Required:

                        if ((Settings.AllowFreeFormReason) || ((Settings.CannedReasons != null) && (Settings.CannedReasons.Length > 0)))
                        {
                            using (RequestReasonDialog reasonDialog = new RequestReasonDialog())
                            {
                                switch (reasonDialog.ShowDialog(this))
                                {
                                    case DialogResult.Cancel:
                                        dialogSatisfied = false;
                                        MessageBox.Show(this, Properties.Resources.MandatoryReasonNotProvided, Properties.Resources.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                                        break;
                                    case DialogResult.OK:
                                        if (string.Compare(reasonDialog.Reason, string.Format("{0}: ", Properties.Resources.OtherReason), true) == 0)
                                        { // User didn't really provide a reason. The string is blank.
                                            dialogSatisfied = false;
                                            MessageBox.Show(this, Properties.Resources.MandatoryReasonNotProvided, Properties.Resources.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                                        }
                                        else
                                        {
                                            dialogSatisfied = true;
                                            ApplicationLog.WriteEvent(string.Format(Properties.Resources.ReasonProvidedByUser, reasonDialog.Reason), EventID.ReasonProvidedByUser, System.Diagnostics.EventLogEntryType.Information);
                                        }
                                        break;
                                    default:
                                        // Not sure how we got to this point, because it should never happen.
                                        // Better to be safe than sorry, so no admin rights.
                                        dialogSatisfied = false;
                                        break;
                                }
                            }
                        }
                        else
                        {
                            MessageBox.Show(this, Properties.Resources.ReasonDialogBoxPrevented, Properties.Resources.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
                            dialogSatisfied = false;
                        }

                        break;

                    default:
                        // TODO: i18n
                        ApplicationLog.WriteEvent(string.Format("Unexpected value for the reason prompt setting: {0:N0}", ((int)(Settings.PromptForReason))), EventID.DebugMessage, System.Diagnostics.EventLogEntryType.Warning);
                        dialogSatisfied = true;
                        break;
                }

                return dialogSatisfied;
            }
        }

        /// <summary>
        /// This function runs when RunWorkerAsync() is called by the "grant admin rights" BackgroundWorker object.
        /// </summary>
        /// <param name="sender">
        /// The BackgroundWorker that triggered the event.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        private void addUserBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport);
            ChannelFactory<IAdminGroup> namedPipeFactory = new ChannelFactory<IAdminGroup>(binding, Settings.NamedPipeServiceBaseAddress);
            IAdminGroup channel = namedPipeFactory.CreateChannel();

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

            namedPipeFactory.Close();
        }


        /// <summary>
        /// Occurs when the background operation has completed, has been canceled, or has raised an exception.
        /// </summary>
        /// <param name="sender">
        /// The "grant admin rights" BackgroundWorker object, which triggered the event.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        private void addUserBackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                System.Text.StringBuilder message = new System.Text.StringBuilder(Properties.Resources.UIMessageErrorWhileAdding);
                message.Append(System.Environment.NewLine);
                message.Append(Properties.Resources.ErrorMessage);
                message.Append(": ");
                message.Append(e.Error.Message);

                MessageBox.Show(this, message.ToString(), Properties.Resources.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, 0);
            }

            this.UpdateUserAdministratorStatus();

            if (this.userIsAdmin)
            { // Display a message that the user now has administrator rights.
                this.appStatus.Text = Properties.Resources.ApplicationIsReady;
                this.userWasAdminOnLastCheck = this.userIsAdmin;
                this.notifyIconTimer.Start();
                this.notifyIcon.Visible = true;
                this.Visible = false;
                this.ShowInTaskbar = false;
                notifyIcon.ShowBalloonTip(5000, Properties.Resources.ApplicationName, string.Format(Properties.Resources.UIMessageAddedToGroup, LocalAdministratorGroup.LocalAdminGroupName), ToolTipIcon.Info);
            }
            else if (!buttonStateWorker.IsBusy)
            {
                buttonStateWorker.RunWorkerAsync();
            }
        }


        /// <summary>
        /// Handles the Click event for the rights removal button.
        /// </summary>
        /// <param name="sender">
        /// The button being clicked.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        private void ClickRemoveRightsButton(object sender, EventArgs e)
        {
            this.DisableButtons();
            this.appStatus.Text = string.Format(Properties.Resources.UIMessageRemovingFromGroup, LocalAdministratorGroup.LocalAdminGroupName);
            removeUserBackgroundWorker.RunWorkerAsync();
        }


        /// <summary>
        /// This function runs when RunWorkerAsync() is called by the rights removal BackgroundWorker object.
        /// </summary>
        /// <param name="sender">
        /// The BackgroundWorker that triggered the event.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        private void removeUserBackgroundWorker_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport);
            ChannelFactory<IAdminGroup> namedPipeFactory = new ChannelFactory<IAdminGroup>(binding, Settings.NamedPipeServiceBaseAddress);
            IAdminGroup channel = namedPipeFactory.CreateChannel();
            channel.RemoveUserFromAdministratorsGroup(RemovalReason.UserRequest);
            namedPipeFactory.Close();
        }


        /// <summary>
        /// Occurs when the background operation has completed, has been canceled, or has raised an exception.
        /// </summary>
        /// <param name="sender">
        /// The rights removal BackgroundWorker object, which triggered the event.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        private void removeUserBackgroundWorker_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                System.Text.StringBuilder message = new System.Text.StringBuilder(Properties.Resources.UIMessageErrorWhileRemoving);
                message.Append(System.Environment.NewLine);
                message.Append(Properties.Resources.UIMessageEnsureServiceRunning);
                message.Append(System.Environment.NewLine);
                message.Append(Properties.Resources.ErrorMessage);
                message.Append(": ");
                message.Append(e.Error.Message);
                message.Append(System.Environment.NewLine);
                message.Append(Properties.Resources.StackTrace);
                message.Append(": ");
                message.Append(e.Error.StackTrace);
                message.Append(System.Environment.NewLine);

                if (e.Error.InnerException != null)
                {
                    message.Append(System.Environment.NewLine);
                    message.Append(e.Error.InnerException.Message);
                    if (e.Error.InnerException.InnerException != null)
                    { // This is quite ridiculous.
                        message.Append(System.Environment.NewLine);
                        message.Append(e.Error.InnerException.InnerException.Message);
                    }
                }

                MessageBox.Show(this, message.ToString(), Properties.Resources.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1, 0);
            }

            if (!buttonStateWorker.IsBusy)
            {
                buttonStateWorker.RunWorkerAsync();
            }
        }

        
        /// <summary>
        /// Disables the add and remove buttons.
        /// </summary>
        private void DisableButtons()
        {
            this.addMeButton.Enabled = false;
            this.removeMeButton.Enabled = false;
        }


        /// <summary>
        /// Handles the Load event for the form.
        /// </summary>
        /// <param name="sender">
        /// The form being loaded.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        private void FormLoad(object sender, EventArgs e)
        {
            this.DisableButtons();
            this.appStatus.Text = Properties.Resources.CheckingAdminStatus;
            buttonStateWorker.RunWorkerAsync();
        }


        /// <summary>
        /// Updates the variables which store the user's administrator status.
        /// </summary>
        private void UpdateUserAdministratorStatus()
        {
            this.userIsAdmin = LocalAdministratorGroup.IsMemberOfAdministrators(WindowsIdentity.GetCurrent());
            this.userIsDirectAdmin = LocalAdministratorGroup.IsMemberOfAdministratorsDirectly(WindowsIdentity.GetCurrent());
        }


        /// <summary>
        /// This function runs when RunWorkerAsync() is called by the button state BackgroundWorker object.
        /// </summary>
        /// <param name="sender">
        /// The BackgroundWorker that triggered the event.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        private void DoButtonStateWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
            this.UpdateUserAdministratorStatus();
        }


        /// <summary>
        /// Occurs when the background operation has completed, has been canceled, or has raised an exception.
        /// </summary>
        /// <param name="sender">
        /// The button state BackgroundWorker object, which triggered the event.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        private void ButtonStateWorkCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            AdminGroupManipulator adminGroupManipulator = new AdminGroupManipulator();
            bool userIsAuthorizedLocally = adminGroupManipulator.UserIsAuthorized(WindowsIdentity.GetCurrent(), Settings.LocalAllowedEntities, Settings.LocalDeniedEntities);

            /*
            NetNamedPipeBinding binding = new NetNamedPipeBinding(NetNamedPipeSecurityMode.Transport);
            ChannelFactory<IAdminGroup> namedPipeFactory = new ChannelFactory<IAdminGroup>(binding, Settings.NamedPipeServiceBaseAddress);
            IAdminGroup channel = namedPipeFactory.CreateChannel();
            bool userIsAuthorizedLocally = false;
            try
            {
                userIsAuthorizedLocally = channel.UserIsAuthorized(WindowsIdentity.GetCurrent(), Settings.LocalAllowedEntities, Settings.LocalDeniedEntities);
                namedPipeFactory.Close();
            }
            catch (EndpointNotFoundException exception)
            {
                System.Text.StringBuilder message = new System.Text.StringBuilder(exception.Message);
                if (null != exception.InnerException)
                {
                    message.Append(Environment.NewLine);
                    message.Append(string.Format("{0}:", Properties.Resources.InnerException));
                    message.Append(Environment.NewLine);
                    message.Append(exception.InnerException.Message);
                }
                MessageBox.Show(this, exception.Message, Properties.Resources.ApplicationName, MessageBoxButtons.OK, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button1);
            }
            catch (CommunicationObjectFaultedException)
            {
                // This typically happens when trying to dispose of the ChannelFactory<> object.
            }
            */

            // Enable the "grant admin rights" button, if the user is not already
            // an administrator and is authorized to obtain those rights.
            this.addMeButton.Enabled = !this.userIsAdmin && userIsAuthorizedLocally;
            if (addMeButton.Enabled)
            {
                addMeButton.Text = Properties.Resources.GrantRightsButtonText;
            }
            else if (this.userIsAdmin)
            {
                addMeButton.Text = Properties.Resources.UIMessageAlreadyHaveRights;
            }
            else if (!userIsAuthorizedLocally)
            {
                addMeButton.Text = Properties.Resources.UIMessageUnauthorized;
            }

            // Enable the rights removal button if the user is directly a
            // member of the administrators group.
            this.removeMeButton.Enabled = this.userIsDirectAdmin;

            this.appStatus.Text = Properties.Resources.ApplicationIsReady;

            if (this.addMeButton.Enabled)
            {
                this.addMeButton.Focus();
            }
            else if (this.removeMeButton.Enabled)
            {
                this.removeMeButton.Focus();
            }
        }


        /// <summary>
        /// Handles the MouseDoubleClick event for the notification area icon.
        /// </summary>
        /// <param name="sender">
        /// The notification icon that is being double-clicked.
        /// </param>
        /// <param name="e">
        /// Data specific to this event.
        /// </param>
        private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            this.ShowInTaskbar = true;
            this.Visible = true;
        }


        /// <summary>
        /// Handles the VisibleChanged event for the form.
        /// </summary>
        /// <param name="sender">
        /// The form whose visibility has changed.
        /// </param>
        /// <param name="e">
        /// Data specific to the event.
        /// </param>
        private void SubmitRequestForm_VisibleChanged(object sender, EventArgs e)
        {
            // Update the enabled/disabled state of the buttons, if the worker
            // is not already doing so.
            if (!buttonStateWorker.IsBusy)
            {
                buttonStateWorker.RunWorkerAsync();
            }
        }


        /// <summary>
        /// Handles the BalloonTipClosed event for the notification icon.
        /// </summary>
        /// <param name="sender">
        /// The notification icon whose balloon tip was closed.
        /// </param>
        /// <param name="e">
        /// Data specific to the event.
        /// </param>
        private void notifyIcon_BalloonTipClosed(object sender, EventArgs e)
        {
            this.UpdateFormAfterBalloonTip();
        }

        private void notifyIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            this.UpdateFormAfterBalloonTip();
        }

        private void UpdateFormAfterBalloonTip()
        {
            if (!this.userIsAdmin)
            {
                if (Settings.CloseApplicationOnExpiration)
                {
                    this.Close();
                }
                else
                { // Do not close the form.

                    // Update the enabled/disabled state of the buttons, if the worker is not already doing so.
                    if (!buttonStateWorker.IsBusy) { buttonStateWorker.RunWorkerAsync(); }

                    this.notifyIcon.Visible = false;
                    this.Visible = true;
                    this.ShowInTaskbar = true;
                }
            }            
        }
    }
}
