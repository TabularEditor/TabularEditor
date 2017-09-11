namespace TabularEditor
{
    partial class DependencyForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DependencyForm));
            this.treeObjects = new System.Windows.Forms.TreeView();
            this.tabularTreeImages = new System.Windows.Forms.ImageList(this.components);
            this.btnCancel = new System.Windows.Forms.Button();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // treeObjects
            // 
            this.treeObjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeObjects.ImageIndex = 0;
            this.treeObjects.ImageList = this.tabularTreeImages;
            this.treeObjects.Location = new System.Drawing.Point(12, 85);
            this.treeObjects.Name = "treeObjects";
            this.treeObjects.SelectedImageIndex = 0;
            this.treeObjects.ShowNodeToolTips = true;
            this.treeObjects.Size = new System.Drawing.Size(316, 200);
            this.treeObjects.TabIndex = 0;
            this.treeObjects.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeObjects_BeforeCollapse);
            this.treeObjects.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeObjects_BeforeExpand);
            this.treeObjects.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeObjects_NodeMouseDoubleClick);
            this.treeObjects.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.treeObjects_KeyPress);
            this.treeObjects.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeObjects_MouseDown);
            // 
            // tabularTreeImages
            // 
            this.tabularTreeImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("tabularTreeImages.ImageStream")));
            this.tabularTreeImages.TransparentColor = System.Drawing.Color.Transparent;
            this.tabularTreeImages.Images.SetKeyName(0, "folder");
            this.tabularTreeImages.Images.SetKeyName(1, "folderOpen");
            this.tabularTreeImages.Images.SetKeyName(2, "table");
            this.tabularTreeImages.Images.SetKeyName(3, "hierarchy");
            this.tabularTreeImages.Images.SetKeyName(4, "column");
            this.tabularTreeImages.Images.SetKeyName(5, "calculator");
            this.tabularTreeImages.Images.SetKeyName(6, "kpi");
            this.tabularTreeImages.Images.SetKeyName(7, "measure");
            this.tabularTreeImages.Images.SetKeyName(8, "sigma");
            this.tabularTreeImages.Images.SetKeyName(9, "cube");
            this.tabularTreeImages.Images.SetKeyName(10, "link");
            this.tabularTreeImages.Images.SetKeyName(11, "level");
            this.tabularTreeImages.Images.SetKeyName(12, "calccolumn");
            this.tabularTreeImages.Images.SetKeyName(13, "level01");
            this.tabularTreeImages.Images.SetKeyName(14, "level02");
            this.tabularTreeImages.Images.SetKeyName(15, "level03");
            this.tabularTreeImages.Images.SetKeyName(16, "level04");
            this.tabularTreeImages.Images.SetKeyName(17, "level05");
            this.tabularTreeImages.Images.SetKeyName(18, "level06");
            this.tabularTreeImages.Images.SetKeyName(19, "level07");
            this.tabularTreeImages.Images.SetKeyName(20, "level08");
            this.tabularTreeImages.Images.SetKeyName(21, "level09");
            this.tabularTreeImages.Images.SetKeyName(22, "level10");
            this.tabularTreeImages.Images.SetKeyName(23, "level11");
            this.tabularTreeImages.Images.SetKeyName(24, "level12");
            this.tabularTreeImages.Images.SetKeyName(25, "warning");
            this.tabularTreeImages.Images.SetKeyName(26, "question");
            this.tabularTreeImages.Images.SetKeyName(27, "method");
            this.tabularTreeImages.Images.SetKeyName(28, "property");
            this.tabularTreeImages.Images.SetKeyName(29, "exmethod");
            this.tabularTreeImages.Images.SetKeyName(30, "enum");
            this.tabularTreeImages.Images.SetKeyName(31, "calctable");
            this.tabularTreeImages.Images.SetKeyName(32, "perspective");
            this.tabularTreeImages.Images.SetKeyName(33, "translation");
            this.tabularTreeImages.Images.SetKeyName(34, "role");
            this.tabularTreeImages.Images.SetKeyName(35, "culture");
            this.tabularTreeImages.Images.SetKeyName(36, "datasource");
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(253, 291);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Close";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // radioButton1
            // 
            this.radioButton1.AutoSize = true;
            this.radioButton1.Location = new System.Drawing.Point(13, 13);
            this.radioButton1.Name = "radioButton1";
            this.radioButton1.Size = new System.Drawing.Size(191, 17);
            this.radioButton1.TabIndex = 4;
            this.radioButton1.Text = "Show objects on which {0} depend";
            this.radioButton1.UseVisualStyleBackColor = true;
            this.radioButton1.CheckedChanged += new System.EventHandler(this.DependencyChange);
            // 
            // radioButton2
            // 
            this.radioButton2.AutoSize = true;
            this.radioButton2.Checked = true;
            this.radioButton2.Location = new System.Drawing.Point(13, 36);
            this.radioButton2.Name = "radioButton2";
            this.radioButton2.Size = new System.Drawing.Size(181, 17);
            this.radioButton2.TabIndex = 5;
            this.radioButton2.TabStop = true;
            this.radioButton2.Text = "Show objects that depend on {0}";
            this.radioButton2.UseVisualStyleBackColor = true;
            this.radioButton2.CheckedChanged += new System.EventHandler(this.DependencyChange);
            // 
            // radioButton3
            // 
            this.radioButton3.AutoSize = true;
            this.radioButton3.Location = new System.Drawing.Point(13, 59);
            this.radioButton3.Name = "radioButton3";
            this.radioButton3.Size = new System.Drawing.Size(190, 17);
            this.radioButton3.TabIndex = 6;
            this.radioButton3.TabStop = true;
            this.radioButton3.Text = "Show relationships starting from {0}";
            this.radioButton3.UseVisualStyleBackColor = true;
            this.radioButton3.CheckedChanged += new System.EventHandler(this.DependencyChange);
            // 
            // DependencyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(340, 326);
            this.Controls.Add(this.radioButton3);
            this.Controls.Add(this.radioButton2);
            this.Controls.Add(this.radioButton1);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.treeObjects);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(331, 212);
            this.Name = "DependencyForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Object Dependencies";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeObjects;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.ImageList tabularTreeImages;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton3;
    }
}