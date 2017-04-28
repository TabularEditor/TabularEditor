namespace TabularEditor.UI.Dialogs
{
    partial class PreferencesForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PreferencesForm));
            System.Windows.Forms.TreeNode treeNode1 = new System.Windows.Forms.TreeNode("Data Sources");
            System.Windows.Forms.TreeNode treeNode2 = new System.Windows.Forms.TreeNode("Perspectives");
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("Relationships");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("Roles");
            System.Windows.Forms.TreeNode treeNode5 = new System.Windows.Forms.TreeNode("Annotations");
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Columns");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Hierarchies");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Measures");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Partitions");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Tables", new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode6,
            treeNode7,
            treeNode8,
            treeNode9});
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Translations");
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkFixup = new System.Windows.Forms.CheckBox();
            this.chkAutoBackup = new System.Windows.Forms.CheckBox();
            this.chkIgnoreTimestamps = new System.Windows.Forms.CheckBox();
            this.chkIgnoreInfObjects = new System.Windows.Forms.CheckBox();
            this.chkIgnoreInfProps = new System.Windows.Forms.CheckBox();
            this.chkSplitMultiline = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.grpDeployment = new System.Windows.Forms.GroupBox();
            this.btnFolder = new System.Windows.Forms.Button();
            this.txtBackupPath = new System.Windows.Forms.TextBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkAutoUpdate = new System.Windows.Forms.CheckBox();
            this.btnVersionCheck = new System.Windows.Forms.Button();
            this.lblAvailableVersion = new System.Windows.Forms.Label();
            this.lblCurrentVersion = new System.Windows.Forms.Label();
            this.chkPrefixFiles = new System.Windows.Forms.CheckBox();
            this.groupBox3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.grpDeployment.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(186, 342);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(267, 342);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // chkFixup
            // 
            this.chkFixup.AutoSize = true;
            this.chkFixup.Location = new System.Drawing.Point(6, 19);
            this.chkFixup.Name = "chkFixup";
            this.chkFixup.Size = new System.Drawing.Size(138, 17);
            this.chkFixup.TabIndex = 1;
            this.chkFixup.Text = "Automatic formula fix-up";
            this.toolTip1.SetToolTip(this.chkFixup, "When this is checked, expressions of Measures, Calculated Columns and Calculated " +
        "Tables will automatically be updated, when an object name is changed.");
            this.chkFixup.UseVisualStyleBackColor = true;
            // 
            // chkAutoBackup
            // 
            this.chkAutoBackup.AutoSize = true;
            this.chkAutoBackup.Location = new System.Drawing.Point(6, 19);
            this.chkAutoBackup.Name = "chkAutoBackup";
            this.chkAutoBackup.Size = new System.Drawing.Size(168, 17);
            this.chkAutoBackup.TabIndex = 6;
            this.chkAutoBackup.Text = "Save Model.bim backup here:";
            this.toolTip1.SetToolTip(this.chkAutoBackup, resources.GetString("chkAutoBackup.ToolTip"));
            this.chkAutoBackup.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreTimestamps
            // 
            this.chkIgnoreTimestamps.AutoSize = true;
            this.chkIgnoreTimestamps.Location = new System.Drawing.Point(6, 42);
            this.chkIgnoreTimestamps.Name = "chkIgnoreTimestamps";
            this.chkIgnoreTimestamps.Size = new System.Drawing.Size(111, 17);
            this.chkIgnoreTimestamps.TabIndex = 0;
            this.chkIgnoreTimestamps.Text = "Ignore timestamps";
            this.toolTip1.SetToolTip(this.chkIgnoreTimestamps, "If checked, editing timestamps are not serialized into the .json files");
            this.chkIgnoreTimestamps.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreInfObjects
            // 
            this.chkIgnoreInfObjects.AutoSize = true;
            this.chkIgnoreInfObjects.Location = new System.Drawing.Point(6, 19);
            this.chkIgnoreInfObjects.Name = "chkIgnoreInfObjects";
            this.chkIgnoreInfObjects.Size = new System.Drawing.Size(131, 17);
            this.chkIgnoreInfObjects.TabIndex = 1;
            this.chkIgnoreInfObjects.Text = "Ignore inferred objects";
            this.toolTip1.SetToolTip(this.chkIgnoreInfObjects, "If checked, objects whose lifetime is controlled by the server (RowNumber, attrib" +
        "ute hierarchies, etc.) are not serialized into the .json files");
            this.chkIgnoreInfObjects.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreInfProps
            // 
            this.chkIgnoreInfProps.AutoSize = true;
            this.chkIgnoreInfProps.Location = new System.Drawing.Point(143, 19);
            this.chkIgnoreInfProps.Name = "chkIgnoreInfProps";
            this.chkIgnoreInfProps.Size = new System.Drawing.Size(143, 17);
            this.chkIgnoreInfProps.TabIndex = 2;
            this.chkIgnoreInfProps.Text = "Ignore inferred properties";
            this.toolTip1.SetToolTip(this.chkIgnoreInfProps, "If checked, objects whose value is controlled by the server (Column.State, etc.) " +
        "are not serialized into the .json files");
            this.chkIgnoreInfProps.UseVisualStyleBackColor = true;
            // 
            // chkSplitMultiline
            // 
            this.chkSplitMultiline.AutoSize = true;
            this.chkSplitMultiline.Location = new System.Drawing.Point(143, 42);
            this.chkSplitMultiline.Name = "chkSplitMultiline";
            this.chkSplitMultiline.Size = new System.Drawing.Size(114, 17);
            this.chkSplitMultiline.TabIndex = 3;
            this.chkSplitMultiline.Text = "Split multiline string";
            this.toolTip1.SetToolTip(this.chkSplitMultiline, "If checked, expressions and other multiline string properties are serialized as a" +
        " JSON array.");
            this.chkSplitMultiline.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.chkFixup);
            this.groupBox3.Location = new System.Drawing.Point(6, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(310, 286);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Editing";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(330, 324);
            this.tabControl1.TabIndex = 5;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.grpDeployment);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(322, 298);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Deployment";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // grpDeployment
            // 
            this.grpDeployment.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDeployment.Controls.Add(this.btnFolder);
            this.grpDeployment.Controls.Add(this.txtBackupPath);
            this.grpDeployment.Controls.Add(this.chkAutoBackup);
            this.grpDeployment.Location = new System.Drawing.Point(7, 6);
            this.grpDeployment.Name = "grpDeployment";
            this.grpDeployment.Size = new System.Drawing.Size(309, 286);
            this.grpDeployment.TabIndex = 6;
            this.grpDeployment.TabStop = false;
            this.grpDeployment.Text = "Deployment Options";
            // 
            // btnFolder
            // 
            this.btnFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFolder.Enabled = false;
            this.btnFolder.Location = new System.Drawing.Point(276, 42);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(27, 20);
            this.btnFolder.TabIndex = 8;
            this.btnFolder.Text = "...";
            this.btnFolder.UseVisualStyleBackColor = true;
            // 
            // txtBackupPath
            // 
            this.txtBackupPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBackupPath.Enabled = false;
            this.txtBackupPath.Location = new System.Drawing.Point(6, 42);
            this.txtBackupPath.Name = "txtBackupPath";
            this.txtBackupPath.Size = new System.Drawing.Size(264, 20);
            this.txtBackupPath.TabIndex = 7;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupBox4);
            this.tabPage4.Controls.Add(this.groupBox2);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(322, 298);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Save to Folder";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.treeView1);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Location = new System.Drawing.Point(6, 100);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(310, 192);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Serialization Levels";
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.CheckBoxes = true;
            this.treeView1.Location = new System.Drawing.Point(6, 54);
            this.treeView1.Name = "treeView1";
            treeNode1.Checked = true;
            treeNode1.Name = "Data Sources";
            treeNode1.Text = "Data Sources";
            treeNode2.Checked = true;
            treeNode2.Name = "Perspectives";
            treeNode2.Text = "Perspectives";
            treeNode3.Checked = true;
            treeNode3.Name = "Relationships";
            treeNode3.Text = "Relationships";
            treeNode4.Checked = true;
            treeNode4.Name = "Roles";
            treeNode4.Text = "Roles";
            treeNode5.Name = "Annotations";
            treeNode5.Text = "Annotations";
            treeNode6.Checked = true;
            treeNode6.Name = "Columns";
            treeNode6.Text = "Columns";
            treeNode7.Checked = true;
            treeNode7.Name = "Hierarchies";
            treeNode7.Text = "Hierarchies";
            treeNode8.Checked = true;
            treeNode8.Name = "Measures";
            treeNode8.Text = "Measures";
            treeNode9.Checked = true;
            treeNode9.Name = "Partitions";
            treeNode9.Text = "Partitions";
            treeNode10.Checked = true;
            treeNode10.Name = "Tables";
            treeNode10.Text = "Tables";
            treeNode11.Checked = true;
            treeNode11.Name = "Translations";
            treeNode11.Text = "Translations";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode10,
            treeNode11});
            this.treeView1.PathSeparator = "/";
            this.treeView1.ShowLines = false;
            this.treeView1.Size = new System.Drawing.Size(298, 132);
            this.treeView1.TabIndex = 1;
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(297, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "Check the type of objects you wish to serialize as individual files (unchecked ob" +
    "jects are serialized in the parent object):";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.chkPrefixFiles);
            this.groupBox2.Controls.Add(this.chkSplitMultiline);
            this.groupBox2.Controls.Add(this.chkIgnoreInfProps);
            this.groupBox2.Controls.Add(this.chkIgnoreInfObjects);
            this.groupBox2.Controls.Add(this.chkIgnoreTimestamps);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(310, 88);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Serialization Options";
            // 
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(322, 298);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "DAX";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.groupBox1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(322, 298);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "General";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.chkAutoUpdate);
            this.groupBox1.Controls.Add(this.btnVersionCheck);
            this.groupBox1.Controls.Add(this.lblAvailableVersion);
            this.groupBox1.Controls.Add(this.lblCurrentVersion);
            this.groupBox1.Location = new System.Drawing.Point(6, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(310, 99);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tabular Editor Updates";
            // 
            // chkAutoUpdate
            // 
            this.chkAutoUpdate.AutoSize = true;
            this.chkAutoUpdate.Location = new System.Drawing.Point(6, 75);
            this.chkAutoUpdate.Name = "chkAutoUpdate";
            this.chkAutoUpdate.Size = new System.Drawing.Size(204, 17);
            this.chkAutoUpdate.TabIndex = 4;
            this.chkAutoUpdate.Text = "Check for updated version on start-up";
            this.chkAutoUpdate.UseVisualStyleBackColor = true;
            // 
            // btnVersionCheck
            // 
            this.btnVersionCheck.Location = new System.Drawing.Point(6, 46);
            this.btnVersionCheck.Name = "btnVersionCheck";
            this.btnVersionCheck.Size = new System.Drawing.Size(126, 23);
            this.btnVersionCheck.TabIndex = 3;
            this.btnVersionCheck.Text = "Check for updates";
            this.btnVersionCheck.UseVisualStyleBackColor = true;
            // 
            // lblAvailableVersion
            // 
            this.lblAvailableVersion.AutoSize = true;
            this.lblAvailableVersion.Cursor = System.Windows.Forms.Cursors.Hand;
            this.lblAvailableVersion.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Underline, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblAvailableVersion.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.lblAvailableVersion.Location = new System.Drawing.Point(6, 51);
            this.lblAvailableVersion.Name = "lblAvailableVersion";
            this.lblAvailableVersion.Size = new System.Drawing.Size(126, 13);
            this.lblAvailableVersion.TabIndex = 2;
            this.lblAvailableVersion.Text = "Available version: 2.0.0.0";
            this.lblAvailableVersion.Visible = false;
            // 
            // lblCurrentVersion
            // 
            this.lblCurrentVersion.AutoSize = true;
            this.lblCurrentVersion.Location = new System.Drawing.Point(6, 26);
            this.lblCurrentVersion.Name = "lblCurrentVersion";
            this.lblCurrentVersion.Size = new System.Drawing.Size(117, 13);
            this.lblCurrentVersion.TabIndex = 1;
            this.lblCurrentVersion.Text = "Current version: 2.0.0.0";
            // 
            // chkPrefixFiles
            // 
            this.chkPrefixFiles.AutoSize = true;
            this.chkPrefixFiles.Location = new System.Drawing.Point(6, 65);
            this.chkPrefixFiles.Name = "chkPrefixFiles";
            this.chkPrefixFiles.Size = new System.Drawing.Size(99, 17);
            this.chkPrefixFiles.TabIndex = 4;
            this.chkPrefixFiles.Text = "Prefix filenames";
            this.toolTip1.SetToolTip(this.chkPrefixFiles, "If checked, files and folders representing individual objects, will be prefixed b" +
        "y a number indicating the object\'s order in the Model.bim metadata.");
            this.chkPrefixFiles.UseVisualStyleBackColor = true;
            // 
            // PreferencesForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(354, 377);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(370, 416);
            this.Name = "PreferencesForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Tabular Editor Preferences";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PreferencesForm_FormClosed);
            this.Load += new System.EventHandler(this.PreferencesForm_Load);
            this.Shown += new System.EventHandler(this.PreferencesForm_Shown);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.grpDeployment.ResumeLayout(false);
            this.grpDeployment.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkFixup;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.GroupBox grpDeployment;
        private System.Windows.Forms.Button btnFolder;
        private System.Windows.Forms.TextBox txtBackupPath;
        private System.Windows.Forms.CheckBox chkAutoBackup;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkAutoUpdate;
        private System.Windows.Forms.Button btnVersionCheck;
        private System.Windows.Forms.Label lblAvailableVersion;
        private System.Windows.Forms.Label lblCurrentVersion;
        private System.Windows.Forms.CheckBox chkIgnoreTimestamps;
        private System.Windows.Forms.CheckBox chkIgnoreInfObjects;
        private System.Windows.Forms.CheckBox chkIgnoreInfProps;
        private System.Windows.Forms.CheckBox chkSplitMultiline;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TreeView treeView1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkPrefixFiles;
    }
}