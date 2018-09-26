using TOM = Microsoft.AnalysisServices.Tabular;
using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;
using TabularEditor.UIServices;
using TabularEditor.TOMWrapper.Serialization;

namespace TabularEditor.UI.Dialogs
{
    public partial class PreferencesForm : Form
    {
        public PreferencesForm()
        {
            InitializeComponent();
            chkCopyIncludeOLS.Enabled = true;
        }

        private void SetCurrentModelTabVisible(bool visible)
        {
            if (visible && !tabControl1.TabPages.Contains(tabCurrentModel)) tabControl1.TabPages.Add(tabCurrentModel);
            else if (!visible && tabControl1.TabPages.Contains(tabCurrentModel)) tabControl1.TabPages.Remove(tabCurrentModel);
        }

        TabularModelHandler Handler;

        public DialogResult Show(TabularModelHandler handler)
        {
            Handler = handler;

            LoadSettings();

            var showCm = handler?.HasSerializeOptions ?? false;
            SetCurrentModelTabVisible(showCm);
            if (showCm) LoadSettingsCM();

            return ShowDialog();
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

        private void LoadSettingsCM()
        {
            var options = Handler.SerializeOptions;

            chkIgnoreTimestampsCM.Checked = options.IgnoreTimestamps;
            chkIgnoreInfObjectsCM.Checked = options.IgnoreInferredObjects;
            chkIgnoreInfPropsCM.Checked = options.IgnoreInferredProperties;
            chkSplitMultilineCM.Checked = options.SplitMultilineStrings;

            LoadCheckedNodes(treeView2.Nodes, options.Levels);
            chkPrefixFilesCM.Checked = options.PrefixFilenames;
            chkLocalPerspectivesCM.Checked = options.LocalPerspectives;
            chkLocalTranslationsCM.Checked = options.LocalTranslations;
            chkLocalRelationshipsCM.Checked = options.LocalRelationships;
            SetNodeVisible("Perspectives", !chkLocalPerspectivesCM.Checked, treeView2);
            SetNodeVisible("Translations", !chkLocalTranslationsCM.Checked, treeView2);
            SetNodeVisible("Relationships", !chkLocalRelationshipsCM.Checked, treeView2);
        }

        private void SaveSettings()
        {
            Preferences.Current.BackupLocation = chkAutoBackup.Checked ? txtBackupPath.Text : string.Empty;
            Preferences.Current.CheckForUpdates = chkAutoUpdate.Checked;
            Preferences.Current.FormulaFixup = chkFixup.Checked;
            Preferences.Current.AllowUnsupportedPBIFeatures = chkAllowUnsupportedPBIFeatures.Checked;

            Preferences.Current.IgnoreTimestamps = chkIgnoreTimestamps.Checked;
            Preferences.Current.IgnoreInferredObjects = chkIgnoreInfObjects.Checked;
            Preferences.Current.IgnoreInferredProperties = chkIgnoreInfProps.Checked;
            Preferences.Current.SplitMultilineStrings = chkSplitMultiline.Checked;
            Preferences.Current.SaveToFolder_PrefixFiles = chkPrefixFiles.Checked;
            Preferences.Current.SaveToFolder_LocalPerspectives = chkLocalPerspectives.Checked;
            Preferences.Current.SaveToFolder_LocalTranslations = chkLocalTranslations.Checked;
            Preferences.Current.SaveToFolder_LocalRelationships = chkLocalRelationships.Checked;
            Preferences.Current.Copy_IncludeTranslations = chkCopyIncludeTranslations.Checked;
            Preferences.Current.Copy_IncludePerspectives = chkCopyIncludePerspectives.Checked;
            Preferences.Current.Copy_IncludeRLS = chkCopyIncludeRLS.Checked;
            Preferences.Current.Copy_IncludeOLS = chkCopyIncludeOLS.Checked;

            Preferences.Current.SaveToFolder_Levels = new HashSet<string>();
            SaveCheckedNodes(treeView1.Nodes, Preferences.Current.SaveToFolder_Levels);
        }

        private void SaveSettingsCM()
        {
            var options = new SerializeOptions();

            options.IgnoreTimestamps = chkIgnoreTimestampsCM.Checked;
            options.IgnoreInferredObjects = chkIgnoreInfObjectsCM.Checked;
            options.IgnoreInferredProperties = chkIgnoreInfPropsCM.Checked;
            options.SplitMultilineStrings = chkSplitMultilineCM.Checked;
            options.PrefixFilenames = chkPrefixFilesCM.Checked;
            options.LocalPerspectives = chkLocalPerspectivesCM.Checked;
            options.LocalTranslations = chkLocalTranslationsCM.Checked;
            options.LocalRelationships = chkLocalRelationshipsCM.Checked;
            options.Levels = new HashSet<string>();
            SaveCheckedNodes(treeView2.Nodes, options.Levels);

            if(Handler.SerializeOptions != options) Handler.SerializeOptions = options;
        }

        private void LoadSettings()
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

            chkIgnoreTimestamps.Checked = Preferences.Current.IgnoreTimestamps;
            chkIgnoreInfObjects.Checked = Preferences.Current.IgnoreInferredObjects;
            chkIgnoreInfProps.Checked = Preferences.Current.IgnoreInferredProperties;
            chkSplitMultiline.Checked = Preferences.Current.SplitMultilineStrings;

            chkPrefixFiles.Checked = Preferences.Current.SaveToFolder_PrefixFiles;
            chkLocalPerspectives.Checked = Preferences.Current.SaveToFolder_LocalPerspectives;
            chkLocalTranslations.Checked = Preferences.Current.SaveToFolder_LocalTranslations;
            chkLocalRelationships.Checked = Preferences.Current.SaveToFolder_LocalRelationships;
            SetNodeVisible("Perspectives", !chkLocalPerspectives.Checked, treeView1);
            SetNodeVisible("Translations", !chkLocalTranslations.Checked, treeView1);
            SetNodeVisible("Relationships", !chkLocalRelationships.Checked, treeView1);
        }

        private void PreferencesForm_Shown(object sender, EventArgs e)
        {
        }

        private void PreferencesForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if(DialogResult == DialogResult.OK)
            {
                SaveSettings();

                Preferences.Current.Save();

                if (tabControl1.TabPages.Contains(tabCurrentModel)) SaveSettingsCM();
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

        private void SetNodeVisible(string nodeKey, bool visible, TreeView treeView)
        {
            if (treeView.Nodes.ContainsKey(nodeKey))
            {
                if (!visible)
                {
                    removedNodes.Add(nodeKey, treeView.Nodes[nodeKey].Checked);
                    treeView1.Nodes[nodeKey].Remove();
                }
            }
            else
            {
                if (visible)
                {
                    treeView.Nodes.Add(nodeKey, nodeKey).Checked = removedNodes[nodeKey];
                    removedNodes.Remove(nodeKey);
                }
            }

            treeView.Sort();
        }

        private void chkLocalPerspectives_CheckedChanged(object sender, EventArgs e)
        {
            SetNodeVisible("Perspectives", !chkLocalPerspectives.Checked, treeView1);
        }

        private void chkLocalTranslations_CheckedChanged(object sender, EventArgs e)
        {
            SetNodeVisible("Translations", !chkLocalTranslations.Checked, treeView1);
        }

        private void chkLocalRelationships_CheckedChanged(object sender, EventArgs e)
        {
            SetNodeVisible("Relationships", !chkLocalRelationships.Checked, treeView1);
        }

        private void chkLocalPerspectivesCM_CheckedChanged(object sender, EventArgs e)
        {
            SetNodeVisible("Perspectives", !chkLocalPerspectivesCM.Checked, treeView2);
        }

        private void chkLocalTranslationsCM_CheckedChanged(object sender, EventArgs e)
        {
            SetNodeVisible("Translations", !chkLocalTranslationsCM.Checked, treeView2);
        }

        private void chkLocalRelationshipsCM_CheckedChanged(object sender, EventArgs e)
        {
            SetNodeVisible("Relationships", !chkLocalRelationshipsCM.Checked, treeView2);
        }

        private void treeView2_AfterCheck(object sender, TreeViewEventArgs e)
        {
            if (!e.Node.Checked)
                foreach (TreeNode child in e.Node.Nodes) child.Checked = false;
            if (e.Node.Checked)
            {
                TreeNode p = e.Node;
                while (p.Parent != null)
                {
                    p.Parent.Checked = true;
                    p = p.Parent;
                }
            }
        }
    }
}
