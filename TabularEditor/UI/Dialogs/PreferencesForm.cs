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
using Microsoft.AnalysisServices.Tabular.Tmdl;
using Microsoft.AnalysisServices.Tabular.Serialization;
using static System.Windows.Forms.Design.AxImporter;

namespace TabularEditor.UI.Dialogs
{
    public partial class PreferencesForm : Form
    {
        public PreferencesForm()
        {
            InitializeComponent();
            chkCopyIncludeOLS.Enabled = true;
            cmbSerializationMode.SelectionChangeCommitted += CmbSerializationMode_SelectionChangeCommitted;
            cmbIndentMode.SelectionChangeCommitted += CmbIndentMode_SelectionChangeCommitted;
            cmbIndentModeCM.SelectionChangeCommitted += CmbIndentModeCM_SelectionChangeCommitted;
        }

        private void CmbIndentModeCM_SelectionChangeCommitted(object sender, EventArgs e)
        {
            txtIndentLevelCM.Enabled = cmbIndentModeCM.SelectedIndex == 0;
        }

        private void CmbIndentMode_SelectionChangeCommitted(object sender, EventArgs e)
        {
            txtIndentLevel.Enabled = cmbIndentMode.SelectedIndex == 0;
        }

        private bool tmdlWarningShown = false;

        private void UpdateTmdlUi()
        {
            var useLegacy = cmbSerializationMode.SelectedIndex == 0;
            grpSaveToFolderOptions.Visible = useLegacy;
            chkPrefixFiles.Enabled = useLegacy;
            chkLocalPerspectives.Enabled = useLegacy;
            chkLocalRelationships.Enabled = useLegacy;
            chkLocalTranslations.Enabled = useLegacy;
            tvDefaultSerialization.Enabled = useLegacy;

            grpTmdlOptions.Visible = !useLegacy;
        }

        private void CmbSerializationMode_SelectionChangeCommitted(object sender, EventArgs e)
        {
            if(cmbSerializationMode.SelectedIndex == 1 && !tmdlWarningShown)
            {
                var td = new TaskDialog();
                td.HyperlinksEnabled = true;
                td.Caption = "TMDL";
                td.Text = "TMDL (Tabular Model Definition Language)\n\nUse TMDL as the default Save to Folder format in Tabular Editor?";
                td.Icon = TaskDialogStandardIcon.Information;
                td.FooterIcon = TaskDialogStandardIcon.None;
                td.HyperlinkClick += Td_HyperlinkClick;
                td.StandardButtons = TaskDialogStandardButtons.Ok | TaskDialogStandardButtons.Cancel;
                var result = td.Show();
                tmdlWarningShown = true;
                if (result == TaskDialogResult.Cancel) cmbSerializationMode.SelectedIndex = 0;
            }

            UpdateTmdlUi();
        }

        private void Td_HyperlinkClick(object sender, TaskDialogHyperlinkClickedEventArgs e)
        {
            Process.Start(e.LinkText);
        }

        private void SetCurrentModelTabVisible(bool visible)
        {
            if (visible && !tabControl1.TabPages.Contains(tabCurrentModel)) tabControl1.TabPages.Add(tabCurrentModel);
            else if (!visible && tabControl1.TabPages.Contains(tabCurrentModel)) tabControl1.TabPages.Remove(tabCurrentModel);
        }

        TabularModelHandler Handler;

        private bool CurrentModelIsTmdl => Handler?.SourceType == ModelSourceType.TMDL;

        public DialogResult Show(TabularModelHandler handler)
        {
            Handler = handler;

            LoadSettings();

            var showCm = handler != null && (handler.HasSerializeOptions || !handler.IsConnected);
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

            chkIgnoreLineageTagsCM.Checked = options.IgnoreLineageTags;
            chkIgnoreTimestampsCM.Checked = options.IgnoreTimestamps;
            chkIgnoreInfObjectsCM.Checked = options.IgnoreInferredObjects;
            chkIgnoreInfPropsCM.Checked = options.IgnoreInferredProperties;
            chkSplitMultilineCM.Checked = options.SplitMultilineStrings;
            chkIgnorePrivacySettingsCM.Checked = options.IgnorePrivacySettings;
            chkIgnoreIncrementalRefreshPartitionsCM.Checked = options.IgnoreIncrementalRefreshPartitions;
            chkIncludeSensitiveCM.Checked = options.IncludeSensitive;

            if (CurrentModelIsTmdl)
            {
                grpTmdlOptionsCM.Visible = true;
                grpSaveToFolder.Visible = false;

                chkIncludeRefsCM.Checked = options.TmdlOptions.IncludeRefs;
                cmbIndentModeCM.SelectedIndex = options.TmdlOptions.SpacesIndentation > 0 ? 0 : 1;
                txtIndentLevelCM.Value = options.TmdlOptions.SpacesIndentation > 0 ? options.TmdlOptions.SpacesIndentation : 4;
                txtIndentLevelCM.Enabled = options.TmdlOptions.SpacesIndentation > 0;
                cmbEncodingCM.SelectedIndex = (int)options.TmdlOptions.Encoding;
                cmbCasingStyleCM.SelectedIndex = options.TmdlOptions.CasingStyle switch { TmdlCasingStyle.CamelCase => 0, TmdlCasingStyle.Pascalcase => 1, _ => 2 };
                cmbExpressionTrimStyleCM.SelectedIndex = options.TmdlOptions.ExpressionTrimStyle switch { TmdlExpressionTrimStyle.NoTrim => 0, TmdlExpressionTrimStyle.TrimTrailingWhitespaces => 1, _ => 2 };
                cmbNewLineStyleCM.SelectedIndex = options.TmdlOptions.NewLineStyle switch { NewLineStyle.SystemDefault => 0, NewLineStyle.WindowsStyle => 1, _ => 2 };
            }
            else
            {
                grpSaveToFolder.Visible = true;
                grpTmdlOptionsCM.Visible = false;

                LoadCheckedNodes(treeView2.Nodes, options.Levels);
                chkPrefixFilesCM.Checked = options.PrefixFilenames;
                chkAlsoSaveAsBimCM.Checked = options.AlsoSaveAsBim;
                chkLocalPerspectivesCM.Checked = options.LocalPerspectives;
                chkLocalTranslationsCM.Checked = options.LocalTranslations;
                chkLocalRelationshipsCM.Checked = options.LocalRelationships;
                SetNodeVisible("Perspectives", !chkLocalPerspectivesCM.Checked, treeView2);
                SetNodeVisible("Translations", !chkLocalTranslationsCM.Checked, treeView2);
                SetNodeVisible("Relationships", !chkLocalRelationshipsCM.Checked, treeView2);
            }
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

            Preferences.Current.IgnoreLineageTags = chkIgnoreLineageTags.Checked;
            Preferences.Current.IgnoreTimestamps = chkIgnoreTimestamps.Checked;
            Preferences.Current.IgnoreInferredObjects = chkIgnoreInfObjects.Checked;
            Preferences.Current.IgnoreInferredProperties = chkIgnoreInfProps.Checked;
            Preferences.Current.SplitMultilineStrings = chkSplitMultiline.Checked;
            Preferences.Current.IgnorePrivacySettings = chkIgnorePrivacySettings.Checked;
            Preferences.Current.IncludeSensitive = chkIncludeSensitive.Checked;
            Preferences.Current.IgnoreIncrementalRefreshPartitions = chkIgnoreIncrementalRefreshPartitions.Checked;
            Preferences.Current.UseTMDL = cmbSerializationMode.SelectedIndex == 1;
            Preferences.Current.SaveToFolder_PrefixFiles = chkPrefixFiles.Checked;
            Preferences.Current.SaveToFolder_AlsoSaveAsBim = chkAlsoSaveAsBim.Checked;
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

            Preferences.Current.TmdlOptions = Preferences.Current.TmdlOptions with
            {

                IncludeRefs = chkIncludeRefs.Checked,
                SpacesIndentation = cmbIndentMode.SelectedIndex == 0 ? (int)txtIndentLevel.Value : 0,
                Encoding = (SerializationEncoding)cmbEncoding.SelectedIndex,
                CasingStyle = cmbCasingStyle.SelectedIndex switch { 0 => TmdlCasingStyle.CamelCase, 1 => TmdlCasingStyle.Pascalcase, _ => TmdlCasingStyle.LowerCase },
                ExpressionTrimStyle = cmbExpressionTrimStyle.SelectedIndex switch { 0 => TmdlExpressionTrimStyle.NoTrim, 1 => TmdlExpressionTrimStyle.TrimTrailingWhitespaces, _ => TmdlExpressionTrimStyle.TrimLeadingCommonWhitespaces },
                NewLineStyle = cmbNewLineStyle.SelectedIndex switch { 0 => NewLineStyle.SystemDefault, 1 => NewLineStyle.WindowsStyle, _ => NewLineStyle.UnixStyle }
            };

            ProxyCache.ClearProxyCache();

            Preferences.Current.SaveToFolder_Levels = new HashSet<string>();
            SaveCheckedNodes(tvDefaultSerialization.Nodes, Preferences.Current.SaveToFolder_Levels);
        }

        private void SaveSettingsCM()
        {
            var options = new SerializeOptions();

            options.IgnoreLineageTags = chkIgnoreLineageTagsCM.Checked;
            options.IgnoreTimestamps = chkIgnoreTimestampsCM.Checked;
            options.IgnoreInferredObjects = chkIgnoreInfObjectsCM.Checked;
            options.IgnoreInferredProperties = chkIgnoreInfPropsCM.Checked;
            options.SplitMultilineStrings = chkSplitMultilineCM.Checked;
            options.PrefixFilenames = chkPrefixFilesCM.Checked;
            options.AlsoSaveAsBim = chkAlsoSaveAsBimCM.Checked;
            options.LocalPerspectives = chkLocalPerspectivesCM.Checked;
            options.LocalTranslations = chkLocalTranslationsCM.Checked;
            options.LocalRelationships = chkLocalRelationshipsCM.Checked;
            options.IgnorePrivacySettings = chkIgnorePrivacySettingsCM.Checked;
            options.IncludeSensitive = chkIncludeSensitiveCM.Checked;
            options.IgnoreIncrementalRefreshPartitions = chkIgnoreIncrementalRefreshPartitionsCM.Checked;
            options.Levels = new HashSet<string>();
            SaveCheckedNodes(treeView2.Nodes, options.Levels);

            if(CurrentModelIsTmdl)
            {
                options.TmdlOptions = options.TmdlOptions with
                {

                    IncludeRefs = chkIncludeRefsCM.Checked,
                    SpacesIndentation = cmbIndentModeCM.SelectedIndex == 0 ? (int)txtIndentLevelCM.Value : 0,
                    Encoding = (SerializationEncoding)cmbEncodingCM.SelectedIndex,
                    CasingStyle = cmbCasingStyleCM.SelectedIndex switch { 0 => TmdlCasingStyle.CamelCase, 1 => TmdlCasingStyle.Pascalcase, _ => TmdlCasingStyle.LowerCase },
                    ExpressionTrimStyle = cmbExpressionTrimStyleCM.SelectedIndex switch { 0 => TmdlExpressionTrimStyle.NoTrim, 1 => TmdlExpressionTrimStyle.TrimTrailingWhitespaces, _ => TmdlExpressionTrimStyle.TrimLeadingCommonWhitespaces },
                    NewLineStyle = cmbNewLineStyleCM.SelectedIndex switch { 0 => NewLineStyle.SystemDefault, 1 => NewLineStyle.WindowsStyle, _ => NewLineStyle.UnixStyle }
                };
            }

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

            chkAutoUpdate.Checked = Preferences.Current.CheckForUpdates && !Policies.Instance.DisableUpdates;
            chkAutoUpdate.Enabled = !Policies.Instance.DisableUpdates;
            btnVersionCheck.Enabled = !Policies.Instance.DisableUpdates;
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

            LoadCheckedNodes(tvDefaultSerialization.Nodes, Preferences.Current.SaveToFolder_Levels);

            chkIgnoreLineageTags.Checked = Preferences.Current.IgnoreLineageTags;
            chkIgnoreTimestamps.Checked = Preferences.Current.IgnoreTimestamps;
            chkIgnoreInfObjects.Checked = Preferences.Current.IgnoreInferredObjects;
            chkIgnoreInfProps.Checked = Preferences.Current.IgnoreInferredProperties;
            chkSplitMultiline.Checked = Preferences.Current.SplitMultilineStrings;
            chkIgnorePrivacySettings.Checked = Preferences.Current.IgnorePrivacySettings;
            chkIncludeSensitive.Checked = Preferences.Current.IncludeSensitive;
            chkIgnoreIncrementalRefreshPartitions.Checked = Preferences.Current.IgnoreIncrementalRefreshPartitions;

            cmbSerializationMode.SelectedIndex = Preferences.Current.UseTMDL ? 1 : 0;
            UpdateTmdlUi();
            chkPrefixFiles.Checked = Preferences.Current.SaveToFolder_PrefixFiles;
            chkAlsoSaveAsBim.Checked = Preferences.Current.SaveToFolder_AlsoSaveAsBim;
            chkLocalPerspectives.Checked = Preferences.Current.SaveToFolder_LocalPerspectives;
            chkLocalTranslations.Checked = Preferences.Current.SaveToFolder_LocalTranslations;
            chkLocalRelationships.Checked = Preferences.Current.SaveToFolder_LocalRelationships;
            SetNodeVisible("Perspectives", !chkLocalPerspectives.Checked, tvDefaultSerialization);
            SetNodeVisible("Translations", !chkLocalTranslations.Checked, tvDefaultSerialization);
            SetNodeVisible("Relationships", !chkLocalRelationships.Checked, tvDefaultSerialization);

            chkSystemProxy.Checked = Preferences.Current.ProxyUseSystem;
            txtProxyAddress.Text = Preferences.Current.ProxyAddress;
            txtProxyUser.Text = Preferences.Current.ProxyUser;
            txtProxyPassword.Text = Preferences.Current.ProxyPasswordEncrypted.Decrypt();

            txtCompilerOptions.Text = Preferences.Current.ScriptCompilerOptions;
            txtCompilerPath.Text = Preferences.Current.ScriptCompilerDirectoryPath;

            chkIncludeRefs.Checked = Preferences.Current.TmdlOptions.IncludeRefs;
            cmbIndentMode.SelectedIndex = Preferences.Current.TmdlOptions.SpacesIndentation > 0 ? 0 : 1;
            txtIndentLevel.Value = Preferences.Current.TmdlOptions.SpacesIndentation > 0 ? Preferences.Current.TmdlOptions.SpacesIndentation : 4;
            txtIndentLevel.Enabled = Preferences.Current.TmdlOptions.SpacesIndentation > 0;
            cmbEncoding.SelectedIndex = (int)Preferences.Current.TmdlOptions.Encoding;
            cmbCasingStyle.SelectedIndex = Preferences.Current.TmdlOptions.CasingStyle switch { TmdlCasingStyle.CamelCase => 0, TmdlCasingStyle.Pascalcase => 1, _ => 2 };
            cmbExpressionTrimStyle.SelectedIndex = Preferences.Current.TmdlOptions.ExpressionTrimStyle switch { TmdlExpressionTrimStyle.NoTrim => 0, TmdlExpressionTrimStyle.TrimTrailingWhitespaces => 1, _ => 2 };
            cmbNewLineStyle.SelectedIndex = Preferences.Current.TmdlOptions.NewLineStyle switch { NewLineStyle.SystemDefault => 0, NewLineStyle.WindowsStyle => 1, _ => 2 };

            UpdateProxyUI();

        }

        private void PreferencesForm_Shown(object sender, EventArgs e)
        {
            tmdlWarningShown = false;
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
            var removedNodes = treeView == tvDefaultSerialization ? removedNodes1 : removedNodes2;

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
            SetNodeVisible("Perspectives", !chkLocalPerspectives.Checked, tvDefaultSerialization);
        }

        private void chkLocalTranslations_CheckedChanged(object sender, EventArgs e)
        {
            SetNodeVisible("Translations", !chkLocalTranslations.Checked, tvDefaultSerialization);
        }

        private void chkLocalRelationships_CheckedChanged(object sender, EventArgs e)
        {
            SetNodeVisible("Relationships", !chkLocalRelationships.Checked, tvDefaultSerialization);
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
