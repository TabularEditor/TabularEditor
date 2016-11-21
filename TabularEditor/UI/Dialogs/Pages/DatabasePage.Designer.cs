namespace TabularEditor.UI.Dialogs.Pages
{
    partial class DatabasePage
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
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.DatabaseIcon = new System.Windows.Forms.DataGridViewImageColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lastUpdateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lastProcessedDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.databaseBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.pnlDatabaseID = new System.Windows.Forms.Panel();
            this.label1 = new System.Windows.Forms.Label();
            this.txtDatabaseName = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.databaseBindingSource)).BeginInit();
            this.pnlDatabaseID.SuspendLayout();
            this.SuspendLayout();
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.AutoGenerateColumns = false;
            this.dataGridView1.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridView1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridView1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.DatabaseIcon,
            this.nameDataGridViewTextBoxColumn,
            this.descriptionDataGridViewTextBoxColumn,
            this.lastUpdateDataGridViewTextBoxColumn,
            this.lastProcessedDataGridViewTextBoxColumn});
            this.dataGridView1.DataSource = this.databaseBindingSource;
            this.dataGridView1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridView1.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.dataGridView1.Location = new System.Drawing.Point(0, 0);
            this.dataGridView1.MultiSelect = false;
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersVisible = false;
            this.dataGridView1.RowTemplate.Height = 18;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(507, 214);
            this.dataGridView1.StandardTab = true;
            this.dataGridView1.TabIndex = 0;
            this.dataGridView1.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellDoubleClick);
            this.dataGridView1.SelectionChanged += new System.EventHandler(this.dataGridView1_SelectionChanged_1);
            this.dataGridView1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView1_KeyDown);
            // 
            // DatabaseIcon
            // 
            this.DatabaseIcon.FillWeight = 20F;
            this.DatabaseIcon.Frozen = true;
            this.DatabaseIcon.HeaderText = "";
            this.DatabaseIcon.Image = global::TabularEditor.Resources.DatabaseMethod_16x;
            this.DatabaseIcon.Name = "DatabaseIcon";
            this.DatabaseIcon.ReadOnly = true;
            this.DatabaseIcon.Width = 20;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "ID";
            this.nameDataGridViewTextBoxColumn.HeaderText = "ID";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            this.nameDataGridViewTextBoxColumn.ReadOnly = true;
            this.nameDataGridViewTextBoxColumn.Width = 200;
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Description";
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            this.descriptionDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // lastUpdateDataGridViewTextBoxColumn
            // 
            this.lastUpdateDataGridViewTextBoxColumn.DataPropertyName = "LastUpdate";
            this.lastUpdateDataGridViewTextBoxColumn.HeaderText = "Last Update";
            this.lastUpdateDataGridViewTextBoxColumn.Name = "lastUpdateDataGridViewTextBoxColumn";
            this.lastUpdateDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // lastProcessedDataGridViewTextBoxColumn
            // 
            this.lastProcessedDataGridViewTextBoxColumn.DataPropertyName = "LastProcessed";
            this.lastProcessedDataGridViewTextBoxColumn.HeaderText = "Last Processed";
            this.lastProcessedDataGridViewTextBoxColumn.Name = "lastProcessedDataGridViewTextBoxColumn";
            this.lastProcessedDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // databaseBindingSource
            // 
            this.databaseBindingSource.DataSource = typeof(Microsoft.AnalysisServices.Tabular.Database);
            // 
            // pnlDatabaseID
            // 
            this.pnlDatabaseID.Controls.Add(this.label1);
            this.pnlDatabaseID.Controls.Add(this.txtDatabaseName);
            this.pnlDatabaseID.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.pnlDatabaseID.Location = new System.Drawing.Point(0, 214);
            this.pnlDatabaseID.Name = "pnlDatabaseID";
            this.pnlDatabaseID.Size = new System.Drawing.Size(507, 26);
            this.pnlDatabaseID.TabIndex = 1;
            this.pnlDatabaseID.Visible = false;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(-3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "Database ID";
            // 
            // txtDatabaseName
            // 
            this.txtDatabaseName.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtDatabaseName.Location = new System.Drawing.Point(79, 6);
            this.txtDatabaseName.Name = "txtDatabaseName";
            this.txtDatabaseName.Size = new System.Drawing.Size(428, 20);
            this.txtDatabaseName.TabIndex = 0;
            this.txtDatabaseName.TextChanged += new System.EventHandler(this.txtDatabaseID_TextChanged);
            // 
            // DatabasePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.pnlDatabaseID);
            this.Name = "DatabasePage";
            this.Size = new System.Drawing.Size(507, 240);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.databaseBindingSource)).EndInit();
            this.pnlDatabaseID.ResumeLayout(false);
            this.pnlDatabaseID.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Panel pnlDatabaseID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtDatabaseName;
        private System.Windows.Forms.BindingSource databaseBindingSource;
        private System.Windows.Forms.DataGridViewImageColumn DatabaseIcon;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastUpdateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastProcessedDataGridViewTextBoxColumn;
    }
}
