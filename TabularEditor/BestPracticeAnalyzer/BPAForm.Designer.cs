namespace TabularEditor.UI.Dialogs
{
    partial class BPAForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(BPAForm));
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.bpaResultGoTo = new System.Windows.Forms.ToolStripMenuItem();
            this.bpaResultGoToSep = new System.Windows.Forms.ToolStripSeparator();
            this.bpaResultIgnore = new System.Windows.Forms.ToolStripMenuItem();
            this.bpaResultIgnoreSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.bpaResultIgnoreRule = new System.Windows.Forms.ToolStripMenuItem();
            this.bpaResultScript = new System.Windows.Forms.ToolStripMenuItem();
            this.bpaResultScriptSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.bpaResultScriptRule = new System.Windows.Forms.ToolStripMenuItem();
            this.bpaResultFix = new System.Windows.Forms.ToolStripMenuItem();
            this.bpaResultFixSelected = new System.Windows.Forms.ToolStripMenuItem();
            this.bpaResultFixRule = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.btnManageRules = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnGoto = new System.Windows.Forms.ToolStripButton();
            this.btnScript = new System.Windows.Forms.ToolStripButton();
            this.btnIgnore = new System.Windows.Forms.ToolStripButton();
            this.tvResults = new Aga.Controls.Tree.TreeViewAdv();
            this.colObject = new Aga.Controls.Tree.TreeColumn();
            this.colType = new Aga.Controls.Tree.TreeColumn();
            this.txtObjectName = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.txtObjectType = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.contextMenuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // splitter1
            // 
            this.splitter1.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 25);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(379, 3);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bpaResultGoTo,
            this.bpaResultGoToSep,
            this.bpaResultIgnore,
            this.bpaResultScript,
            this.bpaResultFix});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(169, 98);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // bpaResultGoTo
            // 
            this.bpaResultGoTo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.bpaResultGoTo.Name = "bpaResultGoTo";
            this.bpaResultGoTo.Size = new System.Drawing.Size(168, 22);
            this.bpaResultGoTo.Text = "Go to object...";
            this.bpaResultGoTo.Click += new System.EventHandler(this.bpaResultGoTo_Click);
            // 
            // bpaResultGoToSep
            // 
            this.bpaResultGoToSep.Name = "bpaResultGoToSep";
            this.bpaResultGoToSep.Size = new System.Drawing.Size(165, 6);
            // 
            // bpaResultIgnore
            // 
            this.bpaResultIgnore.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bpaResultIgnoreSelected,
            this.bpaResultIgnoreRule});
            this.bpaResultIgnore.Name = "bpaResultIgnore";
            this.bpaResultIgnore.Size = new System.Drawing.Size(168, 22);
            this.bpaResultIgnore.Text = "Ignore rule";
            // 
            // bpaResultIgnoreSelected
            // 
            this.bpaResultIgnoreSelected.Name = "bpaResultIgnoreSelected";
            this.bpaResultIgnoreSelected.Size = new System.Drawing.Size(154, 22);
            this.bpaResultIgnoreSelected.Text = "Selected object";
            this.bpaResultIgnoreSelected.Click += new System.EventHandler(this.bpaResultIgnoreSelected_Click);
            // 
            // bpaResultIgnoreRule
            // 
            this.bpaResultIgnoreRule.Name = "bpaResultIgnoreRule";
            this.bpaResultIgnoreRule.Size = new System.Drawing.Size(154, 22);
            this.bpaResultIgnoreRule.Text = "Entire rule";
            this.bpaResultIgnoreRule.Click += new System.EventHandler(this.bpaResultIgnoreRule_Click);
            // 
            // bpaResultScript
            // 
            this.bpaResultScript.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bpaResultScriptSelected,
            this.bpaResultScriptRule});
            this.bpaResultScript.Name = "bpaResultScript";
            this.bpaResultScript.Size = new System.Drawing.Size(168, 22);
            this.bpaResultScript.Text = "Generate fix script";
            // 
            // bpaResultScriptSelected
            // 
            this.bpaResultScriptSelected.Name = "bpaResultScriptSelected";
            this.bpaResultScriptSelected.Size = new System.Drawing.Size(154, 22);
            this.bpaResultScriptSelected.Text = "Selected object";
            this.bpaResultScriptSelected.Click += new System.EventHandler(this.bpaResultScriptSelected_Click);
            // 
            // bpaResultScriptRule
            // 
            this.bpaResultScriptRule.Name = "bpaResultScriptRule";
            this.bpaResultScriptRule.Size = new System.Drawing.Size(154, 22);
            this.bpaResultScriptRule.Text = "Entire rule";
            this.bpaResultScriptRule.Click += new System.EventHandler(this.bpaResultScriptRule_Click);
            // 
            // bpaResultFix
            // 
            this.bpaResultFix.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bpaResultFixSelected,
            this.bpaResultFixRule});
            this.bpaResultFix.Name = "bpaResultFix";
            this.bpaResultFix.Size = new System.Drawing.Size(168, 22);
            this.bpaResultFix.Text = "Apply fix";
            // 
            // bpaResultFixSelected
            // 
            this.bpaResultFixSelected.Enabled = false;
            this.bpaResultFixSelected.Name = "bpaResultFixSelected";
            this.bpaResultFixSelected.Size = new System.Drawing.Size(154, 22);
            this.bpaResultFixSelected.Text = "Selected object";
            // 
            // bpaResultFixRule
            // 
            this.bpaResultFixRule.Enabled = false;
            this.bpaResultFixRule.Name = "bpaResultFixRule";
            this.bpaResultFixRule.Size = new System.Drawing.Size(154, 22);
            this.bpaResultFixRule.Text = "Entire rule";
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 585);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(379, 22);
            this.statusStrip1.TabIndex = 7;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(0, 17);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnRefresh,
            this.btnManageRules,
            this.toolStripSeparator1,
            this.btnGoto,
            this.btnScript,
            this.btnIgnore});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(379, 25);
            this.toolStrip1.TabIndex = 9;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnRefresh
            // 
            this.btnRefresh.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnRefresh.Image = ((System.Drawing.Image)(resources.GetObject("btnRefresh.Image")));
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(50, 22);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnAnalyzeAll_Click);
            // 
            // btnManageRules
            // 
            this.btnManageRules.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnManageRules.Image = ((System.Drawing.Image)(resources.GetObject("btnManageRules.Image")));
            this.btnManageRules.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnManageRules.Name = "btnManageRules";
            this.btnManageRules.Size = new System.Drawing.Size(91, 22);
            this.btnManageRules.Text = "Manage rules...";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnGoto
            // 
            this.btnGoto.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnGoto.Enabled = false;
            this.btnGoto.Image = ((System.Drawing.Image)(resources.GetObject("btnGoto.Image")));
            this.btnGoto.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGoto.Name = "btnGoto";
            this.btnGoto.Size = new System.Drawing.Size(37, 22);
            this.btnGoto.Text = "Goto";
            this.btnGoto.Click += new System.EventHandler(this.btnGoto_Click);
            // 
            // btnScript
            // 
            this.btnScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnScript.Enabled = false;
            this.btnScript.Image = ((System.Drawing.Image)(resources.GetObject("btnScript.Image")));
            this.btnScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnScript.Name = "btnScript";
            this.btnScript.Size = new System.Drawing.Size(41, 22);
            this.btnScript.Text = "Script";
            this.btnScript.Click += new System.EventHandler(this.btnScript_Click);
            // 
            // btnIgnore
            // 
            this.btnIgnore.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnIgnore.Enabled = false;
            this.btnIgnore.Image = ((System.Drawing.Image)(resources.GetObject("btnIgnore.Image")));
            this.btnIgnore.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(45, 22);
            this.btnIgnore.Text = "Ignore";
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // tvResults
            // 
            this.tvResults.BackColor = System.Drawing.SystemColors.Window;
            this.tvResults.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvResults.Columns.Add(this.colObject);
            this.tvResults.Columns.Add(this.colType);
            this.tvResults.DefaultToolTipProvider = null;
            this.tvResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvResults.DragDropMarkColor = System.Drawing.Color.Black;
            this.tvResults.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tvResults.FullRowSelect = true;
            this.tvResults.GridLineStyle = Aga.Controls.Tree.GridLineStyle.Horizontal;
            this.tvResults.HideSelection = true;
            this.tvResults.Indent = 8;
            this.tvResults.LineColor = System.Drawing.SystemColors.ControlDark;
            this.tvResults.Location = new System.Drawing.Point(0, 28);
            this.tvResults.Model = null;
            this.tvResults.Name = "tvResults";
            this.tvResults.NodeControls.Add(this.txtObjectName);
            this.tvResults.NodeControls.Add(this.txtObjectType);
            this.tvResults.SelectedNode = null;
            this.tvResults.SelectionMode = Aga.Controls.Tree.TreeSelectionMode.MultiSameParent;
            this.tvResults.ShowLines = false;
            this.tvResults.ShowNodeToolTips = true;
            this.tvResults.ShowPlusMinus = false;
            this.tvResults.Size = new System.Drawing.Size(379, 557);
            this.tvResults.TabIndex = 10;
            this.tvResults.UseColumns = true;
            this.tvResults.NodeMouseDoubleClick += new System.EventHandler<Aga.Controls.Tree.TreeNodeAdvMouseEventArgs>(this.tvResults_NodeMouseDoubleClick);
            this.tvResults.SelectionChanged += new System.EventHandler(this.tvResults_SelectionChanged);
            this.tvResults.Collapsed += new System.EventHandler<Aga.Controls.Tree.TreeViewAdvEventArgs>(this.tvResults_Collapsed);
            this.tvResults.Expanded += new System.EventHandler<Aga.Controls.Tree.TreeViewAdvEventArgs>(this.tvResults_Expanded);
            this.tvResults.Resize += new System.EventHandler(this.tvResults_Resize);
            // 
            // colObject
            // 
            this.colObject.Header = "Object";
            this.colObject.SortOrder = System.Windows.Forms.SortOrder.None;
            this.colObject.TooltipText = null;
            this.colObject.Width = 307;
            this.colObject.WidthChanged += new System.EventHandler(this.colObject_WidthChanged);
            // 
            // colType
            // 
            this.colType.Header = "Type";
            this.colType.SortOrder = System.Windows.Forms.SortOrder.None;
            this.colType.TooltipText = null;
            this.colType.Width = 70;
            // 
            // txtObjectName
            // 
            this.txtObjectName.DataPropertyName = "ObjectName";
            this.txtObjectName.DisplayHiddenContentInToolTip = false;
            this.txtObjectName.LeftMargin = 3;
            this.txtObjectName.ParentColumn = this.colObject;
            this.txtObjectName.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            this.txtObjectName.UseCompatibleTextRendering = true;
            this.txtObjectName.VirtualMode = true;
            this.txtObjectName.DrawText += new System.EventHandler<Aga.Controls.Tree.NodeControls.DrawEventArgs>(this.txtObjectName_DrawText);
            this.txtObjectName.ValueNeeded += new System.EventHandler<Aga.Controls.Tree.NodeControls.NodeControlValueEventArgs>(this.txtObjectName_ValueNeeded);
            // 
            // txtObjectType
            // 
            this.txtObjectType.DataPropertyName = "ObjectType";
            this.txtObjectType.DisplayHiddenContentInToolTip = false;
            this.txtObjectType.LeftMargin = 3;
            this.txtObjectType.ParentColumn = this.colType;
            this.txtObjectType.UseCompatibleTextRendering = true;
            // 
            // BPAForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(379, 607);
            this.Controls.Add(this.tvResults);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.toolStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "BPAForm";
            this.Text = "Best Practice Analyzer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BPAForm_FormClosing);
            this.contextMenuStrip1.ResumeLayout(false);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem bpaResultGoTo;
        private System.Windows.Forms.ToolStripSeparator bpaResultGoToSep;
        private System.Windows.Forms.ToolStripMenuItem bpaResultIgnore;
        private System.Windows.Forms.ToolStripMenuItem bpaResultIgnoreSelected;
        private System.Windows.Forms.ToolStripMenuItem bpaResultIgnoreRule;
        private System.Windows.Forms.ToolStripMenuItem bpaResultScript;
        private System.Windows.Forms.ToolStripMenuItem bpaResultScriptSelected;
        private System.Windows.Forms.ToolStripMenuItem bpaResultScriptRule;
        private System.Windows.Forms.ToolStripMenuItem bpaResultFix;
        private System.Windows.Forms.ToolStripMenuItem bpaResultFixSelected;
        private System.Windows.Forms.ToolStripMenuItem bpaResultFixRule;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnRefresh;
        private System.Windows.Forms.ToolStripButton btnManageRules;
        private Aga.Controls.Tree.TreeViewAdv tvResults;
        private Aga.Controls.Tree.TreeColumn colObject;
        private Aga.Controls.Tree.TreeColumn colType;
        private Aga.Controls.Tree.NodeControls.NodeTextBox txtObjectName;
        private Aga.Controls.Tree.NodeControls.NodeTextBox txtObjectType;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnGoto;
        private System.Windows.Forms.ToolStripButton btnScript;
        private System.Windows.Forms.ToolStripButton btnIgnore;
    }
}