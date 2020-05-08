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
    using System.Windows.Forms;

    public partial class RequestReasonDialog : Form
    {
        public RequestReasonDialog()
        {
            InitializeComponent();

            this.Icon = Properties.Resources.SecurityLock;

            if ((Settings.CannedReasons != null) && (Settings.CannedReasons.Length > 0))
            {
                this.responseComboBox.Items.AddRange(Settings.CannedReasons);
            }

            this.reasonTextBox.Enabled = Settings.AllowFreeFormReason;

            if (this.reasonTextBox.Enabled)
            {
                this.responseComboBox.Items.Add(Properties.Resources.OtherReason);
            }
            
            if (this.responseComboBox.Items.Count > 0)
            {
                this.responseComboBox.SelectedIndex = 0;
            }            

            if (this.responseComboBox.Items.Count <= 1)
            {
                this.responseComboBox.Enabled = false;
            }
        }

        private void FormLoadHandler(object sender, EventArgs e)
        {
            if (this.reasonTextBox.Enabled && ((Settings.CannedReasons == null) || Settings.CannedReasons.Length == 0))
            {
                this.reasonTextBox.Focus();
            }
            else
            {
                this.responseComboBox.Focus();
            }
            SetOKButtonState();
        }

        private void ReasonTextBoxChangedHandler(object sender, EventArgs e)
        {
            this.responseComboBox.SelectedItem = Properties.Resources.OtherReason;
            SetOKButtonState();
        }

        private void ResponseComboBoxSelectionChangeCommitted(object sender, EventArgs e)
        {
            SetOKButtonState();
        }

        private void SetOKButtonState()
        {
            //if (Settings.PromptForReason == ReasonPrompt.Required)
            //{
                if (this.responseComboBox.SelectedIndex >= 0)
                { // Something is selected in the combo box.
                    string selectedItemText = ((string)this.responseComboBox.SelectedItem).Trim();
                    if (string.Compare(selectedItemText, Properties.Resources.OtherReason, true) == 0)
                    { // The "Other" item is selected in the combo box.
                        // Enable the OK button if there is something in the text box.
                        this.okButton.Enabled = this.reasonTextBox.Text.Trim().Length > 0;
                    }
                    else
                    {
                        this.okButton.Enabled = true;
                    }
                }
                else
                { // Nothing is selected in the combo box.
                    // Enable the OK button if there is something in the text box.
                    this.okButton.Enabled = this.reasonTextBox.Text.Trim().Length > 0;
                }
            //}
        }

        private void CancelButtonClickHandler(object sender, EventArgs e)
        {
            this.Close();
        }

        public string Reason
        {
            get
            {
                if (this.responseComboBox.SelectedIndex >= 0)
                { // Something is selected in the combo box.
                    string selectedItemText = ((string)this.responseComboBox.SelectedItem).Trim();
                    if (string.Compare(selectedItemText, Properties.Resources.OtherReason, true) == 0)
                    {
                        return string.Format("{0}: {1}", Properties.Resources.OtherReason, this.reasonTextBox.Text.Trim());
                    }
                    else
                    {
                        return selectedItemText;
                    }
                }
                else
                { // Nothing is selected in the combo box. Return the contents of the text box, even if empty.
                    return this.reasonTextBox.Text.Trim();
                }
            }
        }

    }
}
