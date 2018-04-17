namespace TabularEditor.UI.Dialogs
{
    partial class MarkAsDateTableForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MarkAsDateTableForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkDateTable = new System.Windows.Forms.CheckBox();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbKeyColumn = new System.Windows.Forms.ComboBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.cmbKeyColumn);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.chkDateTable);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(353, 196);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Date Table";
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(7, 20);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(340, 58);
            this.label2.TabIndex = 0;
            this.label2.Text = resources.GetString("label2.Text");
            // 
            // chkDateTable
            // 
            this.chkDateTable.AutoSize = true;
            this.chkDateTable.Location = new System.Drawing.Point(10, 84);
            this.chkDateTable.Name = "chkDateTable";
            this.chkDateTable.Size = new System.Drawing.Size(171, 17);
            this.chkDateTable.TabIndex = 2;
            this.chkDateTable.Text = "Mark \'<table>\' as a Date Table";
            this.chkDateTable.UseVisualStyleBackColor = true;
            this.chkDateTable.CheckedChanged += new System.EventHandler(this.chkDateTable_CheckedChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(7, 111);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(340, 46);
            this.label1.TabIndex = 3;
            this.label1.Text = "Choose a column from the table to act as key. The column must contain contiguous," +
    " unique values and the Data Type property of the column must be set to DateTime." +
    "";
            // 
            // cmbKeyColumn
            // 
            this.cmbKeyColumn.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbKeyColumn.Enabled = false;
            this.cmbKeyColumn.FormattingEnabled = true;
            this.cmbKeyColumn.Location = new System.Drawing.Point(10, 161);
            this.cmbKeyColumn.Name = "cmbKeyColumn";
            this.cmbKeyColumn.Size = new System.Drawing.Size(332, 21);
            this.cmbKeyColumn.TabIndex = 4;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(210, 215);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 1;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(291, 215);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 2;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // MarkAsDateTableForm
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(378, 247);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MarkAsDateTableForm";
            this.Text = "Mark as Date Table";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ComboBox cmbKeyColumn;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkDateTable;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}