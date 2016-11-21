namespace TabularEditor.UI.Dialogs.Pages
{
    partial class ConnectPage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.rdbUsernamePassword = new System.Windows.Forms.RadioButton();
            this.rdbIntegrated = new System.Windows.Forms.RadioButton();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPassword = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtUsername = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtServer = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // rdbUsernamePassword
            // 
            this.rdbUsernamePassword.AutoSize = true;
            this.rdbUsernamePassword.Location = new System.Drawing.Point(87, 50);
            this.rdbUsernamePassword.Name = "rdbUsernamePassword";
            this.rdbUsernamePassword.Size = new System.Drawing.Size(143, 17);
            this.rdbUsernamePassword.TabIndex = 26;
            this.rdbUsernamePassword.Text = "Username and Password";
            this.rdbUsernamePassword.UseVisualStyleBackColor = true;
            this.rdbUsernamePassword.CheckedChanged += new System.EventHandler(this.ValidateUI);
            // 
            // rdbIntegrated
            // 
            this.rdbIntegrated.AutoSize = true;
            this.rdbIntegrated.Checked = true;
            this.rdbIntegrated.Location = new System.Drawing.Point(87, 27);
            this.rdbIntegrated.Name = "rdbIntegrated";
            this.rdbIntegrated.Size = new System.Drawing.Size(114, 17);
            this.rdbIntegrated.TabIndex = 25;
            this.rdbIntegrated.TabStop = true;
            this.rdbIntegrated.Text = "Integrated Security";
            this.rdbIntegrated.UseVisualStyleBackColor = true;
            this.rdbIntegrated.CheckedChanged += new System.EventHandler(this.ValidateUI);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(-3, 29);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(78, 13);
            this.label5.TabIndex = 32;
            this.label5.Text = "Authentication:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(-3, 103);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 13);
            this.label4.TabIndex = 31;
            this.label4.Text = "Password:";
            // 
            // txtPassword
            // 
            this.txtPassword.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtPassword.Enabled = false;
            this.txtPassword.Location = new System.Drawing.Point(87, 100);
            this.txtPassword.MaximumSize = new System.Drawing.Size(180, 20);
            this.txtPassword.Name = "txtPassword";
            this.txtPassword.Size = new System.Drawing.Size(145, 20);
            this.txtPassword.TabIndex = 28;
            this.txtPassword.UseSystemPasswordChar = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(-3, 77);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 13);
            this.label3.TabIndex = 30;
            this.label3.Text = "Username:";
            // 
            // txtUsername
            // 
            this.txtUsername.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUsername.Enabled = false;
            this.txtUsername.Location = new System.Drawing.Point(87, 74);
            this.txtUsername.MaximumSize = new System.Drawing.Size(180, 20);
            this.txtUsername.Name = "txtUsername";
            this.txtUsername.Size = new System.Drawing.Size(145, 20);
            this.txtUsername.TabIndex = 27;
            this.txtUsername.TextChanged += new System.EventHandler(this.ValidateUI);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(-3, 3);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 13);
            this.label2.TabIndex = 29;
            this.label2.Text = "Server:";
            // 
            // txtServer
            // 
            this.txtServer.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtServer.Location = new System.Drawing.Point(87, 0);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(145, 20);
            this.txtServer.TabIndex = 24;
            this.txtServer.TextChanged += new System.EventHandler(this.ValidateUI);
            // 
            // ConnectPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.rdbUsernamePassword);
            this.Controls.Add(this.rdbIntegrated);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtPassword);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtUsername);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtServer);
            this.MinimumSize = new System.Drawing.Size(232, 120);
            this.Name = "ConnectPage";
            this.Size = new System.Drawing.Size(232, 120);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rdbUsernamePassword;
        private System.Windows.Forms.RadioButton rdbIntegrated;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPassword;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtUsername;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtServer;
    }
}
