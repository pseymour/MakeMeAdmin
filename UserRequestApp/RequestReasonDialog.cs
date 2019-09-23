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
            
            if ((Settings.CannedReasons != null) && (Settings.CannedReasons.Length > 0))
            {
                this.cannedResponseComboBox.Items.AddRange(Settings.CannedReasons);
            }

            this.reasonTextBox.Enabled = Settings.AllowFreeFormReason;

            if (this.reasonTextBox.Enabled)
            {
                this.cannedResponseComboBox.Items.Add("Other");
            }

            /*
            if (this.cannedResponseComboBox.Items.Count > 0)
            {
                this.cannedResponseComboBox.SelectedIndex = 0;
            }
            */

            if (this.cannedResponseComboBox.Items.Count <= 1)
            {
                this.cannedResponseComboBox.Enabled = false;
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
                this.cannedResponseComboBox.Focus();
            }
        }

        private void ReasonTextBoxChangedHandler(object sender, EventArgs e)
        {
            this.cannedResponseComboBox.SelectedItem = "Other";
        }

        private void CancelButtonClickHandler(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
