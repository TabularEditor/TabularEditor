using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs.Controls;
using TabularEditor.UI;
using TabularEditor.UI.Dialogs;
using TabularEditor.UIServices;

namespace TabularEditor.BestPracticeAnalyzer
{
    public partial class BPAManagerAddCollectionDialog : Form
    {
        public BPAManagerAddCollectionDialog()
        {
            InitializeComponent();
        }

        Analyzer analyzer;
        Form parent;

        static public bool Show(Analyzer analyzer, Form parent)
        {
            var form = new BPAManagerAddCollectionDialog();
            form.analyzer = analyzer;
            form.parent = parent;
            
            if (form.ShowDialog(parent) == DialogResult.Cancel) return false;

            if (form.rdbNewFile.Checked) return form.NewFile();
            if (form.rdbLocalFile.Checked) return form.LocalFile();
            if (form.rdbUrl.Checked) return form.Url();

            return true;
        }

        public bool NewFile()
        {
            var sfd = new CommonSaveFileDialog("New Rule File");
            sfd.EnsurePathExists = true;
            sfd.InitialDirectory = analyzer.BasePath;
            sfd.DefaultFileName = "BPARules.json";
            sfd.DefaultExtension = "json";
            sfd.Filters.Add(new CommonFileDialogFilter("JSON file", "*.json"));
            sfd.Filters.Add(new CommonFileDialogFilter("All files", "*.*"));

            var localPathCheckBox = new CommonFileDialogCheckBox("Use relative path", true) { Visible = false };
            if(UIController.Current.File_Current != null)
            {
                sfd.Controls.Add(localPathCheckBox);
                localPathCheckBox.Visible = true;
                sfd.FolderChanging += (s, e) =>
                {
                    var relativePath = FileSystemHelper.GetRelativePath(analyzer.BasePath, e.Folder);
                    if(relativePath.Length >= 2 && relativePath[1] == ':')
                    {
                        localPathCheckBox.Visible = false;
                    } else
                    {
                        localPathCheckBox.Visible = true;
                    }
                };
            }

            parent.Enabled = false;
            if(sfd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                parent.Enabled = true;

                try
                {
                    File.WriteAllText(sfd.FileName, "[]");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Unable to create rule file", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                var fileName = localPathCheckBox.Visible && localPathCheckBox.IsChecked
                    ? FileSystemHelper.GetRelativePath(analyzer.BasePath, sfd.FileName) : sfd.FileName;

                if (!analyzer.ExternalRuleCollections.Any(
                    rc => rc.FilePath != null && FileSystemHelper.GetAbsolutePath(analyzer.BasePath, rc.FilePath).EqualsI(sfd.FileName)
                    ))
                    analyzer.ExternalRuleCollections.Add(BestPracticeCollection.GetCollectionFromFile(analyzer.BasePath, fileName));
                return true;
            }
            parent.Enabled = true;
            return false;
        }
        public bool LocalFile()
        {
            var startDir = FileSystemHelper.DirectoryFromPath(UIController.Current.File_Current) ?? Environment.CurrentDirectory;

            var sfd = new CommonOpenFileDialog("Open Rule File");
            sfd.EnsureFileExists = true;
            sfd.InitialDirectory = startDir;
            sfd.DefaultExtension = "json";
            sfd.Filters.Add(new CommonFileDialogFilter("JSON file", "*.json"));
            sfd.Filters.Add(new CommonFileDialogFilter("All files", "*.*"));

            var localPathCheckBox = new CommonFileDialogCheckBox("Use relative path", true) { Visible = false };
            if (UIController.Current.File_Current != null)
            {
                sfd.Controls.Add(localPathCheckBox);
                localPathCheckBox.Visible = true;
                sfd.FolderChanging += (s, e) =>
                {
                    var relativePath = FileSystemHelper.GetRelativePath(startDir, e.Folder);
                    if (relativePath.Length >= 2 && relativePath[1] == ':')
                    {
                        localPathCheckBox.Visible = false;
                    }
                    else
                    {
                        localPathCheckBox.Visible = true;
                    }
                };
            }

            parent.Enabled = false;
            if (sfd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                parent.Enabled = true;
                var fileName = localPathCheckBox.Visible && localPathCheckBox.IsChecked
                    ? FileSystemHelper.GetRelativePath(startDir, sfd.FileName) : sfd.FileName;

                if (!analyzer.ExternalRuleCollections.Any(
                    rc => rc.FilePath != null && FileSystemHelper.GetAbsolutePath(analyzer.BasePath, rc.FilePath).EqualsI(sfd.FileName)
                    ))
                    analyzer.ExternalRuleCollections.Add(BestPracticeCollection.GetCollectionFromFile(analyzer.BasePath, fileName));
                return true;
            }
            parent.Enabled = true;
            return false;
        }
        public bool Url()
        {
            if (UrlInputDialog.Show(out string url))
            {
                if(!analyzer.ExternalRuleCollections.Any(rc => rc.Url != null && rc.Url.EqualsI(url)))
                    analyzer.ExternalRuleCollections.Add(BestPracticeCollection.GetCollectionFromUrl(url));
                return true;
            }
            else
                return false;
                
        }
    }
}
