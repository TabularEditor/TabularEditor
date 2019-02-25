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
            this.listView2 = new System.Windows.Forms.ListView();
            this.columnHeader6 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader8 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.columnHeader7 = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
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
            this.listView1.Size = new System.Drawing.Size(753, 166);
            this.listView1.TabIndex = 0;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.Details;
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
            this.splitter1.Size = new System.Drawing.Size(753, 3);
            this.splitter1.TabIndex = 4;
            this.splitter1.TabStop = false;
            // 
            // listView2
            // 
            this.listView2.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader6,
            this.columnHeader8,
            this.columnHeader7});
            this.listView2.ContextMenuStrip = this.contextMenuStrip1;
            this.listView2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView2.FullRowSelect = true;
            this.listView2.GridLines = true;
            this.listView2.Location = new System.Drawing.Point(0, 194);
            this.listView2.Name = "listView2";
            this.listView2.Size = new System.Drawing.Size(753, 204);
            this.listView2.TabIndex = 5;
            this.listView2.UseCompatibleStateImageBehavior = false;
            this.listView2.View = System.Windows.Forms.View.Details;
            this.listView2.ColumnClick += new System.Windows.Forms.ColumnClickEventHandler(this.listView1_ColumnClick);
            this.listView2.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.listView2_MouseDoubleClick);
            // 
            // columnHeader6
            // 
            this.columnHeader6.Text = "Object";
            this.columnHeader6.Width = 200;
            // 
            // columnHeader8
            // 
            this.columnHeader8.Text = "Object Type";
            this.columnHeader8.Width = 100;
            // 
            // columnHeader7
            // 
            this.columnHeader7.Text = "Rule Name";
            this.columnHeader7.Width = 200;
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
            this.statusStrip1.Location = new System.Drawing.Point(0, 398);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(753, 22);
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
            this.btnAnalyzeAll});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(753, 25);
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
            this.btnAnalyzeAll.Size = new System.Drawing.Size(67, 22);
            this.btnAnalyzeAll.Text = "Analyze all";
            this.btnAnalyzeAll.Click += new System.EventHandler(this.btnAnalyzeAll_Click);
            // 
            // BPAForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(753, 420);
            this.Controls.Add(this.listView2);
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
        private System.Windows.Forms.ListView listView2;
        private System.Windows.Forms.ColumnHeader columnHeader6;
        private System.Windows.Forms.ColumnHeader columnHeader7;
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
        private System.Windows.Forms.ColumnHeader columnHeader8;
        private System.Windows.Forms.ColumnHeader columnHeader9;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnAdd;
        private System.Windows.Forms.ToolStripButton btnEdit;
        private System.Windows.Forms.ToolStripButton btnDelete;
        private System.Windows.Forms.ToolStripButton btnMakeLocal;
        private System.Windows.Forms.ToolStripButton btnAnalyzeAll;
    }
}