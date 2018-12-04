namespace TabularEditor.UI.Dialogs.Pages
{
    partial class ChooseDataSourcePage
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
            this.label1 = new System.Windows.Forms.Label();
            this.rdbExisting = new System.Windows.Forms.RadioButton();
            this.rdbNew = new System.Windows.Forms.RadioButton();
            this.rdbTemporary = new System.Windows.Forms.RadioButton();
            this.listView1 = new System.Windows.Forms.ListView();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(221, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "How would you like to import table metadata?";
            // 
            // rdbExisting
            // 
            this.rdbExisting.AutoSize = true;
            this.rdbExisting.Location = new System.Drawing.Point(6, 34);
            this.rdbExisting.Name = "rdbExisting";
            this.rdbExisting.Size = new System.Drawing.Size(249, 17);
            this.rdbExisting.TabIndex = 3;
            this.rdbExisting.TabStop = true;
            this.rdbExisting.Text = "Choose an existing Data Source from the model";
            this.rdbExisting.UseVisualStyleBackColor = true;
            this.rdbExisting.CheckedChanged += new System.EventHandler(this.rdbExisting_CheckedChanged);
            // 
            // rdbNew
            // 
            this.rdbNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdbNew.AutoSize = true;
            this.rdbNew.Location = new System.Drawing.Point(6, 196);
            this.rdbNew.Name = "rdbNew";
            this.rdbNew.Size = new System.Drawing.Size(262, 17);
            this.rdbNew.TabIndex = 4;
            this.rdbNew.TabStop = true;
            this.rdbNew.Text = "Create a new Data Source and add it to the model";
            this.rdbNew.UseVisualStyleBackColor = true;
            // 
            // rdbTemporary
            // 
            this.rdbTemporary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.rdbTemporary.AutoSize = true;
            this.rdbTemporary.Location = new System.Drawing.Point(6, 219);
            this.rdbTemporary.Name = "rdbTemporary";
            this.rdbTemporary.Size = new System.Drawing.Size(368, 17);
            this.rdbTemporary.TabIndex = 5;
            this.rdbTemporary.TabStop = true;
            this.rdbTemporary.Text = "Use a temporary connection (no Data Source will be added to the model)";
            this.rdbTemporary.UseVisualStyleBackColor = true;
            // 
            // listView1
            // 
            this.listView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.listView1.Location = new System.Drawing.Point(25, 57);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(547, 133);
            this.listView1.TabIndex = 6;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            this.listView1.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
            // 
            // radioButton1
            // 
            this.radioButton1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(6, 242);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(261, 17);
            this.radioButton1.TabIndex = 7;
            this.radioButton1.TabStop = true;
            this.radioButton1.Text = "Manually import metadata from another application";
            this.radioButton1.UseVisualStyleBackColor = true;
            // 
            // ChooseDataSourcePage
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.rdbTemporary);
            this.Controls.Add(this.rdbNew);
            this.Controls.Add(this.rdbExisting);
            this.Controls.Add(this.label1);
            this.Name = "ChooseDataSourcePage";
            this.Size = new System.Drawing.Size(601, 266);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.RadioButton rdbExisting;
        private System.Windows.Forms.RadioButton rdbNew;
        private System.Windows.Forms.RadioButton rdbTemporary;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.RadioButton radioButton1;
    }
}
