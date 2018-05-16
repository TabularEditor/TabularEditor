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
            this.treeObjects = new System.Windows.Forms.TreeView();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.copyTreeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.copyTreeAsJSONToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.goToObjectToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btnCancel = new System.Windows.Forms.Button();
            this.radioButton1 = new System.Windows.Forms.RadioButton();
            this.radioButton2 = new System.Windows.Forms.RadioButton();
            this.radioButton3 = new System.Windows.Forms.RadioButton();
            this.chkShowInactive = new System.Windows.Forms.CheckBox();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeObjects
            // 
            this.treeObjects.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeObjects.ContextMenuStrip = this.contextMenuStrip1;
            this.treeObjects.Location = new System.Drawing.Point(12, 85);
            this.treeObjects.Name = "treeObjects";
            this.treeObjects.ShowNodeToolTips = true;
            this.treeObjects.Size = new System.Drawing.Size(316, 200);
            this.treeObjects.TabIndex = 0;
            this.treeObjects.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeObjects_BeforeCollapse);
            this.treeObjects.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeObjects_BeforeExpand);
            this.treeObjects.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeObjects_NodeMouseDoubleClick);
            this.treeObjects.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.treeObjects_KeyPress);
            this.treeObjects.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeObjects_MouseDown);
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.copyTreeToolStripMenuItem,
            this.copyTreeAsJSONToolStripMenuItem,
            this.goToObjectToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(215, 70);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // copyTreeToolStripMenuItem
            // 
            this.copyTreeToolStripMenuItem.Name = "copyTreeToolStripMenuItem";
            this.copyTreeToolStripMenuItem.ShortcutKeyDisplayString = "";
            this.copyTreeToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyTreeToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.copyTreeToolStripMenuItem.Text = "Copy Tree as ASCII";
            this.copyTreeToolStripMenuItem.Click += new System.EventHandler(this.copyTreeToolStripMenuItem_Click);
            // 
            // copyTreeAsJSONToolStripMenuItem
            // 
            this.copyTreeAsJSONToolStripMenuItem.Name = "copyTreeAsJSONToolStripMenuItem";
            this.copyTreeAsJSONToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.copyTreeAsJSONToolStripMenuItem.Text = "Copy Tree as JSON";
            this.copyTreeAsJSONToolStripMenuItem.Click += new System.EventHandler(this.copyTreeAsJSONToolStripMenuItem_Click);
            // 
            // goToObjectToolStripMenuItem
            // 
            this.goToObjectToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            this.goToObjectToolStripMenuItem.Name = "goToObjectToolStripMenuItem";
            this.goToObjectToolStripMenuItem.Size = new System.Drawing.Size(214, 22);
            this.goToObjectToolStripMenuItem.Text = "Go to Object";
            this.goToObjectToolStripMenuItem.Click += new System.EventHandler(this.goToObjectToolStripMenuItem_Click);
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
            // chkShowInactive
            // 
            this.chkShowInactive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.chkShowInactive.AutoSize = true;
            this.chkShowInactive.Checked = true;
            this.chkShowInactive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkShowInactive.Location = new System.Drawing.Point(12, 295);
            this.chkShowInactive.Name = "chkShowInactive";
            this.chkShowInactive.Size = new System.Drawing.Size(154, 17);
            this.chkShowInactive.TabIndex = 7;
            this.chkShowInactive.Text = "Show inactive relationships";
            this.chkShowInactive.UseVisualStyleBackColor = true;
            this.chkShowInactive.Visible = false;
            this.chkShowInactive.CheckedChanged += new System.EventHandler(this.chkShowInactive_CheckedChanged);
            // 
            // DependencyForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(340, 326);
            this.Controls.Add(this.chkShowInactive);
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
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeObjects;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.RadioButton radioButton1;
        private System.Windows.Forms.RadioButton radioButton2;
        private System.Windows.Forms.RadioButton radioButton3;
        private System.Windows.Forms.CheckBox chkShowInactive;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem copyTreeToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem goToObjectToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem copyTreeAsJSONToolStripMenuItem;
    }
}