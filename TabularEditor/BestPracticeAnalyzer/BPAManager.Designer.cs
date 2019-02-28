namespace TabularEditor.BestPracticeAnalyzer
{
    partial class BPAManager
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
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.btnClone = new System.Windows.Forms.Button();
            this.btnDown = new System.Windows.Forms.Button();
            this.btnUp = new System.Windows.Forms.Button();
            this.tvRules = new Aga.Controls.Tree.TreeViewAdv();
            this.colName = new Aga.Controls.Tree.TreeColumn();
            this.colScope = new Aga.Controls.Tree.TreeColumn();
            this.colDefinition = new Aga.Controls.Tree.TreeColumn();
            this.colSeverity = new Aga.Controls.Tree.TreeColumn();
            this.chkRuleEnabled = new Aga.Controls.Tree.NodeControls.NodeCheckBox();
            this.txtName = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.txtScope = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.txtSeverity = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.txtDefinition = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.tvRuleDefinitions = new Aga.Controls.Tree.TreeViewAdv();
            this.txtRuleDefinitionName = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.btnDeleteRule = new System.Windows.Forms.Button();
            this.btnMoveTo = new System.Windows.Forms.Button();
            this.btnEditRule = new System.Windows.Forms.Button();
            this.btnNewRule = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRemoveRuleDefinition = new System.Windows.Forms.Button();
            this.btnAddRuleDefinition = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Location = new System.Drawing.Point(13, 13);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(549, 387);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.btnClone);
            this.tabPage1.Controls.Add(this.btnDown);
            this.tabPage1.Controls.Add(this.btnUp);
            this.tabPage1.Controls.Add(this.tvRules);
            this.tabPage1.Controls.Add(this.tvRuleDefinitions);
            this.tabPage1.Controls.Add(this.btnDeleteRule);
            this.tabPage1.Controls.Add(this.btnMoveTo);
            this.tabPage1.Controls.Add(this.btnEditRule);
            this.tabPage1.Controls.Add(this.btnNewRule);
            this.tabPage1.Controls.Add(this.label2);
            this.tabPage1.Controls.Add(this.btnRemoveRuleDefinition);
            this.tabPage1.Controls.Add(this.btnAddRuleDefinition);
            this.tabPage1.Controls.Add(this.label1);
            this.tabPage1.Location = new System.Drawing.Point(4, 22);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(3);
            this.tabPage1.Size = new System.Drawing.Size(541, 361);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Current model";
            this.tabPage1.UseVisualStyleBackColor = true;
            // 
            // btnClone
            // 
            this.btnClone.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClone.Enabled = false;
            this.btnClone.Location = new System.Drawing.Point(457, 188);
            this.btnClone.Name = "btnClone";
            this.btnClone.Size = new System.Drawing.Size(75, 23);
            this.btnClone.TabIndex = 15;
            this.btnClone.Text = "Clone rule";
            this.btnClone.UseVisualStyleBackColor = true;
            this.btnClone.Click += new System.EventHandler(this.btnClone_Click);
            // 
            // btnDown
            // 
            this.btnDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDown.Enabled = false;
            this.btnDown.Location = new System.Drawing.Point(498, 104);
            this.btnDown.Name = "btnDown";
            this.btnDown.Size = new System.Drawing.Size(34, 23);
            this.btnDown.TabIndex = 14;
            this.btnDown.Text = "∨";
            this.btnDown.UseVisualStyleBackColor = true;
            this.btnDown.Click += new System.EventHandler(this.btnDown_Click);
            // 
            // btnUp
            // 
            this.btnUp.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUp.Enabled = false;
            this.btnUp.Location = new System.Drawing.Point(457, 104);
            this.btnUp.Name = "btnUp";
            this.btnUp.Size = new System.Drawing.Size(34, 23);
            this.btnUp.TabIndex = 13;
            this.btnUp.Text = "∧";
            this.btnUp.UseVisualStyleBackColor = true;
            this.btnUp.Click += new System.EventHandler(this.btnUp_Click);
            // 
            // tvRules
            // 
            this.tvRules.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvRules.BackColor = System.Drawing.SystemColors.Window;
            this.tvRules.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvRules.Columns.Add(this.colName);
            this.tvRules.Columns.Add(this.colScope);
            this.tvRules.Columns.Add(this.colDefinition);
            this.tvRules.Columns.Add(this.colSeverity);
            this.tvRules.DefaultToolTipProvider = null;
            this.tvRules.DragDropMarkColor = System.Drawing.Color.Black;
            this.tvRules.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.tvRules.FullRowSelect = true;
            this.tvRules.Indent = 0;
            this.tvRules.LineColor = System.Drawing.SystemColors.ControlDark;
            this.tvRules.Location = new System.Drawing.Point(10, 159);
            this.tvRules.Model = null;
            this.tvRules.Name = "tvRules";
            this.tvRules.NodeControls.Add(this.chkRuleEnabled);
            this.tvRules.NodeControls.Add(this.txtName);
            this.tvRules.NodeControls.Add(this.txtScope);
            this.tvRules.NodeControls.Add(this.txtSeverity);
            this.tvRules.NodeControls.Add(this.txtDefinition);
            this.tvRules.SelectedNode = null;
            this.tvRules.SelectionMode = Aga.Controls.Tree.TreeSelectionMode.Multi;
            this.tvRules.ShowLines = false;
            this.tvRules.ShowNodeToolTips = true;
            this.tvRules.Size = new System.Drawing.Size(441, 196);
            this.tvRules.TabIndex = 12;
            this.tvRules.Text = "tvRuleDefinitions";
            this.tvRules.UseColumns = true;
            this.tvRules.SelectionChanged += new System.EventHandler(this.tvRules_SelectionChanged);
            // 
            // colName
            // 
            this.colName.Header = "Rule name";
            this.colName.SortOrder = System.Windows.Forms.SortOrder.None;
            this.colName.TooltipText = null;
            this.colName.Width = 250;
            // 
            // colScope
            // 
            this.colScope.Header = "Scope";
            this.colScope.SortOrder = System.Windows.Forms.SortOrder.None;
            this.colScope.TooltipText = null;
            this.colScope.Width = 106;
            // 
            // colDefinition
            // 
            this.colDefinition.Header = "Collection";
            this.colDefinition.IsVisible = false;
            this.colDefinition.SortOrder = System.Windows.Forms.SortOrder.None;
            this.colDefinition.TooltipText = null;
            this.colDefinition.Width = 106;
            // 
            // colSeverity
            // 
            this.colSeverity.Header = "Severity";
            this.colSeverity.SortOrder = System.Windows.Forms.SortOrder.None;
            this.colSeverity.TooltipText = null;
            this.colSeverity.Width = 60;
            // 
            // chkRuleEnabled
            // 
            this.chkRuleEnabled.DataPropertyName = "Enabled";
            this.chkRuleEnabled.EditEnabled = true;
            this.chkRuleEnabled.LeftMargin = 0;
            this.chkRuleEnabled.ParentColumn = this.colName;
            this.chkRuleEnabled.CheckStateChanged += new System.EventHandler<Aga.Controls.Tree.TreePathEventArgs>(this.chkRuleEnabled_CheckStateChanged);
            this.chkRuleEnabled.IsEditEnabledValueNeeded += new System.EventHandler<Aga.Controls.Tree.NodeControls.NodeControlValueEventArgs>(this.chkRuleEnabled_IsEditEnabledValueNeeded);
            this.chkRuleEnabled.IsVisibleValueNeeded += new System.EventHandler<Aga.Controls.Tree.NodeControls.NodeControlValueEventArgs>(this.chkRuleEnabled_IsVisibleValueNeeded);
            // 
            // txtName
            // 
            this.txtName.DataPropertyName = "Name";
            this.txtName.LeftMargin = 0;
            this.txtName.ParentColumn = this.colName;
            this.txtName.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            this.txtName.UseCompatibleTextRendering = true;
            this.txtName.DrawText += new System.EventHandler<Aga.Controls.Tree.NodeControls.DrawEventArgs>(this.txtName_DrawText);
            // 
            // txtScope
            // 
            this.txtScope.DataPropertyName = "ScopeString";
            this.txtScope.LeftMargin = 3;
            this.txtScope.ParentColumn = this.colScope;
            this.txtScope.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            this.txtScope.UseCompatibleTextRendering = true;
            // 
            // txtSeverity
            // 
            this.txtSeverity.DataPropertyName = "Severity";
            this.txtSeverity.LeftMargin = 3;
            this.txtSeverity.ParentColumn = this.colSeverity;
            this.txtSeverity.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.txtSeverity.UseCompatibleTextRendering = true;
            // 
            // txtDefinition
            // 
            this.txtDefinition.LeftMargin = 3;
            this.txtDefinition.ParentColumn = this.colDefinition;
            this.txtDefinition.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            this.txtDefinition.UseCompatibleTextRendering = true;
            this.txtDefinition.VirtualMode = true;
            this.txtDefinition.ValueNeeded += new System.EventHandler<Aga.Controls.Tree.NodeControls.NodeControlValueEventArgs>(this.txtDefinition_ValueNeeded);
            // 
            // tvRuleDefinitions
            // 
            this.tvRuleDefinitions.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tvRuleDefinitions.BackColor = System.Drawing.SystemColors.Window;
            this.tvRuleDefinitions.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvRuleDefinitions.DefaultToolTipProvider = null;
            this.tvRuleDefinitions.DragDropMarkColor = System.Drawing.Color.Black;
            this.tvRuleDefinitions.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tvRuleDefinitions.FullRowSelect = true;
            this.tvRuleDefinitions.Indent = 0;
            this.tvRuleDefinitions.LineColor = System.Drawing.SystemColors.ControlDark;
            this.tvRuleDefinitions.Location = new System.Drawing.Point(10, 24);
            this.tvRuleDefinitions.Model = null;
            this.tvRuleDefinitions.Name = "tvRuleDefinitions";
            this.tvRuleDefinitions.NodeControls.Add(this.txtRuleDefinitionName);
            this.tvRuleDefinitions.SelectedNode = null;
            this.tvRuleDefinitions.ShowLines = false;
            this.tvRuleDefinitions.ShowPlusMinus = false;
            this.tvRuleDefinitions.Size = new System.Drawing.Size(441, 102);
            this.tvRuleDefinitions.TabIndex = 11;
            this.tvRuleDefinitions.Text = "tvRuleDefinitions";
            this.tvRuleDefinitions.SelectionChanged += new System.EventHandler(this.tvRuleDefinitions_SelectionChanged);
            // 
            // txtRuleDefinitionName
            // 
            this.txtRuleDefinitionName.DataPropertyName = "Name";
            this.txtRuleDefinitionName.LeftMargin = 0;
            this.txtRuleDefinitionName.ParentColumn = null;
            this.txtRuleDefinitionName.UseCompatibleTextRendering = true;
            // 
            // btnDeleteRule
            // 
            this.btnDeleteRule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteRule.Enabled = false;
            this.btnDeleteRule.Location = new System.Drawing.Point(457, 246);
            this.btnDeleteRule.Name = "btnDeleteRule";
            this.btnDeleteRule.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteRule.TabIndex = 10;
            this.btnDeleteRule.Text = "Delete rule";
            this.btnDeleteRule.UseVisualStyleBackColor = true;
            this.btnDeleteRule.Click += new System.EventHandler(this.btnDeleteRule_Click);
            // 
            // btnMoveTo
            // 
            this.btnMoveTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveTo.Enabled = false;
            this.btnMoveTo.Location = new System.Drawing.Point(457, 333);
            this.btnMoveTo.Name = "btnMoveTo";
            this.btnMoveTo.Size = new System.Drawing.Size(75, 23);
            this.btnMoveTo.TabIndex = 8;
            this.btnMoveTo.Text = "Move to...";
            this.btnMoveTo.UseVisualStyleBackColor = true;
            // 
            // btnEditRule
            // 
            this.btnEditRule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditRule.Enabled = false;
            this.btnEditRule.Location = new System.Drawing.Point(457, 217);
            this.btnEditRule.Name = "btnEditRule";
            this.btnEditRule.Size = new System.Drawing.Size(75, 23);
            this.btnEditRule.TabIndex = 7;
            this.btnEditRule.Text = "Edit rule...";
            this.btnEditRule.UseVisualStyleBackColor = true;
            this.btnEditRule.Click += new System.EventHandler(this.btnEditRule_Click);
            // 
            // btnNewRule
            // 
            this.btnNewRule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewRule.Enabled = false;
            this.btnNewRule.Location = new System.Drawing.Point(457, 159);
            this.btnNewRule.Name = "btnNewRule";
            this.btnNewRule.Size = new System.Drawing.Size(75, 23);
            this.btnNewRule.TabIndex = 6;
            this.btnNewRule.Text = "New rule...";
            this.btnNewRule.UseVisualStyleBackColor = true;
            this.btnNewRule.Click += new System.EventHandler(this.btnNewRule_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 143);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(96, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Rules in collection:";
            // 
            // btnRemoveRuleDefinition
            // 
            this.btnRemoveRuleDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveRuleDefinition.Enabled = false;
            this.btnRemoveRuleDefinition.Location = new System.Drawing.Point(457, 51);
            this.btnRemoveRuleDefinition.Name = "btnRemoveRuleDefinition";
            this.btnRemoveRuleDefinition.Size = new System.Drawing.Size(75, 23);
            this.btnRemoveRuleDefinition.TabIndex = 3;
            this.btnRemoveRuleDefinition.Text = "Remove";
            this.btnRemoveRuleDefinition.UseVisualStyleBackColor = true;
            this.btnRemoveRuleDefinition.Click += new System.EventHandler(this.btnRemoveRuleDefinition_Click);
            // 
            // btnAddRuleDefinition
            // 
            this.btnAddRuleDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddRuleDefinition.Location = new System.Drawing.Point(457, 22);
            this.btnAddRuleDefinition.Name = "btnAddRuleDefinition";
            this.btnAddRuleDefinition.Size = new System.Drawing.Size(75, 23);
            this.btnAddRuleDefinition.TabIndex = 2;
            this.btnAddRuleDefinition.Text = "Add...";
            this.btnAddRuleDefinition.UseVisualStyleBackColor = true;
            this.btnAddRuleDefinition.Click += new System.EventHandler(this.btnAddRuleDefinition_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Rule collections:";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(406, 406);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Cancel";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(487, 406);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // BPAManager
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(574, 441);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.tabControl1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(474, 450);
            this.Name = "BPAManager";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Manage Best Practice Rules";
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.Button btnRemoveRuleDefinition;
        private System.Windows.Forms.Button btnAddRuleDefinition;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnMoveTo;
        private System.Windows.Forms.Button btnEditRule;
        private System.Windows.Forms.Button btnNewRule;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnDeleteRule;
        private System.Windows.Forms.Button btnClose;
        private Aga.Controls.Tree.TreeViewAdv tvRules;
        private Aga.Controls.Tree.TreeColumn colName;
        private Aga.Controls.Tree.TreeColumn colScope;
        private Aga.Controls.Tree.TreeColumn colSeverity;
        private Aga.Controls.Tree.NodeControls.NodeCheckBox chkRuleEnabled;
        private Aga.Controls.Tree.NodeControls.NodeTextBox txtName;
        private Aga.Controls.Tree.NodeControls.NodeTextBox txtScope;
        private Aga.Controls.Tree.NodeControls.NodeTextBox txtSeverity;
        private Aga.Controls.Tree.TreeViewAdv tvRuleDefinitions;
        private Aga.Controls.Tree.NodeControls.NodeTextBox txtRuleDefinitionName;
        private System.Windows.Forms.Button btnOK;
        private Aga.Controls.Tree.TreeColumn colDefinition;
        private Aga.Controls.Tree.NodeControls.NodeTextBox txtDefinition;
        private System.Windows.Forms.Button btnDown;
        private System.Windows.Forms.Button btnUp;
        private System.Windows.Forms.Button btnClone;
    }
}