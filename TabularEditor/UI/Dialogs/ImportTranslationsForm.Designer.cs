namespace TabularEditor.UI.Dialogs
{
    partial class ImportTranslationsForm
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
            this.txtJsonFile = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnBrowse = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.chkOverwriteExisting = new System.Windows.Forms.CheckBox();
            this.chkIgnoreInvalid = new System.Windows.Forms.CheckBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnImport = new System.Windows.Forms.Button();
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtJsonFile
            // 
            this.txtJsonFile.Location = new System.Drawing.Point(79, 33);
            this.txtJsonFile.Name = "txtJsonFile";
            this.txtJsonFile.Size = new System.Drawing.Size(339, 20);
            this.txtJsonFile.TabIndex = 0;
            this.txtJsonFile.TextChanged += new System.EventHandler(this.textBox1_TextChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnBrowse);
            this.groupBox1.Controls.Add(this.txtJsonFile);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(424, 90);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Translations JSON File";
            // 
            // btnBrowse
            // 
            this.btnBrowse.Location = new System.Drawing.Point(343, 59);
            this.btnBrowse.Name = "btnBrowse";
            this.btnBrowse.Size = new System.Drawing.Size(75, 23);
            this.btnBrowse.TabIndex = 1;
            this.btnBrowse.Text = "Browse";
            this.btnBrowse.UseVisualStyleBackColor = true;
            this.btnBrowse.Click += new System.EventHandler(this.btnBrowse_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 36);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 13);
            this.label1.TabIndex = 2;
            this.label1.Text = "File name:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.chkIgnoreInvalid);
            this.groupBox2.Controls.Add(this.chkOverwriteExisting);
            this.groupBox2.Location = new System.Drawing.Point(13, 110);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(424, 82);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Import Options";
            // 
            // chkOverwriteExisting
            // 
            this.chkOverwriteExisting.AutoSize = true;
            this.chkOverwriteExisting.Location = new System.Drawing.Point(9, 29);
            this.chkOverwriteExisting.Name = "chkOverwriteExisting";
            this.chkOverwriteExisting.Size = new System.Drawing.Size(165, 17);
            this.chkOverwriteExisting.TabIndex = 0;
            this.chkOverwriteExisting.Text = "Overwrite existing translations";
            this.chkOverwriteExisting.UseVisualStyleBackColor = true;
            // 
            // chkIgnoreInvalid
            // 
            this.chkIgnoreInvalid.AutoSize = true;
            this.chkIgnoreInvalid.Location = new System.Drawing.Point(9, 52);
            this.chkIgnoreInvalid.Name = "chkIgnoreInvalid";
            this.chkIgnoreInvalid.Size = new System.Drawing.Size(126, 17);
            this.chkIgnoreInvalid.TabIndex = 1;
            this.chkIgnoreInvalid.Text = "Ignore invalid objects";
            this.chkIgnoreInvalid.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(362, 198);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnImport
            // 
            this.btnImport.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnImport.Enabled = false;
            this.btnImport.Location = new System.Drawing.Point(281, 198);
            this.btnImport.Name = "btnImport";
            this.btnImport.Size = new System.Drawing.Size(75, 23);
            this.btnImport.TabIndex = 4;
            this.btnImport.Text = "Import";
            this.btnImport.UseVisualStyleBackColor = true;
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "json";
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "JSON Translations File|*.json|All files|*.*";
            // 
            // ImportTranslationsForm
            // 
            this.AcceptButton = this.btnImport;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(449, 229);
            this.Controls.Add(this.btnImport);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportTranslationsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Import Translations";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtJsonFile;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBrowse;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkIgnoreInvalid;
        private System.Windows.Forms.CheckBox chkOverwriteExisting;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnImport;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
    }
}