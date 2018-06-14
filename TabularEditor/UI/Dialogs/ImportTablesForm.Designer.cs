namespace TabularEditor.UI.Dialogs
{
    partial class ImportTablesForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ImportTablesForm));
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabSingle = new System.Windows.Forms.TabPage();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnImportFromQuery = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.tabPQ = new System.Windows.Forms.TabPage();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnImportFromClipboard = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.treeViewAdv1 = new Aga.Controls.Tree.TreeViewAdv();
            this.colImport = new Aga.Controls.Tree.TreeColumn();
            this.colSourceName = new Aga.Controls.Tree.TreeColumn();
            this.colDataType = new Aga.Controls.Tree.TreeColumn();
            this.colImportedName = new Aga.Controls.Tree.TreeColumn();
            this.nodeCheckBox1 = new Aga.Controls.Tree.NodeControls.NodeCheckBox();
            this.nodeTextBox1 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.nodeComboBox1 = new Aga.Controls.Tree.NodeControls.NodeComboBox();
            this.nodeTextBox2 = new Aga.Controls.Tree.NodeControls.NodeTextBox();
            this.btnImport = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.tabControl1.SuspendLayout();
            this.tabSingle.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.tabPQ.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.tabControl1.Controls.Add(this.tabSingle);
            this.tabControl1.Controls.Add(this.tabPQ);
            this.tabControl1.Location = new System.Drawing.Point(13, 13);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(341, 356);
            this.tabControl1.TabIndex = 0;
            // 
            // tabSingle
            // 
            this.tabSingle.Controls.Add(this.groupBox5);
            this.tabSingle.Controls.Add(this.groupBox4);
            this.tabSingle.Location = new System.Drawing.Point(4, 22);
            this.tabSingle.Name = "tabSingle";
            this.tabSingle.Padding = new System.Windows.Forms.Padding(3);
            this.tabSingle.Size = new System.Drawing.Size(333, 330);
            this.tabSingle.TabIndex = 1;
            this.tabSingle.Text = "SQL Schema";
            this.tabSingle.UseVisualStyleBackColor = true;
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.btnImportFromQuery);
            this.groupBox5.Controls.Add(this.textBox1);
            this.groupBox5.Controls.Add(this.label4);
            this.groupBox5.Controls.Add(this.comboBox1);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Location = new System.Drawing.Point(7, 110);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(316, 214);
            this.groupBox5.TabIndex = 2;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "OLE DB";
            // 
            // btnImportFromQuery
            // 
            this.btnImportFromQuery.Enabled = false;
            this.btnImportFromQuery.Location = new System.Drawing.Point(124, 185);
            this.btnImportFromQuery.Name = "btnImportFromQuery";
            this.btnImportFromQuery.Size = new System.Drawing.Size(186, 23);
            this.btnImportFromQuery.TabIndex = 4;
            this.btnImportFromQuery.Text = "Import table metadata from query";
            this.btnImportFromQuery.UseVisualStyleBackColor = true;
            this.btnImportFromQuery.Click += new System.EventHandler(this.btnImportFromQuery_Click);
            // 
            // textBox1
            // 
            this.textBox1.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.textBox1.Location = new System.Drawing.Point(9, 74);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(301, 104);
            this.textBox1.TabIndex = 3;
            this.textBox1.Text = "SELECT * FROM ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(38, 13);
            this.label4.TabIndex = 2;
            this.label4.Text = "Query:";
            // 
            // comboBox1
            // 
            this.comboBox1.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Location = new System.Drawing.Point(9, 33);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(301, 21);
            this.comboBox1.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 16);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 13);
            this.label3.TabIndex = 0;
            this.label3.Text = "Data source:";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.label2);
            this.groupBox4.Location = new System.Drawing.Point(6, 6);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(317, 97);
            this.groupBox4.TabIndex = 1;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Information";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.Location = new System.Drawing.Point(7, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(304, 75);
            this.label2.TabIndex = 0;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // tabPQ
            // 
            this.tabPQ.Controls.Add(this.groupBox2);
            this.tabPQ.Controls.Add(this.groupBox1);
            this.tabPQ.Location = new System.Drawing.Point(4, 22);
            this.tabPQ.Name = "tabPQ";
            this.tabPQ.Size = new System.Drawing.Size(333, 330);
            this.tabPQ.TabIndex = 2;
            this.tabPQ.Text = "Power Query Schema";
            this.tabPQ.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnImportFromClipboard);
            this.groupBox2.Location = new System.Drawing.Point(7, 107);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(317, 54);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Import Table Metadata";
            // 
            // btnImportFromClipboard
            // 
            this.btnImportFromClipboard.Location = new System.Drawing.Point(10, 20);
            this.btnImportFromClipboard.Name = "btnImportFromClipboard";
            this.btnImportFromClipboard.Size = new System.Drawing.Size(214, 23);
            this.btnImportFromClipboard.TabIndex = 0;
            this.btnImportFromClipboard.Text = "Import table metadata from clipboard";
            this.btnImportFromClipboard.UseVisualStyleBackColor = true;
            this.btnImportFromClipboard.Click += new System.EventHandler(this.button1_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(7, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(317, 97);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Information";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Location = new System.Drawing.Point(7, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(304, 75);
            this.label1.TabIndex = 0;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.treeViewAdv1);
            this.groupBox3.Location = new System.Drawing.Point(360, 12);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(436, 358);
            this.groupBox3.TabIndex = 3;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Imported columns";
            // 
            // treeViewAdv1
            // 
            this.treeViewAdv1.BackColor = System.Drawing.SystemColors.Window;
            this.treeViewAdv1.Columns.Add(this.colImport);
            this.treeViewAdv1.Columns.Add(this.colSourceName);
            this.treeViewAdv1.Columns.Add(this.colDataType);
            this.treeViewAdv1.Columns.Add(this.colImportedName);
            this.treeViewAdv1.DefaultToolTipProvider = null;
            this.treeViewAdv1.DragDropMarkColor = System.Drawing.Color.Black;
            this.treeViewAdv1.FullRowSelect = true;
            this.treeViewAdv1.LineColor = System.Drawing.SystemColors.ControlDark;
            this.treeViewAdv1.Location = new System.Drawing.Point(6, 19);
            this.treeViewAdv1.Model = null;
            this.treeViewAdv1.Name = "treeViewAdv1";
            this.treeViewAdv1.NodeControls.Add(this.nodeCheckBox1);
            this.treeViewAdv1.NodeControls.Add(this.nodeTextBox1);
            this.treeViewAdv1.NodeControls.Add(this.nodeComboBox1);
            this.treeViewAdv1.NodeControls.Add(this.nodeTextBox2);
            this.treeViewAdv1.SelectedNode = null;
            this.treeViewAdv1.ShowLines = false;
            this.treeViewAdv1.ShowPlusMinus = false;
            this.treeViewAdv1.Size = new System.Drawing.Size(424, 328);
            this.treeViewAdv1.TabIndex = 0;
            this.treeViewAdv1.Text = "treeViewAdv1";
            this.treeViewAdv1.UseColumns = true;
            // 
            // colImport
            // 
            this.colImport.Header = "Import?";
            this.colImport.SortOrder = System.Windows.Forms.SortOrder.None;
            this.colImport.TooltipText = null;
            // 
            // colSourceName
            // 
            this.colSourceName.Header = "Source Column";
            this.colSourceName.SortOrder = System.Windows.Forms.SortOrder.None;
            this.colSourceName.TooltipText = null;
            this.colSourceName.Width = 140;
            // 
            // colDataType
            // 
            this.colDataType.Header = "Data Type";
            this.colDataType.SortOrder = System.Windows.Forms.SortOrder.None;
            this.colDataType.TooltipText = null;
            this.colDataType.Width = 80;
            // 
            // colImportedName
            // 
            this.colImportedName.Header = "Imported Name";
            this.colImportedName.SortOrder = System.Windows.Forms.SortOrder.None;
            this.colImportedName.TooltipText = null;
            this.colImportedName.Width = 150;
            // 
            // nodeCheckBox1
            // 
            this.nodeCheckBox1.DataPropertyName = "Import";
            this.nodeCheckBox1.EditEnabled = true;
            this.nodeCheckBox1.LeftMargin = 0;
            this.nodeCheckBox1.ParentColumn = this.colImport;
            this.nodeCheckBox1.CheckStateChanged += new System.EventHandler<Aga.Controls.Tree.TreePathEventArgs>(this.nodeCheckBox1_CheckStateChanged);
            // 
            // nodeTextBox1
            // 
            this.nodeTextBox1.DataPropertyName = "SourceColumn";
            this.nodeTextBox1.IncrementalSearchEnabled = true;
            this.nodeTextBox1.LeftMargin = 3;
            this.nodeTextBox1.ParentColumn = this.colSourceName;
            this.nodeTextBox1.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // nodeComboBox1
            // 
            this.nodeComboBox1.DataPropertyName = "DataType";
            this.nodeComboBox1.DropDownItems.Add("Integer");
            this.nodeComboBox1.DropDownItems.Add("Real");
            this.nodeComboBox1.DropDownItems.Add("Boolean");
            this.nodeComboBox1.DropDownItems.Add("Text");
            this.nodeComboBox1.DropDownItems.Add("Date/Time");
            this.nodeComboBox1.DropDownItems.Add("Currency");
            this.nodeComboBox1.EditEnabled = true;
            this.nodeComboBox1.IncrementalSearchEnabled = true;
            this.nodeComboBox1.LeftMargin = 3;
            this.nodeComboBox1.ParentColumn = this.colDataType;
            // 
            // nodeTextBox2
            // 
            this.nodeTextBox2.DataPropertyName = "Name";
            this.nodeTextBox2.EditEnabled = true;
            this.nodeTextBox2.EditOnClick = true;
            this.nodeTextBox2.IncrementalSearchEnabled = true;
            this.nodeTextBox2.LeftMargin = 3;
            this.nodeTextBox2.ParentColumn = this.colImportedName;
            this.nodeTextBox2.Trimming = System.Drawing.StringTrimming.EllipsisCharacter;
            // 
            // btnImport
            // 
            this.btnImport.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnImport.Enabled = false;
            this.btnImport.Location = new System.Drawing.Point(721, 376);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 4;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(640, 376);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // ImportTablesForm
            // 
            this.AcceptButton = this.btnImport;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(808, 411);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.tabControl1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(539, 450);
            this.Name = "ImportTablesForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Import Tables";
            this.tabControl1.ResumeLayout(false);
            this.tabSingle.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.tabPQ.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabSingle;
        private System.Windows.Forms.TabPage tabPQ;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnImportFromClipboard;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private Aga.Controls.Tree.TreeColumn colImport;
        private Aga.Controls.Tree.TreeColumn colSourceName;
        private Aga.Controls.Tree.TreeColumn colDataType;
        private Aga.Controls.Tree.TreeColumn colImportedName;
        private Aga.Controls.Tree.NodeControls.NodeCheckBox nodeCheckBox1;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox1;
        private Aga.Controls.Tree.NodeControls.NodeComboBox nodeComboBox1;
        private Aga.Controls.Tree.NodeControls.NodeTextBox nodeTextBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private Aga.Controls.Tree.TreeViewAdv treeViewAdv1;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox comboBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnImportFromQuery;
    }
}