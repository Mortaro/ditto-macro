namespace Ditto
{
    partial class Launcher
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Launcher));
            this.label1 = new System.Windows.Forms.Label();
            this.WebsiteLink = new System.Windows.Forms.LinkLabel();
            this.LaunchClientButton = new System.Windows.Forms.Button();
            this.UpdateTitleLabel = new System.Windows.Forms.Label();
            this.UpdateDescriptionLabel = new System.Windows.Forms.Label();
            this.CloseButton = new System.Windows.Forms.Button();
            this.NewMacroButton = new System.Windows.Forms.Button();
            this.LoadMacroButton = new System.Windows.Forms.Button();
            this.LoadMacroDialog = new System.Windows.Forms.OpenFileDialog();
            this.SaveMacroDialog = new System.Windows.Forms.SaveFileDialog();
            this.UpdateLinkLabel = new System.Windows.Forms.LinkLabel();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.Black;
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.label1.Location = new System.Drawing.Point(51, 412);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(272, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Ditto Online is a project from the creator of Ditto Macro";
            // 
            // WebsiteLink
            // 
            this.WebsiteLink.ActiveLinkColor = System.Drawing.Color.DarkRed;
            this.WebsiteLink.AutoSize = true;
            this.WebsiteLink.BackColor = System.Drawing.SystemColors.ActiveCaptionText;
            this.WebsiteLink.LinkBehavior = System.Windows.Forms.LinkBehavior.AlwaysUnderline;
            this.WebsiteLink.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.WebsiteLink.Location = new System.Drawing.Point(77, 397);
            this.WebsiteLink.Name = "WebsiteLink";
            this.WebsiteLink.Size = new System.Drawing.Size(216, 13);
            this.WebsiteLink.TabIndex = 1;
            this.WebsiteLink.TabStop = true;
            this.WebsiteLink.Text = "visit www.dittokal.com for more information";
            this.WebsiteLink.VisitedLinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.WebsiteLink.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.WebsiteLink_LinkClicked);
            // 
            // LaunchClientButton
            // 
            this.LaunchClientButton.BackColor = System.Drawing.Color.Transparent;
            this.LaunchClientButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LaunchClientButton.Enabled = false;
            this.LaunchClientButton.FlatAppearance.BorderSize = 0;
            this.LaunchClientButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.LaunchClientButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.LaunchClientButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LaunchClientButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LaunchClientButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.LaunchClientButton.Location = new System.Drawing.Point(38, 358);
            this.LaunchClientButton.Name = "LaunchClientButton";
            this.LaunchClientButton.Size = new System.Drawing.Size(92, 26);
            this.LaunchClientButton.TabIndex = 2;
            this.LaunchClientButton.Text = "Launch Client";
            this.LaunchClientButton.UseVisualStyleBackColor = false;
            this.LaunchClientButton.Visible = false;
            this.LaunchClientButton.Click += new System.EventHandler(this.LaunchClientButton_Click);
            // 
            // UpdateTitleLabel
            // 
            this.UpdateTitleLabel.BackColor = System.Drawing.Color.Transparent;
            this.UpdateTitleLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.UpdateTitleLabel.Location = new System.Drawing.Point(30, 270);
            this.UpdateTitleLabel.Name = "UpdateTitleLabel";
            this.UpdateTitleLabel.Size = new System.Drawing.Size(300, 13);
            this.UpdateTitleLabel.TabIndex = 3;
            this.UpdateTitleLabel.Text = "Kalonline is Updating";
            this.UpdateTitleLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // UpdateDescriptionLabel
            // 
            this.UpdateDescriptionLabel.BackColor = System.Drawing.Color.Transparent;
            this.UpdateDescriptionLabel.ForeColor = System.Drawing.SystemColors.Control;
            this.UpdateDescriptionLabel.Location = new System.Drawing.Point(30, 287);
            this.UpdateDescriptionLabel.Name = "UpdateDescriptionLabel";
            this.UpdateDescriptionLabel.Size = new System.Drawing.Size(300, 13);
            this.UpdateDescriptionLabel.TabIndex = 4;
            this.UpdateDescriptionLabel.Text = "checking for updates...";
            this.UpdateDescriptionLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // CloseButton
            // 
            this.CloseButton.BackColor = System.Drawing.Color.Transparent;
            this.CloseButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CloseButton.FlatAppearance.BorderSize = 0;
            this.CloseButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.CloseButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.CloseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CloseButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CloseButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.CloseButton.Location = new System.Drawing.Point(338, -3);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(20, 20);
            this.CloseButton.TabIndex = 6;
            this.CloseButton.Text = "x";
            this.CloseButton.UseVisualStyleBackColor = false;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // NewMacroButton
            // 
            this.NewMacroButton.BackColor = System.Drawing.Color.Transparent;
            this.NewMacroButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.NewMacroButton.FlatAppearance.BorderSize = 0;
            this.NewMacroButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.NewMacroButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.NewMacroButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.NewMacroButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.NewMacroButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.NewMacroButton.Location = new System.Drawing.Point(135, 358);
            this.NewMacroButton.Name = "NewMacroButton";
            this.NewMacroButton.Size = new System.Drawing.Size(92, 26);
            this.NewMacroButton.TabIndex = 7;
            this.NewMacroButton.Text = "New Macro";
            this.NewMacroButton.UseVisualStyleBackColor = false;
            this.NewMacroButton.Click += new System.EventHandler(this.NewMacroButton_Click);
            // 
            // LoadMacroButton
            // 
            this.LoadMacroButton.BackColor = System.Drawing.Color.Transparent;
            this.LoadMacroButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.LoadMacroButton.FlatAppearance.BorderSize = 0;
            this.LoadMacroButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.LoadMacroButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.LoadMacroButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.LoadMacroButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.LoadMacroButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.LoadMacroButton.Location = new System.Drawing.Point(232, 358);
            this.LoadMacroButton.Name = "LoadMacroButton";
            this.LoadMacroButton.Size = new System.Drawing.Size(92, 26);
            this.LoadMacroButton.TabIndex = 8;
            this.LoadMacroButton.Text = "Load Macro";
            this.LoadMacroButton.UseVisualStyleBackColor = false;
            this.LoadMacroButton.Click += new System.EventHandler(this.LoadMacroButton_Click);
            // 
            // LoadMacroDialog
            // 
            this.LoadMacroDialog.DefaultExt = "ditto";
            this.LoadMacroDialog.Title = "Load Ditto Macro";
            // 
            // SaveMacroDialog
            // 
            this.SaveMacroDialog.DefaultExt = "ditto";
            this.SaveMacroDialog.Title = "Save Ditto Macro";
            // 
            // UpdateLinkLabel
            // 
            this.UpdateLinkLabel.AutoSize = true;
            this.UpdateLinkLabel.BackColor = System.Drawing.Color.Transparent;
            this.UpdateLinkLabel.DisabledLinkColor = System.Drawing.Color.White;
            this.UpdateLinkLabel.ForeColor = System.Drawing.Color.White;
            this.UpdateLinkLabel.LinkColor = System.Drawing.Color.White;
            this.UpdateLinkLabel.Location = new System.Drawing.Point(128, 305);
            this.UpdateLinkLabel.Name = "UpdateLinkLabel";
            this.UpdateLinkLabel.Size = new System.Drawing.Size(100, 13);
            this.UpdateLinkLabel.TabIndex = 9;
            this.UpdateLinkLabel.TabStop = true;
            this.UpdateLinkLabel.Text = "view update details";
            this.UpdateLinkLabel.Visible = false;
            this.UpdateLinkLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.UpdateLinkLabel_LinkClicked);
            // 
            // Launcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(362, 447);
            this.Controls.Add(this.UpdateLinkLabel);
            this.Controls.Add(this.LoadMacroButton);
            this.Controls.Add(this.NewMacroButton);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.UpdateDescriptionLabel);
            this.Controls.Add(this.UpdateTitleLabel);
            this.Controls.Add(this.LaunchClientButton);
            this.Controls.Add(this.WebsiteLink);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Launcher";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Ditto Online";
            this.Load += new System.EventHandler(this.Launcher_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Launcher_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.LinkLabel WebsiteLink;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button NewMacroButton;
        private System.Windows.Forms.Button LoadMacroButton;
        private System.Windows.Forms.OpenFileDialog LoadMacroDialog;
        private System.Windows.Forms.SaveFileDialog SaveMacroDialog;
        public System.Windows.Forms.Label UpdateTitleLabel;
        public System.Windows.Forms.Label UpdateDescriptionLabel;
        public System.Windows.Forms.LinkLabel UpdateLinkLabel;
        public System.Windows.Forms.Button LaunchClientButton;
    }
}

