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
            System.Windows.Forms.TreeNode treeNode23 = new System.Windows.Forms.TreeNode("Data Sources");
            System.Windows.Forms.TreeNode treeNode24 = new System.Windows.Forms.TreeNode("Perspectives");
            System.Windows.Forms.TreeNode treeNode25 = new System.Windows.Forms.TreeNode("Relationships");
            System.Windows.Forms.TreeNode treeNode26 = new System.Windows.Forms.TreeNode("Roles");
            System.Windows.Forms.TreeNode treeNode27 = new System.Windows.Forms.TreeNode("Annotations");
            System.Windows.Forms.TreeNode treeNode28 = new System.Windows.Forms.TreeNode("Columns");
            System.Windows.Forms.TreeNode treeNode29 = new System.Windows.Forms.TreeNode("Hierarchies");
            System.Windows.Forms.TreeNode treeNode30 = new System.Windows.Forms.TreeNode("Measures");
            System.Windows.Forms.TreeNode treeNode31 = new System.Windows.Forms.TreeNode("Partitions");
            System.Windows.Forms.TreeNode treeNode32 = new System.Windows.Forms.TreeNode("Tables", new System.Windows.Forms.TreeNode[] {
            treeNode27,
            treeNode28,
            treeNode29,
            treeNode30,
            treeNode31});
            System.Windows.Forms.TreeNode treeNode33 = new System.Windows.Forms.TreeNode("Translations");
            System.Windows.Forms.TreeNode treeNode34 = new System.Windows.Forms.TreeNode("Data Sources");
            System.Windows.Forms.TreeNode treeNode35 = new System.Windows.Forms.TreeNode("Perspectives");
            System.Windows.Forms.TreeNode treeNode36 = new System.Windows.Forms.TreeNode("Relationships");
            System.Windows.Forms.TreeNode treeNode37 = new System.Windows.Forms.TreeNode("Roles");
            System.Windows.Forms.TreeNode treeNode38 = new System.Windows.Forms.TreeNode("Annotations");
            System.Windows.Forms.TreeNode treeNode39 = new System.Windows.Forms.TreeNode("Columns");
            System.Windows.Forms.TreeNode treeNode40 = new System.Windows.Forms.TreeNode("Hierarchies");
            System.Windows.Forms.TreeNode treeNode41 = new System.Windows.Forms.TreeNode("Measures");
            System.Windows.Forms.TreeNode treeNode42 = new System.Windows.Forms.TreeNode("Partitions");
            System.Windows.Forms.TreeNode treeNode43 = new System.Windows.Forms.TreeNode("Tables", new System.Windows.Forms.TreeNode[] {
            treeNode38,
            treeNode39,
            treeNode40,
            treeNode41,
            treeNode42});
            System.Windows.Forms.TreeNode treeNode44 = new System.Windows.Forms.TreeNode("Translations");
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.chkFixup = new System.Windows.Forms.CheckBox();
            this.chkIgnoreTimestamps = new System.Windows.Forms.CheckBox();
            this.chkIgnoreInfObjects = new System.Windows.Forms.CheckBox();
            this.chkIgnoreInfProps = new System.Windows.Forms.CheckBox();
            this.chkSplitMultiline = new System.Windows.Forms.CheckBox();
            this.chkLocalTranslations = new System.Windows.Forms.CheckBox();
            this.chkLocalPerspectives = new System.Windows.Forms.CheckBox();
            this.chkLocalRelationships = new System.Windows.Forms.CheckBox();
            this.chkAllowUnsupportedPBIFeatures = new System.Windows.Forms.CheckBox();
            this.chkCopyIncludeOLS = new System.Windows.Forms.CheckBox();
            this.chkCopyIncludeRLS = new System.Windows.Forms.CheckBox();
            this.chkCopyIncludePerspectives = new System.Windows.Forms.CheckBox();
            this.chkCopyIncludeTranslations = new System.Windows.Forms.CheckBox();
            this.chkAutoBackup = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.grpDeployment = new System.Windows.Forms.GroupBox();
            this.btnFolder = new System.Windows.Forms.Button();
            this.txtBackupPath = new System.Windows.Forms.TextBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkAutoUpdate = new System.Windows.Forms.CheckBox();
            this.btnVersionCheck = new System.Windows.Forms.Button();
            this.lblAvailableVersion = new System.Windows.Forms.Label();
            this.lblCurrentVersion = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tabCurrentModel = new System.Windows.Forms.TabPage();
            this.chkPrefixFiles = new System.Windows.Forms.CheckBox();
            this.grpSaveToFolder = new System.Windows.Forms.GroupBox();
            this.chkPrefixFilesCM = new System.Windows.Forms.CheckBox();
            this.chkLocalRelationshipsCM = new System.Windows.Forms.CheckBox();
            this.chkLocalTranslationsCM = new System.Windows.Forms.CheckBox();
            this.chkLocalPerspectivesCM = new System.Windows.Forms.CheckBox();
            this.treeView2 = new System.Windows.Forms.TreeView();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.chkSplitMultilineCM = new System.Windows.Forms.CheckBox();
            this.chkIgnoreInfPropsCM = new System.Windows.Forms.CheckBox();
            this.chkIgnoreInfObjectsCM = new System.Windows.Forms.CheckBox();
            this.chkIgnoreTimestampsCM = new System.Windows.Forms.CheckBox();
            this.groupBox3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.grpDeployment.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.tabCurrentModel.SuspendLayout();
            this.grpSaveToFolder.SuspendLayout();
            this.groupBox8.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(233, 407);
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
            this.btnCancel.Location = new System.Drawing.Point(314, 407);
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
            // chkLocalTranslations
            // 
            this.chkLocalTranslations.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkLocalTranslations.AutoSize = true;
            this.chkLocalTranslations.Location = new System.Drawing.Point(6, 238);
            this.chkLocalTranslations.Name = "chkLocalTranslations";
            this.chkLocalTranslations.Size = new System.Drawing.Size(171, 17);
            this.chkLocalTranslations.TabIndex = 8;
            this.chkLocalTranslations.Text = "Serialize translations per-object";
            this.toolTip1.SetToolTip(this.chkLocalTranslations, "If checked, all translatable objects (measures, columns, etc.) will have their tr" +
        "anslations stored as an annotation within the object itself.");
            this.chkLocalTranslations.UseVisualStyleBackColor = true;
            this.chkLocalTranslations.CheckedChanged += new System.EventHandler(this.chkLocalTranslations_CheckedChanged);
            // 
            // chkLocalPerspectives
            // 
            this.chkLocalPerspectives.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkLocalPerspectives.AutoSize = true;
            this.chkLocalPerspectives.Location = new System.Drawing.Point(6, 215);
            this.chkLocalPerspectives.Name = "chkLocalPerspectives";
            this.chkLocalPerspectives.Size = new System.Drawing.Size(227, 17);
            this.chkLocalPerspectives.TabIndex = 7;
            this.chkLocalPerspectives.Text = "Serialize perspective information per-object";
            this.toolTip1.SetToolTip(this.chkLocalPerspectives, "If checked, all objects that can be toggled in a perspective, will have their per" +
        "spective membership information stored as an annotation within the object itself" +
        ".");
            this.chkLocalPerspectives.UseVisualStyleBackColor = true;
            this.chkLocalPerspectives.CheckedChanged += new System.EventHandler(this.chkLocalPerspectives_CheckedChanged);
            // 
            // chkLocalRelationships
            // 
            this.chkLocalRelationships.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkLocalRelationships.AutoSize = true;
            this.chkLocalRelationships.Location = new System.Drawing.Point(6, 261);
            this.chkLocalRelationships.Name = "chkLocalRelationships";
            this.chkLocalRelationships.Size = new System.Drawing.Size(231, 17);
            this.chkLocalRelationships.TabIndex = 9;
            this.chkLocalRelationships.Text = "Serialize relationships by their starting tables";
            this.toolTip1.SetToolTip(this.chkLocalRelationships, "If checked, all relationships will be stored as a separate file on the \"From\" sid" +
        "e of the relationship (typically fact tables).");
            this.chkLocalRelationships.UseVisualStyleBackColor = true;
            this.chkLocalRelationships.CheckedChanged += new System.EventHandler(this.chkLocalRelationships_CheckedChanged);
            // 
            // chkAllowUnsupportedPBIFeatures
            // 
            this.chkAllowUnsupportedPBIFeatures.AutoSize = true;
            this.chkAllowUnsupportedPBIFeatures.Location = new System.Drawing.Point(6, 21);
            this.chkAllowUnsupportedPBIFeatures.Name = "chkAllowUnsupportedPBIFeatures";
            this.chkAllowUnsupportedPBIFeatures.Size = new System.Drawing.Size(268, 17);
            this.chkAllowUnsupportedPBIFeatures.TabIndex = 0;
            this.chkAllowUnsupportedPBIFeatures.Text = "Allow unsupported Power BI features (experimental)";
            this.toolTip1.SetToolTip(this.chkAllowUnsupportedPBIFeatures, "Checking this, will let you edit all TOM objects and properties when connected to" +
        " a Power BI data model. USE ONLY FOR EXPERIMENTAL PURPOSES.");
            this.chkAllowUnsupportedPBIFeatures.UseVisualStyleBackColor = true;
            // 
            // chkCopyIncludeOLS
            // 
            this.chkCopyIncludeOLS.AutoSize = true;
            this.chkCopyIncludeOLS.Location = new System.Drawing.Point(6, 88);
            this.chkCopyIncludeOLS.Name = "chkCopyIncludeOLS";
            this.chkCopyIncludeOLS.Size = new System.Drawing.Size(157, 17);
            this.chkCopyIncludeOLS.TabIndex = 3;
            this.chkCopyIncludeOLS.Text = "Include object-level security";
            this.toolTip1.SetToolTip(this.chkCopyIncludeOLS, "Include object-level security when copying tables, columns, hierarchies or measur" +
        "es (Compatibility Level 1400 only)");
            this.chkCopyIncludeOLS.UseVisualStyleBackColor = true;
            // 
            // chkCopyIncludeRLS
            // 
            this.chkCopyIncludeRLS.AutoSize = true;
            this.chkCopyIncludeRLS.Location = new System.Drawing.Point(6, 65);
            this.chkCopyIncludeRLS.Name = "chkCopyIncludeRLS";
            this.chkCopyIncludeRLS.Size = new System.Drawing.Size(145, 17);
            this.chkCopyIncludeRLS.TabIndex = 2;
            this.chkCopyIncludeRLS.Text = "Include row-level security";
            this.toolTip1.SetToolTip(this.chkCopyIncludeRLS, "Include row-level security when copying tables.");
            this.chkCopyIncludeRLS.UseVisualStyleBackColor = true;
            // 
            // chkCopyIncludePerspectives
            // 
            this.chkCopyIncludePerspectives.AutoSize = true;
            this.chkCopyIncludePerspectives.Location = new System.Drawing.Point(6, 42);
            this.chkCopyIncludePerspectives.Name = "chkCopyIncludePerspectives";
            this.chkCopyIncludePerspectives.Size = new System.Drawing.Size(178, 17);
            this.chkCopyIncludePerspectives.TabIndex = 1;
            this.chkCopyIncludePerspectives.Text = "Include perspective membership";
            this.toolTip1.SetToolTip(this.chkCopyIncludePerspectives, "Include perspective membership when copying objects that can be toggled in perspe" +
        "ctives");
            this.chkCopyIncludePerspectives.UseVisualStyleBackColor = true;
            // 
            // chkCopyIncludeTranslations
            // 
            this.chkCopyIncludeTranslations.AutoSize = true;
            this.chkCopyIncludeTranslations.Location = new System.Drawing.Point(6, 19);
            this.chkCopyIncludeTranslations.Name = "chkCopyIncludeTranslations";
            this.chkCopyIncludeTranslations.Size = new System.Drawing.Size(117, 17);
            this.chkCopyIncludeTranslations.TabIndex = 0;
            this.chkCopyIncludeTranslations.Text = "Include translations";
            this.toolTip1.SetToolTip(this.chkCopyIncludeTranslations, "Include translations when copying translatable objects");
            this.chkCopyIncludeTranslations.UseVisualStyleBackColor = true;
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
            this.chkAutoBackup.CheckedChanged += new System.EventHandler(this.chkAutoBackup_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.chkFixup);
            this.groupBox3.Location = new System.Drawing.Point(6, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(357, 50);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Editing";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Controls.Add(this.tabCurrentModel);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(377, 389);
            this.tabControl1.TabIndex = 2;
            // 
            // tabPage3
            // 
            this.tabPage3.Controls.Add(this.grpDeployment);
            this.tabPage3.Controls.Add(this.groupBox7);
            this.tabPage3.Controls.Add(this.groupBox6);
            this.tabPage3.Controls.Add(this.groupBox1);
            this.tabPage3.Location = new System.Drawing.Point(4, 22);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage3.Size = new System.Drawing.Size(369, 363);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "General";
            this.tabPage3.UseVisualStyleBackColor = true;
            // 
            // grpDeployment
            // 
            this.grpDeployment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDeployment.Controls.Add(this.btnFolder);
            this.grpDeployment.Controls.Add(this.txtBackupPath);
            this.grpDeployment.Controls.Add(this.chkAutoBackup);
            this.grpDeployment.Location = new System.Drawing.Point(6, 285);
            this.grpDeployment.Name = "grpDeployment";
            this.grpDeployment.Size = new System.Drawing.Size(356, 72);
            this.grpDeployment.TabIndex = 7;
            this.grpDeployment.TabStop = false;
            this.grpDeployment.Text = "Deployment Options";
            // 
            // btnFolder
            // 
            this.btnFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFolder.Enabled = false;
            this.btnFolder.Location = new System.Drawing.Point(323, 42);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(27, 20);
            this.btnFolder.TabIndex = 8;
            this.btnFolder.Text = "...";
            this.btnFolder.UseVisualStyleBackColor = true;
            this.btnFolder.Click += new System.EventHandler(this.btnFolder_Click);
            // 
            // txtBackupPath
            // 
            this.txtBackupPath.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtBackupPath.Enabled = false;
            this.txtBackupPath.Location = new System.Drawing.Point(6, 42);
            this.txtBackupPath.Name = "txtBackupPath";
            this.txtBackupPath.Size = new System.Drawing.Size(311, 20);
            this.txtBackupPath.TabIndex = 7;
            // 
            // groupBox7
            // 
            this.groupBox7.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox7.Controls.Add(this.chkCopyIncludeOLS);
            this.groupBox7.Controls.Add(this.chkCopyIncludeRLS);
            this.groupBox7.Controls.Add(this.chkCopyIncludePerspectives);
            this.groupBox7.Controls.Add(this.chkCopyIncludeTranslations);
            this.groupBox7.Location = new System.Drawing.Point(6, 162);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(357, 117);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Clipboard Operations";
            // 
            // groupBox6
            // 
            this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox6.Controls.Add(this.chkAllowUnsupportedPBIFeatures);
            this.groupBox6.Location = new System.Drawing.Point(6, 111);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(357, 45);
            this.groupBox6.TabIndex = 2;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Power BI Support";
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
            this.groupBox1.Size = new System.Drawing.Size(357, 99);
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
            this.btnVersionCheck.Click += new System.EventHandler(this.btnVersionCheck_Click);
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
            this.lblAvailableVersion.Click += new System.EventHandler(this.lblAvailableVersion_Click);
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
            // tabPage2
            // 
            this.tabPage2.Controls.Add(this.groupBox3);
            this.tabPage2.Location = new System.Drawing.Point(4, 22);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage2.Size = new System.Drawing.Size(369, 363);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "DAX";
            this.tabPage2.UseVisualStyleBackColor = true;
            // 
            // tabPage4
            // 
            this.tabPage4.Controls.Add(this.groupBox4);
            this.tabPage4.Controls.Add(this.groupBox2);
            this.tabPage4.Location = new System.Drawing.Point(4, 22);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage4.Size = new System.Drawing.Size(369, 363);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Serialization";
            this.tabPage4.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.chkPrefixFiles);
            this.groupBox4.Controls.Add(this.chkLocalRelationships);
            this.groupBox4.Controls.Add(this.chkLocalTranslations);
            this.groupBox4.Controls.Add(this.chkLocalPerspectives);
            this.groupBox4.Controls.Add(this.treeView1);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Location = new System.Drawing.Point(6, 77);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(357, 283);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Save to Folder Serialization";
            // 
            // treeView1
            // 
            this.treeView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView1.CheckBoxes = true;
            this.treeView1.Location = new System.Drawing.Point(6, 73);
            this.treeView1.Name = "treeView1";
            treeNode23.Checked = true;
            treeNode23.Name = "Data Sources";
            treeNode23.Text = "Data Sources";
            treeNode24.Checked = true;
            treeNode24.Name = "Perspectives";
            treeNode24.Text = "Perspectives";
            treeNode25.Checked = true;
            treeNode25.Name = "Relationships";
            treeNode25.Text = "Relationships";
            treeNode26.Checked = true;
            treeNode26.Name = "Roles";
            treeNode26.Text = "Roles";
            treeNode27.Name = "Annotations";
            treeNode27.Text = "Annotations";
            treeNode28.Checked = true;
            treeNode28.Name = "Columns";
            treeNode28.Text = "Columns";
            treeNode29.Checked = true;
            treeNode29.Name = "Hierarchies";
            treeNode29.Text = "Hierarchies";
            treeNode30.Checked = true;
            treeNode30.Name = "Measures";
            treeNode30.Text = "Measures";
            treeNode31.Checked = true;
            treeNode31.Name = "Partitions";
            treeNode31.Text = "Partitions";
            treeNode32.Checked = true;
            treeNode32.Name = "Tables";
            treeNode32.Text = "Tables";
            treeNode33.Checked = true;
            treeNode33.Name = "Translations";
            treeNode33.Text = "Translations";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode23,
            treeNode24,
            treeNode25,
            treeNode26,
            treeNode32,
            treeNode33});
            this.treeView1.PathSeparator = "/";
            this.treeView1.ShowLines = false;
            this.treeView1.Size = new System.Drawing.Size(345, 136);
            this.treeView1.TabIndex = 1;
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(6, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(344, 31);
            this.label1.TabIndex = 0;
            this.label1.Text = "Check the type of objects you wish to serialize as individual files (unchecked ob" +
    "jects are serialized in the parent object):";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.chkSplitMultiline);
            this.groupBox2.Controls.Add(this.chkIgnoreInfProps);
            this.groupBox2.Controls.Add(this.chkIgnoreInfObjects);
            this.groupBox2.Controls.Add(this.chkIgnoreTimestamps);
            this.groupBox2.Location = new System.Drawing.Point(6, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(357, 65);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General Serialization Settings";
            // 
            // tabCurrentModel
            // 
            this.tabCurrentModel.Controls.Add(this.grpSaveToFolder);
            this.tabCurrentModel.Controls.Add(this.groupBox8);
            this.tabCurrentModel.Location = new System.Drawing.Point(4, 22);
            this.tabCurrentModel.Name = "tabCurrentModel";
            this.tabCurrentModel.Padding = new System.Windows.Forms.Padding(3);
            this.tabCurrentModel.Size = new System.Drawing.Size(369, 363);
            this.tabCurrentModel.TabIndex = 5;
            this.tabCurrentModel.Text = "Current Model";
            this.tabCurrentModel.UseVisualStyleBackColor = true;
            // 
            // chkPrefixFiles
            // 
            this.chkPrefixFiles.AutoSize = true;
            this.chkPrefixFiles.Location = new System.Drawing.Point(6, 19);
            this.chkPrefixFiles.Name = "chkPrefixFiles";
            this.chkPrefixFiles.Size = new System.Drawing.Size(99, 17);
            this.chkPrefixFiles.TabIndex = 10;
            this.chkPrefixFiles.Text = "Prefix filenames";
            this.toolTip1.SetToolTip(this.chkPrefixFiles, "If checked, files and folders representing individual objects, will be prefixed b" +
        "y a number indicating the object\'s order in the Model.bim metadata.");
            this.chkPrefixFiles.UseVisualStyleBackColor = true;
            // 
            // grpSaveToFolder
            // 
            this.grpSaveToFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSaveToFolder.Controls.Add(this.chkPrefixFilesCM);
            this.grpSaveToFolder.Controls.Add(this.chkLocalRelationshipsCM);
            this.grpSaveToFolder.Controls.Add(this.chkLocalTranslationsCM);
            this.grpSaveToFolder.Controls.Add(this.chkLocalPerspectivesCM);
            this.grpSaveToFolder.Controls.Add(this.treeView2);
            this.grpSaveToFolder.Controls.Add(this.label2);
            this.grpSaveToFolder.Location = new System.Drawing.Point(6, 77);
            this.grpSaveToFolder.Name = "grpSaveToFolder";
            this.grpSaveToFolder.Size = new System.Drawing.Size(357, 283);
            this.grpSaveToFolder.TabIndex = 6;
            this.grpSaveToFolder.TabStop = false;
            this.grpSaveToFolder.Text = "Current Model \"Save to Folder\" Settings";
            // 
            // chkPrefixFilesCM
            // 
            this.chkPrefixFilesCM.AutoSize = true;
            this.chkPrefixFilesCM.Location = new System.Drawing.Point(6, 19);
            this.chkPrefixFilesCM.Name = "chkPrefixFilesCM";
            this.chkPrefixFilesCM.Size = new System.Drawing.Size(99, 17);
            this.chkPrefixFilesCM.TabIndex = 10;
            this.chkPrefixFilesCM.Text = "Prefix filenames";
            this.toolTip1.SetToolTip(this.chkPrefixFilesCM, "If checked, files and folders representing individual objects, will be prefixed b" +
        "y a number indicating the object\'s order in the Model.bim metadata.");
            this.chkPrefixFilesCM.UseVisualStyleBackColor = true;
            // 
            // chkLocalRelationshipsCM
            // 
            this.chkLocalRelationshipsCM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkLocalRelationshipsCM.AutoSize = true;
            this.chkLocalRelationshipsCM.Location = new System.Drawing.Point(6, 261);
            this.chkLocalRelationshipsCM.Name = "chkLocalRelationshipsCM";
            this.chkLocalRelationshipsCM.Size = new System.Drawing.Size(231, 17);
            this.chkLocalRelationshipsCM.TabIndex = 9;
            this.chkLocalRelationshipsCM.Text = "Serialize relationships by their starting tables";
            this.toolTip1.SetToolTip(this.chkLocalRelationshipsCM, "If checked, all relationships will be stored as a separate file on the \"From\" sid" +
        "e of the relationship (typically fact tables).");
            this.chkLocalRelationshipsCM.UseVisualStyleBackColor = true;
            this.chkLocalRelationshipsCM.CheckedChanged += new System.EventHandler(this.chkLocalRelationshipsCM_CheckedChanged);
            // 
            // chkLocalTranslationsCM
            // 
            this.chkLocalTranslationsCM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkLocalTranslationsCM.AutoSize = true;
            this.chkLocalTranslationsCM.Location = new System.Drawing.Point(6, 238);
            this.chkLocalTranslationsCM.Name = "chkLocalTranslationsCM";
            this.chkLocalTranslationsCM.Size = new System.Drawing.Size(171, 17);
            this.chkLocalTranslationsCM.TabIndex = 8;
            this.chkLocalTranslationsCM.Text = "Serialize translations per-object";
            this.toolTip1.SetToolTip(this.chkLocalTranslationsCM, "If checked, all translatable objects (measures, columns, etc.) will have their tr" +
        "anslations stored as an annotation within the object itself.");
            this.chkLocalTranslationsCM.UseVisualStyleBackColor = true;
            this.chkLocalTranslationsCM.CheckedChanged += new System.EventHandler(this.chkLocalTranslationsCM_CheckedChanged);
            // 
            // chkLocalPerspectivesCM
            // 
            this.chkLocalPerspectivesCM.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkLocalPerspectivesCM.AutoSize = true;
            this.chkLocalPerspectivesCM.Location = new System.Drawing.Point(6, 215);
            this.chkLocalPerspectivesCM.Name = "chkLocalPerspectivesCM";
            this.chkLocalPerspectivesCM.Size = new System.Drawing.Size(227, 17);
            this.chkLocalPerspectivesCM.TabIndex = 7;
            this.chkLocalPerspectivesCM.Text = "Serialize perspective information per-object";
            this.toolTip1.SetToolTip(this.chkLocalPerspectivesCM, "If checked, all objects that can be toggled in a perspective, will have their per" +
        "spective membership information stored as an annotation within the object itself" +
        ".");
            this.chkLocalPerspectivesCM.UseVisualStyleBackColor = true;
            this.chkLocalPerspectivesCM.CheckedChanged += new System.EventHandler(this.chkLocalPerspectivesCM_CheckedChanged);
            // 
            // treeView2
            // 
            this.treeView2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView2.CheckBoxes = true;
            this.treeView2.Location = new System.Drawing.Point(6, 73);
            this.treeView2.Name = "treeView2";
            treeNode34.Checked = true;
            treeNode34.Name = "Data Sources";
            treeNode34.Text = "Data Sources";
            treeNode35.Checked = true;
            treeNode35.Name = "Perspectives";
            treeNode35.Text = "Perspectives";
            treeNode36.Checked = true;
            treeNode36.Name = "Relationships";
            treeNode36.Text = "Relationships";
            treeNode37.Checked = true;
            treeNode37.Name = "Roles";
            treeNode37.Text = "Roles";
            treeNode38.Name = "Annotations";
            treeNode38.Text = "Annotations";
            treeNode39.Checked = true;
            treeNode39.Name = "Columns";
            treeNode39.Text = "Columns";
            treeNode40.Checked = true;
            treeNode40.Name = "Hierarchies";
            treeNode40.Text = "Hierarchies";
            treeNode41.Checked = true;
            treeNode41.Name = "Measures";
            treeNode41.Text = "Measures";
            treeNode42.Checked = true;
            treeNode42.Name = "Partitions";
            treeNode42.Text = "Partitions";
            treeNode43.Checked = true;
            treeNode43.Name = "Tables";
            treeNode43.Text = "Tables";
            treeNode44.Checked = true;
            treeNode44.Name = "Translations";
            treeNode44.Text = "Translations";
            this.treeView2.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode34,
            treeNode35,
            treeNode36,
            treeNode37,
            treeNode43,
            treeNode44});
            this.treeView2.PathSeparator = "/";
            this.treeView2.ShowLines = false;
            this.treeView2.Size = new System.Drawing.Size(345, 136);
            this.treeView2.TabIndex = 1;
            this.treeView2.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView2_AfterCheck);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(6, 39);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(344, 31);
            this.label2.TabIndex = 0;
            this.label2.Text = "Check the type of objects you wish to serialize as individual files (unchecked ob" +
    "jects are serialized in the parent object):";
            // 
            // groupBox8
            // 
            this.groupBox8.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox8.Controls.Add(this.chkSplitMultilineCM);
            this.groupBox8.Controls.Add(this.chkIgnoreInfPropsCM);
            this.groupBox8.Controls.Add(this.chkIgnoreInfObjectsCM);
            this.groupBox8.Controls.Add(this.chkIgnoreTimestampsCM);
            this.groupBox8.Location = new System.Drawing.Point(6, 6);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(357, 65);
            this.groupBox8.TabIndex = 5;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Current Model Serialization Settings";
            // 
            // chkSplitMultilineCM
            // 
            this.chkSplitMultilineCM.AutoSize = true;
            this.chkSplitMultilineCM.Location = new System.Drawing.Point(143, 42);
            this.chkSplitMultilineCM.Name = "chkSplitMultilineCM";
            this.chkSplitMultilineCM.Size = new System.Drawing.Size(114, 17);
            this.chkSplitMultilineCM.TabIndex = 3;
            this.chkSplitMultilineCM.Text = "Split multiline string";
            this.toolTip1.SetToolTip(this.chkSplitMultilineCM, "If checked, expressions and other multiline string properties are serialized as a" +
        " JSON array.");
            this.chkSplitMultilineCM.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreInfPropsCM
            // 
            this.chkIgnoreInfPropsCM.AutoSize = true;
            this.chkIgnoreInfPropsCM.Location = new System.Drawing.Point(143, 19);
            this.chkIgnoreInfPropsCM.Name = "chkIgnoreInfPropsCM";
            this.chkIgnoreInfPropsCM.Size = new System.Drawing.Size(143, 17);
            this.chkIgnoreInfPropsCM.TabIndex = 2;
            this.chkIgnoreInfPropsCM.Text = "Ignore inferred properties";
            this.toolTip1.SetToolTip(this.chkIgnoreInfPropsCM, "If checked, objects whose value is controlled by the server (Column.State, etc.) " +
        "are not serialized into the .json files");
            this.chkIgnoreInfPropsCM.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreInfObjectsCM
            // 
            this.chkIgnoreInfObjectsCM.AutoSize = true;
            this.chkIgnoreInfObjectsCM.Location = new System.Drawing.Point(6, 19);
            this.chkIgnoreInfObjectsCM.Name = "chkIgnoreInfObjectsCM";
            this.chkIgnoreInfObjectsCM.Size = new System.Drawing.Size(131, 17);
            this.chkIgnoreInfObjectsCM.TabIndex = 1;
            this.chkIgnoreInfObjectsCM.Text = "Ignore inferred objects";
            this.toolTip1.SetToolTip(this.chkIgnoreInfObjectsCM, "If checked, objects whose lifetime is controlled by the server (RowNumber, attrib" +
        "ute hierarchies, etc.) are not serialized into the .json files");
            this.chkIgnoreInfObjectsCM.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreTimestampsCM
            // 
            this.chkIgnoreTimestampsCM.AutoSize = true;
            this.chkIgnoreTimestampsCM.Location = new System.Drawing.Point(6, 42);
            this.chkIgnoreTimestampsCM.Name = "chkIgnoreTimestampsCM";
            this.chkIgnoreTimestampsCM.Size = new System.Drawing.Size(111, 17);
            this.chkIgnoreTimestampsCM.TabIndex = 0;
            this.chkIgnoreTimestampsCM.Text = "Ignore timestamps";
            this.toolTip1.SetToolTip(this.chkIgnoreTimestampsCM, "If checked, editing timestamps are not serialized into the .json files");
            this.chkIgnoreTimestampsCM.UseVisualStyleBackColor = true;
            // 
            // PreferencesForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(401, 442);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(417, 481);
            this.Name = "PreferencesForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Tabular Editor Preferences";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PreferencesForm_FormClosed);
            this.Load += new System.EventHandler(this.PreferencesForm_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.tabControl1.ResumeLayout(false);
            this.tabPage3.ResumeLayout(false);
            this.grpDeployment.ResumeLayout(false);
            this.grpDeployment.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage4.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.tabCurrentModel.ResumeLayout(false);
            this.grpSaveToFolder.ResumeLayout(false);
            this.grpSaveToFolder.PerformLayout();
            this.groupBox8.ResumeLayout(false);
            this.groupBox8.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkFixup;
        private System.Windows.Forms.TabControl tabControl1;
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
        private System.Windows.Forms.CheckBox chkLocalTranslations;
        private System.Windows.Forms.CheckBox chkLocalPerspectives;
        private System.Windows.Forms.CheckBox chkLocalRelationships;
        private System.Windows.Forms.GroupBox grpDeployment;
        private System.Windows.Forms.Button btnFolder;
        private System.Windows.Forms.TextBox txtBackupPath;
        private System.Windows.Forms.CheckBox chkAutoBackup;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.CheckBox chkCopyIncludeOLS;
        private System.Windows.Forms.CheckBox chkCopyIncludeRLS;
        private System.Windows.Forms.CheckBox chkCopyIncludePerspectives;
        private System.Windows.Forms.CheckBox chkCopyIncludeTranslations;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.CheckBox chkAllowUnsupportedPBIFeatures;
        private System.Windows.Forms.TabPage tabCurrentModel;
        private System.Windows.Forms.CheckBox chkPrefixFiles;
        private System.Windows.Forms.GroupBox grpSaveToFolder;
        private System.Windows.Forms.CheckBox chkPrefixFilesCM;
        private System.Windows.Forms.CheckBox chkLocalRelationshipsCM;
        private System.Windows.Forms.CheckBox chkLocalTranslationsCM;
        private System.Windows.Forms.CheckBox chkLocalPerspectivesCM;
        private System.Windows.Forms.TreeView treeView2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox8;
        private System.Windows.Forms.CheckBox chkSplitMultilineCM;
        private System.Windows.Forms.CheckBox chkIgnoreInfPropsCM;
        private System.Windows.Forms.CheckBox chkIgnoreInfObjectsCM;
        private System.Windows.Forms.CheckBox chkIgnoreTimestampsCM;
    }
}