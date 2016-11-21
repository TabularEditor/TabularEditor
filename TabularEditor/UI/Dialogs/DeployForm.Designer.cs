namespace TabularEditor.UI.Dialogs
{
    partial class DeployForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeployForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.panel2 = new System.Windows.Forms.Panel();
            this.panel6 = new System.Windows.Forms.Panel();
            this.lblStep4 = new System.Windows.Forms.Label();
            this.lblStep3 = new System.Windows.Forms.Label();
            this.lblStep2 = new System.Windows.Forms.Label();
            this.lblStep1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.btnDeploy = new System.Windows.Forms.Button();
            this.btnNext = new System.Windows.Forms.Button();
            this.btnPrev = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.panel4 = new System.Windows.Forms.Panel();
            this.panel5 = new System.Windows.Forms.Panel();
            this.pnlPage1 = new System.Windows.Forms.Panel();
            this.page2 = new TabularEditor.UI.Dialogs.Pages.DatabasePage();
            this.page1 = new TabularEditor.UI.Dialogs.Pages.ConnectPage();
            this.page4 = new System.Windows.Forms.Panel();
            this.tvSummary = new System.Windows.Forms.TreeView();
            this.page3 = new System.Windows.Forms.Panel();
            this.chkDeployStructure = new System.Windows.Forms.CheckBox();
            this.chkDeployPartitions = new System.Windows.Forms.CheckBox();
            this.chkDeployRoleMembers = new System.Windows.Forms.CheckBox();
            this.chkDeployRoles = new System.Windows.Forms.CheckBox();
            this.chkDeployConnections = new System.Windows.Forms.CheckBox();
            this.lblHint = new System.Windows.Forms.Label();
            this.btnTMSL = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.pnlPage1.SuspendLayout();
            this.page4.SuspendLayout();
            this.page3.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.pictureBox1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 2);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(647, 70);
            this.panel1.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(84, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(193, 16);
            this.label1.TabIndex = 1;
            this.label1.Text = "Choose Destination Server";
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = ((System.Drawing.Image)(resources.GetObject("pictureBox1.Image")));
            this.pictureBox1.Location = new System.Drawing.Point(12, 12);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(48, 48);
            this.pictureBox1.TabIndex = 0;
            this.pictureBox1.TabStop = false;
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Control;
            this.panel2.Controls.Add(this.panel6);
            this.panel2.Controls.Add(this.lblStep4);
            this.panel2.Controls.Add(this.lblStep3);
            this.panel2.Controls.Add(this.lblStep2);
            this.panel2.Controls.Add(this.lblStep1);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.panel2.Location = new System.Drawing.Point(0, 72);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(135, 358);
            this.panel2.TabIndex = 0;
            // 
            // panel6
            // 
            this.panel6.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panel6.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel6.Location = new System.Drawing.Point(133, 0);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(2, 358);
            this.panel6.TabIndex = 0;
            // 
            // lblStep4
            // 
            this.lblStep4.AutoSize = true;
            this.lblStep4.Location = new System.Drawing.Point(12, 103);
            this.lblStep4.Name = "lblStep4";
            this.lblStep4.Size = new System.Drawing.Size(50, 13);
            this.lblStep4.TabIndex = 3;
            this.lblStep4.Text = "Summary";
            // 
            // lblStep3
            // 
            this.lblStep3.AutoSize = true;
            this.lblStep3.Location = new System.Drawing.Point(12, 75);
            this.lblStep3.Name = "lblStep3";
            this.lblStep3.Size = new System.Drawing.Size(102, 13);
            this.lblStep3.TabIndex = 2;
            this.lblStep3.Text = "Deployment Options";
            // 
            // lblStep2
            // 
            this.lblStep2.AutoSize = true;
            this.lblStep2.Location = new System.Drawing.Point(12, 47);
            this.lblStep2.Name = "lblStep2";
            this.lblStep2.Size = new System.Drawing.Size(53, 13);
            this.lblStep2.TabIndex = 1;
            this.lblStep2.Text = "Database";
            // 
            // lblStep1
            // 
            this.lblStep1.AutoSize = true;
            this.lblStep1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblStep1.Location = new System.Drawing.Point(12, 19);
            this.lblStep1.Name = "lblStep1";
            this.lblStep1.Size = new System.Drawing.Size(112, 13);
            this.lblStep1.TabIndex = 0;
            this.lblStep1.Text = "Destination Server";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.btnDeploy);
            this.panel3.Controls.Add(this.btnNext);
            this.panel3.Controls.Add(this.btnPrev);
            this.panel3.Controls.Add(this.btnCancel);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel3.Location = new System.Drawing.Point(135, 381);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(512, 49);
            this.panel3.TabIndex = 5;
            // 
            // btnDeploy
            // 
            this.btnDeploy.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnDeploy.Enabled = false;
            this.btnDeploy.Location = new System.Drawing.Point(344, 14);
            this.btnDeploy.Name = "btnDeploy";
            this.btnDeploy.Size = new System.Drawing.Size(75, 23);
            this.btnDeploy.TabIndex = 3;
            this.btnDeploy.Text = "Deploy";
            this.btnDeploy.UseVisualStyleBackColor = true;
            this.btnDeploy.Click += new System.EventHandler(this.btnDeploy_Click);
            // 
            // btnNext
            // 
            this.btnNext.Enabled = false;
            this.btnNext.Location = new System.Drawing.Point(263, 14);
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(75, 23);
            this.btnNext.TabIndex = 2;
            this.btnNext.Text = "Next >";
            this.btnNext.UseVisualStyleBackColor = true;
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnPrev
            // 
            this.btnPrev.Enabled = false;
            this.btnPrev.Location = new System.Drawing.Point(182, 14);
            this.btnPrev.Name = "btnPrev";
            this.btnPrev.Size = new System.Drawing.Size(75, 23);
            this.btnPrev.TabIndex = 1;
            this.btnPrev.Text = "< Previous";
            this.btnPrev.UseVisualStyleBackColor = true;
            this.btnPrev.Click += new System.EventHandler(this.btnPrev_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(425, 14);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // panel4
            // 
            this.panel4.BackColor = System.Drawing.SystemColors.Control;
            this.panel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(647, 2);
            this.panel4.TabIndex = 3;
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.SystemColors.ButtonHighlight;
            this.panel5.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel5.Location = new System.Drawing.Point(135, 379);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(512, 2);
            this.panel5.TabIndex = 2;
            // 
            // pnlPage1
            // 
            this.pnlPage1.Controls.Add(this.lblHint);
            this.pnlPage1.Controls.Add(this.page4);
            this.pnlPage1.Controls.Add(this.page3);
            this.pnlPage1.Controls.Add(this.page2);
            this.pnlPage1.Controls.Add(this.page1);
            this.pnlPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.pnlPage1.Location = new System.Drawing.Point(135, 72);
            this.pnlPage1.Name = "pnlPage1";
            this.pnlPage1.Size = new System.Drawing.Size(512, 307);
            this.pnlPage1.TabIndex = 0;
            // 
            // page2
            // 
            this.page2.AllowNew = true;
            this.page2.Location = new System.Drawing.Point(15, 63);
            this.page2.Name = "page2";
            this.page2.Server = null;
            this.page2.Size = new System.Drawing.Size(485, 232);
            this.page2.TabIndex = 1;
            this.page2.Visible = false;
            this.page2.Validation += new TabularEditor.UI.Dialogs.ValidationEventHandler(this.Page_Validation);
            this.page2.Accept += new System.EventHandler(this.page2_Accept);
            // 
            // page1
            // 
            this.page1.IntegratedSecurity = true;
            this.page1.Location = new System.Drawing.Point(15, 63);
            this.page1.MinimumSize = new System.Drawing.Size(232, 120);
            this.page1.Name = "page1";
            this.page1.Password = "";
            this.page1.ServerName = "";
            this.page1.Size = new System.Drawing.Size(485, 232);
            this.page1.TabIndex = 0;
            this.page1.UserName = "";
            this.page1.Validation += new TabularEditor.UI.Dialogs.ValidationEventHandler(this.Page_Validation);
            // 
            // page4
            // 
            this.page4.Controls.Add(this.btnTMSL);
            this.page4.Controls.Add(this.tvSummary);
            this.page4.Location = new System.Drawing.Point(15, 63);
            this.page4.Name = "page4";
            this.page4.Size = new System.Drawing.Size(485, 232);
            this.page4.TabIndex = 3;
            this.page4.Visible = false;
            // 
            // tvSummary
            // 
            this.tvSummary.Location = new System.Drawing.Point(0, 0);
            this.tvSummary.Name = "tvSummary";
            this.tvSummary.Size = new System.Drawing.Size(485, 203);
            this.tvSummary.TabIndex = 0;
            // 
            // page3
            // 
            this.page3.Controls.Add(this.chkDeployStructure);
            this.page3.Controls.Add(this.chkDeployPartitions);
            this.page3.Controls.Add(this.chkDeployRoleMembers);
            this.page3.Controls.Add(this.chkDeployRoles);
            this.page3.Controls.Add(this.chkDeployConnections);
            this.page3.Location = new System.Drawing.Point(15, 63);
            this.page3.Name = "page3";
            this.page3.Size = new System.Drawing.Size(485, 232);
            this.page3.TabIndex = 2;
            this.page3.Visible = false;
            // 
            // chkDeployStructure
            // 
            this.chkDeployStructure.AutoSize = true;
            this.chkDeployStructure.Checked = true;
            this.chkDeployStructure.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDeployStructure.Location = new System.Drawing.Point(167, 36);
            this.chkDeployStructure.Name = "chkDeployStructure";
            this.chkDeployStructure.Size = new System.Drawing.Size(137, 17);
            this.chkDeployStructure.TabIndex = 0;
            this.chkDeployStructure.Text = "Deploy Model Structure";
            this.chkDeployStructure.UseVisualStyleBackColor = true;
            this.chkDeployStructure.CheckedChanged += new System.EventHandler(this.chkDeployStructure_CheckedChanged);
            // 
            // chkDeployPartitions
            // 
            this.chkDeployPartitions.AutoSize = true;
            this.chkDeployPartitions.Location = new System.Drawing.Point(167, 108);
            this.chkDeployPartitions.Name = "chkDeployPartitions";
            this.chkDeployPartitions.Size = new System.Drawing.Size(135, 17);
            this.chkDeployPartitions.TabIndex = 2;
            this.chkDeployPartitions.Text = "Deploy Table Partitions";
            this.chkDeployPartitions.UseVisualStyleBackColor = true;
            // 
            // chkDeployRoleMembers
            // 
            this.chkDeployRoleMembers.AutoSize = true;
            this.chkDeployRoleMembers.Location = new System.Drawing.Point(167, 180);
            this.chkDeployRoleMembers.Name = "chkDeployRoleMembers";
            this.chkDeployRoleMembers.Size = new System.Drawing.Size(130, 17);
            this.chkDeployRoleMembers.TabIndex = 4;
            this.chkDeployRoleMembers.Text = "Deploy Role Members";
            this.chkDeployRoleMembers.UseVisualStyleBackColor = true;
            // 
            // chkDeployRoles
            // 
            this.chkDeployRoles.AutoSize = true;
            this.chkDeployRoles.Checked = true;
            this.chkDeployRoles.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkDeployRoles.Location = new System.Drawing.Point(167, 144);
            this.chkDeployRoles.Name = "chkDeployRoles";
            this.chkDeployRoles.Size = new System.Drawing.Size(89, 17);
            this.chkDeployRoles.TabIndex = 3;
            this.chkDeployRoles.Text = "Deploy Roles";
            this.chkDeployRoles.UseVisualStyleBackColor = true;
            this.chkDeployRoles.CheckedChanged += new System.EventHandler(this.chkDeployRoles_CheckedChanged);
            // 
            // chkDeployConnections
            // 
            this.chkDeployConnections.AutoSize = true;
            this.chkDeployConnections.Location = new System.Drawing.Point(167, 72);
            this.chkDeployConnections.Name = "chkDeployConnections";
            this.chkDeployConnections.Size = new System.Drawing.Size(121, 17);
            this.chkDeployConnections.TabIndex = 1;
            this.chkDeployConnections.Text = "Deploy Connections";
            this.chkDeployConnections.UseVisualStyleBackColor = true;
            // 
            // lblHint
            // 
            this.lblHint.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHint.Location = new System.Drawing.Point(12, 19);
            this.lblHint.Name = "lblHint";
            this.lblHint.Size = new System.Drawing.Size(488, 41);
            this.lblHint.TabIndex = 0;
            this.lblHint.Text = "Enter the server name of the SQL Server 2016 Analysis Services (Tabular) instance" +
    " you want to deploy to. For Azure Analysis Services, use \"asazure://<servername>" +
    "\".";
            // 
            // btnTMSL
            // 
            this.btnTMSL.Location = new System.Drawing.Point(369, 209);
            this.btnTMSL.Name = "btnTMSL";
            this.btnTMSL.Size = new System.Drawing.Size(116, 23);
            this.btnTMSL.TabIndex = 1;
            this.btnTMSL.Text = "TMSL Script";
            this.btnTMSL.UseVisualStyleBackColor = true;
            this.btnTMSL.Click += new System.EventHandler(this.btnTMSL_Click);
            // 
            // DeployForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(647, 430);
            this.Controls.Add(this.pnlPage1);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.panel4);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "DeployForm";
            this.Text = "Tabular Editor Deployment Wizard";
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.DeployForm_KeyDown);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.pnlPage1.ResumeLayout(false);
            this.page4.ResumeLayout(false);
            this.page3.ResumeLayout(false);
            this.page3.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblStep4;
        private System.Windows.Forms.Label lblStep3;
        private System.Windows.Forms.Label lblStep2;
        private System.Windows.Forms.Label lblStep1;
        private System.Windows.Forms.Button btnDeploy;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnPrev;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.Panel pnlPage1;
        private System.Windows.Forms.Label lblHint;
        private Pages.ConnectPage page1;
        private Pages.DatabasePage page2;
        private System.Windows.Forms.Panel page3;
        private System.Windows.Forms.CheckBox chkDeployRoleMembers;
        private System.Windows.Forms.CheckBox chkDeployRoles;
        private System.Windows.Forms.CheckBox chkDeployConnections;
        private System.Windows.Forms.CheckBox chkDeployPartitions;
        private System.Windows.Forms.CheckBox chkDeployStructure;
        private System.Windows.Forms.Panel page4;
        private System.Windows.Forms.TreeView tvSummary;
        private System.Windows.Forms.Button btnTMSL;
    }
}