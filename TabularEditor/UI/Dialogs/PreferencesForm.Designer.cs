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
            System.Windows.Forms.TreeNode treeNode6 = new System.Windows.Forms.TreeNode("Calculation Items");
            System.Windows.Forms.TreeNode treeNode7 = new System.Windows.Forms.TreeNode("Columns");
            System.Windows.Forms.TreeNode treeNode8 = new System.Windows.Forms.TreeNode("Hierarchies");
            System.Windows.Forms.TreeNode treeNode9 = new System.Windows.Forms.TreeNode("Measures");
            System.Windows.Forms.TreeNode treeNode10 = new System.Windows.Forms.TreeNode("Partitions");
            System.Windows.Forms.TreeNode treeNode11 = new System.Windows.Forms.TreeNode("Tables", new System.Windows.Forms.TreeNode[] {
            treeNode5,
            treeNode6,
            treeNode7,
            treeNode8,
            treeNode9,
            treeNode10});
            System.Windows.Forms.TreeNode treeNode12 = new System.Windows.Forms.TreeNode("Translations");
            System.Windows.Forms.TreeNode treeNode13 = new System.Windows.Forms.TreeNode("Data Sources");
            System.Windows.Forms.TreeNode treeNode14 = new System.Windows.Forms.TreeNode("Perspectives");
            System.Windows.Forms.TreeNode treeNode15 = new System.Windows.Forms.TreeNode("Relationships");
            System.Windows.Forms.TreeNode treeNode16 = new System.Windows.Forms.TreeNode("Roles");
            System.Windows.Forms.TreeNode treeNode17 = new System.Windows.Forms.TreeNode("Annotations");
            System.Windows.Forms.TreeNode treeNode18 = new System.Windows.Forms.TreeNode("Calculation Items");
            System.Windows.Forms.TreeNode treeNode19 = new System.Windows.Forms.TreeNode("Columns");
            System.Windows.Forms.TreeNode treeNode20 = new System.Windows.Forms.TreeNode("Hierarchies");
            System.Windows.Forms.TreeNode treeNode21 = new System.Windows.Forms.TreeNode("Measures");
            System.Windows.Forms.TreeNode treeNode22 = new System.Windows.Forms.TreeNode("Partitions");
            System.Windows.Forms.TreeNode treeNode23 = new System.Windows.Forms.TreeNode("Tables", new System.Windows.Forms.TreeNode[] {
            treeNode17,
            treeNode18,
            treeNode19,
            treeNode20,
            treeNode21,
            treeNode22});
            System.Windows.Forms.TreeNode treeNode24 = new System.Windows.Forms.TreeNode("Translations");
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
            this.chkAlsoSaveAsBim = new System.Windows.Forms.CheckBox();
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
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.treeView1 = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.tabCurrentModel = new System.Windows.Forms.TabPage();
            this.grpSaveToFolder = new System.Windows.Forms.GroupBox();
            this.chkAlsoSaveAsBimCM = new System.Windows.Forms.CheckBox();
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
            this.btnOK.Location = new System.Drawing.Point(311, 501);
            this.btnOK.Margin = new System.Windows.Forms.Padding(4);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(100, 28);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(419, 501);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(100, 28);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // chkFixup
            // 
            this.chkFixup.AutoSize = true;
            this.chkFixup.Location = new System.Drawing.Point(8, 23);
            this.chkFixup.Margin = new System.Windows.Forms.Padding(4);
            this.chkFixup.Name = "chkFixup";
            this.chkFixup.Size = new System.Drawing.Size(181, 21);
            this.chkFixup.TabIndex = 1;
            this.chkFixup.Text = "Automatic formula fix-up";
            this.toolTip1.SetToolTip(this.chkFixup, "When this is checked, expressions of Measures, Calculated Columns and Calculated " +
        "Tables will automatically be updated, when an object name is changed.");
            this.chkFixup.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreTimestamps
            // 
            this.chkIgnoreTimestamps.AutoSize = true;
            this.chkIgnoreTimestamps.Location = new System.Drawing.Point(8, 52);
            this.chkIgnoreTimestamps.Margin = new System.Windows.Forms.Padding(4);
            this.chkIgnoreTimestamps.Name = "chkIgnoreTimestamps";
            this.chkIgnoreTimestamps.Size = new System.Drawing.Size(145, 21);
            this.chkIgnoreTimestamps.TabIndex = 0;
            this.chkIgnoreTimestamps.Text = "Ignore timestamps";
            this.toolTip1.SetToolTip(this.chkIgnoreTimestamps, "If checked, editing timestamps are not serialized into the .json files");
            this.chkIgnoreTimestamps.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreInfObjects
            // 
            this.chkIgnoreInfObjects.AutoSize = true;
            this.chkIgnoreInfObjects.Location = new System.Drawing.Point(8, 23);
            this.chkIgnoreInfObjects.Margin = new System.Windows.Forms.Padding(4);
            this.chkIgnoreInfObjects.Name = "chkIgnoreInfObjects";
            this.chkIgnoreInfObjects.Size = new System.Drawing.Size(172, 21);
            this.chkIgnoreInfObjects.TabIndex = 1;
            this.chkIgnoreInfObjects.Text = "Ignore inferred objects";
            this.toolTip1.SetToolTip(this.chkIgnoreInfObjects, "If checked, objects whose lifetime is controlled by the server (RowNumber, attrib" +
        "ute hierarchies, etc.) are not serialized into the .json files");
            this.chkIgnoreInfObjects.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreInfProps
            // 
            this.chkIgnoreInfProps.AutoSize = true;
            this.chkIgnoreInfProps.Location = new System.Drawing.Point(191, 23);
            this.chkIgnoreInfProps.Margin = new System.Windows.Forms.Padding(4);
            this.chkIgnoreInfProps.Name = "chkIgnoreInfProps";
            this.chkIgnoreInfProps.Size = new System.Drawing.Size(191, 21);
            this.chkIgnoreInfProps.TabIndex = 2;
            this.chkIgnoreInfProps.Text = "Ignore inferred properties";
            this.toolTip1.SetToolTip(this.chkIgnoreInfProps, "If checked, objects whose value is controlled by the server (Column.State, etc.) " +
        "are not serialized into the .json files");
            this.chkIgnoreInfProps.UseVisualStyleBackColor = true;
            // 
            // chkSplitMultiline
            // 
            this.chkSplitMultiline.AutoSize = true;
            this.chkSplitMultiline.Location = new System.Drawing.Point(191, 52);
            this.chkSplitMultiline.Margin = new System.Windows.Forms.Padding(4);
            this.chkSplitMultiline.Name = "chkSplitMultiline";
            this.chkSplitMultiline.Size = new System.Drawing.Size(151, 21);
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
            this.chkLocalTranslations.Location = new System.Drawing.Point(8, 267);
            this.chkLocalTranslations.Margin = new System.Windows.Forms.Padding(4);
            this.chkLocalTranslations.Name = "chkLocalTranslations";
            this.chkLocalTranslations.Size = new System.Drawing.Size(229, 21);
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
            this.chkLocalPerspectives.Location = new System.Drawing.Point(8, 239);
            this.chkLocalPerspectives.Margin = new System.Windows.Forms.Padding(4);
            this.chkLocalPerspectives.Name = "chkLocalPerspectives";
            this.chkLocalPerspectives.Size = new System.Drawing.Size(303, 21);
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
            this.chkLocalRelationships.Location = new System.Drawing.Point(8, 295);
            this.chkLocalRelationships.Margin = new System.Windows.Forms.Padding(4);
            this.chkLocalRelationships.Name = "chkLocalRelationships";
            this.chkLocalRelationships.Size = new System.Drawing.Size(312, 21);
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
            this.chkAllowUnsupportedPBIFeatures.Location = new System.Drawing.Point(8, 26);
            this.chkAllowUnsupportedPBIFeatures.Margin = new System.Windows.Forms.Padding(4);
            this.chkAllowUnsupportedPBIFeatures.Name = "chkAllowUnsupportedPBIFeatures";
            this.chkAllowUnsupportedPBIFeatures.Size = new System.Drawing.Size(355, 21);
            this.chkAllowUnsupportedPBIFeatures.TabIndex = 0;
            this.chkAllowUnsupportedPBIFeatures.Text = "Allow unsupported Power BI features (experimental)";
            this.toolTip1.SetToolTip(this.chkAllowUnsupportedPBIFeatures, "Checking this, will let you edit all TOM objects and properties when connected to" +
        " a Power BI data model. USE ONLY FOR EXPERIMENTAL PURPOSES.");
            this.chkAllowUnsupportedPBIFeatures.UseVisualStyleBackColor = true;
            // 
            // chkChangeDetectionLocalServer
            // 
            this.chkChangeDetectionLocalServer.AutoSize = true;
            this.chkChangeDetectionLocalServer.Location = new System.Drawing.Point(8, 82);
            this.chkChangeDetectionLocalServer.Margin = new System.Windows.Forms.Padding(4);
            this.chkChangeDetectionLocalServer.Name = "chkChangeDetectionLocalServer";
            this.chkChangeDetectionLocalServer.Size = new System.Drawing.Size(355, 21);
            this.chkChangeDetectionLocalServer.TabIndex = 0;
            this.chkChangeDetectionLocalServer.Text = "Detect changes on local AS instance (experimental)";
            this.toolTip1.SetToolTip(this.chkChangeDetectionLocalServer, resources.GetString("chkChangeDetectionLocalServer.ToolTip"));
            this.chkChangeDetectionLocalServer.UseVisualStyleBackColor = true;
            // 
            // chkCopyIncludeOLS
            // 
            this.chkCopyIncludeOLS.AutoSize = true;
            this.chkCopyIncludeOLS.Location = new System.Drawing.Point(255, 52);
            this.chkCopyIncludeOLS.Margin = new System.Windows.Forms.Padding(4);
            this.chkCopyIncludeOLS.Name = "chkCopyIncludeOLS";
            this.chkCopyIncludeOLS.Size = new System.Drawing.Size(204, 21);
            this.chkCopyIncludeOLS.TabIndex = 3;
            this.chkCopyIncludeOLS.Text = "Include object-level security";
            this.toolTip1.SetToolTip(this.chkCopyIncludeOLS, "Include object-level security when copying tables, columns, hierarchies or measur" +
        "es (Compatibility Level 1400 only)");
            this.chkCopyIncludeOLS.UseVisualStyleBackColor = true;
            // 
            // chkCopyIncludeRLS
            // 
            this.chkCopyIncludeRLS.AutoSize = true;
            this.chkCopyIncludeRLS.Location = new System.Drawing.Point(255, 23);
            this.chkCopyIncludeRLS.Margin = new System.Windows.Forms.Padding(4);
            this.chkCopyIncludeRLS.Name = "chkCopyIncludeRLS";
            this.chkCopyIncludeRLS.Size = new System.Drawing.Size(188, 21);
            this.chkCopyIncludeRLS.TabIndex = 2;
            this.chkCopyIncludeRLS.Text = "Include row-level security";
            this.toolTip1.SetToolTip(this.chkCopyIncludeRLS, "Include row-level security when copying tables.");
            this.chkCopyIncludeRLS.UseVisualStyleBackColor = true;
            // 
            // chkCopyIncludePerspectives
            // 
            this.chkCopyIncludePerspectives.AutoSize = true;
            this.chkCopyIncludePerspectives.Location = new System.Drawing.Point(8, 52);
            this.chkCopyIncludePerspectives.Margin = new System.Windows.Forms.Padding(4);
            this.chkCopyIncludePerspectives.Name = "chkCopyIncludePerspectives";
            this.chkCopyIncludePerspectives.Size = new System.Drawing.Size(233, 21);
            this.chkCopyIncludePerspectives.TabIndex = 1;
            this.chkCopyIncludePerspectives.Text = "Include perspective membership";
            this.toolTip1.SetToolTip(this.chkCopyIncludePerspectives, "Include perspective membership when copying objects that can be toggled in perspe" +
        "ctives");
            this.chkCopyIncludePerspectives.UseVisualStyleBackColor = true;
            // 
            // chkCopyIncludeTranslations
            // 
            this.chkCopyIncludeTranslations.AutoSize = true;
            this.chkCopyIncludeTranslations.Location = new System.Drawing.Point(8, 23);
            this.chkCopyIncludeTranslations.Margin = new System.Windows.Forms.Padding(4);
            this.chkCopyIncludeTranslations.Name = "chkCopyIncludeTranslations";
            this.chkCopyIncludeTranslations.Size = new System.Drawing.Size(152, 21);
            this.chkCopyIncludeTranslations.TabIndex = 0;
            this.chkCopyIncludeTranslations.Text = "Include translations";
            this.toolTip1.SetToolTip(this.chkCopyIncludeTranslations, "Include translations when copying translatable objects");
            this.chkCopyIncludeTranslations.UseVisualStyleBackColor = true;
            // 
            // chkAutoBackup
            // 
            this.chkAutoBackup.AutoSize = true;
            this.chkAutoBackup.Location = new System.Drawing.Point(8, 54);
            this.chkAutoBackup.Margin = new System.Windows.Forms.Padding(4);
            this.chkAutoBackup.Name = "chkAutoBackup";
            this.chkAutoBackup.Size = new System.Drawing.Size(217, 21);
            this.chkAutoBackup.TabIndex = 6;
            this.chkAutoBackup.Text = "Save Model.bim backup here:";
            this.toolTip1.SetToolTip(this.chkAutoBackup, resources.GetString("chkAutoBackup.ToolTip"));
            this.chkAutoBackup.UseVisualStyleBackColor = true;
            this.chkAutoBackup.CheckedChanged += new System.EventHandler(this.chkAutoBackup_CheckedChanged);
            // 
            // chkPrefixFiles
            // 
            this.chkPrefixFiles.AutoSize = true;
            this.chkPrefixFiles.Location = new System.Drawing.Point(8, 23);
            this.chkPrefixFiles.Margin = new System.Windows.Forms.Padding(4);
            this.chkPrefixFiles.Name = "chkPrefixFiles";
            this.chkPrefixFiles.Size = new System.Drawing.Size(129, 21);
            this.chkPrefixFiles.TabIndex = 10;
            this.chkPrefixFiles.Text = "Prefix filenames";
            this.toolTip1.SetToolTip(this.chkPrefixFiles, "If checked, files and folders representing individual objects, will be prefixed b" +
        "y a number indicating the object\'s order in the Model.bim metadata.");
            this.chkPrefixFiles.UseVisualStyleBackColor = true;
            // 
            // chkPrefixFilesCM
            // 
            this.chkPrefixFilesCM.AutoSize = true;
            this.chkPrefixFilesCM.Location = new System.Drawing.Point(8, 23);
            this.chkPrefixFilesCM.Margin = new System.Windows.Forms.Padding(4);
            this.chkPrefixFilesCM.Name = "chkPrefixFilesCM";
            this.chkPrefixFilesCM.Size = new System.Drawing.Size(129, 21);
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
            this.chkLocalRelationshipsCM.Location = new System.Drawing.Point(8, 295);
            this.chkLocalRelationshipsCM.Margin = new System.Windows.Forms.Padding(4);
            this.chkLocalRelationshipsCM.Name = "chkLocalRelationshipsCM";
            this.chkLocalRelationshipsCM.Size = new System.Drawing.Size(312, 21);
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
            this.chkLocalTranslationsCM.Location = new System.Drawing.Point(8, 267);
            this.chkLocalTranslationsCM.Margin = new System.Windows.Forms.Padding(4);
            this.chkLocalTranslationsCM.Name = "chkLocalTranslationsCM";
            this.chkLocalTranslationsCM.Size = new System.Drawing.Size(229, 21);
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
            this.chkLocalPerspectivesCM.Location = new System.Drawing.Point(8, 239);
            this.chkLocalPerspectivesCM.Margin = new System.Windows.Forms.Padding(4);
            this.chkLocalPerspectivesCM.Name = "chkLocalPerspectivesCM";
            this.chkLocalPerspectivesCM.Size = new System.Drawing.Size(303, 21);
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
            this.chkSplitMultilineCM.Location = new System.Drawing.Point(191, 52);
            this.chkSplitMultilineCM.Margin = new System.Windows.Forms.Padding(4);
            this.chkSplitMultilineCM.Name = "chkSplitMultilineCM";
            this.chkSplitMultilineCM.Size = new System.Drawing.Size(151, 21);
            this.chkSplitMultilineCM.TabIndex = 3;
            this.chkSplitMultilineCM.Text = "Split multiline string";
            this.toolTip1.SetToolTip(this.chkSplitMultilineCM, "If checked, expressions and other multiline string properties are serialized as a" +
        " JSON array.");
            this.chkSplitMultilineCM.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreInfPropsCM
            // 
            this.chkIgnoreInfPropsCM.AutoSize = true;
            this.chkIgnoreInfPropsCM.Location = new System.Drawing.Point(191, 23);
            this.chkIgnoreInfPropsCM.Margin = new System.Windows.Forms.Padding(4);
            this.chkIgnoreInfPropsCM.Name = "chkIgnoreInfPropsCM";
            this.chkIgnoreInfPropsCM.Size = new System.Drawing.Size(191, 21);
            this.chkIgnoreInfPropsCM.TabIndex = 2;
            this.chkIgnoreInfPropsCM.Text = "Ignore inferred properties";
            this.toolTip1.SetToolTip(this.chkIgnoreInfPropsCM, "If checked, objects whose value is controlled by the server (Column.State, etc.) " +
        "are not serialized into the .json files");
            this.chkIgnoreInfPropsCM.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreInfObjectsCM
            // 
            this.chkIgnoreInfObjectsCM.AutoSize = true;
            this.chkIgnoreInfObjectsCM.Location = new System.Drawing.Point(8, 23);
            this.chkIgnoreInfObjectsCM.Margin = new System.Windows.Forms.Padding(4);
            this.chkIgnoreInfObjectsCM.Name = "chkIgnoreInfObjectsCM";
            this.chkIgnoreInfObjectsCM.Size = new System.Drawing.Size(172, 21);
            this.chkIgnoreInfObjectsCM.TabIndex = 1;
            this.chkIgnoreInfObjectsCM.Text = "Ignore inferred objects";
            this.toolTip1.SetToolTip(this.chkIgnoreInfObjectsCM, "If checked, objects whose lifetime is controlled by the server (RowNumber, attrib" +
        "ute hierarchies, etc.) are not serialized into the .json files");
            this.chkIgnoreInfObjectsCM.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreTimestampsCM
            // 
            this.chkIgnoreTimestampsCM.AutoSize = true;
            this.chkIgnoreTimestampsCM.Location = new System.Drawing.Point(8, 52);
            this.chkIgnoreTimestampsCM.Margin = new System.Windows.Forms.Padding(4);
            this.chkIgnoreTimestampsCM.Name = "chkIgnoreTimestampsCM";
            this.chkIgnoreTimestampsCM.Size = new System.Drawing.Size(145, 21);
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
            this.cmbSeparators.Location = new System.Drawing.Point(9, 25);
            this.cmbSeparators.Margin = new System.Windows.Forms.Padding(4);
            this.cmbSeparators.Name = "cmbSeparators";
            this.cmbSeparators.Size = new System.Drawing.Size(237, 24);
            this.cmbSeparators.TabIndex = 0;
            this.toolTip1.SetToolTip(this.cmbSeparators, resources.GetString("cmbSeparators.ToolTip"));
            // 
            // chkBackgroundBpa
            // 
            this.chkBackgroundBpa.AutoSize = true;
            this.chkBackgroundBpa.Location = new System.Drawing.Point(8, 54);
            this.chkBackgroundBpa.Margin = new System.Windows.Forms.Padding(4);
            this.chkBackgroundBpa.Name = "chkBackgroundBpa";
            this.chkBackgroundBpa.Size = new System.Drawing.Size(292, 21);
            this.chkBackgroundBpa.TabIndex = 1;
            this.chkBackgroundBpa.Text = "Background scan for Best Practice issues";
            this.toolTip1.SetToolTip(this.chkBackgroundBpa, "Checking this, will let you edit all TOM objects and properties when connected to" +
        " a Power BI data model. USE ONLY FOR EXPERIMENTAL PURPOSES.");
            this.chkBackgroundBpa.UseVisualStyleBackColor = true;
            // 
            // chkAnnotateDeploymentMetadata
            // 
            this.chkAnnotateDeploymentMetadata.AutoSize = true;
            this.chkAnnotateDeploymentMetadata.Location = new System.Drawing.Point(9, 25);
            this.chkAnnotateDeploymentMetadata.Margin = new System.Windows.Forms.Padding(4);
            this.chkAnnotateDeploymentMetadata.Name = "chkAnnotateDeploymentMetadata";
            this.chkAnnotateDeploymentMetadata.Size = new System.Drawing.Size(302, 21);
            this.chkAnnotateDeploymentMetadata.TabIndex = 9;
            this.chkAnnotateDeploymentMetadata.Text = "Add metadata annotation upon deployment";
            this.toolTip1.SetToolTip(this.chkAnnotateDeploymentMetadata, resources.GetString("chkAnnotateDeploymentMetadata.ToolTip"));
            this.chkAnnotateDeploymentMetadata.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreLineageTags
            // 
            this.chkIgnoreLineageTags.AutoSize = true;
            this.chkIgnoreLineageTags.Location = new System.Drawing.Point(8, 80);
            this.chkIgnoreLineageTags.Margin = new System.Windows.Forms.Padding(4);
            this.chkIgnoreLineageTags.Name = "chkIgnoreLineageTags";
            this.chkIgnoreLineageTags.Size = new System.Drawing.Size(151, 21);
            this.chkIgnoreLineageTags.TabIndex = 4;
            this.chkIgnoreLineageTags.Text = "Ignore lineage tags";
            this.toolTip1.SetToolTip(this.chkIgnoreLineageTags, "If checked, expressions and other multiline string properties are serialized as a" +
        " JSON array.");
            this.chkIgnoreLineageTags.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreLineageTagsCM
            // 
            this.chkIgnoreLineageTagsCM.AutoSize = true;
            this.chkIgnoreLineageTagsCM.Location = new System.Drawing.Point(8, 80);
            this.chkIgnoreLineageTagsCM.Margin = new System.Windows.Forms.Padding(4);
            this.chkIgnoreLineageTagsCM.Name = "chkIgnoreLineageTagsCM";
            this.chkIgnoreLineageTagsCM.Size = new System.Drawing.Size(151, 21);
            this.chkIgnoreLineageTagsCM.TabIndex = 5;
            this.chkIgnoreLineageTagsCM.Text = "Ignore lineage tags";
            this.toolTip1.SetToolTip(this.chkIgnoreLineageTagsCM, "If checked, expressions and other multiline string properties are serialized as a" +
        " JSON array.");
            this.chkIgnoreLineageTagsCM.UseVisualStyleBackColor = true;
            // 
            // chkAlsoSaveAsBim
            // 
            this.chkAlsoSaveAsBim.AutoSize = true;
            this.chkAlsoSaveAsBim.Location = new System.Drawing.Point(191, 23);
            this.chkAlsoSaveAsBim.Name = "chkAlsoSaveAsBim";
            this.chkAlsoSaveAsBim.Size = new System.Drawing.Size(140, 21);
            this.chkAlsoSaveAsBim.TabIndex = 5;
            this.chkAlsoSaveAsBim.Text = "Also save as .bim";
            this.toolTip1.SetToolTip(this.chkAlsoSaveAsBim, "If checked, a .bim file will be saved along with the folder serialized version.");
            this.chkAlsoSaveAsBim.UseVisualStyleBackColor = true;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.chkFixup);
            this.groupBox3.Location = new System.Drawing.Point(8, 7);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(476, 60);
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
            this.tabControl1.Location = new System.Drawing.Point(16, 15);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(503, 479);
            this.tabControl1.TabIndex = 2;
            // 
            // tabFeatures
            // 
            this.tabFeatures.Controls.Add(this.grpDeployment);
            this.tabFeatures.Controls.Add(this.groupBox7);
            this.tabFeatures.Controls.Add(this.groupBox6);
            this.tabFeatures.Location = new System.Drawing.Point(4, 25);
            this.tabFeatures.Margin = new System.Windows.Forms.Padding(4);
            this.tabFeatures.Name = "tabFeatures";
            this.tabFeatures.Padding = new System.Windows.Forms.Padding(4);
            this.tabFeatures.Size = new System.Drawing.Size(495, 450);
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
            this.grpDeployment.Location = new System.Drawing.Point(8, 222);
            this.grpDeployment.Margin = new System.Windows.Forms.Padding(4);
            this.grpDeployment.Name = "grpDeployment";
            this.grpDeployment.Padding = new System.Windows.Forms.Padding(4);
            this.grpDeployment.Size = new System.Drawing.Size(475, 121);
            this.grpDeployment.TabIndex = 7;
            this.grpDeployment.TabStop = false;
            this.grpDeployment.Text = "Deployment Options";
            // 
            // btnFolder
            // 
            this.btnFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFolder.Enabled = false;
            this.btnFolder.Location = new System.Drawing.Point(431, 82);
            this.btnFolder.Margin = new System.Windows.Forms.Padding(4);
            this.btnFolder.Name = "btnFolder";
            this.btnFolder.Size = new System.Drawing.Size(36, 25);
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
            this.txtBackupPath.Location = new System.Drawing.Point(8, 82);
            this.txtBackupPath.Margin = new System.Windows.Forms.Padding(4);
            this.txtBackupPath.Name = "txtBackupPath";
            this.txtBackupPath.Size = new System.Drawing.Size(413, 22);
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
            this.groupBox7.Location = new System.Drawing.Point(8, 134);
            this.groupBox7.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox7.Size = new System.Drawing.Size(476, 80);
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
            this.groupBox6.Location = new System.Drawing.Point(8, 7);
            this.groupBox6.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox6.Size = new System.Drawing.Size(476, 119);
            this.groupBox6.TabIndex = 2;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Features";
            // 
            // tabGeneral
            // 
            this.tabGeneral.Controls.Add(this.groupBox10);
            this.tabGeneral.Controls.Add(this.groupBox9);
            this.tabGeneral.Controls.Add(this.groupBox1);
            this.tabGeneral.Location = new System.Drawing.Point(4, 25);
            this.tabGeneral.Margin = new System.Windows.Forms.Padding(4);
            this.tabGeneral.Name = "tabGeneral";
            this.tabGeneral.Padding = new System.Windows.Forms.Padding(4);
            this.tabGeneral.Size = new System.Drawing.Size(495, 450);
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
            this.groupBox10.Location = new System.Drawing.Point(8, 170);
            this.groupBox10.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox10.Name = "groupBox10";
            this.groupBox10.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox10.Size = new System.Drawing.Size(476, 140);
            this.groupBox10.TabIndex = 4;
            this.groupBox10.TabStop = false;
            this.groupBox10.Text = "Advanced Scripting";
            // 
            // linkLabel1
            // 
            this.linkLabel1.LinkArea = new System.Windows.Forms.LinkArea(127, 9);
            this.linkLabel1.Location = new System.Drawing.Point(8, 27);
            this.linkLabel1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.linkLabel1.Name = "linkLabel1";
            this.linkLabel1.Size = new System.Drawing.Size(468, 37);
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
            this.label7.Location = new System.Drawing.Point(8, 112);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(117, 17);
            this.label7.TabIndex = 11;
            this.label7.Text = "Compiler options:";
            // 
            // txtCompilerOptions
            // 
            this.txtCompilerOptions.Location = new System.Drawing.Point(132, 108);
            this.txtCompilerOptions.Margin = new System.Windows.Forms.Padding(4);
            this.txtCompilerOptions.Name = "txtCompilerOptions";
            this.txtCompilerOptions.Size = new System.Drawing.Size(335, 22);
            this.txtCompilerOptions.TabIndex = 10;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(8, 80);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(99, 17);
            this.label6.TabIndex = 9;
            this.label6.Text = "Compiler path:";
            // 
            // txtCompilerPath
            // 
            this.txtCompilerPath.Location = new System.Drawing.Point(132, 76);
            this.txtCompilerPath.Margin = new System.Windows.Forms.Padding(4);
            this.txtCompilerPath.Name = "txtCompilerPath";
            this.txtCompilerPath.Size = new System.Drawing.Size(335, 22);
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
            this.groupBox9.Location = new System.Drawing.Point(8, 7);
            this.groupBox9.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox9.Name = "groupBox9";
            this.groupBox9.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox9.Size = new System.Drawing.Size(476, 155);
            this.groupBox9.TabIndex = 3;
            this.groupBox9.TabStop = false;
            this.groupBox9.Text = "Proxy settings";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 121);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(73, 17);
            this.label5.TabIndex = 9;
            this.label5.Text = "Password:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 89);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 17);
            this.label4.TabIndex = 8;
            this.label4.Text = "Username:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 57);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 17);
            this.label3.TabIndex = 7;
            this.label3.Text = "Proxy address:";
            // 
            // txtProxyPassword
            // 
            this.txtProxyPassword.Enabled = false;
            this.txtProxyPassword.Location = new System.Drawing.Point(132, 117);
            this.txtProxyPassword.Margin = new System.Windows.Forms.Padding(4);
            this.txtProxyPassword.Name = "txtProxyPassword";
            this.txtProxyPassword.PasswordChar = '●';
            this.txtProxyPassword.Size = new System.Drawing.Size(187, 22);
            this.txtProxyPassword.TabIndex = 6;
            // 
            // txtProxyUser
            // 
            this.txtProxyUser.Enabled = false;
            this.txtProxyUser.Location = new System.Drawing.Point(132, 85);
            this.txtProxyUser.Margin = new System.Windows.Forms.Padding(4);
            this.txtProxyUser.Name = "txtProxyUser";
            this.txtProxyUser.Size = new System.Drawing.Size(187, 22);
            this.txtProxyUser.TabIndex = 5;
            // 
            // txtProxyAddress
            // 
            this.txtProxyAddress.Enabled = false;
            this.txtProxyAddress.Location = new System.Drawing.Point(132, 53);
            this.txtProxyAddress.Margin = new System.Windows.Forms.Padding(4);
            this.txtProxyAddress.Name = "txtProxyAddress";
            this.txtProxyAddress.Size = new System.Drawing.Size(335, 22);
            this.txtProxyAddress.TabIndex = 4;
            // 
            // chkSystemProxy
            // 
            this.chkSystemProxy.AutoSize = true;
            this.chkSystemProxy.Checked = true;
            this.chkSystemProxy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSystemProxy.Location = new System.Drawing.Point(9, 25);
            this.chkSystemProxy.Margin = new System.Windows.Forms.Padding(4);
            this.chkSystemProxy.Name = "chkSystemProxy";
            this.chkSystemProxy.Size = new System.Drawing.Size(141, 21);
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
            this.groupBox1.Location = new System.Drawing.Point(8, 318);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(476, 122);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Tabular Editor Updates";
            // 
            // chkAutoUpdate
            // 
            this.chkAutoUpdate.AutoSize = true;
            this.chkAutoUpdate.Location = new System.Drawing.Point(8, 92);
            this.chkAutoUpdate.Margin = new System.Windows.Forms.Padding(4);
            this.chkAutoUpdate.Name = "chkAutoUpdate";
            this.chkAutoUpdate.Size = new System.Drawing.Size(269, 21);
            this.chkAutoUpdate.TabIndex = 4;
            this.chkAutoUpdate.Text = "Check for updated version on start-up";
            this.chkAutoUpdate.UseVisualStyleBackColor = true;
            this.chkAutoUpdate.Click += new System.EventHandler(this.chkAutoUpdate_CheckedChanged);
            // 
            // chkSkipPatch
            // 
            this.chkSkipPatch.AutoSize = true;
            this.chkSkipPatch.Location = new System.Drawing.Point(307, 92);
            this.chkSkipPatch.Margin = new System.Windows.Forms.Padding(4);
            this.chkSkipPatch.Name = "chkSkipPatch";
            this.chkSkipPatch.Size = new System.Drawing.Size(137, 21);
            this.chkSkipPatch.TabIndex = 5;
            this.chkSkipPatch.Text = "Skip patch builds";
            this.chkSkipPatch.UseVisualStyleBackColor = true;
            // 
            // btnVersionCheck
            // 
            this.btnVersionCheck.Location = new System.Drawing.Point(8, 57);
            this.btnVersionCheck.Margin = new System.Windows.Forms.Padding(4);
            this.btnVersionCheck.Name = "btnVersionCheck";
            this.btnVersionCheck.Size = new System.Drawing.Size(168, 28);
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
            this.lblAvailableVersion.Location = new System.Drawing.Point(8, 63);
            this.lblAvailableVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblAvailableVersion.Name = "lblAvailableVersion";
            this.lblAvailableVersion.Size = new System.Drawing.Size(167, 17);
            this.lblAvailableVersion.TabIndex = 2;
            this.lblAvailableVersion.Text = "Available version: 2.0.0.0";
            this.lblAvailableVersion.Visible = false;
            this.lblAvailableVersion.Click += new System.EventHandler(this.lblAvailableVersion_Click);
            // 
            // lblCurrentVersion
            // 
            this.lblCurrentVersion.AutoSize = true;
            this.lblCurrentVersion.Location = new System.Drawing.Point(8, 32);
            this.lblCurrentVersion.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCurrentVersion.Name = "lblCurrentVersion";
            this.lblCurrentVersion.Size = new System.Drawing.Size(157, 17);
            this.lblCurrentVersion.TabIndex = 1;
            this.lblCurrentVersion.Text = "Current version: 2.0.0.0";
            // 
            // tabDAX
            // 
            this.tabDAX.Controls.Add(this.groupBox5);
            this.tabDAX.Controls.Add(this.groupBox3);
            this.tabDAX.Location = new System.Drawing.Point(4, 25);
            this.tabDAX.Margin = new System.Windows.Forms.Padding(4);
            this.tabDAX.Name = "tabDAX";
            this.tabDAX.Padding = new System.Windows.Forms.Padding(4);
            this.tabDAX.Size = new System.Drawing.Size(495, 450);
            this.tabDAX.TabIndex = 1;
            this.tabDAX.Text = "DAX";
            this.tabDAX.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox5.Controls.Add(this.cmbSeparators);
            this.groupBox5.Location = new System.Drawing.Point(8, 76);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox5.Size = new System.Drawing.Size(476, 65);
            this.groupBox5.TabIndex = 5;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Separators";
            // 
            // tabSerialization
            // 
            this.tabSerialization.Controls.Add(this.groupBox4);
            this.tabSerialization.Controls.Add(this.groupBox2);
            this.tabSerialization.Location = new System.Drawing.Point(4, 25);
            this.tabSerialization.Margin = new System.Windows.Forms.Padding(4);
            this.tabSerialization.Name = "tabSerialization";
            this.tabSerialization.Padding = new System.Windows.Forms.Padding(4);
            this.tabSerialization.Size = new System.Drawing.Size(495, 450);
            this.tabSerialization.TabIndex = 3;
            this.tabSerialization.Text = "Serialization";
            this.tabSerialization.UseVisualStyleBackColor = true;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.chkAlsoSaveAsBim);
            this.groupBox4.Controls.Add(this.chkPrefixFiles);
            this.groupBox4.Controls.Add(this.chkLocalRelationships);
            this.groupBox4.Controls.Add(this.chkLocalTranslations);
            this.groupBox4.Controls.Add(this.chkLocalPerspectives);
            this.groupBox4.Controls.Add(this.treeView1);
            this.groupBox4.Controls.Add(this.label1);
            this.groupBox4.Location = new System.Drawing.Point(8, 121);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox4.Size = new System.Drawing.Size(476, 322);
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
            this.treeView1.Location = new System.Drawing.Point(8, 90);
            this.treeView1.Margin = new System.Windows.Forms.Padding(4);
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
            treeNode6.Name = "Calculation Items";
            treeNode6.Text = "Calculation Items";
            treeNode7.Checked = true;
            treeNode7.Name = "Columns";
            treeNode7.Text = "Columns";
            treeNode8.Checked = true;
            treeNode8.Name = "Hierarchies";
            treeNode8.Text = "Hierarchies";
            treeNode9.Checked = true;
            treeNode9.Name = "Measures";
            treeNode9.Text = "Measures";
            treeNode10.Checked = true;
            treeNode10.Name = "Partitions";
            treeNode10.Text = "Partitions";
            treeNode11.Checked = true;
            treeNode11.Name = "Tables";
            treeNode11.Text = "Tables";
            treeNode12.Checked = true;
            treeNode12.Name = "Translations";
            treeNode12.Text = "Translations";
            this.treeView1.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode1,
            treeNode2,
            treeNode3,
            treeNode4,
            treeNode11,
            treeNode12});
            this.treeView1.PathSeparator = "/";
            this.treeView1.ShowLines = false;
            this.treeView1.Size = new System.Drawing.Size(459, 141);
            this.treeView1.TabIndex = 1;
            this.treeView1.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView1_AfterCheck);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(8, 48);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(459, 38);
            this.label1.TabIndex = 0;
            this.label1.Text = "Check the type of objects you wish to serialize as individual files (unchecked ob" +
    "jects are serialized in the parent object):";
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
            this.groupBox2.Location = new System.Drawing.Point(8, 7);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox2.Size = new System.Drawing.Size(476, 106);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "General Serialization Settings";
            // 
            // tabCurrentModel
            // 
            this.tabCurrentModel.Controls.Add(this.grpSaveToFolder);
            this.tabCurrentModel.Controls.Add(this.groupBox8);
            this.tabCurrentModel.Location = new System.Drawing.Point(4, 25);
            this.tabCurrentModel.Margin = new System.Windows.Forms.Padding(4);
            this.tabCurrentModel.Name = "tabCurrentModel";
            this.tabCurrentModel.Padding = new System.Windows.Forms.Padding(4);
            this.tabCurrentModel.Size = new System.Drawing.Size(495, 450);
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
            this.grpSaveToFolder.Location = new System.Drawing.Point(8, 121);
            this.grpSaveToFolder.Margin = new System.Windows.Forms.Padding(4);
            this.grpSaveToFolder.Name = "grpSaveToFolder";
            this.grpSaveToFolder.Padding = new System.Windows.Forms.Padding(4);
            this.grpSaveToFolder.Size = new System.Drawing.Size(476, 322);
            this.grpSaveToFolder.TabIndex = 6;
            this.grpSaveToFolder.TabStop = false;
            this.grpSaveToFolder.Text = "Current Model \"Save to Folder\" Settings";
            // 
            // chkAlsoSaveAsBimCM
            // 
            this.chkAlsoSaveAsBimCM.AutoSize = true;
            this.chkAlsoSaveAsBimCM.Location = new System.Drawing.Point(191, 23);
            this.chkAlsoSaveAsBimCM.Name = "chkAlsoSaveAsBimCM";
            this.chkAlsoSaveAsBimCM.Size = new System.Drawing.Size(140, 21);
            this.chkAlsoSaveAsBimCM.TabIndex = 11;
            this.chkAlsoSaveAsBimCM.Text = "Also save as .bim";
            this.toolTip1.SetToolTip(this.chkAlsoSaveAsBimCM, "If checked, a .bim file will be saved along with the folder serialized version.");
            this.chkAlsoSaveAsBimCM.UseVisualStyleBackColor = true;
            // 
            // treeView2
            // 
            this.treeView2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeView2.CheckBoxes = true;
            this.treeView2.Location = new System.Drawing.Point(8, 90);
            this.treeView2.Margin = new System.Windows.Forms.Padding(4);
            this.treeView2.Name = "treeView2";
            treeNode13.Checked = true;
            treeNode13.Name = "Data Sources";
            treeNode13.Text = "Data Sources";
            treeNode14.Checked = true;
            treeNode14.Name = "Perspectives";
            treeNode14.Text = "Perspectives";
            treeNode15.Checked = true;
            treeNode15.Name = "Relationships";
            treeNode15.Text = "Relationships";
            treeNode16.Checked = true;
            treeNode16.Name = "Roles";
            treeNode16.Text = "Roles";
            treeNode17.Name = "Annotations";
            treeNode17.Text = "Annotations";
            treeNode18.Checked = true;
            treeNode18.Name = "Calculation Items";
            treeNode18.Text = "Calculation Items";
            treeNode19.Checked = true;
            treeNode19.Name = "Columns";
            treeNode19.Text = "Columns";
            treeNode20.Checked = true;
            treeNode20.Name = "Hierarchies";
            treeNode20.Text = "Hierarchies";
            treeNode21.Checked = true;
            treeNode21.Name = "Measures";
            treeNode21.Text = "Measures";
            treeNode22.Checked = true;
            treeNode22.Name = "Partitions";
            treeNode22.Text = "Partitions";
            treeNode23.Checked = true;
            treeNode23.Name = "Tables";
            treeNode23.Text = "Tables";
            treeNode24.Checked = true;
            treeNode24.Name = "Translations";
            treeNode24.Text = "Translations";
            this.treeView2.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode13,
            treeNode14,
            treeNode15,
            treeNode16,
            treeNode23,
            treeNode24});
            this.treeView2.PathSeparator = "/";
            this.treeView2.ShowLines = false;
            this.treeView2.Size = new System.Drawing.Size(459, 141);
            this.treeView2.TabIndex = 1;
            this.treeView2.AfterCheck += new System.Windows.Forms.TreeViewEventHandler(this.treeView2_AfterCheck);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(8, 48);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(459, 38);
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
            this.groupBox8.Location = new System.Drawing.Point(8, 7);
            this.groupBox8.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox8.Name = "groupBox8";
            this.groupBox8.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox8.Size = new System.Drawing.Size(476, 106);
            this.groupBox8.TabIndex = 5;
            this.groupBox8.TabStop = false;
            this.groupBox8.Text = "Current Model Serialization Settings";
            // 
            // PreferencesForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(535, 544);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Margin = new System.Windows.Forms.Padding(4);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(550, 581);
            this.Name = "PreferencesForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Tabular Editor Preferences";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.PreferencesForm_FormClosed);
            this.Load += new System.EventHandler(this.PreferencesForm_Load);
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
        private System.Windows.Forms.CheckBox chkAlsoSaveAsBim;
        private System.Windows.Forms.CheckBox chkAlsoSaveAsBimCM;
    }
}