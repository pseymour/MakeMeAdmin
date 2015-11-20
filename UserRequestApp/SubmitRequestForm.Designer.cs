// <copyright file="SubmitRequestForm.Designer.cs" company="Sinclair Community College">
// Copyright (c) Sinclair Community College. All rights reserved.
// </copyright>

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
        /// The exit button.
        /// </summary>
        private System.Windows.Forms.Button exitButton;

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
            this.addMeButton = new System.Windows.Forms.Button();
            this.exitButton = new System.Windows.Forms.Button();
            this.toolTip = new System.Windows.Forms.ToolTip(this.components);
            this.removeMeButton = new System.Windows.Forms.Button();
            this.buttonStateWorker = new System.ComponentModel.BackgroundWorker();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.appStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.addUserBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.removeUserBackgroundWorker = new System.ComponentModel.BackgroundWorker();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // addMeButton
            // 
            this.addMeButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.addMeButton.Location = new System.Drawing.Point(12, 12);
            this.addMeButton.Name = "addMeButton";
            this.addMeButton.Size = new System.Drawing.Size(232, 30);
            this.addMeButton.TabIndex = 0;
            this.addMeButton.Text = "&Grant Me Administrator Rights";
            this.toolTip.SetToolTip(this.addMeButton, "Add the current Windows user to the Administrators group.");
            this.addMeButton.UseVisualStyleBackColor = true;
            this.addMeButton.Click += new System.EventHandler(this.ClickSubmitButton);
            // 
            // exitButton
            // 
            this.exitButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.exitButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.exitButton.Location = new System.Drawing.Point(12, 84);
            this.exitButton.Name = "exitButton";
            this.exitButton.Size = new System.Drawing.Size(232, 30);
            this.exitButton.TabIndex = 2;
            this.exitButton.Text = "E&xit";
            this.toolTip.SetToolTip(this.exitButton, "Exit the application.");
            this.exitButton.UseVisualStyleBackColor = true;
            this.exitButton.Click += new System.EventHandler(this.ClickExitButton);
            // 
            // removeMeButton
            // 
            this.removeMeButton.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.removeMeButton.Location = new System.Drawing.Point(12, 48);
            this.removeMeButton.Name = "removeMeButton";
            this.removeMeButton.Size = new System.Drawing.Size(232, 30);
            this.removeMeButton.TabIndex = 1;
            this.removeMeButton.Text = "&Remove My Administrator Rights";
            this.toolTip.SetToolTip(this.removeMeButton, "Remove the current Windows user from the Administrators group.");
            this.removeMeButton.UseVisualStyleBackColor = true;
            this.removeMeButton.Click += new System.EventHandler(this.ClickRemoveRightsButton);
            // 
            // buttonStateWorker
            // 
            this.buttonStateWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.DoButtonStateWork);
            this.buttonStateWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.ButtonStateWorkCompleted);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.appStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 125);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(256, 22);
            this.statusStrip1.TabIndex = 3;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // appStatus
            // 
            this.appStatus.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.appStatus.Name = "appStatus";
            this.appStatus.Size = new System.Drawing.Size(241, 17);
            this.appStatus.Spring = true;
            this.appStatus.Text = "Ready.";
            this.appStatus.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
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
            this.notifyIcon.Text = "Make Me Admin";
            this.notifyIcon.BalloonTipClosed += new System.EventHandler(this.notifyIcon_BalloonTipClosed);
            this.notifyIcon.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseDoubleClick);
            // 
            // SubmitRequestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.exitButton;
            this.ClientSize = new System.Drawing.Size(256, 147);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.removeMeButton);
            this.Controls.Add(this.exitButton);
            this.Controls.Add(this.addMeButton);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SubmitRequestForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Make Me Admin";
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

