namespace TabularEditor.UI.Dialogs.Pages
{
    partial class ImportTablesPage
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportTablesPage));
            this.treeViewAdv1 = new Aga.Controls.Tree.TreeViewAdv();
            this.colCheckBox = new Aga.Controls.Tree.TreeColumn();
            this.colIcon = new Aga.Controls.Tree.TreeColumn();
            this.colName = new Aga.Controls.Tree.TreeColumn();
            this.colSpinner = new Aga.Controls.Tree.TreeColumn();
            this.nodeCheckBox1 = new Aga.Controls.Tree.NodeControls.NodeCheckBox();
            this.nodeIcon1 = new Aga.Controls.Tree.NodeControls.NodeIcon();
            this.nodeTextBox1 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.expandingIcon1 = new Aga.Controls.Tree.NodeControls.ExpandingIcon();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.previewPane = new System.Windows.Forms.SplitContainer();
            this.panel4 = new System.Windows.Forms.Panel();
            this.lblError = new System.Windows.Forms.Label();
            this.loadingPreviewSpinner = new System.Windows.Forms.PictureBox();
            this.dataGridView1 = new TabularEditor.UI.Extensions.DoubleBufferedGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.label2 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.txtSql = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.chkEnablePreview = new System.Windows.Forms.CheckBox();
            this.lblHeader = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cmbRowReduction = new System.Windows.Forms.ComboBox();
            this.lblRowReduction = new System.Windows.Forms.Label();
            this.pnlODBC = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbQuotes = new System.Windows.Forms.ComboBox();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.previewPane)).BeginInit();
            this.previewPane.Panel1.SuspendLayout();
            this.previewPane.Panel2.SuspendLayout();
            this.previewPane.SuspendLayout();
            this.panel4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.loadingPreviewSpinner)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel1.SuspendLayout();
            this.pnlODBC.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeViewAdv1
            // 
            this.treeViewAdv1.AsyncExpanding = true;
            this.treeViewAdv1.BackColor = System.Drawing.SystemColors.Window;
            this.treeViewAdv1.Columns.Add(this.colCheckBox);
            this.treeViewAdv1.Columns.Add(this.colIcon);
            this.treeViewAdv1.Columns.Add(this.colName);
            this.treeViewAdv1.Columns.Add(this.colSpinner);
            this.treeViewAdv1.DefaultToolTipProvider = null;
            this.treeViewAdv1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeViewAdv1.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeViewAdv1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeViewAdv1.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeViewAdv1.LoadOnDemand = true;
            this.treeViewAdv1.Location = new System.Drawing.Point(0, 0);
            this.treeViewAdv1.Model = null;
            this.treeViewAdv1.Name = "treeViewAdv1";
            this.treeViewAdv1.NodeControls.Add(this.nodeCheckBox1);
            this.treeViewAdv1.NodeControls.Add(this.nodeIcon1);
            this.treeViewAdv1.NodeControls.Add(this.nodeTextBox1);
            this.treeViewAdv1.NodeControls.Add(this.expandingIcon1);
            this.treeViewAdv1.SelectedNode = null;
            this.treeViewAdv1.Size = new System.Drawing.Size(336, 460);
            this.treeViewAdv1.TabIndex = 0;
            this.treeViewAdv1.Text = "treeViewAdv1";
            this.treeViewAdv1.SelectionChanged += new System.EventHandler(this.treeViewAdv1_SelectionChanged);
            this.treeViewAdv1.Expanded += new System.EventHandler<Aga.Controls.Tree.TreeViewAdvEventArgs>(this.treeViewAdv1_Expanded);
            // 
            // colCheckBox
            // 
            this.colCheckBox.Header = "";
            this.colCheckBox.SortOrder = System.Windows.Forms.SortOrder.None;
            this.colCheckBox.TooltipText = null;
            // 
            // colIcon
            // 
            this.colIcon.Header = "";
            this.colIcon.SortOrder = System.Windows.Forms.SortOrder.None;
            this.colIcon.TooltipText = null;
            // 
            // colName
            // 
            this.colName.Header = "";
            this.colName.SortOrder = System.Windows.Forms.SortOrder.None;
            this.colName.TooltipText = null;
            // 
            // colSpinner
            // 
            this.colSpinner.Header = "";
            this.colSpinner.SortOrder = System.Windows.Forms.SortOrder.None;
            this.colSpinner.TooltipText = null;
            // 
            // nodeCheckBox1
            // 
            this.nodeCheckBox1.DataPropertyName = "Selected";
            this.nodeCheckBox1.EditEnabled = true;
            this.nodeCheckBox1.LeftMargin = 0;
            this.nodeCheckBox1.ParentColumn = this.colCheckBox;
            this.nodeCheckBox1.CheckStateChanged += new System.EventHandler<Aga.Controls.Tree.TreePathEventArgs>(this.nodeCheckBox1_CheckStateChanged);
            this.nodeCheckBox1.IsVisibleValueNeeded += new System.EventHandler<Aga.Controls.Tree.NodeControls.NodeControlValueEventArgs>(this.nodeCheckBox1_IsVisibleValueNeeded);
            // 
            // nodeIcon1
            // 
            this.nodeIcon1.LeftMargin = 1;
            this.nodeIcon1.ParentColumn = this.colIcon;
            this.nodeIcon1.ScaleMode = Aga.Controls.Tree.ImageScaleMode.Clip;
            this.nodeIcon1.VirtualMode = true;
            this.nodeIcon1.ValueNeeded += new System.EventHandler<Aga.Controls.Tree.NodeControls.NodeControlValueEventArgs>(this.nodeIcon1_ValueNeeded);
            this.nodeIcon1.IsVisibleValueNeeded += new System.EventHandler<Aga.Controls.Tree.NodeControls.NodeControlValueEventArgs>(this.nodeIcon1_IsVisibleValueNeeded);
            // 
            // nodeTextBox1
            // 
            this.nodeTextBox1.DataPropertyName = "DisplayName";
            this.nodeTextBox1.IncrementalSearchEnabled = true;
            this.nodeTextBox1.LeftMargin = 3;
            this.nodeTextBox1.ParentColumn = this.colName;
            // 
            // expandingIcon1
            // 
            this.expandingIcon1.LeftMargin = 0;
            this.expandingIcon1.ParentColumn = null;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.splitContainer1.Location = new System.Drawing.Point(0, 31);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeViewAdv1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.previewPane);
            this.splitContainer1.Size = new System.Drawing.Size(818, 460);
            this.splitContainer1.SplitterDistance = 336;
            this.splitContainer1.TabIndex = 1;
            // 
            // previewPane
            // 
            this.previewPane.Dock = System.Windows.Forms.DockStyle.Fill;
            this.previewPane.Location = new System.Drawing.Point(0, 0);
            this.previewPane.Name = "previewPane";
            this.previewPane.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // previewPane.Panel1
            // 
            this.previewPane.Panel1.Controls.Add(this.panel4);
            // 
            // previewPane.Panel2
            // 
            this.previewPane.Panel2.Controls.Add(this.panel3);
            this.previewPane.Size = new System.Drawing.Size(478, 460);
            this.previewPane.SplitterDistance = 319;
            this.previewPane.TabIndex = 4;
            // 
            // panel4
            // 
            this.panel4.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel4.Controls.Add(this.lblError);
            this.panel4.Controls.Add(this.loadingPreviewSpinner);
            this.panel4.Controls.Add(this.dataGridView1);
            this.panel4.Controls.Add(this.panel2);
            this.panel4.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel4.Location = new System.Drawing.Point(0, 0);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(478, 319);
            this.panel4.TabIndex = 3;
            // 
            // lblError
            // 
            this.lblError.AutoSize = true;
            this.lblError.BackColor = System.Drawing.SystemColors.Window;
            this.lblError.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblError.Location = new System.Drawing.Point(4, 25);
            this.lblError.Name = "lblError";
            this.lblError.Size = new System.Drawing.Size(35, 13);
            this.lblError.TabIndex = 2;
            this.lblError.Text = "label3";
            this.lblError.Visible = false;
            // 
            // loadingPreviewSpinner
            // 
            this.loadingPreviewSpinner.Image = ((System.Drawing.Image)(resources.GetObject("loadingPreviewSpinner.Image")));
            this.loadingPreviewSpinner.Location = new System.Drawing.Point(3, 25);
            this.loadingPreviewSpinner.Name = "loadingPreviewSpinner";
            this.loadingPreviewSpinner.Size = new System.Drawing.Size(16, 16);
            this.loadingPreviewSpinner.TabIndex = 1;
            this.loadingPreviewSpinner.TabStop = false;
            this.loadingPreviewSpinner.Visible = false;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableAlwaysIncludeHeaderText;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 22);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.dataGridView1.Size = new System.Drawing.Size(474, 293);
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.dataGridView1_DataError);
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.SystemColors.Window;
            this.panel2.Controls.Add(this.chkSelectAll);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(474, 22);
            this.panel2.TabIndex = 1;
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkSelectAll.Checked = true;
            this.chkSelectAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSelectAll.Location = new System.Drawing.Point(360, 4);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(111, 17);
            this.chkSelectAll.TabIndex = 2;
            this.chkSelectAll.Text = "Select all columns";
            this.toolTip1.SetToolTip(this.chkSelectAll, "Check this to generate a \"SELECT * FROM\" query. To generate a query with individu" +
        "al column names, toggle a column in the preview.");
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.Visible = false;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Preview (first 200 rows):";
            // 
            // panel3
            // 
            this.panel3.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.panel3.Controls.Add(this.txtSql);
            this.panel3.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel3.Location = new System.Drawing.Point(0, 0);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(478, 137);
            this.panel3.TabIndex = 4;
            // 
            // txtSql
            // 
            this.txtSql.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtSql.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtSql.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtSql.Location = new System.Drawing.Point(0, 0);
            this.txtSql.Multiline = true;
            this.txtSql.Name = "txtSql";
            this.txtSql.ReadOnly = true;
            this.txtSql.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.txtSql.Size = new System.Drawing.Size(474, 133);
            this.txtSql.TabIndex = 3;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chkEnablePreview);
            this.panel1.Controls.Add(this.lblHeader);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(818, 31);
            this.panel1.TabIndex = 1;
            // 
            // chkEnablePreview
            // 
            this.chkEnablePreview.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.chkEnablePreview.AutoSize = true;
            this.chkEnablePreview.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.chkEnablePreview.Checked = true;
            this.chkEnablePreview.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkEnablePreview.Location = new System.Drawing.Point(714, 8);
            this.chkEnablePreview.Name = "chkEnablePreview";
            this.chkEnablePreview.Size = new System.Drawing.Size(99, 17);
            this.chkEnablePreview.TabIndex = 4;
            this.chkEnablePreview.Text = "Enable preview";
            this.toolTip1.SetToolTip(this.chkEnablePreview, "Disabling preview will cause all queries to be generated as \"SELECT * FROM ...\"");
            this.chkEnablePreview.UseVisualStyleBackColor = true;
            this.chkEnablePreview.CheckedChanged += new System.EventHandler(this.chkEnablePreview_CheckedChanged);
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Location = new System.Drawing.Point(3, 9);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(285, 13);
            this.lblHeader.TabIndex = 0;
            this.lblHeader.Text = "Choose the tables you want to import from this data source:";
            // 
            // cmbRowReduction
            // 
            this.cmbRowReduction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbRowReduction.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRowReduction.FormattingEnabled = true;
            this.cmbRowReduction.Items.AddRange(new object[] {
            "(None)",
            "TOP with NOLOCK",
            "FIRST",
            "LIMIT and OFFSET",
            "LIMIT",
            "SAMPLE",
            "ANSI SQL-compatible",
            "TOP (without NOLOCK)" });
            this.cmbRowReduction.Location = new System.Drawing.Point(415, 8);
            this.cmbRowReduction.Name = "cmbRowReduction";
            this.cmbRowReduction.Size = new System.Drawing.Size(142, 21);
            this.cmbRowReduction.TabIndex = 2;
            this.cmbRowReduction.SelectedIndexChanged += new System.EventHandler(this.cmbRowReduction_SelectedIndexChanged);
            // 
            // lblRowReduction
            // 
            this.lblRowReduction.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblRowReduction.AutoSize = true;
            this.lblRowReduction.Location = new System.Drawing.Point(308, 11);
            this.lblRowReduction.Name = "lblRowReduction";
            this.lblRowReduction.Size = new System.Drawing.Size(101, 13);
            this.lblRowReduction.TabIndex = 3;
            this.lblRowReduction.Text = "Reduce rows using:";
            this.lblRowReduction.Click += new System.EventHandler(this.label1_Click);
            // 
            // pnlODBC
            // 
            this.pnlODBC.Controls.Add(this.label1);
            this.pnlODBC.Controls.Add(this.cmbQuotes);
            this.pnlODBC.Controls.Add(this.lblRowReduction);
            this.pnlODBC.Controls.Add(this.cmbRowReduction);
            this.pnlODBC.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlODBC.Location = new System.Drawing.Point(0, 491);
            this.pnlODBC.Name = "pnlODBC";
            this.pnlODBC.Size = new System.Drawing.Size(818, 29);
            this.pnlODBC.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(585, 11);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(85, 13);
            this.label1.TabIndex = 5;
            this.label1.Text = "Identifier quotes:";
            // 
            // cmbQuotes
            // 
            this.cmbQuotes.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbQuotes.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbQuotes.FormattingEnabled = true;
            this.cmbQuotes.Items.AddRange(new object[] {
            "(None)",
            "[Square brackets]",
            "\"Double quotes\"",
            "\'Single quotes\'",
            "`Backticks`"});
            this.cmbQuotes.Location = new System.Drawing.Point(676, 8);
            this.cmbQuotes.Name = "cmbQuotes";
            this.cmbQuotes.Size = new System.Drawing.Size(142, 21);
            this.cmbQuotes.TabIndex = 4;
            this.cmbQuotes.SelectedIndexChanged += new System.EventHandler(this.cmbQuotes_SelectedIndexChanged);
            // 
            // ImportTablesPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.pnlODBC);
            this.Controls.Add(this.panel1);
            this.Name = "ImportTablesPage";
            this.Size = new System.Drawing.Size(818, 520);
            this.Load += new System.EventHandler(this.ImportTablesPage_Load);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.previewPane.Panel1.ResumeLayout(false);
            this.previewPane.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.previewPane)).EndInit();
            this.previewPane.ResumeLayout(false);
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.loadingPreviewSpinner)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.pnlODBC.ResumeLayout(false);
            this.pnlODBC.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private Aga.Controls.Tree.TreeViewAdv treeViewAdv1;
        private Aga.Controls.Tree.TreeColumn colCheckBox;
        private Aga.Controls.Tree.TreeColumn colIcon;
        private Aga.Controls.Tree.TreeColumn colName;
        private Aga.Controls.Tree.NodeControls.NodeCheckBox nodeCheckBox1;
        private Aga.Controls.Tree.NodeControls.NodeIcon nodeIcon1;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox1;
        private Aga.Controls.Tree.TreeColumn colSpinner;
        private Aga.Controls.Tree.NodeControls.ExpandingIcon expandingIcon1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private TabularEditor.UI.Extensions.DoubleBufferedGridView dataGridView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.PictureBox loadingPreviewSpinner;
        private System.Windows.Forms.Label lblError;
        private System.Windows.Forms.SplitContainer previewPane;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.TextBox txtSql;
        private System.Windows.Forms.CheckBox chkSelectAll;
        private System.Windows.Forms.ToolTip toolTip1;
        public System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.CheckBox chkEnablePreview;
        private System.Windows.Forms.Label lblRowReduction;
        private System.Windows.Forms.ComboBox cmbRowReduction;
        private System.Windows.Forms.Panel pnlODBC;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbQuotes;
    }
}
