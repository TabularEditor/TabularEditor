namespace TabularEditor.Scripting
{
    partial class ScriptOutputForm
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
            this.chkDontShow = new System.Windows.Forms.CheckBox();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.btnClose = new System.Windows.Forms.Button();
            this.DataTextBox = new System.Windows.Forms.TextBox();
            this.DataPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.DataProperties = new System.Windows.Forms.Panel();
            this.DataSplitter = new System.Windows.Forms.Splitter();
            this.DataListView = new System.Windows.Forms.ListView();
            this.NameCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.TypeCol = ((System.Windows.Forms.ColumnHeader)(new System.Windows.Forms.ColumnHeader()));
            this.btnCopy = new System.Windows.Forms.Button();
            this.DataProperties.SuspendLayout();
            this.SuspendLayout();
            // 
            // chkDontShow
            // 
            this.chkDontShow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkDontShow.AutoSize = true;
            this.chkDontShow.Location = new System.Drawing.Point(12, 327);
            this.chkDontShow.Name = "chkDontShow";
            this.chkDontShow.Size = new System.Drawing.Size(143, 17);
            this.chkDontShow.TabIndex = 0;
            this.chkDontShow.Text = "Don\'t show more outputs";
            this.toolTip1.SetToolTip(this.chkDontShow, "Check this to prevent this dialog from showing up on any further Output()-stateme" +
        "nts in the current script execution.\r\nThe dialog may show up again the next time" +
        " a script is executed.");
            this.chkDontShow.UseVisualStyleBackColor = true;
            // 
            // btnClose
            // 
            this.btnClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnClose.Location = new System.Drawing.Point(448, 323);
            this.btnClose.Name = "btnClose";
            this.btnClose.Size = new System.Drawing.Size(76, 23);
            this.btnClose.TabIndex = 2;
            this.btnClose.Text = "Close";
            this.btnClose.UseVisualStyleBackColor = true;
            this.btnClose.Click += new System.EventHandler(this.btnClose_Click);
            // 
            // DataTextBox
            // 
            this.DataTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DataTextBox.Location = new System.Drawing.Point(13, 13);
            this.DataTextBox.Multiline = true;
            this.DataTextBox.Name = "DataTextBox";
            this.DataTextBox.ReadOnly = true;
            this.DataTextBox.Size = new System.Drawing.Size(511, 304);
            this.DataTextBox.TabIndex = 3;
            this.DataTextBox.Visible = false;
            // 
            // DataPropertyGrid
            // 
            this.DataPropertyGrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.DataPropertyGrid.LineColor = System.Drawing.SystemColors.ControlDark;
            this.DataPropertyGrid.Location = new System.Drawing.Point(242, 0);
            this.DataPropertyGrid.Name = "DataPropertyGrid";
            this.DataPropertyGrid.Size = new System.Drawing.Size(269, 305);
            this.DataPropertyGrid.TabIndex = 4;
            // 
            // DataProperties
            // 
            this.DataProperties.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.DataProperties.Controls.Add(this.DataPropertyGrid);
            this.DataProperties.Controls.Add(this.DataSplitter);
            this.DataProperties.Controls.Add(this.DataListView);
            this.DataProperties.Location = new System.Drawing.Point(13, 12);
            this.DataProperties.Name = "DataProperties";
            this.DataProperties.Size = new System.Drawing.Size(511, 305);
            this.DataProperties.TabIndex = 5;
            this.DataProperties.Visible = false;
            // 
            // DataSplitter
            // 
            this.DataSplitter.Location = new System.Drawing.Point(239, 0);
            this.DataSplitter.Name = "DataSplitter";
            this.DataSplitter.Size = new System.Drawing.Size(3, 305);
            this.DataSplitter.TabIndex = 6;
            this.DataSplitter.TabStop = false;
            // 
            // DataListView
            // 
            this.DataListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.NameCol,
            this.TypeCol});
            this.DataListView.Dock = System.Windows.Forms.DockStyle.Left;
            this.DataListView.FullRowSelect = true;
            this.DataListView.Location = new System.Drawing.Point(0, 0);
            this.DataListView.MultiSelect = false;
            this.DataListView.Name = "DataListView";
            this.DataListView.Size = new System.Drawing.Size(239, 305);
            this.DataListView.TabIndex = 5;
            this.DataListView.UseCompatibleStateImageBehavior = false;
            this.DataListView.View = System.Windows.Forms.View.Details;
            this.DataListView.VirtualMode = true;
            this.DataListView.RetrieveVirtualItem += new System.Windows.Forms.RetrieveVirtualItemEventHandler(this.DataListView_RetrieveVirtualItem);
            this.DataListView.SelectedIndexChanged += new System.EventHandler(this.DataListView_SelectedIndexChanged);
            this.DataListView.MouseDoubleClick += new System.Windows.Forms.MouseEventHandler(this.DataListView_MouseDoubleClick);
            // 
            // NameCol
            // 
            this.NameCol.Text = "Name";
            this.NameCol.Width = 130;
            // 
            // TypeCol
            // 
            this.TypeCol.Text = "Type";
            this.TypeCol.Width = 80;
            // 
            // btnCopy
            // 
            this.btnCopy.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCopy.Location = new System.Drawing.Point(312, 323);
            this.btnCopy.Name = "btnCopy";
            this.btnCopy.Size = new System.Drawing.Size(130, 23);
            this.btnCopy.TabIndex = 6;
            this.btnCopy.Text = "Copy to clipboard";
            this.btnCopy.UseVisualStyleBackColor = true;
            this.btnCopy.Click += new System.EventHandler(this.btnCopy_Click);
            // 
            // ScriptOutputForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnClose;
            this.ClientSize = new System.Drawing.Size(536, 358);
            this.Controls.Add(this.btnCopy);
            this.Controls.Add(this.DataProperties);
            this.Controls.Add(this.DataTextBox);
            this.Controls.Add(this.btnClose);
            this.Controls.Add(this.chkDontShow);
            this.MinimizeBox = false;
            this.Name = "ScriptOutputForm";
            this.ShowIcon = false;
            this.Text = "Script Output";
            this.DataProperties.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox chkDontShow;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.Button btnClose;
        private System.Windows.Forms.TextBox DataTextBox;
        private System.Windows.Forms.PropertyGrid DataPropertyGrid;
        private System.Windows.Forms.Panel DataProperties;
        private System.Windows.Forms.Splitter DataSplitter;
        private System.Windows.Forms.ListView DataListView;
        private System.Windows.Forms.ColumnHeader NameCol;
        private System.Windows.Forms.ColumnHeader TypeCol;
        private System.Windows.Forms.Button btnCopy;
    }
}