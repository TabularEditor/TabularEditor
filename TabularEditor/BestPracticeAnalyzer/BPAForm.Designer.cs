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
            this.menContext = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.bpaResultGoTo = new System.Windows.Forms.ToolStripMenuItem();
            this.bpaResultIgnore = new System.Windows.Forms.ToolStripMenuItem();
            this.bpaResultUnignore = new System.Windows.Forms.ToolStripMenuItem();
            this.bpaResultGoToSep = new System.Windows.Forms.ToolStripSeparator();
            this.bpaResultScript = new System.Windows.Forms.ToolStripMenuItem();
            this.bpaResultFix = new System.Windows.Forms.ToolStripMenuItem();
            this.bpaResultSep2 = new System.Windows.Forms.ToolStripSeparator();
            this.bpaResultCopy = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnManageRules = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnGoto = new System.Windows.Forms.ToolStripButton();
            this.btnIgnore = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnScript = new System.Windows.Forms.ToolStripButton();
            this.btnFix = new System.Windows.Forms.ToolStripButton();
            this.btnRefresh = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnShowIgnored = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.btnCollapseAll = new System.Windows.Forms.ToolStripButton();
            this.btnExpandAll = new System.Windows.Forms.ToolStripButton();
            this.tvResults = new Aga.Controls.Tree.TreeViewAdv();
            this.colObject = new Aga.Controls.Tree.TreeColumn();
            this.colType = new Aga.Controls.Tree.TreeColumn();
            this.txtObjectName = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.txtObjectType = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.menContext.SuspendLayout();
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
            // menContext
            // 
            this.menContext.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bpaResultGoTo,
            this.bpaResultIgnore,
            this.bpaResultUnignore,
            this.bpaResultGoToSep,
            this.bpaResultScript,
            this.bpaResultFix,
            this.bpaResultSep2,
            this.bpaResultCopy});
            this.menContext.Name = "contextMenuStrip1";
            this.menContext.Size = new System.Drawing.Size(169, 120);
            this.menContext.Opening += new System.ComponentModel.CancelEventHandler(this.menContext_Opening);
            // 
            // bpaResultGoTo
            // 
            this.bpaResultGoTo.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.bpaResultGoTo.Image = global::TabularEditor.Resources.GoToDefinition_16x;
            this.bpaResultGoTo.Name = "bpaResultGoTo";
            this.bpaResultGoTo.Size = new System.Drawing.Size(168, 22);
            this.bpaResultGoTo.Text = "Go to object...";
            this.bpaResultGoTo.Click += new System.EventHandler(this.btnGoto_Click);
            // 
            // bpaResultIgnore
            // 
            this.bpaResultIgnore.Image = global::TabularEditor.Resources.CloakHide_16x;
            this.bpaResultIgnore.Name = "bpaResultIgnore";
            this.bpaResultIgnore.Size = new System.Drawing.Size(168, 22);
            this.bpaResultIgnore.Text = "Ignore rule";
            this.bpaResultIgnore.Click += new System.EventHandler(this.bpaResultIgnore_Click);
            // 
            // bpaResultUnignore
            // 
            this.bpaResultUnignore.Name = "bpaResultUnignore";
            this.bpaResultUnignore.Size = new System.Drawing.Size(168, 22);
            this.bpaResultUnignore.Text = "Unignore rule";
            this.bpaResultUnignore.Click += new System.EventHandler(this.bpaResultUnignore_Click);
            // 
            // bpaResultGoToSep
            // 
            this.bpaResultGoToSep.Name = "bpaResultGoToSep";
            this.bpaResultGoToSep.Size = new System.Drawing.Size(165, 6);
            // 
            // bpaResultScript
            // 
            this.bpaResultScript.Image = global::TabularEditor.Resources.Script_16x;
            this.bpaResultScript.Name = "bpaResultScript";
            this.bpaResultScript.Size = new System.Drawing.Size(168, 22);
            this.bpaResultScript.Text = "Generate fix script";
            this.bpaResultScript.Click += new System.EventHandler(this.btnScript_Click);
            // 
            // bpaResultFix
            // 
            this.bpaResultFix.Image = global::TabularEditor.Resources.Repair_16x;
            this.bpaResultFix.Name = "bpaResultFix";
            this.bpaResultFix.Size = new System.Drawing.Size(168, 22);
            this.bpaResultFix.Text = "Apply fix";
            this.bpaResultFix.Click += new System.EventHandler(this.btnFix_Click);
            // 
            // bpaResultSep2
            // 
            this.bpaResultSep2.Name = "bpaResultSep2";
            this.bpaResultSep2.Size = new System.Drawing.Size(165, 6);
            // 
            // bpaResultCopy
            // 
            this.bpaResultCopy.Name = "bpaResultCopy";
            this.bpaResultCopy.Size = new System.Drawing.Size(168, 22);
            this.bpaResultCopy.Text = "Copy to clipboard";
            this.bpaResultCopy.Click += new System.EventHandler(this.btnCopy_Click);
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
            this.btnManageRules,
            this.toolStripSeparator1,
            this.btnGoto,
            this.btnIgnore,
            this.toolStripSeparator2,
            this.btnScript,
            this.btnFix,
            this.btnRefresh,
            this.toolStripSeparator3,
            this.btnShowIgnored,
            this.toolStripSeparator4,
            this.btnCollapseAll,
            this.btnExpandAll});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(379, 25);
            this.toolStrip1.TabIndex = 9;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnManageRules
            // 
            this.btnManageRules.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnManageRules.Image = global::TabularEditor.Resources.Checklist_16x;
            this.btnManageRules.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnManageRules.Name = "btnManageRules";
            this.btnManageRules.Size = new System.Drawing.Size(23, 22);
            this.btnManageRules.Text = "Manage rules...";
            this.btnManageRules.Click += new System.EventHandler(this.btnManageRules_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnGoto
            // 
            this.btnGoto.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnGoto.Enabled = false;
            this.btnGoto.Image = global::TabularEditor.Resources.GoToDefinition_16x;
            this.btnGoto.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnGoto.Name = "btnGoto";
            this.btnGoto.Size = new System.Drawing.Size(23, 22);
            this.btnGoto.Text = "Go to object";
            this.btnGoto.Click += new System.EventHandler(this.btnGoto_Click);
            // 
            // btnIgnore
            // 
            this.btnIgnore.CheckOnClick = true;
            this.btnIgnore.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnIgnore.Enabled = false;
            this.btnIgnore.Image = global::TabularEditor.Resources.CloakHide_16x;
            this.btnIgnore.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnIgnore.Name = "btnIgnore";
            this.btnIgnore.Size = new System.Drawing.Size(23, 22);
            this.btnIgnore.Text = "Ignore";
            this.btnIgnore.Click += new System.EventHandler(this.btnIgnore_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnScript
            // 
            this.btnScript.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnScript.Enabled = false;
            this.btnScript.Image = global::TabularEditor.Resources.Script_16x;
            this.btnScript.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnScript.Name = "btnScript";
            this.btnScript.Size = new System.Drawing.Size(23, 22);
            this.btnScript.Text = "Generate fix script";
            this.btnScript.Click += new System.EventHandler(this.btnScript_Click);
            // 
            // btnFix
            // 
            this.btnFix.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFix.Enabled = false;
            this.btnFix.Image = global::TabularEditor.Resources.Repair_16x;
            this.btnFix.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFix.Name = "btnFix";
            this.btnFix.Size = new System.Drawing.Size(23, 22);
            this.btnFix.Text = "Apply fix";
            this.btnFix.Click += new System.EventHandler(this.btnFix_Click);
            // 
            // btnRefresh
            // 
            this.btnRefresh.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnRefresh.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRefresh.Image = global::TabularEditor.Resources.Refresh_16x;
            this.btnRefresh.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRefresh.Name = "btnRefresh";
            this.btnRefresh.Size = new System.Drawing.Size(23, 22);
            this.btnRefresh.Text = "Refresh";
            this.btnRefresh.Click += new System.EventHandler(this.btnRefresh_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // btnShowIgnored
            // 
            this.btnShowIgnored.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnShowIgnored.CheckOnClick = true;
            this.btnShowIgnored.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnShowIgnored.Image = ((System.Drawing.Image)(resources.GetObject("btnShowIgnored.Image")));
            this.btnShowIgnored.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnShowIgnored.Name = "btnShowIgnored";
            this.btnShowIgnored.Size = new System.Drawing.Size(84, 22);
            this.btnShowIgnored.Text = "Show ignored";
            this.btnShowIgnored.Click += new System.EventHandler(this.btnShowIgnored_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // btnCollapseAll
            // 
            this.btnCollapseAll.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnCollapseAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnCollapseAll.Image = global::TabularEditor.Resources.CollapseAll;
            this.btnCollapseAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnCollapseAll.Name = "btnCollapseAll";
            this.btnCollapseAll.Size = new System.Drawing.Size(23, 22);
            this.btnCollapseAll.Text = "Collapse All";
            this.btnCollapseAll.Click += new System.EventHandler(this.btnCollapseAll_Click);
            // 
            // btnExpandAll
            // 
            this.btnExpandAll.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnExpandAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnExpandAll.Image = global::TabularEditor.Resources.ExpandAll;
            this.btnExpandAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnExpandAll.Name = "btnExpandAll";
            this.btnExpandAll.Size = new System.Drawing.Size(23, 22);
            this.btnExpandAll.Text = "Expand All";
            this.btnExpandAll.Click += new System.EventHandler(this.btnExpandAll_Click);
            // 
            // tvResults
            // 
            this.tvResults.BackColor = System.Drawing.SystemColors.Window;
            this.tvResults.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.tvResults.Columns.Add(this.colObject);
            this.tvResults.Columns.Add(this.colType);
            this.tvResults.ContextMenuStrip = this.menContext;
            this.tvResults.DefaultToolTipProvider = null;
            this.tvResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tvResults.DragDropMarkColor = System.Drawing.Color.Black;
            this.tvResults.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.tvResults.FullRowSelect = true;
            this.tvResults.GridLineStyle = Aga.Controls.Tree.GridLineStyle.Horizontal;
            this.tvResults.HideSelection = true;
            this.tvResults.Indent = 0;
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
            this.txtObjectName.LeftMargin = 0;
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
            this.txtObjectType.DrawText += new System.EventHandler<Aga.Controls.Tree.NodeControls.DrawEventArgs>(this.txtObjectType_DrawText);
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
            this.MinimumSize = new System.Drawing.Size(290, 130);
            this.Name = "BPAForm";
            this.Text = "Best Practice Analyzer";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.BPAForm_FormClosing);
            this.menContext.ResumeLayout(false);
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
        private System.Windows.Forms.ContextMenuStrip menContext;
        private System.Windows.Forms.ToolStripMenuItem bpaResultGoTo;
        private System.Windows.Forms.ToolStripSeparator bpaResultGoToSep;
        private System.Windows.Forms.ToolStripMenuItem bpaResultIgnore;
        private System.Windows.Forms.ToolStripMenuItem bpaResultScript;
        private System.Windows.Forms.ToolStripMenuItem bpaResultFix;
        private System.Windows.Forms.ToolStripSeparator bpaResultSep2;
        private System.Windows.Forms.ToolStripMenuItem bpaResultCopy;
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
        private System.Windows.Forms.ToolStripButton btnShowIgnored;
        private System.Windows.Forms.ToolStripButton btnFix;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem bpaResultUnignore;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton btnCollapseAll;
        private System.Windows.Forms.ToolStripButton btnExpandAll;
    }
}