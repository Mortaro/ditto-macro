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
            this.CreditsLabel = new System.Windows.Forms.Label();
            this.ConnectionStatusLabel = new System.Windows.Forms.Label();
            this.CloseButton = new System.Windows.Forms.Button();
            this.NewMacroButton = new System.Windows.Forms.Button();
            this.LoadMacroButton = new System.Windows.Forms.Button();
            this.LoadMacroDialog = new System.Windows.Forms.OpenFileDialog();
            this.SaveMacroDialog = new System.Windows.Forms.SaveFileDialog();
            this.SaveMacroButton = new System.Windows.Forms.Button();
            this.HostInput = new System.Windows.Forms.TextBox();
            this.PasswordInput = new System.Windows.Forms.TextBox();
            this.PerformanceMode = new System.Windows.Forms.CheckBox();
            this.NetworkMode = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // CreditsLabel
            // 
            this.CreditsLabel.AutoSize = true;
            this.CreditsLabel.BackColor = System.Drawing.Color.Black;
            this.CreditsLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.CreditsLabel.Location = new System.Drawing.Point(50, 208);
            this.CreditsLabel.Name = "CreditsLabel";
            this.CreditsLabel.Size = new System.Drawing.Size(259, 13);
            this.CreditsLabel.TabIndex = 0;
            this.CreditsLabel.Text = "Ditto Macro is a project created by Christian Mortaro";
            this.CreditsLabel.Click += new System.EventHandler(this.CreditsLabel_Click);
            // 
            // ConnectionStatusLabel
            // 
            this.ConnectionStatusLabel.BackColor = System.Drawing.Color.Transparent;
            this.ConnectionStatusLabel.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.ConnectionStatusLabel.Location = new System.Drawing.Point(8, 2);
            this.ConnectionStatusLabel.Name = "ConnectionStatusLabel";
            this.ConnectionStatusLabel.Size = new System.Drawing.Size(121, 19);
            this.ConnectionStatusLabel.TabIndex = 3;
            this.ConnectionStatusLabel.Text = "Ditto Macro Offline";
            this.ConnectionStatusLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
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
            this.LoadMacroButton.Location = new System.Drawing.Point(37, 358);
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
            // SaveMacroButton
            // 
            this.SaveMacroButton.BackColor = System.Drawing.Color.Transparent;
            this.SaveMacroButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.SaveMacroButton.FlatAppearance.BorderSize = 0;
            this.SaveMacroButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.SaveMacroButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.SaveMacroButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.SaveMacroButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.SaveMacroButton.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.SaveMacroButton.Location = new System.Drawing.Point(232, 358);
            this.SaveMacroButton.Name = "SaveMacroButton";
            this.SaveMacroButton.Size = new System.Drawing.Size(92, 26);
            this.SaveMacroButton.TabIndex = 9;
            this.SaveMacroButton.Text = "Save Macro";
            this.SaveMacroButton.UseVisualStyleBackColor = false;
            this.SaveMacroButton.Click += new System.EventHandler(this.SaveMacroButton_Click);
            // 
            // HostInput
            // 
            this.HostInput.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.HostInput.BackColor = System.Drawing.Color.Black;
            this.HostInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.HostInput.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.HostInput.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.HostInput.Location = new System.Drawing.Point(27, 266);
            this.HostInput.Margin = new System.Windows.Forms.Padding(5);
            this.HostInput.MinimumSize = new System.Drawing.Size(305, 25);
            this.HostInput.Multiline = true;
            this.HostInput.Name = "HostInput";
            this.HostInput.Size = new System.Drawing.Size(305, 25);
            this.HostInput.TabIndex = 10;
            this.HostInput.TextChanged += new System.EventHandler(this.HostInput_TextChanged);
            // 
            // PasswordInput
            // 
            this.PasswordInput.BackColor = System.Drawing.Color.Black;
            this.PasswordInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.PasswordInput.Font = new System.Drawing.Font("Tahoma", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.PasswordInput.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.PasswordInput.Location = new System.Drawing.Point(27, 298);
            this.PasswordInput.Margin = new System.Windows.Forms.Padding(5);
            this.PasswordInput.MinimumSize = new System.Drawing.Size(305, 25);
            this.PasswordInput.Multiline = true;
            this.PasswordInput.Name = "PasswordInput";
            this.PasswordInput.Size = new System.Drawing.Size(305, 25);
            this.PasswordInput.TabIndex = 11;
            this.PasswordInput.TextChanged += new System.EventHandler(this.PasswordInput_TextChanged);
            // 
            // PerformanceMode
            // 
            this.PerformanceMode.AutoSize = true;
            this.PerformanceMode.BackColor = System.Drawing.Color.Transparent;
            this.PerformanceMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.PerformanceMode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.PerformanceMode.Location = new System.Drawing.Point(80, 401);
            this.PerformanceMode.Name = "PerformanceMode";
            this.PerformanceMode.Size = new System.Drawing.Size(113, 17);
            this.PerformanceMode.TabIndex = 12;
            this.PerformanceMode.Text = "Performance Mode";
            this.PerformanceMode.UseVisualStyleBackColor = false;
            this.PerformanceMode.CheckedChanged += new System.EventHandler(this.PerformanceMode_CheckedChanged);
            // 
            // NetworkMode
            // 
            this.NetworkMode.AutoSize = true;
            this.NetworkMode.BackColor = System.Drawing.Color.Transparent;
            this.NetworkMode.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.NetworkMode.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.NetworkMode.Location = new System.Drawing.Point(194, 401);
            this.NetworkMode.Name = "NetworkMode";
            this.NetworkMode.Size = new System.Drawing.Size(92, 17);
            this.NetworkMode.TabIndex = 13;
            this.NetworkMode.Text = "Network Mode";
            this.NetworkMode.UseVisualStyleBackColor = false;
            this.NetworkMode.CheckedChanged += new System.EventHandler(this.NetworkMode_CheckedChanged);
            // 
            // Launcher
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(362, 449);
            this.Controls.Add(this.NetworkMode);
            this.Controls.Add(this.PerformanceMode);
            this.Controls.Add(this.PasswordInput);
            this.Controls.Add(this.HostInput);
            this.Controls.Add(this.SaveMacroButton);
            this.Controls.Add(this.LoadMacroButton);
            this.Controls.Add(this.NewMacroButton);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.ConnectionStatusLabel);
            this.Controls.Add(this.CreditsLabel);
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

        private System.Windows.Forms.Label CreditsLabel;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.Button NewMacroButton;
        private System.Windows.Forms.Button LoadMacroButton;
        private System.Windows.Forms.OpenFileDialog LoadMacroDialog;
        private System.Windows.Forms.SaveFileDialog SaveMacroDialog;
        public System.Windows.Forms.Label ConnectionStatusLabel;
        private System.Windows.Forms.Button SaveMacroButton;
        private System.Windows.Forms.TextBox HostInput;
        private System.Windows.Forms.TextBox PasswordInput;
        public System.Windows.Forms.CheckBox PerformanceMode;
        public System.Windows.Forms.CheckBox NetworkMode;
    }
}

