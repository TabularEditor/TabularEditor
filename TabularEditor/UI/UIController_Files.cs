using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;
using TabularEditor.UIServices;

namespace TabularEditor.UI
{
    public partial class UIController
    {
        public void File_Open(string fileName)
        {
            File_Current = fileName;
            Handler = new TabularModelHandler(fileName);
            Handler.AutoFixup = Preferences.Current.FormulaFixup;
            LoadTabularModelToUI();
            RecentFiles.Add(fileName);
            UI.FormMain.PopulateRecentFilesList();
        }

        /// <summary>
        /// Call this method before calling File_Open() or Database_Connect() to check whether the
        /// currently loaded model has unsaved changes.
        /// </summary>
        /// <returns>True if the currently loaded model has unsaved changed.</returns>
        public bool DiscardChangesCheck()
        {
            if (Handler != null && Handler.HasUnsavedChanges)
            {
                if (MessageBox.Show("You have made changes to the model which have not yet been saved. Continue without saving changes?", "Unsaved changes", MessageBoxButtons.OKCancel, MessageBoxIcon.Question)
                    == DialogResult.Cancel) return true;
            }
            return false;
        }

        private void UndoManager_UndoActionAdded(object sender, EventArgs e)
        {
            UpdateUIText();
        }

        string File_Current;

        public void File_Open(bool fromFolder = false)
        {
            if (DiscardChangesCheck()) return;

            var oldFile = File_Current;
            var oldHandler = Handler;

            string fileName;
            if(fromFolder)
            {
                var dlg = new FolderBrowserDialog();
                if (dlg.ShowDialog() == DialogResult.Cancel) return;
                fileName = dlg.SelectedPath;
            } else
            {
                if (UI.OpenBimDialog.ShowDialog() == DialogResult.Cancel) return;
                fileName = UI.OpenBimDialog.FileName;
            }

            try
            {
                File_Open(fileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error loading Model from disk", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Handler = oldHandler;
                File_Current = oldFile;
            }
        }

        public void File_SaveAs()
        {
            var res = UI.SaveBimDialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                UI.StatusLabel.Text = "Saving...";
                Handler.SaveFile(UI.SaveBimDialog.FileName);
                if (File_Current != null) File_Current = UI.SaveBimDialog.FileName;
                UpdateUIText();
            }
        }

        public void File_SaveToFolder()
        {
            using (var fbd = new FolderBrowserDialog())
            {
                var res = fbd.ShowDialog();
                if(res == DialogResult.OK && !string.IsNullOrWhiteSpace(fbd.SelectedPath))
                {
                    UI.StatusLabel.Text = "Saving...";
                    Handler.SaveToFolder(fbd.SelectedPath);
                    UpdateUIText();
                }
            }
        }

        public void Save()
        {
            UI.StatusLabel.Text = "Saving...";

            if (Handler.IsConnected)
            {
                Database_Save();
            }
            else
            {
                try
                {
                    if (Directory.Exists(File_Current))
                        Handler.SaveToFolder(File_Current);
                    else
                        Handler.SaveFile(File_Current);
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Could not save metadata to file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
            UpdateUIText();
        }
    }
}
