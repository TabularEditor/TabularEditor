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
            this.tvRules = new Aga.Controls.Tree.TreeViewAdv();
            this.colName = new Aga.Controls.Tree.TreeColumn();
            this.colScope = new Aga.Controls.Tree.TreeColumn();
            this.colSeverity = new Aga.Controls.Tree.TreeColumn();
            this.chkRuleEnabled = new Aga.Controls.Tree.NodeControls.NodeCheckBox();
            this.txtName = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.txtScope = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.txtSeverity = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.tvRuleDefinitions = new Aga.Controls.Tree.TreeViewAdv();
            this.txtRuleDefinitionName = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.btnDeleteRule = new System.Windows.Forms.Button();
            this.btnCopyTo = new System.Windows.Forms.Button();
            this.btnMoveTo = new System.Windows.Forms.Button();
            this.btnEditRule = new System.Windows.Forms.Button();
            this.btnNewRule = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.btnRemoveRuleDefinition = new System.Windows.Forms.Button();
            this.btnAddRuleDefinition = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.btnClose = new System.Windows.Forms.Button();
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
            this.tabControl1.Size = new System.Drawing.Size(526, 382);
            this.tabControl1.TabIndex = 0;
            // 
            // tabPage1
            // 
            this.tabPage1.Controls.Add(this.tvRules);
            this.tabPage1.Controls.Add(this.tvRuleDefinitions);
            this.tabPage1.Controls.Add(this.btnDeleteRule);
            this.tabPage1.Controls.Add(this.btnCopyTo);
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
            this.tabPage1.Size = new System.Drawing.Size(518, 356);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "Current model";
            this.tabPage1.UseVisualStyleBackColor = true;
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
            this.tvRules.Columns.Add(this.colSeverity);
            this.tvRules.DefaultToolTipProvider = null;
            this.tvRules.DragDropMarkColor = System.Drawing.Color.Black;
            this.tvRules.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F);
            this.tvRules.FullRowSelect = true;
            this.tvRules.Indent = 0;
            this.tvRules.LineColor = System.Drawing.SystemColors.ControlDark;
            this.tvRules.Location = new System.Drawing.Point(10, 145);
            this.tvRules.Model = null;
            this.tvRules.Name = "tvRules";
            this.tvRules.NodeControls.Add(this.chkRuleEnabled);
            this.tvRules.NodeControls.Add(this.txtName);
            this.tvRules.NodeControls.Add(this.txtScope);
            this.tvRules.NodeControls.Add(this.txtSeverity);
            this.tvRules.SelectedNode = null;
            this.tvRules.SelectionMode = Aga.Controls.Tree.TreeSelectionMode.Multi;
            this.tvRules.ShowLines = false;
            this.tvRules.ShowPlusMinus = false;
            this.tvRules.Size = new System.Drawing.Size(418, 205);
            this.tvRules.TabIndex = 12;
            this.tvRules.Text = "tvRuleDefinitions";
            this.tvRules.UseColumns = true;
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
            // 
            // txtName
            // 
            this.txtName.DataPropertyName = "Name";
            this.txtName.LeftMargin = 3;
            this.txtName.ParentColumn = this.colName;
            this.txtName.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            this.txtName.UseCompatibleTextRendering = true;
            // 
            // txtScope
            // 
            this.txtScope.DataPropertyName = "Scope";
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
            this.tvRuleDefinitions.Size = new System.Drawing.Size(418, 102);
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
            this.btnDeleteRule.Location = new System.Drawing.Point(434, 202);
            this.btnDeleteRule.Name = "btnDeleteRule";
            this.btnDeleteRule.Size = new System.Drawing.Size(75, 23);
            this.btnDeleteRule.TabIndex = 10;
            this.btnDeleteRule.Text = "Delete";
            this.btnDeleteRule.UseVisualStyleBackColor = true;
            // 
            // btnCopyTo
            // 
            this.btnCopyTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopyTo.Location = new System.Drawing.Point(434, 299);
            this.btnCopyTo.Name = "btnCopyTo";
            this.btnCopyTo.Size = new System.Drawing.Size(75, 23);
            this.btnCopyTo.TabIndex = 9;
            this.btnCopyTo.Text = "Copy to...";
            this.btnCopyTo.UseVisualStyleBackColor = true;
            // 
            // btnMoveTo
            // 
            this.btnMoveTo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnMoveTo.Location = new System.Drawing.Point(434, 328);
            this.btnMoveTo.Name = "btnMoveTo";
            this.btnMoveTo.Size = new System.Drawing.Size(75, 23);
            this.btnMoveTo.TabIndex = 8;
            this.btnMoveTo.Text = "Move to...";
            this.btnMoveTo.UseVisualStyleBackColor = true;
            // 
            // btnEditRule
            // 
            this.btnEditRule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEditRule.Location = new System.Drawing.Point(434, 173);
            this.btnEditRule.Name = "btnEditRule";
            this.btnEditRule.Size = new System.Drawing.Size(75, 23);
            this.btnEditRule.TabIndex = 7;
            this.btnEditRule.Text = "Edit...";
            this.btnEditRule.UseVisualStyleBackColor = true;
            // 
            // btnNewRule
            // 
            this.btnNewRule.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnNewRule.Location = new System.Drawing.Point(434, 144);
            this.btnNewRule.Name = "btnNewRule";
            this.btnNewRule.Size = new System.Drawing.Size(75, 23);
            this.btnNewRule.TabIndex = 6;
            this.btnNewRule.Text = "New...";
            this.btnNewRule.UseVisualStyleBackColor = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 129);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(37, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Rules:";
            // 
            // btnRemoveRuleDefinition
            // 
            this.btnRemoveRuleDefinition.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnRemoveRuleDefinition.Location = new System.Drawing.Point(434, 51);
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
            this.btnAddRuleDefinition.Location = new System.Drawing.Point(434, 22);
            this.btnAddRuleDefinition.Name = "btnAddRuleDefinition";
            this.btnAddRuleDefinition.Size = new System.Drawing.Size(75, 23);
            this.btnAddRuleDefinition.TabIndex = 2;
            this.btnAddRuleDefinition.Text = "Add...";
            this.btnAddRuleDefinition.UseVisualStyleBackColor = true;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 7);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Rule definitions:";
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(464, 401);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(75, 23);
            this.btnClose.TabIndex = 1;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            // 
            // BPAManager
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(551, 436);
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
        private System.Windows.Forms.Button btnCopyTo;
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
    }
}