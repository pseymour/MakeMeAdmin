namespace SinclairCC.MakeMeAdmin
{
    partial class SubmitRequestForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
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
            this.hostNameLabel = new System.Windows.Forms.Label();
            this.mruComboBox = new System.Windows.Forms.ComboBox();
            this.requestButton = new System.Windows.Forms.Button();
            this.clearHistoryButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // hostNameLabel
            // 
            this.hostNameLabel.AutoSize = true;
            this.hostNameLabel.Location = new System.Drawing.Point(18, 14);
            this.hostNameLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.hostNameLabel.Name = "hostNameLabel";
            this.hostNameLabel.Size = new System.Drawing.Size(89, 20);
            this.hostNameLabel.TabIndex = 0;
            this.hostNameLabel.Text = "Host Name";
            // 
            // mruComboBox
            // 
            this.mruComboBox.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.Suggest;
            this.mruComboBox.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.CustomSource;
            this.mruComboBox.FormattingEnabled = true;
            this.mruComboBox.Location = new System.Drawing.Point(18, 38);
            this.mruComboBox.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.mruComboBox.Name = "mruComboBox";
            this.mruComboBox.Size = new System.Drawing.Size(439, 28);
            this.mruComboBox.TabIndex = 1;
            this.mruComboBox.TextChanged += new System.EventHandler(this.mruComboBox_TextChanged);
            // 
            // requestButton
            // 
            this.requestButton.Location = new System.Drawing.Point(18, 80);
            this.requestButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.requestButton.Name = "requestButton";
            this.requestButton.Size = new System.Drawing.Size(208, 85);
            this.requestButton.TabIndex = 2;
            this.requestButton.Text = "&Request Admin Rights";
            this.requestButton.UseVisualStyleBackColor = true;
            this.requestButton.Click += new System.EventHandler(this.requestButton_Click);
            // 
            // clearHistoryButton
            // 
            this.clearHistoryButton.Location = new System.Drawing.Point(250, 80);
            this.clearHistoryButton.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.clearHistoryButton.Name = "clearHistoryButton";
            this.clearHistoryButton.Size = new System.Drawing.Size(208, 85);
            this.clearHistoryButton.TabIndex = 3;
            this.clearHistoryButton.Text = "&Clear History";
            this.clearHistoryButton.UseVisualStyleBackColor = true;
            this.clearHistoryButton.Click += new System.EventHandler(this.clearHistoryButton_Click);
            // 
            // SubmitRequestForm
            // 
            this.AcceptButton = this.requestButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(477, 179);
            this.Controls.Add(this.clearHistoryButton);
            this.Controls.Add(this.requestButton);
            this.Controls.Add(this.mruComboBox);
            this.Controls.Add(this.hostNameLabel);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SubmitRequestForm";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Make Me Admin Remote";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.SubmitRequestForm_FormClosed);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label hostNameLabel;
        private System.Windows.Forms.ComboBox mruComboBox;
        private System.Windows.Forms.Button requestButton;
        private System.Windows.Forms.Button clearHistoryButton;
    }
}

