namespace TabularEditor.UI
{
    partial class SchemaDiffDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.treeViewAdv1 = new Aga.Controls.Tree.TreeViewAdv();
            this.colTree = new Aga.Controls.Tree.TreeColumn();
            this.colSourceColName = new Aga.Controls.Tree.TreeColumn();
            this.colCurrentDataType = new Aga.Controls.Tree.TreeColumn();
            this.colNewDataType = new Aga.Controls.Tree.TreeColumn();
            this.colDescription = new Aga.Controls.Tree.TreeColumn();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.nodeCheckBox1 = new Aga.Controls.Tree.NodeControls.NodeCheckBox();
            this.nodeIcon1 = new Aga.Controls.Tree.NodeControls.NodeIcon();
            this.nodeTextBox1 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeTextBox2 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeTextBox3 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeTextBox4 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeTextBox5 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.btnApply = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.chkSelectAll = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(372, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Choose which data source changes you want to apply to your Tabular Model:";
            // 
            // treeViewAdv1
            // 
            this.treeViewAdv1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewAdv1.BackColor = System.Drawing.SystemColors.Window;
            this.treeViewAdv1.Columns.Add(this.colTree);
            this.treeViewAdv1.Columns.Add(this.colSourceColName);
            this.treeViewAdv1.Columns.Add(this.colCurrentDataType);
            this.treeViewAdv1.Columns.Add(this.colNewDataType);
            this.treeViewAdv1.Columns.Add(this.colDescription);
            this.treeViewAdv1.ContextMenuStrip = this.contextMenuStrip1;
            this.treeViewAdv1.DefaultToolTipProvider = null;
            this.treeViewAdv1.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeViewAdv1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeViewAdv1.FullRowSelect = true;
            this.treeViewAdv1.GridLineStyle = ((Aga.Controls.Tree.GridLineStyle)((Aga.Controls.Tree.GridLineStyle.Horizontal | Aga.Controls.Tree.GridLineStyle.Vertical)));
            this.treeViewAdv1.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeViewAdv1.LoadOnDemand = true;
            this.treeViewAdv1.Location = new System.Drawing.Point(12, 36);
            this.treeViewAdv1.Model = null;
            this.treeViewAdv1.Name = "treeViewAdv1";
            this.treeViewAdv1.NodeControls.Add(this.nodeCheckBox1);
            this.treeViewAdv1.NodeControls.Add(this.nodeIcon1);
            this.treeViewAdv1.NodeControls.Add(this.nodeTextBox1);
            this.treeViewAdv1.NodeControls.Add(this.nodeTextBox2);
            this.treeViewAdv1.NodeControls.Add(this.nodeTextBox3);
            this.treeViewAdv1.NodeControls.Add(this.nodeTextBox4);
            this.treeViewAdv1.NodeControls.Add(this.nodeTextBox5);
            this.treeViewAdv1.SelectedNode = null;
            this.treeViewAdv1.Size = new System.Drawing.Size(775, 280);
            this.treeViewAdv1.TabIndex = 1;
            this.treeViewAdv1.Text = "treeViewAdv1";
            this.treeViewAdv1.UseColumns = true;
            // 
            // colTree
            // 
            this.colTree.Header = "Object";
            this.colTree.SortOrder = System.Windows.Forms.SortOrder.None;
            this.colTree.TooltipText = null;
            this.colTree.Width = 250;
            // 
            // colSourceColName
            // 
            this.colSourceColName.Header = "Source Column";
            this.colSourceColName.SortOrder = System.Windows.Forms.SortOrder.None;
            this.colSourceColName.TooltipText = null;
            this.colSourceColName.Width = 150;
            // 
            // colCurrentDataType
            // 
            this.colCurrentDataType.Header = "Model Data Type";
            this.colCurrentDataType.SortOrder = System.Windows.Forms.SortOrder.None;
            this.colCurrentDataType.TooltipText = null;
            this.colCurrentDataType.Width = 100;
            // 
            // colNewDataType
            // 
            this.colNewDataType.Header = "Source Data Type";
            this.colNewDataType.SortOrder = System.Windows.Forms.SortOrder.None;
            this.colNewDataType.TooltipText = null;
            this.colNewDataType.Width = 100;
            // 
            // colDescription
            // 
            this.colDescription.Header = "Change Description";
            this.colDescription.SortOrder = System.Windows.Forms.SortOrder.None;
            this.colDescription.TooltipText = null;
            this.colDescription.Width = 150;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // nodeCheckBox1
            // 
            this.nodeCheckBox1.DataPropertyName = "ChangeInclude";
            this.nodeCheckBox1.EditEnabled = true;
            this.nodeCheckBox1.LeftMargin = 0;
            this.nodeCheckBox1.ParentColumn = this.colTree;
            this.nodeCheckBox1.CheckStateChanged += new System.EventHandler<Aga.Controls.Tree.TreePathEventArgs>(this.nodeCheckBox1_CheckStateChanged);
            // 
            // nodeIcon1
            // 
            this.nodeIcon1.DataPropertyName = "ChangeIcon";
            this.nodeIcon1.LeftMargin = 1;
            this.nodeIcon1.ParentColumn = this.colTree;
            this.nodeIcon1.ScaleMode = Aga.Controls.Tree.ImageScaleMode.Clip;
            this.nodeIcon1.VirtualMode = true;
            this.nodeIcon1.ValueNeeded += new System.EventHandler<Aga.Controls.Tree.NodeControls.NodeControlValueEventArgs>(this.nodeIcon1_ValueNeeded);
            // 
            // nodeTextBox1
            // 
            this.nodeTextBox1.DataPropertyName = "ObjectName";
            this.nodeTextBox1.LeftMargin = 3;
            this.nodeTextBox1.ParentColumn = this.colTree;
            this.nodeTextBox1.UseCompatibleTextRendering = true;
            this.nodeTextBox1.DrawText += new System.EventHandler<Aga.Controls.Tree.NodeControls.DrawEventArgs>(this.nodeTextBox1_DrawText);
            // 
            // nodeTextBox2
            // 
            this.nodeTextBox2.DataPropertyName = "ModelDataType";
            this.nodeTextBox2.LeftMargin = 3;
            this.nodeTextBox2.ParentColumn = this.colCurrentDataType;
            this.nodeTextBox2.UseCompatibleTextRendering = true;
            // 
            // nodeTextBox3
            // 
            this.nodeTextBox3.DataPropertyName = "SourceDataType";
            this.nodeTextBox3.LeftMargin = 3;
            this.nodeTextBox3.ParentColumn = this.colNewDataType;
            this.nodeTextBox3.UseCompatibleTextRendering = true;
            // 
            // nodeTextBox4
            // 
            this.nodeTextBox4.DataPropertyName = "Description";
            this.nodeTextBox4.LeftMargin = 3;
            this.nodeTextBox4.ParentColumn = this.colDescription;
            this.nodeTextBox4.UseCompatibleTextRendering = true;
            // 
            // nodeTextBox5
            // 
            this.nodeTextBox5.DataPropertyName = "SourceColumn";
            this.nodeTextBox5.LeftMargin = 3;
            this.nodeTextBox5.ParentColumn = this.colSourceColName;
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApply.Location = new System.Drawing.Point(712, 322);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 3;
            this.btnApply.Text = "Apply";
            this.btnApply.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(631, 322);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // chkSelectAll
            // 
            this.chkSelectAll.AutoSize = true;
            this.chkSelectAll.Checked = true;
            this.chkSelectAll.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkSelectAll.Location = new System.Drawing.Point(16, 326);
            this.chkSelectAll.Name = "chkSelectAll";
            this.chkSelectAll.Size = new System.Drawing.Size(70, 17);
            this.chkSelectAll.TabIndex = 5;
            this.chkSelectAll.Text = "Select All";
            this.chkSelectAll.UseVisualStyleBackColor = true;
            this.chkSelectAll.CheckedChanged += new System.EventHandler(this.chkSelectAll_CheckedChanged);
            // 
            // SchemaDiffDialog
            // 
            this.AcceptButton = this.btnApply;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(799, 357);
            this.Controls.Add(this.chkSelectAll);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.treeViewAdv1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "SchemaDiffDialog";
            this.ShowIcon = false;
            this.Text = "Data Source Schema changes detected";
            this.Load += new System.EventHandler(this.SchemaDiffDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private Aga.Controls.Tree.TreeViewAdv treeViewAdv1;
        private System.Windows.Forms.Label label1;
        private Aga.Controls.Tree.TreeColumn colTree;
        private Aga.Controls.Tree.NodeControls.NodeIcon nodeIcon1;
        private Aga.Controls.Tree.NodeControls.NodeCheckBox nodeCheckBox1;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox1;
        private Aga.Controls.Tree.TreeColumn colCurrentDataType;
        private Aga.Controls.Tree.TreeColumn colNewDataType;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox2;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox3;
        private Aga.Controls.Tree.TreeColumn colDescription;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox4;
        private Aga.Controls.Tree.TreeColumn colSourceColName;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox5;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.CheckBox chkSelectAll;
    }
}