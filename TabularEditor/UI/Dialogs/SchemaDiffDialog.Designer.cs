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
            this.label1 = new System.Windows.Forms.Label();
            this.treeViewAdv1 = new Aga.Controls.Tree.TreeViewAdv();
            this.colTree = new Aga.Controls.Tree.TreeColumn();
            this.colCurrentDataType = new Aga.Controls.Tree.TreeColumn();
            this.colNewDataType = new Aga.Controls.Tree.TreeColumn();
            this.nodeCheckBox1 = new Aga.Controls.Tree.NodeControls.NodeCheckBox();
            this.nodeIcon1 = new Aga.Controls.Tree.NodeControls.NodeIcon();
            this.nodeTextBox1 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeTextBox2 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeTextBox3 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(353, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "Choose which schema changes you want to apply to your Tabular Model:";
            // 
            // treeViewAdv1
            // 
            this.treeViewAdv1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeViewAdv1.BackColor = System.Drawing.SystemColors.Window;
            this.treeViewAdv1.Columns.Add(this.colTree);
            this.treeViewAdv1.Columns.Add(this.colCurrentDataType);
            this.treeViewAdv1.Columns.Add(this.colNewDataType);
            this.treeViewAdv1.DefaultToolTipProvider = null;
            this.treeViewAdv1.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeViewAdv1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeViewAdv1.FullRowSelect = true;
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
            this.treeViewAdv1.SelectedNode = null;
            this.treeViewAdv1.Size = new System.Drawing.Size(726, 280);
            this.treeViewAdv1.TabIndex = 1;
            this.treeViewAdv1.Text = "treeViewAdv1";
            this.treeViewAdv1.UseColumns = true;
            // 
            // colTree
            // 
            this.colTree.Header = "";
            this.colTree.SortOrder = System.Windows.Forms.SortOrder.None;
            this.colTree.TooltipText = null;
            this.colTree.Width = 250;
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
            // nodeCheckBox1
            // 
            this.nodeCheckBox1.DataPropertyName = "ChangeInclude";
            this.nodeCheckBox1.LeftMargin = 0;
            this.nodeCheckBox1.ParentColumn = this.colTree;
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
            this.nodeTextBox1.DataPropertyName = "ChangeText";
            this.nodeTextBox1.LeftMargin = 3;
            this.nodeTextBox1.ParentColumn = this.colTree;
            // 
            // nodeTextBox2
            // 
            this.nodeTextBox2.DataPropertyName = "ModelDataType";
            this.nodeTextBox2.LeftMargin = 3;
            this.nodeTextBox2.ParentColumn = this.colCurrentDataType;
            // 
            // nodeTextBox3
            // 
            this.nodeTextBox3.DataPropertyName = "SourceDataType";
            this.nodeTextBox3.LeftMargin = 3;
            this.nodeTextBox3.ParentColumn = this.colNewDataType;
            // 
            // SchemaDiffDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 365);
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
    }
}