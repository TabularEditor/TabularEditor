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
            System.Windows.Forms.TreeNode treeNode27 = new System.Windows.Forms.TreeNode("Data Sources");
            System.Windows.Forms.TreeNode treeNode28 = new System.Windows.Forms.TreeNode("Shared Expressions");
            System.Windows.Forms.TreeNode treeNode29 = new System.Windows.Forms.TreeNode("Perspectives");
            System.Windows.Forms.TreeNode treeNode30 = new System.Windows.Forms.TreeNode("Relationships");
            System.Windows.Forms.TreeNode treeNode31 = new System.Windows.Forms.TreeNode("Roles");
            System.Windows.Forms.TreeNode treeNode32 = new System.Windows.Forms.TreeNode("Annotations");
            System.Windows.Forms.TreeNode treeNode33 = new System.Windows.Forms.TreeNode("Calculation Items");
            System.Windows.Forms.TreeNode treeNode34 = new System.Windows.Forms.TreeNode("Columns");
            System.Windows.Forms.TreeNode treeNode35 = new System.Windows.Forms.TreeNode("Hierarchies");
            System.Windows.Forms.TreeNode treeNode36 = new System.Windows.Forms.TreeNode("Measures");
            System.Windows.Forms.TreeNode treeNode37 = new System.Windows.Forms.TreeNode("Partitions");
            System.Windows.Forms.TreeNode treeNode38 = new System.Windows.Forms.TreeNode("Tables", new System.Windows.Forms.TreeNode[] {
            treeNode32,
            treeNode33,
            treeNode34,
            treeNode35,
            treeNode36,
            treeNode37});
            System.Windows.Forms.TreeNode treeNode39 = new System.Windows.Forms.TreeNode("Translations");
            System.Windows.Forms.TreeNode treeNode40 = new System.Windows.Forms.TreeNode("Data Sources");
            System.Windows.Forms.TreeNode treeNode41 = new System.Windows.Forms.TreeNode("Shared Expressions");
            System.Windows.Forms.TreeNode treeNode42 = new System.Windows.Forms.TreeNode("Perspectives");
            System.Windows.Forms.TreeNode treeNode43 = new System.Windows.Forms.TreeNode("Relationships");
            System.Windows.Forms.TreeNode treeNode44 = new System.Windows.Forms.TreeNode("Roles");
            System.Windows.Forms.TreeNode treeNode45 = new System.Windows.Forms.TreeNode("Annotations");
            System.Windows.Forms.TreeNode treeNode46 = new System.Windows.Forms.TreeNode("Calculation Items");
            System.Windows.Forms.TreeNode treeNode47 = new System.Windows.Forms.TreeNode("Columns");
            System.Windows.Forms.TreeNode treeNode48 = new System.Windows.Forms.TreeNode("Hierarchies");
            System.Windows.Forms.TreeNode treeNode49 = new System.Windows.Forms.TreeNode("Measures");
            System.Windows.Forms.TreeNode treeNode50 = new System.Windows.Forms.TreeNode("Partitions");
            System.Windows.Forms.TreeNode treeNode51 = new System.Windows.Forms.TreeNode("Tables", new System.Windows.Forms.TreeNode[] {
            treeNode45,
            treeNode46,
            treeNode47,
            treeNode48,
            treeNode49,
            treeNode50});
            System.Windows.Forms.TreeNode treeNode52 = new System.Windows.Forms.TreeNode("Translations");
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
            this.chkChangeDetectionLocalServer = new System.Windows.Forms.CheckBox();
            this.chkCopyIncludeOLS = new System.Windows.Forms.CheckBox();
            this.chkCopyIncludeRLS = new System.Windows.Forms.CheckBox();
            this.chkCopyIncludePerspectives = new System.Windows.Forms.CheckBox();
            this.chkCopyIncludeTranslations = new System.Windows.Forms.CheckBox();
            this.chkAutoBackup = new System.Windows.Forms.CheckBox();
            this.chkPrefixFiles = new System.Windows.Forms.CheckBox();
            this.chkPrefixFilesCM = new System.Windows.Forms.CheckBox();
            this.chkLocalRelationshipsCM = new System.Windows.Forms.CheckBox();
            this.chkLocalTranslationsCM = new System.Windows.Forms.CheckBox();
            this.chkLocalPerspectivesCM = new System.Windows.Forms.CheckBox();
            this.chkSplitMultilineCM = new System.Windows.Forms.CheckBox();
            this.chkIgnoreInfPropsCM = new System.Windows.Forms.CheckBox();
            this.chkIgnoreInfObjectsCM = new System.Windows.Forms.CheckBox();
            this.chkIgnoreTimestampsCM = new System.Windows.Forms.CheckBox();
            this.cmbSeparators = new System.Windows.Forms.ComboBox();
            this.chkBackgroundBpa = new System.Windows.Forms.CheckBox();
            this.chkAnnotateDeploymentMetadata = new System.Windows.Forms.CheckBox();
            this.chkIgnoreLineageTags = new System.Windows.Forms.CheckBox();
            this.chkIgnoreLineageTagsCM = new System.Windows.Forms.CheckBox();
            this.chkIgnorePrivacySettings = new System.Windows.Forms.CheckBox();
            this.chkIgnorePrivacySettingsCM = new System.Windows.Forms.CheckBox();
            this.chkIgnoreIncrementalRefreshPartitions = new System.Windows.Forms.CheckBox();
            this.chkIgnoreIncrementalRefreshPartitionsCM = new System.Windows.Forms.CheckBox();
            this.chkAlsoSaveAsBim = new System.Windows.Forms.CheckBox();
            this.chkAlsoSaveAsBimCM = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabFeatures = new System.Windows.Forms.TabPage();
            this.grpDeployment = new System.Windows.Forms.GroupBox();
            this.btnFolder = new System.Windows.Forms.Button();
            this.txtBackupPath = new System.Windows.Forms.TextBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.tabGeneral = new System.Windows.Forms.TabPage();
            this.groupBox10 = new System.Windows.Forms.GroupBox();
            this.linkLabel1 = new System.Windows.Forms.LinkLabel();
            this.label7 = new System.Windows.Forms.Label();
            this.txtCompilerOptions = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtCompilerPath = new System.Windows.Forms.TextBox();
            this.groupBox9 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtProxyPassword = new System.Windows.Forms.TextBox();
            this.txtProxyUser = new System.Windows.Forms.TextBox();
            this.txtProxyAddress = new System.Windows.Forms.TextBox();
            this.chkSystemProxy = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkAutoUpdate = new System.Windows.Forms.CheckBox();
            this.chkSkipPatch = new System.Windows.Forms.CheckBox();
            this.btnVersionCheck = new System.Windows.Forms.Button();
            this.lblAvailableVersion = new System.Windows.Forms.Label();
            this.lblCurrentVersion = new System.Windows.Forms.Label();
            this.tabDAX = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.tabSerialization = new System.Windows.Forms.TabPage();
            this.cmbSerializationMode = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.tvDefaultSerialization = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tabCurrentModel = new System.Windows.Forms.TabPage();
            this.grpSaveToFolder = new System.Windows.Forms.GroupBox();
            this.treeView2 = new System.Windows.Forms.TreeView();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox8 = new System.Windows.Forms.GroupBox();
            this.groupBox3.SuspendLayout();
            this.tabControl1.SuspendLayout();
            this.tabFeatures.SuspendLayout();
            this.grpDeployment.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.tabGeneral.SuspendLayout();
            this.groupBox10.SuspendLayout();
            this.groupBox9.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tabDAX.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.tabSerialization.SuspendLayout();
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
            this.btnOK.Location = new System.Drawing.Point(233, 423);
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
            this.btnCancel.Location = new System.Drawing.Point(314, 423);
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
            this.chkLocalTranslations.Location = new System.Drawing.Point(6, 176);
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
            this.chkLocalPerspectives.Location = new System.Drawing.Point(6, 154);
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
            this.chkLocalRelationships.Location = new System.Drawing.Point(6, 199);
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
            // chkChangeDetectionLocalServer
            // 
            this.chkChangeDetectionLocalServer.AutoSize = true;
            this.chkChangeDetectionLocalServer.Location = new System.Drawing.Point(6, 66);
            this.chkChangeDetectionLocalServer.Name = "chkChangeDetectionLocalServer";
            this.chkChangeDetectionLocalServer.Size = new System.Drawing.Size(270, 17);
            this.chkChangeDetectionLocalServer.TabIndex = 0;
            this.chkChangeDetectionLocalServer.Text = "Detect changes on local AS instance (experimental)";
            this.toolTip1.SetToolTip(this.chkChangeDetectionLocalServer, resources.GetString("chkChangeDetectionLocalServer.ToolTip"));
            this.chkChangeDetectionLocalServer.UseVisualStyleBackColor = true;
            // 
            // chkCopyIncludeOLS
            // 
            this.chkCopyIncludeOLS.AutoSize = true;
            this.chkCopyIncludeOLS.Location = new System.Drawing.Point(191, 42);
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
            this.chkCopyIncludeRLS.Location = new System.Drawing.Point(191, 19);
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
            this.chkAutoBackup.Location = new System.Drawing.Point(6, 44);
            this.chkAutoBackup.Name = "chkAutoBackup";
            this.chkAutoBackup.Size = new System.Drawing.Size(168, 17);
            this.chkAutoBackup.TabIndex = 6;
            this.chkAutoBackup.Text = "Save Model.bim backup here:";
            this.toolTip1.SetToolTip(this.chkAutoBackup, resources.GetString("chkAutoBackup.ToolTip"));
            this.chkAutoBackup.UseVisualStyleBackColor = true;
            this.chkAutoBackup.CheckedChanged += new System.EventHandler(this.chkAutoBackup_CheckedChanged);
            // 
            // chkPrefixFiles
            // 
            this.chkPrefixFiles.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkPrefixFiles.AutoSize = true;
            this.chkPrefixFiles.Location = new System.Drawing.Point(6, 131);
            this.chkPrefixFiles.Name = "chkPrefixFiles";
            this.chkPrefixFiles.Size = new System.Drawing.Size(99, 17);
            this.chkPrefixFiles.TabIndex = 10;
            this.chkPrefixFiles.Text = "Prefix filenames";
            this.toolTip1.SetToolTip(this.chkPrefixFiles, "If checked, files and folders representing individual objects, will be prefixed b" +
        "y a number indicating the object\'s order in the Model.bim metadata.");
            this.chkPrefixFiles.UseVisualStyleBackColor = true;
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
            this.chkLocalRelationshipsCM.Location = new System.Drawing.Point(6, 231);
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
            this.chkLocalTranslationsCM.Location = new System.Drawing.Point(6, 208);
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
            this.chkLocalPerspectivesCM.Location = new System.Drawing.Point(6, 185);
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
            // cmbSeparators
            // 
            this.cmbSeparators.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSeparators.FormattingEnabled = true;
            this.cmbSeparators.Items.AddRange(new object[] {
            "US/UK (A, B, C / 1.234)",
            "Others (A; B; C / 1,234)"});
            this.cmbSeparators.Location = new System.Drawing.Point(7, 20);
            this.cmbSeparators.Name = "cmbSeparators";
            this.cmbSeparators.Size = new System.Drawing.Size(179, 21);
            this.cmbSeparators.TabIndex = 0;
            this.toolTip1.SetToolTip(this.cmbSeparators, resources.GetString("cmbSeparators.ToolTip"));
            // 
            // chkBackgroundBpa
            // 
            this.chkBackgroundBpa.AutoSize = true;
            this.chkBackgroundBpa.Location = new System.Drawing.Point(6, 44);
            this.chkBackgroundBpa.Name = "chkBackgroundBpa";
            this.chkBackgroundBpa.Size = new System.Drawing.Size(223, 17);
            this.chkBackgroundBpa.TabIndex = 1;
            this.chkBackgroundBpa.Text = "Background scan for Best Practice issues";
            this.toolTip1.SetToolTip(this.chkBackgroundBpa, "Checking this, will let you edit all TOM objects and properties when connected to" +
        " a Power BI data model. USE ONLY FOR EXPERIMENTAL PURPOSES.");
            this.chkBackgroundBpa.UseVisualStyleBackColor = true;
            // 
            // chkAnnotateDeploymentMetadata
            // 
            this.chkAnnotateDeploymentMetadata.AutoSize = true;
            this.chkAnnotateDeploymentMetadata.Location = new System.Drawing.Point(7, 20);
            this.chkAnnotateDeploymentMetadata.Name = "chkAnnotateDeploymentMetadata";
            this.chkAnnotateDeploymentMetadata.Size = new System.Drawing.Size(229, 17);
            this.chkAnnotateDeploymentMetadata.TabIndex = 9;
            this.chkAnnotateDeploymentMetadata.Text = "Add metadata annotation upon deployment";
            this.toolTip1.SetToolTip(this.chkAnnotateDeploymentMetadata, resources.GetString("chkAnnotateDeploymentMetadata.ToolTip"));
            this.chkAnnotateDeploymentMetadata.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreLineageTags
            // 
            this.chkIgnoreLineageTags.AutoSize = true;
            this.chkIgnoreLineageTags.Location = new System.Drawing.Point(6, 65);
            this.chkIgnoreLineageTags.Name = "chkIgnoreLineageTags";
            this.chkIgnoreLineageTags.Size = new System.Drawing.Size(116, 17);
            this.chkIgnoreLineageTags.TabIndex = 4;
            this.chkIgnoreLineageTags.Text = "Ignore lineage tags";
            this.toolTip1.SetToolTip(this.chkIgnoreLineageTags, "If checked, expressions and other multiline string properties are serialized as a" +
        " JSON array.");
            this.chkIgnoreLineageTags.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreLineageTagsCM
            // 
            this.chkIgnoreLineageTagsCM.AutoSize = true;
            this.chkIgnoreLineageTagsCM.Location = new System.Drawing.Point(6, 65);
            this.chkIgnoreLineageTagsCM.Name = "chkIgnoreLineageTagsCM";
            this.chkIgnoreLineageTagsCM.Size = new System.Drawing.Size(116, 17);
            this.chkIgnoreLineageTagsCM.TabIndex = 5;
            this.chkIgnoreLineageTagsCM.Text = "Ignore lineage tags";
            this.toolTip1.SetToolTip(this.chkIgnoreLineageTagsCM, "If checked, expressions and other multiline string properties are serialized as a" +
        " JSON array.");
            this.chkIgnoreLineageTagsCM.UseVisualStyleBackColor = true;
            // 
            // chkIgnorePrivacySettings
            // 
            this.chkIgnorePrivacySettings.AutoSize = true;
            this.chkIgnorePrivacySettings.Location = new System.Drawing.Point(143, 65);
            this.chkIgnorePrivacySettings.Name = "chkIgnorePrivacySettings";
            this.chkIgnorePrivacySettings.Size = new System.Drawing.Size(132, 17);
            this.chkIgnorePrivacySettings.TabIndex = 4;
            this.chkIgnorePrivacySettings.Text = "Ignore privacy settings";
            this.toolTip1.SetToolTip(this.chkIgnorePrivacySettings, resources.GetString("chkIgnorePrivacySettings.ToolTip"));
            this.chkIgnorePrivacySettings.UseVisualStyleBackColor = true;
            // 
            // chkIgnorePrivacySettingsCM
            // 
            this.chkIgnorePrivacySettingsCM.AutoSize = true;
            this.chkIgnorePrivacySettingsCM.Location = new System.Drawing.Point(143, 65);
            this.chkIgnorePrivacySettingsCM.Name = "chkIgnorePrivacySettingsCM";
            this.chkIgnorePrivacySettingsCM.Size = new System.Drawing.Size(132, 17);
            this.chkIgnorePrivacySettingsCM.TabIndex = 5;
            this.chkIgnorePrivacySettingsCM.Text = "Ignore privacy settings";
            this.toolTip1.SetToolTip(this.chkIgnorePrivacySettingsCM, resources.GetString("chkIgnorePrivacySettingsCM.ToolTip"));
            this.chkIgnorePrivacySettingsCM.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreIncrementalRefreshPartitions
            // 
            this.chkIgnoreIncrementalRefreshPartitions.AutoSize = true;
            this.chkIgnoreIncrementalRefreshPartitions.Location = new System.Drawing.Point(5, 88);
            this.chkIgnoreIncrementalRefreshPartitions.Name = "chkIgnoreIncrementalRefreshPartitions";
            this.chkIgnoreIncrementalRefreshPartitions.Size = new System.Drawing.Size(193, 17);
            this.chkIgnoreIncrementalRefreshPartitions.TabIndex = 4;
            this.chkIgnoreIncrementalRefreshPartitions.Text = "Ignore incremental refresh partitions";
            this.toolTip1.SetToolTip(this.chkIgnoreIncrementalRefreshPartitions, "If checked, partitions governed by incremental refresh, will not be serialized.");
            this.chkIgnoreIncrementalRefreshPartitions.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreIncrementalRefreshPartitionsCM
            // 
            this.chkIgnoreIncrementalRefreshPartitionsCM.AutoSize = true;
            this.chkIgnoreIncrementalRefreshPartitionsCM.Location = new System.Drawing.Point(6, 88);
            this.chkIgnoreIncrementalRefreshPartitionsCM.Name = "chkIgnoreIncrementalRefreshPartitionsCM";
            this.chkIgnoreIncrementalRefreshPartitionsCM.Size = new System.Drawing.Size(193, 17);
            this.chkIgnoreIncrementalRefreshPartitionsCM.TabIndex = 5;
            this.chkIgnoreIncrementalRefreshPartitionsCM.Text = "Ignore incremental refresh partitions";
            this.toolTip1.SetToolTip(this.chkIgnoreIncrementalRefreshPartitionsCM, "If checked, partitions governed by incremental refresh, will not be serialized.");
            this.chkIgnoreIncrementalRefreshPartitionsCM.UseVisualStyleBackColor = true;
            // 
            // chkAlsoSaveAsBim
            // 
            this.chkAlsoSaveAsBim.AutoSize = true;
            this.chkAlsoSaveAsBim.Location = new System.Drawing.Point(269, 14);
            this.chkAlsoSaveAsBim.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.chkAlsoSaveAsBim.Name = "chkAlsoSaveAsBim";
            this.chkAlsoSaveAsBim.Size = new System.Drawing.Size(99, 17);
            this.chkAlsoSaveAsBim.TabIndex = 5;
            this.chkAlsoSaveAsBim.Text = "Save .bim copy";
            this.toolTip1.SetToolTip(this.chkAlsoSaveAsBim, "If checked, a .bim file will be saved along with the folder serialized version.");
            this.chkAlsoSaveAsBim.UseVisualStyleBackColor = true;
            // 
            // chkAlsoSaveAsBimCM
            // 
            this.chkAlsoSaveAsBimCM.AutoSize = true;
            this.chkAlsoSaveAsBimCM.Location = new System.Drawing.Point(143, 19);
            this.chkAlsoSaveAsBimCM.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.chkAlsoSaveAsBimCM.Name = "chkAlsoSaveAsBimCM";
            this.chkAlsoSaveAsBimCM.Size = new System.Drawing.Size(108, 17);
            this.chkAlsoSaveAsBimCM.TabIndex = 11;
            this.chkAlsoSaveAsBimCM.Text = "Also save as .bim";
            this.toolTip1.SetToolTip(this.chkAlsoSaveAsBimCM, "If checked, a .bim file will be saved along with the folder serialized version.");
            this.chkAlsoSaveAsBimCM.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.chkFixup);
            this.groupBox3.Location = new System.Drawing.Point(6, 6);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(357, 49);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Editing";
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabFeatures);
            this.tabControl1.Controls.Add(this.tabGeneral);
            this.tabControl1.Controls.Add(this.tabDAX);
            this.tabControl1.Controls.Add(this.tabSerialization);
            this.tabControl1.Controls.Add(this.tabCurrentModel);
            this.tabControl1.Location = new System.Drawing.Point(12, 12);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(377, 405);
            this.tabControl1.TabIndex = 2;
            // 
            // tabFeatures
            // 
            this.tabFeatures.Controls.Add(this.grpDeployment);
            this.tabFeatures.Controls.Add(this.groupBox7);
            this.tabFeatures.Controls.Add(this.groupBox6);
            this.tabFeatures.Location = new System.Drawing.Point(4, 22);
            this.tabFeatures.Name = "tabFeatures";
            this.tabFeatures.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabFeatures.Size = new System.Drawing.Size(369, 363);
            this.tabFeatures.TabIndex = 2;
            this.tabFeatures.Text = "Features";
            this.tabFeatures.UseVisualStyleBackColor = true;
            // 
            // grpDeployment
            // 
            this.grpDeployment.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpDeployment.Controls.Add(this.chkAnnotateDeploymentMetadata);
            this.grpDeployment.Controls.Add(this.btnFolder);
            this.grpDeployment.Controls.Add(this.txtBackupPath);
            this.grpDeployment.Controls.Add(this.chkAutoBackup);
            this.grpDeployment.Location = new System.Drawing.Point(6, 181);
            this.grpDeployment.Name = "grpDeployment";
            this.grpDeployment.Size = new System.Drawing.Size(356, 98);
            this.grpDeployment.TabIndex = 7;
            this.grpDeployment.TabStop = false;
            this.grpDeployment.Text = "Deployment Options";
            // 
            // btnFolder
            // 
            this.btnFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFolder.Enabled = false;
            this.btnFolder.Location = new System.Drawing.Point(323, 66);
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
            this.txtBackupPath.Location = new System.Drawing.Point(6, 66);
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
            this.groupBox7.Location = new System.Drawing.Point(6, 109);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(357, 65);
            this.groupBox7.TabIndex = 6;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Clipboard Operations";
            // 
            // groupBox6
            // 
            this.groupBox6.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox6.Controls.Add(this.chkBackgroundBpa);
            this.groupBox6.Controls.Add(this.chkAllowUnsupportedPBIFeatures);
            this.groupBox6.Controls.Add(this.chkChangeDetectionLocalServer);
            this.groupBox6.Location = new System.Drawing.Point(6, 6);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(357, 97);
            this.groupBox6.TabIndex = 2;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Features";
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.groupBox10);
            this.tabGeneral.Controls.Add(this.groupBox9);
            this.tabGeneral.Controls.Add(this.groupBox1);
            this.tabGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabGeneral.Size = new System.Drawing.Size(369, 363);
            this.tabGeneral.TabIndex = 6;
            this.tabGeneral.Text = "General";
            this.tabGeneral.UseVisualStyleBackColor = true;
            // 
            // groupBox10
            // 
            this.groupBox10.Controls.Add(this.linkLabel1);
            this.groupBox10.Controls.Add(this.label7);
            this.groupBox10.Controls.Add(this.txtCompilerOptions);
            this.groupBox10.Controls.Add(this.label6);
            this.groupBox10.Controls.Add(this.txtCompilerPath);
            this.groupBox10.Location = new System.Drawing.Point(6, 138);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Size = new System.Drawing.Size(357, 114);
            this.groupBox10.TabIndex = 4;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Advanced Scripting";
            // 
            // linkLabel1
            // 
            this.linkLabel1.LinkArea = new System.Windows.Forms.LinkArea(127, 9);
            this.linkLabel1.Location = new System.Drawing.Point(6, 22);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(351, 30);
            this.linkLabel1.TabIndex = 13;
            this.linkLabel1.TabStop = true;
            this.linkLabel1.Text = "To use new C# language features when scripting, you must configure Tabular Editor" +
    " to use a different compiler, such as Roslyn. More info.";
            this.linkLabel1.UseCompatibleTextRendering = true;
            this.linkLabel1.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.linkLabel1_LinkClicked);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 91);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(87, 13);
            this.label7.TabIndex = 11;
            this.label7.Text = "Compiler options:";
            // 
            // txtCompilerOptions
            // 
            this.txtCompilerOptions.Location = new System.Drawing.Point(99, 88);
            this.txtCompilerOptions.Name = "txtCompilerOptions";
            this.txtCompilerOptions.Size = new System.Drawing.Size(252, 20);
            this.txtCompilerOptions.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 65);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(74, 13);
            this.label6.TabIndex = 9;
            this.label6.Text = "Compiler path:";
            // 
            // txtCompilerPath
            // 
            this.txtCompilerPath.Location = new System.Drawing.Point(99, 62);
            this.txtCompilerPath.Name = "txtCompilerPath";
            this.txtCompilerPath.Size = new System.Drawing.Size(252, 20);
            this.txtCompilerPath.TabIndex = 8;
            // 
            // groupBox9
            // 
            this.groupBox9.Controls.Add(this.label5);
            this.groupBox9.Controls.Add(this.label4);
            this.groupBox9.Controls.Add(this.label3);
            this.groupBox9.Controls.Add(this.txtProxyPassword);
            this.groupBox9.Controls.Add(this.txtProxyUser);
            this.groupBox9.Controls.Add(this.txtProxyAddress);
            this.groupBox9.Controls.Add(this.chkSystemProxy);
            this.groupBox9.Location = new System.Drawing.Point(6, 6);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Size = new System.Drawing.Size(357, 126);
            this.groupBox9.TabIndex = 3;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Proxy settings";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 98);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(56, 13);
            this.label5.TabIndex = 9;
            this.label5.Text = "Password:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 72);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(58, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Username:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 46);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 13);
            this.label3.TabIndex = 7;
            this.label3.Text = "Proxy address:";
            // 
            // txtProxyPassword
            // 
            this.txtProxyPassword.Enabled = false;
            this.txtProxyPassword.Location = new System.Drawing.Point(99, 95);
            this.txtProxyPassword.Name = "txtProxyPassword";
            this.txtProxyPassword.PasswordChar = '●';
            this.txtProxyPassword.Size = new System.Drawing.Size(141, 20);
            this.txtProxyPassword.TabIndex = 6;
            // 
            // txtProxyUser
            // 
            this.txtProxyUser.Enabled = false;
            this.txtProxyUser.Location = new System.Drawing.Point(99, 69);
            this.txtProxyUser.Name = "txtProxyUser";
            this.txtProxyUser.Size = new System.Drawing.Size(141, 20);
            this.txtProxyUser.TabIndex = 5;
            // 
            // txtProxyAddress
            // 
            this.txtProxyAddress.Enabled = false;
            this.txtProxyAddress.Location = new System.Drawing.Point(99, 43);
            this.txtProxyAddress.Name = "txtProxyAddress";
            this.txtProxyAddress.Size = new System.Drawing.Size(252, 20);
            this.txtProxyAddress.TabIndex = 4;
            // 
            // chkSystemProxy
            // 
            this.chkSystemProxy.AutoSize = true;
            this.chkSystemProxy.Checked = true;
            this.chkSystemProxy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSystemProxy.Location = new System.Drawing.Point(7, 20);
            this.chkSystemProxy.Name = "chkSystemProxy";
            this.chkSystemProxy.Size = new System.Drawing.Size(108, 17);
            this.chkSystemProxy.TabIndex = 0;
            this.chkSystemProxy.Text = "Use system proxy";
            this.chkSystemProxy.UseVisualStyleBackColor = true;
            this.chkSystemProxy.CheckedChanged += new System.EventHandler(this.chkSystemProxy_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.chkAutoUpdate);
            this.groupBox1.Controls.Add(this.chkSkipPatch);
            this.groupBox1.Controls.Add(this.btnVersionCheck);
            this.groupBox1.Controls.Add(this.lblAvailableVersion);
            this.groupBox1.Controls.Add(this.lblCurrentVersion);
            this.groupBox1.Location = new System.Drawing.Point(6, 259);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(357, 99);
            this.groupBox1.TabIndex = 2;
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
            this.chkAutoUpdate.Click += new System.EventHandler(this.chkAutoUpdate_CheckedChanged);
            // 
            // chkSkipPatch
            // 
            this.chkSkipPatch.AutoSize = true;
            this.chkSkipPatch.Location = new System.Drawing.Point(230, 75);
            this.chkSkipPatch.Name = "chkSkipPatch";
            this.chkSkipPatch.Size = new System.Drawing.Size(107, 17);
            this.chkSkipPatch.TabIndex = 5;
            this.chkSkipPatch.Text = "Skip patch builds";
            this.chkSkipPatch.UseVisualStyleBackColor = true;
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
            // tabDAX
            // 
            this.tabDAX.Controls.Add(this.groupBox5);
            this.tabDAX.Controls.Add(this.groupBox3);
            this.tabDAX.Location = new System.Drawing.Point(4, 22);
            this.tabDAX.Name = "tabDAX";
            this.tabDAX.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabDAX.Size = new System.Drawing.Size(369, 363);
            this.tabDAX.TabIndex = 1;
            this.tabDAX.Text = "DAX";
            this.tabDAX.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.cmbSeparators);
            this.groupBox5.Location = new System.Drawing.Point(6, 62);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(357, 53);
            this.groupBox5.TabIndex = 5;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Separators";
            // 
            // tabSerialization
            // 
            this.tabSerialization.Controls.Add(this.chkAlsoSaveAsBim);
            this.tabSerialization.Controls.Add(this.cmbSerializationMode);
            this.tabSerialization.Controls.Add(this.label8);
            this.tabSerialization.Controls.Add(this.groupBox4);
            this.tabSerialization.Controls.Add(this.groupBox2);
            this.tabSerialization.Location = new System.Drawing.Point(4, 22);
            this.tabSerialization.Name = "tabSerialization";
            this.tabSerialization.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabSerialization.Size = new System.Drawing.Size(369, 379);
            this.tabSerialization.TabIndex = 3;
            this.tabSerialization.Text = "Serialization";
            this.tabSerialization.UseVisualStyleBackColor = true;
            // 
            // cmbSerializationMode
            // 
            this.cmbSerializationMode.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbSerializationMode.FormattingEnabled = true;
            this.cmbSerializationMode.Items.AddRange(new object[] {
            "Database.json (default)",
            "TMDL (preview)"});
            this.cmbSerializationMode.Location = new System.Drawing.Point(111, 11);
            this.cmbSerializationMode.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cmbSerializationMode.Name = "cmbSerializationMode";
            this.cmbSerializationMode.Size = new System.Drawing.Size(145, 21);
            this.cmbSerializationMode.TabIndex = 11;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 15);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(95, 13);
            this.label8.TabIndex = 12;
            this.label8.Text = "Serialization mode:";
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
            this.groupBox4.Controls.Add(this.tvDefaultSerialization);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Location = new System.Drawing.Point(6, 155);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(357, 221);
            this.groupBox4.TabIndex = 4;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Save to Folder Serialization Options";
            // 
            // tvDefaultSerialization
            // 
            this.tvDefaultSerialization.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvDefaultSerialization.CheckBoxes = true;
            this.tvDefaultSerialization.Location = new System.Drawing.Point(5, 40);
            this.tvDefaultSerialization.Name = "tvDefaultSerialization";
            treeNode27.Checked = true;
            treeNode27.Name = "Data Sources";
            treeNode27.Text = "Data Sources";
            treeNode28.Checked = true;
            treeNode28.Name = "Shared Expressions";
            treeNode28.Text = "Shared Expressions";
            treeNode29.Checked = true;
            treeNode29.Name = "Perspectives";
            treeNode29.Text = "Perspectives";
            treeNode30.Checked = true;
            treeNode30.Name = "Relationships";
            treeNode30.Text = "Relationships";
            treeNode31.Checked = true;
            treeNode31.Name = "Roles";
            treeNode31.Text = "Roles";
            treeNode32.Name = "Annotations";
            treeNode32.Text = "Annotations";
            treeNode33.Checked = true;
            treeNode33.Name = "Calculation Items";
            treeNode33.Text = "Calculation Items";
            treeNode34.Checked = true;
            treeNode34.Name = "Columns";
            treeNode34.Text = "Columns";
            treeNode35.Checked = true;
            treeNode35.Name = "Hierarchies";
            treeNode35.Text = "Hierarchies";
            treeNode36.Checked = true;
            treeNode36.Name = "Measures";
            treeNode36.Text = "Measures";
            treeNode37.Checked = true;
            treeNode37.Name = "Partitions";
            treeNode37.Text = "Partitions";
            treeNode38.Checked = true;
            treeNode38.Name = "Tables";
            treeNode38.Text = "Tables";
            treeNode39.Checked = true;
            treeNode39.Name = "Translations";
            treeNode39.Text = "Translations";
            this.tvDefaultSerialization.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode27,
            treeNode28,
            treeNode29,
            treeNode30,
            treeNode31,
            treeNode38,
            treeNode39});
            this.tvDefaultSerialization.PathSeparator = "/";
            this.tvDefaultSerialization.ShowLines = false;
            this.tvDefaultSerialization.Size = new System.Drawing.Size(345, 83);
            this.tvDefaultSerialization.TabIndex = 1;
            this.tvDefaultSerialization.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(199, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Serialize below objects as individual files:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.chkIgnoreLineageTags);
            this.groupBox2.Controls.Add(this.chkSplitMultiline);
            this.groupBox2.Controls.Add(this.chkIgnoreInfProps);
            this.groupBox2.Controls.Add(this.chkIgnoreInfObjects);
            this.groupBox2.Controls.Add(this.chkIgnoreTimestamps);
            this.groupBox2.Controls.Add(this.chkIgnorePrivacySettings);
            this.groupBox2.Controls.Add(this.chkIgnoreIncrementalRefreshPartitions);
            this.groupBox2.Location = new System.Drawing.Point(6, 38);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(357, 111);
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
            this.tabCurrentModel.Padding = new System.Windows.Forms.Padding(3, 3, 3, 3);
            this.tabCurrentModel.Size = new System.Drawing.Size(369, 379);
            this.tabCurrentModel.TabIndex = 5;
            this.tabCurrentModel.Text = "Current Model";
            this.tabCurrentModel.UseVisualStyleBackColor = true;
            // 
            // grpSaveToFolder
            // 
            this.grpSaveToFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpSaveToFolder.Controls.Add(this.chkAlsoSaveAsBimCM);
            this.grpSaveToFolder.Controls.Add(this.chkPrefixFilesCM);
            this.grpSaveToFolder.Controls.Add(this.chkLocalRelationshipsCM);
            this.grpSaveToFolder.Controls.Add(this.chkLocalTranslationsCM);
            this.grpSaveToFolder.Controls.Add(this.chkLocalPerspectivesCM);
            this.grpSaveToFolder.Controls.Add(this.treeView2);
            this.grpSaveToFolder.Controls.Add(this.label2);
            this.grpSaveToFolder.Location = new System.Drawing.Point(6, 123);
            this.grpSaveToFolder.Name = "grpSaveToFolder";
            this.grpSaveToFolder.Size = new System.Drawing.Size(357, 252);
            this.grpSaveToFolder.TabIndex = 6;
            this.grpSaveToFolder.TabStop = false;
            this.grpSaveToFolder.Text = "Current Model \"Save to Folder\" Settings";
            // 
            // treeView2
            // 
            this.treeView2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView2.CheckBoxes = true;
            this.treeView2.Location = new System.Drawing.Point(6, 73);
            this.treeView2.Name = "treeView2";
            treeNode40.Checked = true;
            treeNode40.Name = "Data Sources";
            treeNode40.Text = "Data Sources";
            treeNode41.Checked = true;
            treeNode41.Name = "Shared Expressions";
            treeNode41.Text = "Shared Expressions";
            treeNode42.Checked = true;
            treeNode42.Name = "Perspectives";
            treeNode42.Text = "Perspectives";
            treeNode43.Checked = true;
            treeNode43.Name = "Relationships";
            treeNode43.Text = "Relationships";
            treeNode44.Checked = true;
            treeNode44.Name = "Roles";
            treeNode44.Text = "Roles";
            treeNode45.Name = "Annotations";
            treeNode45.Text = "Annotations";
            treeNode46.Checked = true;
            treeNode46.Name = "Calculation Items";
            treeNode46.Text = "Calculation Items";
            treeNode47.Checked = true;
            treeNode47.Name = "Columns";
            treeNode47.Text = "Columns";
            treeNode48.Checked = true;
            treeNode48.Name = "Hierarchies";
            treeNode48.Text = "Hierarchies";
            treeNode49.Checked = true;
            treeNode49.Name = "Measures";
            treeNode49.Text = "Measures";
            treeNode50.Checked = true;
            treeNode50.Name = "Partitions";
            treeNode50.Text = "Partitions";
            treeNode51.Checked = true;
            treeNode51.Name = "Tables";
            treeNode51.Text = "Tables";
            treeNode52.Checked = true;
            treeNode52.Name = "Translations";
            treeNode52.Text = "Translations";
            this.treeView2.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode40,
            treeNode41,
            treeNode42,
            treeNode43,
            treeNode44,
            treeNode51,
            treeNode52});
            this.treeView2.PathSeparator = "/";
            this.treeView2.ShowLines = false;
            this.treeView2.Size = new System.Drawing.Size(345, 106);
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
            this.groupBox8.Controls.Add(this.chkIgnoreLineageTagsCM);
            this.groupBox8.Controls.Add(this.chkSplitMultilineCM);
            this.groupBox8.Controls.Add(this.chkIgnoreInfPropsCM);
            this.groupBox8.Controls.Add(this.chkIgnoreInfObjectsCM);
            this.groupBox8.Controls.Add(this.chkIgnoreTimestampsCM);
            this.groupBox8.Controls.Add(this.chkIgnorePrivacySettingsCM);
            this.groupBox8.Controls.Add(this.chkIgnoreIncrementalRefreshPartitionsCM);
            this.groupBox8.Location = new System.Drawing.Point(6, 6);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Size = new System.Drawing.Size(357, 111);
            this.groupBox8.TabIndex = 5;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Current Model Serialization Settings";
            // 
            // PreferencesForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(401, 458);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(416, 476);
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
            this.tabFeatures.ResumeLayout(false);
            this.grpDeployment.ResumeLayout(false);
            this.grpDeployment.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.tabGeneral.ResumeLayout(false);
            this.groupBox10.ResumeLayout(false);
            this.groupBox10.PerformLayout();
            this.groupBox9.ResumeLayout(false);
            this.groupBox9.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.tabDAX.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.tabSerialization.ResumeLayout(false);
            this.tabSerialization.PerformLayout();
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
        private System.Windows.Forms.TabPage tabSerialization;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TabPage tabDAX;
        private System.Windows.Forms.TabPage tabFeatures;
        private System.Windows.Forms.CheckBox chkIgnoreTimestamps;
        private System.Windows.Forms.CheckBox chkIgnoreInfObjects;
        private System.Windows.Forms.CheckBox chkIgnoreInfProps;
        private System.Windows.Forms.CheckBox chkSplitMultiline;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TreeView tvDefaultSerialization;
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
        private System.Windows.Forms.CheckBox chkChangeDetectionLocalServer;
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
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox cmbSeparators;
        private System.Windows.Forms.CheckBox chkBackgroundBpa;
        private System.Windows.Forms.TabPage tabGeneral;
        private System.Windows.Forms.GroupBox groupBox9;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtProxyPassword;
        private System.Windows.Forms.TextBox txtProxyUser;
        private System.Windows.Forms.TextBox txtProxyAddress;
        private System.Windows.Forms.CheckBox chkSystemProxy;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.CheckBox chkAutoUpdate;
        private System.Windows.Forms.CheckBox chkSkipPatch;
        private System.Windows.Forms.Button btnVersionCheck;
        private System.Windows.Forms.Label lblAvailableVersion;
        private System.Windows.Forms.Label lblCurrentVersion;
        private System.Windows.Forms.CheckBox chkAnnotateDeploymentMetadata;
        private System.Windows.Forms.GroupBox groupBox10;
        private System.Windows.Forms.LinkLabel linkLabel1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtCompilerOptions;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtCompilerPath;
        private System.Windows.Forms.CheckBox chkIgnoreLineageTags;
        private System.Windows.Forms.CheckBox chkIgnoreLineageTagsCM;
        private System.Windows.Forms.CheckBox chkIgnorePrivacySettings;
        private System.Windows.Forms.CheckBox chkIgnorePrivacySettingsCM;
        private System.Windows.Forms.CheckBox chkIgnoreIncrementalRefreshPartitions;
        private System.Windows.Forms.CheckBox chkIgnoreIncrementalRefreshPartitionsCM;
        private System.Windows.Forms.CheckBox chkAlsoSaveAsBim;
        private System.Windows.Forms.CheckBox chkAlsoSaveAsBimCM;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox cmbSerializationMode;
    }
}