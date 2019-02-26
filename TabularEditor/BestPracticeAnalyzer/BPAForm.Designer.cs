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
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeader1 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader2 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader3 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader4 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader9 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader5 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
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
            this.btnAdd = new System.Windows.Forms.ToolStripButton();
            this.btnEdit = new System.Windows.Forms.ToolStripButton();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.btnMakeLocal = new System.Windows.Forms.ToolStripButton();
            this.btnAnalyzeAll = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
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
            // listView1
            // 
            this.listView1.CheckBoxes = true;
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2,
            this.columnHeader3,
            this.columnHeader4,
            this.columnHeader9,
            this.columnHeader5});
            this.listView1.Dock = System.Windows.Forms.DockStyle.Top;
            this.listView1.FullRowSelect = true;
            this.listView1.GridLines = true;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 25);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(379, 166);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
            this.listView1.Visible = false;
            this.listView1.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            this.listView1.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.listView1_ItemChecked);
            this.listView1.ItemSelectionChanged += new System.Windows.Forms.ListViewItemSelectionChangedEventHandler(this.listView1_ItemSelectionChanged);
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Enabled";
            this.columnHeader1.Width = 55;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Rule Name";
            this.columnHeader2.Width = 180;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "Scope";
            // 
            // columnHeader4
            // 
            this.columnHeader4.Text = "Severity";
            // 
            // columnHeader9
            // 
            this.columnHeader9.Text = "Category";
            this.columnHeader9.Width = 90;
            // 
            // columnHeader5
            // 
            this.columnHeader5.Text = "Description";
            this.columnHeader5.Width = 300;
            // 
            // splitter1
            // 
            this.splitter1.Cursor = System.Windows.Forms.Cursors.HSplit;
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Top;
            this.splitter1.Location = new System.Drawing.Point(0, 191);
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
            this.btnAdd,
            this.btnEdit,
            this.btnDelete,
            this.btnMakeLocal,
            this.btnAnalyzeAll,
            this.toolStripSeparator1,
            this.toolStripButton1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(379, 25);
            this.toolStrip1.TabIndex = 9;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnAdd
            // 
            this.btnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
            this.btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(67, 22);
            this.btnAdd.Text = "New rule...";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnEdit.Image = ((System.Drawing.Image)(resources.GetObject("btnEdit.Image")));
            this.btnEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(63, 22);
            this.btnEdit.Text = "Edit rule...";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnDelete
            // 
            this.btnDelete.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(67, 22);
            this.btnDelete.Text = "Delete rule";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnMakeLocal
            // 
            this.btnMakeLocal.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnMakeLocal.Image = ((System.Drawing.Image)(resources.GetObject("btnMakeLocal.Image")));
            this.btnMakeLocal.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnMakeLocal.Name = "btnMakeLocal";
            this.btnMakeLocal.Size = new System.Drawing.Size(68, 22);
            this.btnMakeLocal.Text = "Make local";
            this.btnMakeLocal.Click += new System.EventHandler(this.btnMakeLocal_Click);
            // 
            // btnAnalyzeAll
            // 
            this.btnAnalyzeAll.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnAnalyzeAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAnalyzeAll.Image = ((System.Drawing.Image)(resources.GetObject("btnAnalyzeAll.Image")));
            this.btnAnalyzeAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAnalyzeAll.Name = "btnAnalyzeAll";
            this.btnAnalyzeAll.Size = new System.Drawing.Size(50, 22);
            this.btnAnalyzeAll.Text = "Refresh";
            this.btnAnalyzeAll.Click += new System.EventHandler(this.btnAnalyzeAll_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.toolStripButton1.Image = ((System.Drawing.Image)(resources.GetObject("toolStripButton1.Image")));
            this.toolStripButton1.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Size = new System.Drawing.Size(91, 19);
            this.toolStripButton1.Text = "Manage rules...";
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
            this.tvResults.LineColor = System.Drawing.SystemColors.ControlDark;
            this.tvResults.Location = new System.Drawing.Point(0, 194);
            this.tvResults.Model = null;
            this.tvResults.Name = "tvResults";
            this.tvResults.NodeControls.Add(this.txtObjectName);
            this.tvResults.NodeControls.Add(this.txtObjectType);
            this.tvResults.SelectedNode = null;
            this.tvResults.ShowLines = false;
            this.tvResults.ShowNodeToolTips = true;
            this.tvResults.ShowPlusMinus = false;
            this.tvResults.Size = new System.Drawing.Size(379, 391);
            this.tvResults.TabIndex = 10;
            this.tvResults.UseColumns = true;
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
            this.Controls.Add(this.listView1);
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

        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
        private System.Windows.Forms.ColumnHeader columnHeader4;
        private System.Windows.Forms.ColumnHeader columnHeader5;
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
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnAdd;
        private System.Windows.Forms.ToolStripButton btnEdit;
        private System.Windows.Forms.ToolStripButton btnDelete;
        private System.Windows.Forms.ToolStripButton btnMakeLocal;
        private System.Windows.Forms.ToolStripButton btnAnalyzeAll;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton toolStripButton1;
        private Aga.Controls.Tree.TreeViewAdv tvResults;
        private Aga.Controls.Tree.TreeColumn colObject;
        private Aga.Controls.Tree.TreeColumn colType;
        private Aga.Controls.Tree.NodeControls.NodeTextBox txtObjectName;
        private Aga.Controls.Tree.NodeControls.NodeTextBox txtObjectType;
    }
}