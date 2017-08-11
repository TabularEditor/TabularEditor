namespace VisualizeRelationships
{
    partial class VisualizeRelationshipsForm
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
            this.nodesControl1 = new NodeEditor.NodesControl();
            this.SuspendLayout();
            // 
            // nodesControl1
            // 
            this.nodesControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.nodesControl1.Location = new System.Drawing.Point(0, 0);
            this.nodesControl1.Name = "nodesControl1";
            this.nodesControl1.Size = new System.Drawing.Size(890, 568);
            this.nodesControl1.TabIndex = 0;
            // 
            // VisualizeRelationshipsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(890, 568);
            this.Controls.Add(this.nodesControl1);
            this.Name = "VisualizeRelationshipsForm";
            this.Text = "Model Relationships";
            this.ResumeLayout(false);

        }

        #endregion

        private NodeEditor.NodesControl nodesControl1;
    }
}