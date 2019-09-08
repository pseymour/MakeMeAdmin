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
    /// <summary>
    /// This form allows the user to submit a request for administrator-level rights.
    /// </summary>
    internal partial class SubmitRequestForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        
        /// <summary>
        /// The "add me" button.
        /// </summary>
        private System.Windows.Forms.Button addMeButton;

        /// <summary>
        /// A tooltip to explain other controls.
        /// </summary>
        private System.Windows.Forms.ToolTip toolTip;

        /// <summary>
        /// The "remove me" button.
        /// </summary>
        private System.Windows.Forms.Button removeMeButton;

        /// <summary>
        /// A status bar strip.
        /// </summary>
        private System.Windows.Forms.StatusStrip statusStrip1;

        /// <summary>
        /// A label to display application status in the strip.
        /// </summary>
        private System.Windows.Forms.ToolStripStatusLabel appStatus;

        /// <summary>
        /// A background worker to control the state of the various buttons.
        /// </summary>
        private System.ComponentModel.BackgroundWorker buttonStateWorker;

        /// <summary>
        /// A background workr to add the current user to the Administrators group.
        /// </summary>
        private System.ComponentModel.BackgroundWorker addUserBackgroundWorker;

        /// <summary>
        /// A background workr to remove the current user from the Administrators group.
        /// </summary>
        private System.ComponentModel.BackgroundWorker removeUserBackgroundWorker;

        /// <summary>
        /// notification area icon
        /// </summary>
        private System.Windows.Forms.NotifyIcon notifyIcon;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (this.components != null))
            {
                this.components.Dispose();
            }
            
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubmitRequestForm));
            this.addMeButton = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.removeMeButton = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.appStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.buttonStateWorker = new System.ComponentModel.BackgroundWorker();
            this.addUserBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.removeUserBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // addMeButton
            // 
            resources.ApplyResources(this.addMeButton, "addMeButton");
            this.addMeButton.Name = "addMeButton";
            this.toolTip.SetToolTip(this.addMeButton, resources.GetString("addMeButton.ToolTip"));
            this.addMeButton.UseVisualStyleBackColor = true;
            this.addMeButton.Click += new System.EventHandler(this.ClickSubmitButton);
            // 
            // removeMeButton
            // 
            resources.ApplyResources(this.removeMeButton, "removeMeButton");
            this.removeMeButton.Name = "removeMeButton";
            this.toolTip.SetToolTip(this.removeMeButton, resources.GetString("removeMeButton.ToolTip"));
            this.removeMeButton.UseVisualStyleBackColor = true;
            this.removeMeButton.Click += new System.EventHandler(this.ClickRemoveRightsButton);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.appStatus});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // appStatus
            // 
            this.appStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.appStatus.Name = "appStatus";
            resources.ApplyResources(this.appStatus, "appStatus");
            this.appStatus.Spring = true;
            // 
            // buttonStateWorker
            // 
            this.buttonStateWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DoButtonStateWork);
            this.buttonStateWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ButtonStateWorkCompleted);
            // 
            // addUserBackgroundWorker
            // 
            this.addUserBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.addUserBackgroundWorker_DoWork);
            this.addUserBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.addUserBackgroundWorker_RunWorkerCompleted);
            // 
            // removeUserBackgroundWorker
            // 
            this.removeUserBackgroundWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.removeUserBackgroundWorker_DoWork);
            this.removeUserBackgroundWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.removeUserBackgroundWorker_RunWorkerCompleted);
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipIcon = System.Windows.Forms.ToolTipIcon.Info;
            resources.ApplyResources(this.notifyIcon, "notifyIcon");
            this.notifyIcon.BalloonTipClosed += new System.EventHandler(this.notifyIcon_BalloonTipClosed);
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // SubmitRequestForm
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.removeMeButton);
            this.Controls.Add(this.addMeButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SubmitRequestForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Load += new System.EventHandler(this.FormLoad);
            this.VisibleChanged += new System.EventHandler(this.SubmitRequestForm_VisibleChanged);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

    }
}

