namespace TabularEditor
{
    partial class FormDisplayFolderSelect
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormDisplayFolderSelect));
            this.treeFolders = new System.Windows.Forms.TreeView();
            this.tabularTreeImages = new System.Windows.Forms.ImageList(this.components);
            this.label1 = new System.Windows.Forms.Label();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnNewFolder = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // treeFolders
            // 
            this.treeFolders.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeFolders.ImageIndex = 0;
            this.treeFolders.ImageList = this.tabularTreeImages;
            this.treeFolders.Location = new System.Drawing.Point(12, 33);
            this.treeFolders.Name = "treeFolders";
            this.treeFolders.SelectedImageIndex = 0;
            this.treeFolders.Size = new System.Drawing.Size(316, 252);
            this.treeFolders.TabIndex = 0;
            this.treeFolders.BeforeLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeFolders_BeforeLabelEdit);
            this.treeFolders.AfterLabelEdit += new System.Windows.Forms.NodeLabelEditEventHandler(this.treeFolders_AfterLabelEdit);
            this.treeFolders.AfterCollapse += new System.Windows.Forms.TreeViewEventHandler(this.treeFolders_AfterCollapse);
            this.treeFolders.AfterExpand += new System.Windows.Forms.TreeViewEventHandler(this.treeFolders_AfterExpand);
            this.treeFolders.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeFolders_AfterSelect);
            this.treeFolders.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeFolders_MouseDown);
            // 
            // tabularTreeImages
            // 
            this.tabularTreeImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("tabularTreeImages.ImageStream")));
            this.tabularTreeImages.TransparentColor = System.Drawing.Color.Transparent;
            this.tabularTreeImages.Images.SetKeyName(0, "folder");
            this.tabularTreeImages.Images.SetKeyName(1, "folderOpen");
            this.tabularTreeImages.Images.SetKeyName(2, "Table_16x.png");
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(12, 13);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(276, 17);
            this.label1.TabIndex = 1;
            this.label1.Text = "Move the selected object to the following Display Folder:";
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnOK.Location = new System.Drawing.Point(172, 291);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(75, 23);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(253, 291);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // btnNewFolder
            // 
            this.btnNewFolder.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnNewFolder.Location = new System.Drawing.Point(12, 291);
            this.btnNewFolder.Name = "btnNewFolder";
            this.btnNewFolder.Size = new System.Drawing.Size(118, 23);
            this.btnNewFolder.TabIndex = 4;
            this.btnNewFolder.Text = "&Make New Folder";
            this.btnNewFolder.UseVisualStyleBackColor = true;
            this.btnNewFolder.Click += new System.EventHandler(this.btnNewFolder_Click);
            // 
            // FormDisplayFolderSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(340, 326);
            this.ControlBox = false;
            this.Controls.Add(this.btnNewFolder);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.treeFolders);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(331, 212);
            this.Name = "FormDisplayFolderSelect";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Browse Display Folders";
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView treeFolders;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnNewFolder;
        private System.Windows.Forms.ImageList tabularTreeImages;
    }
}