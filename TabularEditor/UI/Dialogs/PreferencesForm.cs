using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using TabularEditor.UIServices;

namespace TabularEditor.UI.Dialogs
{
    public partial class PreferencesForm : Form
    {
        public PreferencesForm()
        {
            InitializeComponent();
            chkCopyIncludeOLS.Enabled = true;
        }

        private void PreferencesForm_Load(object sender, EventArgs e)
        {
            lblCurrentVersion.Text = "Current Version: " + UpdateService.CurrentVersion;
        }

        private void btnVersionCheck_Click(object sender, EventArgs e)
        {
            var newVersion = UpdateService.Check(true);
            if (!newVersion.HasValue) return;
            if(newVersion.Value)
            {
                btnVersionCheck.Visible = false;
                lblAvailableVersion.Visible = true;
                lblAvailableVersion.Text = "Available Version: " + UpdateService.AvailableVersion + " (click to download)";
            } else
            {
                MessageBox.Show("You are currently using the latest version of Tabular Editor.", "No updates available", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void btnFolder_Click(object sender, EventArgs e)
        {
            using (var fbd = new CommonOpenFileDialog() { IsFolderPicker = true, InitialDirectory = txtBackupPath.Text })
            {
                var res = fbd.ShowDialog();
                if (res == CommonFileDialogResult.Ok)
                {
                    txtBackupPath.Text = fbd.FileName;
                }
            }
        }

        private void chkAutoBackup_CheckedChanged(object sender, EventArgs e)
        {
            txtBackupPath.Enabled = chkAutoBackup.Checked;
            btnFolder.Enabled = chkAutoBackup.Checked;
        }

        private void lblAvailableVersion_Click(object sender, EventArgs e)
        {
            UpdateService.OpenDownloadPage();
        }

        private void PreferencesForm_Shown(object sender, EventArgs e)
        {
            if (UpdateService.UpdateAvailable ?? false)
            {
                btnVersionCheck.Visible = false;
                lblAvailableVersion.Visible = true;
                lblAvailableVersion.Text = "Available Version: " + UpdateService.AvailableVersion + " (click to download)";
            }

            chkAutoBackup.Checked = Preferences.Current.BackupOnSave;
            txtBackupPath.Text = Preferences.Current.BackupLocation;
            chkAutoUpdate.Checked = Preferences.Current.CheckForUpdates;
            chkFixup.Checked = Preferences.Current.FormulaFixup;
            chkAllowUnsupportedPBIFeatures.Checked = Preferences.Current.AllowUnsupportedPBIFeatures;

            chkCopyIncludeTranslations.Checked = Preferences.Current.Copy_IncludeTranslations;
            chkCopyIncludePerspectives.Checked = Preferences.Current.Copy_IncludePerspectives;
            chkCopyIncludeRLS.Checked = Preferences.Current.Copy_IncludeRLS;
            chkCopyIncludeOLS.Checked = Preferences.Current.Copy_IncludeOLS;

            LoadCheckedNodes(treeView1.Nodes, Preferences.Current.SaveToFolder_Levels);

            chkIgnoreTimestamps.Checked = Preferences.Current.SaveToFolder_IgnoreTimestamps;
            chkIgnoreInfObjects.Checked = Preferences.Current.SaveToFolder_IgnoreInferredObjects;
            chkIgnoreInfProps.Checked = Preferences.Current.SaveToFolder_IgnoreInferredProperties;
            chkSplitMultiline.Checked = Preferences.Current.SaveToFolder_SplitMultilineStrings;
            chkPrefixFiles.Checked = Preferences.Current.SaveToFolder_PrefixFiles;

            chkLocalPerspectives.Checked = Preferences.Current.SaveToFolder_LocalPerspectives;
            chkLocalTranslations.Checked = Preferences.Current.SaveToFolder_LocalTranslations;
            chkLocalRelationships.Checked = Preferences.Current.SaveToFolder_LocalRelationships;
            SetNodeVisible("Perspectives", !chkLocalPerspectives.Checked);
            SetNodeVisible("Translations", !chkLocalTranslations.Checked);
            SetNodeVisible("Relationships", !chkLocalRelationships.Checked);

            chkIgnoreTimestampsFile.Checked = Preferences.Current.SaveToFile_IgnoreTimestamps;
            chkIgnoreInfObjectsFile.Checked = Preferences.Current.SaveToFile_IgnoreInferredObjects;
            chkIgnoreInfPropsFile.Checked = Preferences.Current.SaveToFile_IgnoreInferredProperties;
            chkSplitMultilineFile.Checked = Preferences.Current.SaveToFile_SplitMultilineStrings;
        }

        private void PreferencesForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(DialogResult == DialogResult.OK)
            {
                Preferences.Current.BackupLocation = chkAutoBackup.Checked ? txtBackupPath.Text : string.Empty;
                Preferences.Current.CheckForUpdates = chkAutoUpdate.Checked;
                Preferences.Current.FormulaFixup = chkFixup.Checked;
                Preferences.Current.AllowUnsupportedPBIFeatures = chkAllowUnsupportedPBIFeatures.Checked;

                Preferences.Current.SaveToFolder_IgnoreTimestamps = chkIgnoreTimestamps.Checked;
                Preferences.Current.SaveToFolder_IgnoreInferredObjects = chkIgnoreInfObjects.Checked;
                Preferences.Current.SaveToFolder_IgnoreInferredProperties = chkIgnoreInfProps.Checked;
                Preferences.Current.SaveToFolder_SplitMultilineStrings = chkSplitMultiline.Checked;
                Preferences.Current.SaveToFolder_PrefixFiles = chkPrefixFiles.Checked;
                Preferences.Current.SaveToFolder_LocalPerspectives = chkLocalPerspectives.Checked;
                Preferences.Current.SaveToFolder_LocalTranslations = chkLocalTranslations.Checked;
                Preferences.Current.SaveToFolder_LocalRelationships = chkLocalRelationships.Checked;
                Preferences.Current.SaveToFile_IgnoreTimestamps = chkIgnoreTimestampsFile.Checked;
                Preferences.Current.SaveToFile_IgnoreInferredObjects = chkIgnoreInfObjectsFile.Checked;
                Preferences.Current.SaveToFile_IgnoreInferredProperties = chkIgnoreInfPropsFile.Checked;
                Preferences.Current.SaveToFile_SplitMultilineStrings = chkSplitMultilineFile.Checked;
                Preferences.Current.Copy_IncludeTranslations = chkCopyIncludeTranslations.Checked;
                Preferences.Current.Copy_IncludePerspectives = chkCopyIncludePerspectives.Checked;
                Preferences.Current.Copy_IncludeRLS = chkCopyIncludeRLS.Checked;
                Preferences.Current.Copy_IncludeOLS = chkCopyIncludeOLS.Checked;

                Preferences.Current.SaveToFolder_Levels = new HashSet<string>();
                SaveCheckedNodes(treeView1.Nodes, Preferences.Current.SaveToFolder_Levels);

                Preferences.Current.Save();
            }
        }

        private void SaveCheckedNodes(TreeNodeCollection nodes, ICollection<string> col)
        {
            foreach(TreeNode node in nodes)
            {
                if (node.Checked)
                {
                    col.Add(node.FullPath);
                    SaveCheckedNodes(node.Nodes, col);
                }
            }
        }

        private void LoadCheckedNodes(TreeNodeCollection nodes, ICollection<string> col)
        {
            foreach(TreeNode node in nodes)
            {
                if(col.Contains(node.FullPath))
                {
                    node.Checked = true;
                } else
                {
                    node.Checked = false;
                }
                LoadCheckedNodes(node.Nodes, col);
            }
        }

        private void treeView1_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (!e.Node.Checked)
                foreach (TreeNode child in e.Node.Nodes) child.Checked = false;
            if (e.Node.Checked)
            {
                TreeNode p = e.Node;
                while(p.Parent != null)
                {
                    p.Parent.Checked = true;
                    p = p.Parent;
                }
            }
        }

        Dictionary<string, bool> removedNodes = new Dictionary<string, bool>();

        private void SetNodeVisible(string nodeKey, bool visible)
        {
            if (treeView1.Nodes.ContainsKey(nodeKey))
            {
                if (!visible)
                {
                    removedNodes.Add(nodeKey, treeView1.Nodes[nodeKey].Checked);
                    treeView1.Nodes[nodeKey].Remove();
                }
            }
            else
            {
                if (visible)
                {
                    treeView1.Nodes.Add(nodeKey, nodeKey).Checked = removedNodes[nodeKey];
                    removedNodes.Remove(nodeKey);
                }
            }

            treeView1.Sort();
        }

        private void chkLocalPerspectives_CheckedChanged(object sender, EventArgs e)
        {
            SetNodeVisible("Perspectives", !chkLocalPerspectives.Checked);
        }

        private void chkLocalTranslations_CheckedChanged(object sender, EventArgs e)
        {
            SetNodeVisible("Translations", !chkLocalTranslations.Checked);
        }

        private void chkLocalRelationships_CheckedChanged(object sender, EventArgs e)
        {
            SetNodeVisible("Relationships", !chkLocalRelationships.Checked);
        }
    }
}
