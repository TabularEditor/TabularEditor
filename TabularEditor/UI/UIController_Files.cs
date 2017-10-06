using Microsoft.WindowsAPICodePack.Dialogs;
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
            var oldFile = File_Current;
            var oldLastWrite = File_LastWrite;
            var oldHandler = Handler;

            using (new Hourglass())
            {
                try
                {
                    Handler = new TabularModelHandler(fileName, Preferences.Current.GetSettings());
                    File_Current = Handler.Source;
                    File_SaveMode = Handler.SourceType;
                    File_LastWrite = File_SaveMode == ModelSourceType.Folder ? GetLastDirChange(File_Current) : File.GetLastWriteTime(File_Current);

                    LoadTabularModelToUI();
                    RecentFiles.Add(fileName);
                    UI.FormMain.PopulateRecentFilesList();

                    // TODO: Use a FileSystemWatcher to watch for changes to the currently loaded file(s)
                    // and handle them appropriately. For now, we just store the LastWriteDate of the loaded
                    // file, to ensure that we don't accidentally overwrite newer changes when saving.
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error loading Model from disk", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Handler = oldHandler;
                    File_Current = oldFile;
                    File_LastWrite = oldLastWrite;
                }
            }
        }

        public void File_New()
        {
            var cl = 1200;
#if CL1400
            var res = MessageBox.Show("Do you want to create a Compatibility Level 1400 model?\n(No = 1200, Yes = 1400).", "Compatibility Level", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            if (res == DialogResult.Cancel) return;
            if (res == DialogResult.Yes) cl = 1400;
#endif

            Handler = new TabularModelHandler(cl, Preferences.Current.GetSettings());
            File_Current = null;
            File_SaveMode = Handler.SourceType;

            LoadTabularModelToUI();
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

            string fileName;
            if(fromFolder)
            {
                using (var dlg = new CommonOpenFileDialog() { IsFolderPicker = true })
                {
                    if (dlg.ShowDialog() == CommonFileDialogResult.Cancel) return;
                    fileName = dlg.FileName;
                }
            } else
            {
                if (UI.OpenBimDialog.ShowDialog() == DialogResult.Cancel) return;
                fileName = UI.OpenBimDialog.FileName;
            }

            File_Open(fileName);
        }

        public void File_SaveAs()
        {
            var res = UI.SaveBimDialog.ShowDialog();

            if (res == DialogResult.OK)
            {
                using (new Hourglass())
                {
                    UI.StatusLabel.Text = "Saving...";
                    Handler.Save(UI.SaveBimDialog.FileName, SaveFormat.ModelSchemaOnly, Preferences.Current.GetSerializeOptions(false));

                    RecentFiles.Add(UI.SaveBimDialog.FileName);
                    UI.FormMain.PopulateRecentFilesList();

                    // If not connected to a database, change the current working file:
                    if (Handler.SourceType != ModelSourceType.Database)
                    {
                        File_Current = UI.SaveBimDialog.FileName;
                        File_SaveMode = ModelSourceType.File;
                    }

                    UpdateUIText();
                }
            }
        }

        public ModelSourceType File_SaveMode { get; private set; }
        public DateTime File_LastWrite { get; private set; }

        public void File_SaveAs_ToFolder()
        {
            using (var fbd = new CommonOpenFileDialog() { IsFolderPicker = true })
            {
                var res = fbd.ShowDialog();
                if(res == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(fbd.FileName))
                {
                    UI.StatusLabel.Text = "Saving...";
                    Handler.Save(fbd.FileName, SaveFormat.TabularEditorFolder, Preferences.Current.GetSerializeOptions(true));

                    RecentFiles.Add(fbd.FileName);
                    UI.FormMain.PopulateRecentFilesList();

                    // If working with a file, change the current file pointer:
                    if (Handler.SourceType != ModelSourceType.Database)
                    {
                        File_SaveMode = ModelSourceType.Folder;
                        File_Current = fbd.FileName;
                    }

                    UpdateUIText();
                }
            }
        }

        public void Save()
        {
            if(File_Current == null && File_SaveMode == ModelSourceType.File)
            {
                File_SaveAs();
                return;
            }

            UI.StatusLabel.Text = "Saving...";

            if (File_SaveMode == ModelSourceType.Database)
            {
                Database_Save();
            }
            else
            {
                try
                {
                    DialogResult mr = DialogResult.OK;
                    if (File_SaveMode == ModelSourceType.Folder)
                    {
                        if (GetLastDirChange(File_Current, File_LastWrite) > File_LastWrite)
                        {
                            mr = MessageBox.Show(
                                "Changes were made to the currently loaded folder structure after the model was loaded in Tabular Editor. Overwrite these changes?", "Overwriting folder structure changes", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                        }
                        if (mr == DialogResult.OK)
                        {
                            Handler.Save(File_Current, SaveFormat.TabularEditorFolder, Preferences.Current.GetSerializeOptions(true), true);
                            File_LastWrite = DateTime.Now;
                        }
                    }
                    else {
                        if (File.GetLastWriteTime(File_Current) > File_LastWrite)
                        {
                            mr = MessageBox.Show(
                                "Changes were made to the currently loaded file after the model was loaded in Tabular Editor. Overwrite these changes?", "Overwriting file changes", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                        }
                        if (mr == DialogResult.OK)
                        {
#if CL1400
                            if (File_SaveMode == ModelSourceType.Pbit)
                                Handler.Save(File_Current, SaveFormat.PowerBiTemplate, SerializeOptions.PowerBi);
                            else
#endif
                                Handler.Save(File_Current, SaveFormat.ModelSchemaOnly, Preferences.Current.GetSerializeOptions(false), true);
                            File_LastWrite = DateTime.Now;
                        }
                    }
                }
                catch (Exception e)
                {
                    MessageBox.Show(e.Message, "Could not save metadata to file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                
            }
            UpdateUIText();
        }

        private DateTime GetLastDirChange(string path)
        {
            return GetLastDirChange(path, DateTime.MaxValue);
        }

        private DateTime GetLastDirChange(string path, DateTime anyAfter)
        {
            var dirs = Directory.EnumerateFiles(path, "*.json", SearchOption.AllDirectories);
            var maxSoFar = DateTime.MinValue;
            foreach(var dir in dirs)
            {
                var dt = Directory.GetLastWriteTime(dir);
                if (dt > maxSoFar) maxSoFar = dt;
                if (maxSoFar > anyAfter) break;
            }
            return maxSoFar;
        }
    }
}
