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

        static public bool Show(Analyzer analyzer)
        {
            var form = new BPAManagerAddCollectionDialog();
            form.analyzer = analyzer;

            if (form.ShowDialog() == DialogResult.Cancel) return false;

            if (form.rdbNewFile.Checked) return form.NewFile();
            if (form.rdbLocalFile.Checked) return form.LocalFile();
            if (form.rdbUrl.Checked) return form.Url();

            return true;
        }

        public bool NewFile()
        {
            var currentFile = UIController.Current.File_Current;
            var startDir = Environment.CurrentDirectory;
            if (currentFile != null) {
                var attr = File.GetAttributes(currentFile);
                if (attr.HasFlag(FileAttributes.Directory))
                    startDir = currentFile;
                else
                    startDir = (new FileInfo(currentFile)).DirectoryName;
            }
            
            var sfd = new CommonSaveFileDialog("New Rule File");
            sfd.EnsurePathExists = true;
            sfd.InitialDirectory = startDir;
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
                    var relativePath = FileSystemHelper.GetRelativePath(startDir, e.Folder);
                    if(relativePath.Length >= 2 && relativePath[1] == ':')
                    {
                        localPathCheckBox.Visible = false;
                    } else
                    {
                        localPathCheckBox.Visible = true;
                    }
                };
            }
            if( sfd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var fileName = localPathCheckBox.Visible && localPathCheckBox.IsChecked
                    ? FileSystemHelper.GetRelativePath(startDir, sfd.FileName) : sfd.FileName;
                if(!analyzer.ExternalRuleCollections.Any(rc => rc.FilePath.EqualsI(fileName)))
                    analyzer.ExternalRuleCollections.Add(BestPracticeCollection.GetCollectionFromFile(fileName));
                return true;
            }
            return false;
        }
        public bool LocalFile()
        {
            var currentFile = UIController.Current.File_Current;
            var startDir = Environment.CurrentDirectory;
            if (currentFile != null)
            {
                var attr = File.GetAttributes(currentFile);
                if (attr.HasFlag(FileAttributes.Directory))
                    startDir = currentFile;
                else
                    startDir = (new FileInfo(currentFile)).DirectoryName;
            }

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
            if (sfd.ShowDialog() == CommonFileDialogResult.Ok)
            {
                var fileName = localPathCheckBox.Visible && localPathCheckBox.IsChecked
                    ? FileSystemHelper.GetRelativePath(startDir, sfd.FileName) : sfd.FileName;
                if (!analyzer.ExternalRuleCollections.Any(rc => rc.FilePath.EqualsI(fileName)))
                    analyzer.ExternalRuleCollections.Add(BestPracticeCollection.GetCollectionFromFile(fileName));
                return true;
            }
            return false;
        }
        public bool Url()
        {
            throw new NotImplementedException();
        }
    }
}
