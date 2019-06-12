namespace Ditto
{
    partial class Macro
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Macro));
            this.StartButton = new System.Windows.Forms.Button();
            this.TargetButton = new System.Windows.Forms.Button();
            this.CoordinatesButton = new System.Windows.Forms.Button();
            this.CloseButton = new System.Windows.Forms.Button();
            this.CommandsInput = new System.Windows.Forms.TextBox();
            this.CommandsDisplay = new System.Windows.Forms.RichTextBox();
            this.TitleLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // StartButton
            // 
            this.StartButton.BackColor = System.Drawing.Color.Transparent;
            this.StartButton.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Center;
            this.StartButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.StartButton.FlatAppearance.BorderSize = 0;
            this.StartButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.StartButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.StartButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StartButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.StartButton.Location = new System.Drawing.Point(79, 245);
            this.StartButton.Name = "StartButton";
            this.StartButton.Size = new System.Drawing.Size(96, 26);
            this.StartButton.TabIndex = 0;
            this.StartButton.Text = "Start";
            this.StartButton.UseVisualStyleBackColor = false;
            this.StartButton.Click += new System.EventHandler(this.StartButton_Click);
            // 
            // TargetButton
            // 
            this.TargetButton.BackColor = System.Drawing.Color.Transparent;
            this.TargetButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.TargetButton.FlatAppearance.BorderSize = 0;
            this.TargetButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.TargetButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.TargetButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.TargetButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TargetButton.Location = new System.Drawing.Point(176, 245);
            this.TargetButton.Name = "TargetButton";
            this.TargetButton.Size = new System.Drawing.Size(48, 26);
            this.TargetButton.TabIndex = 1;
            this.TargetButton.Text = "F8";
            this.TargetButton.UseVisualStyleBackColor = false;
            this.TargetButton.Click += new System.EventHandler(this.TargetButton_Click);
            // 
            // CoordinatesButton
            // 
            this.CoordinatesButton.BackColor = System.Drawing.Color.Transparent;
            this.CoordinatesButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CoordinatesButton.FlatAppearance.BorderSize = 0;
            this.CoordinatesButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.CoordinatesButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.CoordinatesButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CoordinatesButton.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.CoordinatesButton.Location = new System.Drawing.Point(31, 245);
            this.CoordinatesButton.Name = "CoordinatesButton";
            this.CoordinatesButton.Size = new System.Drawing.Size(48, 26);
            this.CoordinatesButton.TabIndex = 2;
            this.CoordinatesButton.Text = "F7";
            this.CoordinatesButton.UseVisualStyleBackColor = false;
            this.CoordinatesButton.Click += new System.EventHandler(this.CoordinatesButton_Click);
            // 
            // CloseButton
            // 
            this.CloseButton.BackColor = System.Drawing.Color.Transparent;
            this.CloseButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.CloseButton.FlatAppearance.BorderSize = 0;
            this.CloseButton.FlatAppearance.MouseDownBackColor = System.Drawing.Color.Transparent;
            this.CloseButton.FlatAppearance.MouseOverBackColor = System.Drawing.Color.Transparent;
            this.CloseButton.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.CloseButton.Location = new System.Drawing.Point(230, -3);
            this.CloseButton.Name = "CloseButton";
            this.CloseButton.Size = new System.Drawing.Size(20, 20);
            this.CloseButton.TabIndex = 3;
            this.CloseButton.Text = "x";
            this.CloseButton.UseVisualStyleBackColor = false;
            this.CloseButton.Click += new System.EventHandler(this.CloseButton_Click);
            // 
            // CommandsInput
            // 
            this.CommandsInput.BackColor = System.Drawing.Color.Black;
            this.CommandsInput.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.CommandsInput.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.CommandsInput.Location = new System.Drawing.Point(12, 31);
            this.CommandsInput.Multiline = true;
            this.CommandsInput.Name = "CommandsInput";
            this.CommandsInput.Size = new System.Drawing.Size(230, 195);
            this.CommandsInput.TabIndex = 4;
            // 
            // CommandsDisplay
            // 
            this.CommandsDisplay.BackColor = System.Drawing.Color.Black;
            this.CommandsDisplay.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.CommandsDisplay.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.CommandsDisplay.Location = new System.Drawing.Point(12, 31);
            this.CommandsDisplay.Name = "CommandsDisplay";
            this.CommandsDisplay.Size = new System.Drawing.Size(230, 195);
            this.CommandsDisplay.TabIndex = 5;
            this.CommandsDisplay.Text = "";
            this.CommandsDisplay.Visible = false;
            // 
            // TitleLabel
            // 
            this.TitleLabel.AutoSize = true;
            this.TitleLabel.BackColor = System.Drawing.Color.Transparent;
            this.TitleLabel.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TitleLabel.Location = new System.Drawing.Point(20, 3);
            this.TitleLabel.Name = "TitleLabel";
            this.TitleLabel.Size = new System.Drawing.Size(35, 13);
            this.TitleLabel.TabIndex = 6;
            this.TitleLabel.Text = "Ditto";
            // 
            // Macro
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("$this.BackgroundImage")));
            this.ClientSize = new System.Drawing.Size(253, 293);
            this.Controls.Add(this.TitleLabel);
            this.Controls.Add(this.CommandsDisplay);
            this.Controls.Add(this.CommandsInput);
            this.Controls.Add(this.CloseButton);
            this.Controls.Add(this.CoordinatesButton);
            this.Controls.Add(this.TargetButton);
            this.Controls.Add(this.StartButton);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "Macro";
            this.Text = "Ditto Macro";
            this.Load += new System.EventHandler(this.Macro_Load);
            this.MouseDown += new System.Windows.Forms.MouseEventHandler(this.Macro_MouseDown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button StartButton;
        private System.Windows.Forms.Button TargetButton;
        private System.Windows.Forms.Button CoordinatesButton;
        private System.Windows.Forms.Button CloseButton;
        private System.Windows.Forms.TextBox CommandsInput;
        private System.Windows.Forms.RichTextBox CommandsDisplay;
        private System.Windows.Forms.Label TitleLabel;
    }
}