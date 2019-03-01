namespace TabularEditor.BestPracticeAnalyzer
{
    partial class BPAManagerAddCollectionDialog
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
            this.rdbNewFile = new System.Windows.Forms.RadioButton();
            this.rdbUrl = new System.Windows.Forms.RadioButton();
            this.rdbLocalFile = new System.Windows.Forms.RadioButton();
            this.lblHeader = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // rdbNewFile
            // 
            this.rdbNewFile.AutoSize = true;
            this.rdbNewFile.Checked = true;
            this.rdbNewFile.Location = new System.Drawing.Point(103, 38);
            this.rdbNewFile.Name = "rdbNewFile";
            this.rdbNewFile.Size = new System.Drawing.Size(123, 17);
            this.rdbNewFile.TabIndex = 0;
            this.rdbNewFile.TabStop = true;
            this.rdbNewFile.Text = "Create new Rule File";
            this.rdbNewFile.UseVisualStyleBackColor = true;
            // 
            // rdbUrl
            // 
            this.rdbUrl.AutoSize = true;
            this.rdbUrl.Location = new System.Drawing.Point(103, 84);
            this.rdbUrl.Name = "rdbUrl";
            this.rdbUrl.Size = new System.Drawing.Size(152, 17);
            this.rdbUrl.TabIndex = 2;
            this.rdbUrl.Text = "Include Rule File from URL";
            this.rdbUrl.UseVisualStyleBackColor = true;
            // 
            // rdbLocalFile
            // 
            this.rdbLocalFile.AutoSize = true;
            this.rdbLocalFile.Location = new System.Drawing.Point(103, 61);
            this.rdbLocalFile.Name = "rdbLocalFile";
            this.rdbLocalFile.Size = new System.Drawing.Size(129, 17);
            this.rdbLocalFile.TabIndex = 3;
            this.rdbLocalFile.Text = "Include local Rule File";
            this.rdbLocalFile.UseVisualStyleBackColor = true;
            // 
            // lblHeader
            // 
            this.lblHeader.AutoSize = true;
            this.lblHeader.Location = new System.Drawing.Point(12, 9);
            this.lblHeader.Name = "lblHeader";
            this.lblHeader.Size = new System.Drawing.Size(150, 13);
            this.lblHeader.TabIndex = 4;
            this.lblHeader.Text = "How do you want to proceed?";
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(102, 116);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(183, 116);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 6;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // BPAManagerAddCollectionDialog
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(358, 151);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.lblHeader);
            this.Controls.Add(this.rdbLocalFile);
            this.Controls.Add(this.rdbUrl);
            this.Controls.Add(this.rdbNewFile);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "BPAManagerAddCollectionDialog";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Add Best Practice Rule collection";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.RadioButton rdbNewFile;
        private System.Windows.Forms.RadioButton rdbUrl;
        private System.Windows.Forms.RadioButton rdbLocalFile;
        private System.Windows.Forms.Label lblHeader;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}