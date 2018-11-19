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
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.panel2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "database");
            this.imageList1.Images.SetKeyName(1, "table");
            this.imageList1.Images.SetKeyName(2, "view");
            this.imageList1.Images.SetKeyName(3, "spinner");
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
            this.treeViewAdv1.Size = new System.Drawing.Size(326, 294);
            this.treeViewAdv1.TabIndex = 0;
            this.treeViewAdv1.Text = "treeViewAdv1";
            this.treeViewAdv1.SelectionChanged += new System.EventHandler(this.treeViewAdv1_SelectionChanged);
            this.treeViewAdv1.Expanding += new System.EventHandler<Aga.Controls.Tree.TreeViewAdvEventArgs>(this.treeViewAdv1_Expanding);
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
            this.splitContainer1.Location = new System.Drawing.Point(0, 22);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.treeViewAdv1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.dataGridView1);
            this.splitContainer1.Panel2.Controls.Add(this.panel2);
            this.splitContainer1.Size = new System.Drawing.Size(576, 294);
            this.splitContainer1.SplitterDistance = 326;
            this.splitContainer1.TabIndex = 1;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.ButtonFace;
            this.dataGridView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.Location = new System.Drawing.Point(0, 22);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.Single;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.Size = new System.Drawing.Size(246, 272);
            this.dataGridView1.TabIndex = 0;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label2);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(246, 22);
            this.panel2.TabIndex = 1;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(4, 4);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(119, 13);
            this.label2.TabIndex = 0;
            this.label2.Text = "Preview (first 200 rows):";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(576, 22);
            this.panel1.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(4, 4);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(285, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Choose the tables you want to import from this data source:";
            // 
            // ImportTablesPage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.panel1);
            this.Name = "ImportTablesPage";
            this.Size = new System.Drawing.Size(576, 316);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
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
        private System.Windows.Forms.ImageList imageList1;
        private Aga.Controls.Tree.TreeColumn colSpinner;
        private Aga.Controls.Tree.NodeControls.ExpandingIcon expandingIcon1;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Label label2;
    }
}
