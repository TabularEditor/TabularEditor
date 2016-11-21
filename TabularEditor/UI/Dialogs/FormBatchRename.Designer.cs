namespace TabularEditor
{
    partial class FormBatchRename
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
            this.txtRename = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.lblExample = new System.Windows.Forms.Label();
            this.chkTranslations = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(226, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "How should the duplicated objects be named?";
            // 
            // txtRename
            // 
            this.txtRename.Location = new System.Drawing.Point(16, 50);
            this.txtRename.Name = "txtRename";
            this.txtRename.Size = new System.Drawing.Size(329, 20);
            this.txtRename.TabIndex = 1;
            this.txtRename.Text = "[Name] Copy";
            this.txtRename.TextChanged += new System.EventHandler(this.txtRename_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(16, 77);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(149, 26);
            this.label2.TabIndex = 2;
            this.label2.Text = "Special syntax:\r\n[Name] = Original object name";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(269, 147);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnOK
            // 
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(188, 147);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 5;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // lblExample
            // 
            this.lblExample.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblExample.Location = new System.Drawing.Point(16, 116);
            this.lblExample.Name = "lblExample";
            this.lblExample.Size = new System.Drawing.Size(328, 21);
            this.lblExample.TabIndex = 6;
            this.lblExample.Text = "My Measure => My Measure Copy";
            this.lblExample.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // chkTranslations
            // 
            this.chkTranslations.AutoSize = true;
            this.chkTranslations.Checked = true;
            this.chkTranslations.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkTranslations.Location = new System.Drawing.Point(19, 151);
            this.chkTranslations.Name = "chkTranslations";
            this.chkTranslations.Size = new System.Drawing.Size(117, 17);
            this.chkTranslations.TabIndex = 7;
            this.chkTranslations.Text = "Include translations";
            this.chkTranslations.UseVisualStyleBackColor = true;
            this.chkTranslations.CheckedChanged += new System.EventHandler(this.chkTranslations_CheckedChanged);
            // 
            // FormBatchRename
            // 
            this.AcceptButton = this.btnOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(358, 182);
            this.ControlBox = false;
            this.Controls.Add(this.chkTranslations);
            this.Controls.Add(this.lblExample);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtRename);
            this.Controls.Add(this.label1);
            this.Name = "FormBatchRename";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "Duplicate objects";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtRename;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Label lblExample;
        private System.Windows.Forms.CheckBox chkTranslations;
    }
}