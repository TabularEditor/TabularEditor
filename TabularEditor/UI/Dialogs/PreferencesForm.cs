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
using System.Diagnostics;

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
            lblCurrentVersion.Text = "Current Version: " + UpdateService.CurrentBuild;
        }

        private bool tmpProxyUseSystem;
        private string tmpProxyAddress;
        private string tmpProxyUser;
        private string tmpProxyPassword;

        private void SaveProxySettings()
        {
            tmpProxyUseSystem = Preferences.Current.ProxyUseSystem;
            tmpProxyAddress = Preferences.Current.ProxyAddress;
            tmpProxyUser = Preferences.Current.ProxyUser;
            tmpProxyPassword = Preferences.Current.ProxyPasswordEncrypted;

            Preferences.Current.ProxyUseSystem = chkSystemProxy.Checked;
            Preferences.Current.ProxyAddress = txtProxyAddress.Text;
            Preferences.Current.ProxyUser = txtProxyUser.Text;
            Preferences.Current.ProxyPasswordEncrypted = txtProxyPassword.Text.Encrypt();
        }
        private void RestoreProxySettings()
        {
            Preferences.Current.ProxyUseSystem = tmpProxyUseSystem;
            Preferences.Current.ProxyAddress = tmpProxyAddress;
            Preferences.Current.ProxyUser = tmpProxyUser;
            Preferences.Current.ProxyPasswordEncrypted = tmpProxyPassword;
        }

        private void btnVersionCheck_Click(object sender, EventArgs e)
        {
            ProxyCache.ClearProxyCache();
            SaveProxySettings();
            var newVersion = UpdateService.Check(true);
            RestoreProxySettings();

            if (newVersion == VersionCheckResult.Unknown) return;
            if(newVersion == VersionCheckResult.NoNewVersion)
            {
                MessageBox.Show("You are currently using the latest version of Tabular Editor.", "No updates available", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                btnVersionCheck.Visible = false;
                lblAvailableVersion.Visible = true;
                lblAvailableVersion.Text = "Available Version: " + UpdateService.AvailableBuild + " (click to download)";
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

        private void chkAutoUpdate_CheckedChanged(object sender, EventArgs e)
        {
            chkSkipPatch.Enabled = chkAutoUpdate.Checked;
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
            Preferences.Current.SkipPatchUpdates = chkSkipPatch.Checked;
            Preferences.Current.FormulaFixup = chkFixup.Checked;
            Preferences.Current.AllowUnsupportedPBIFeatures = chkAllowUnsupportedPBIFeatures.Checked;
            Preferences.Current.ChangeDetectionOnLocalServers = chkChangeDetectionLocalServer.Checked;
            Preferences.Current.UseSemicolonsAsSeparators = cmbSeparators.SelectedIndex == 1;
            Preferences.Current.BackgroundBpa = chkBackgroundBpa.Checked;
            Preferences.Current.AnnotateDeploymentMetadata = chkAnnotateDeploymentMetadata.Checked;

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

            Preferences.Current.ProxyUseSystem = chkSystemProxy.Checked;
            Preferences.Current.ProxyAddress = txtProxyAddress.Text;
            Preferences.Current.ProxyUser = txtProxyUser.Text;
            Preferences.Current.ProxyPasswordEncrypted = txtProxyPassword.Text.Encrypt();

            Preferences.Current.ScriptCompilerOptions = txtCompilerOptions.Text;
            Preferences.Current.ScriptCompilerDirectoryPath = txtCompilerPath.Text;
            
            ProxyCache.ClearProxyCache();

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
            if (UpdateService.AvailableVersion.UpdateAvailable())
            {
                btnVersionCheck.Visible = false;
                lblAvailableVersion.Visible = true;
                lblAvailableVersion.Text = "Available Version: " + UpdateService.AvailableBuild + " (click to download)";
            }

            chkAutoBackup.Checked = Preferences.Current.BackupOnSave;
            txtBackupPath.Text = Preferences.Current.BackupLocation;

            chkAutoUpdate.Checked = Preferences.Current.CheckForUpdates;
            chkSkipPatch.Checked = Preferences.Current.SkipPatchUpdates;
            chkSkipPatch.Enabled = chkAutoUpdate.Checked;

            chkFixup.Checked = Preferences.Current.FormulaFixup;
            cmbSeparators.SelectedIndex = Preferences.Current.UseSemicolonsAsSeparators ? 1 : 0;
            chkAllowUnsupportedPBIFeatures.Checked = Preferences.Current.AllowUnsupportedPBIFeatures;
            chkChangeDetectionLocalServer.Checked = Preferences.Current.ChangeDetectionOnLocalServers;
            chkBackgroundBpa.Checked = Preferences.Current.BackgroundBpa;

            chkCopyIncludeTranslations.Checked = Preferences.Current.Copy_IncludeTranslations;
            chkCopyIncludePerspectives.Checked = Preferences.Current.Copy_IncludePerspectives;
            chkCopyIncludeRLS.Checked = Preferences.Current.Copy_IncludeRLS;
            chkCopyIncludeOLS.Checked = Preferences.Current.Copy_IncludeOLS;

            chkAnnotateDeploymentMetadata.Checked = Preferences.Current.AnnotateDeploymentMetadata;

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

            chkSystemProxy.Checked = Preferences.Current.ProxyUseSystem;
            txtProxyAddress.Text = Preferences.Current.ProxyAddress;
            txtProxyUser.Text = Preferences.Current.ProxyUser;
            txtProxyPassword.Text = Preferences.Current.ProxyPasswordEncrypted.Decrypt();

            txtCompilerOptions.Text = Preferences.Current.ScriptCompilerOptions;
            txtCompilerPath.Text = Preferences.Current.ScriptCompilerDirectoryPath;

            UpdateProxyUI();

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

        Dictionary<string, bool> removedNodes1 = new Dictionary<string, bool>();
        Dictionary<string, bool> removedNodes2 = new Dictionary<string, bool>();

        private void SetNodeVisible(string nodeKey, bool visible, TreeView treeView)
        {
            var removedNodes = treeView == treeView1 ? removedNodes1 : removedNodes2;

            if (treeView.Nodes.ContainsKey(nodeKey))
            {
                if (!visible)
                {
                    removedNodes.Add(nodeKey, treeView.Nodes[nodeKey].Checked);
                    treeView.Nodes[nodeKey].Remove();
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

        private void chkSystemProxy_CheckedChanged(object sender, EventArgs e)
        {
            UpdateProxyUI();
        }

        private void UpdateProxyUI()
        {
            txtProxyAddress.Enabled = !chkSystemProxy.Checked;
            txtProxyUser.Enabled = !chkSystemProxy.Checked;
            txtProxyPassword.Enabled = !chkSystemProxy.Checked;
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            Process.Start("https://docs.tabulareditor.com/Advanced-Scripting.html#compiling-with-roslyn");
        }
    }
}
