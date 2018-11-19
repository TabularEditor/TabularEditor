namespace TabularEditor.UI.Dialogs
{
    partial class ImportTablesWizard
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
            this.importTablesPage1 = new TabularEditor.UI.Dialogs.Pages.ImportTablesPage();
            this.panel1 = new System.Windows.Forms.Panel();
            this.SuspendLayout();
            // 
            // importTablesPage1
            // 
            this.importTablesPage1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.importTablesPage1.Location = new System.Drawing.Point(0, 0);
            this.importTablesPage1.Name = "importTablesPage1";
            this.importTablesPage1.Size = new System.Drawing.Size(839, 423);
            this.importTablesPage1.TabIndex = 0;
            // 
            // panel1
            // 
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 423);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(839, 42);
            this.panel1.TabIndex = 1;
            // 
            // ImportTablesWizard
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(839, 465);
            this.Controls.Add(this.importTablesPage1);
            this.Controls.Add(this.panel1);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ImportTablesWizard";
            this.ShowInTaskbar = false;
            this.Text = "ImportTablesWizard";
            this.ResumeLayout(false);

        }

        #endregion

        private Pages.ImportTablesPage importTablesPage1;
        private System.Windows.Forms.Panel panel1;
    }
}