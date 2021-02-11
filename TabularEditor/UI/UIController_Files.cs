using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;
using TabularEditor.TOMWrapper.Serialization;
using TabularEditor.TOMWrapper.Utils;
using TabularEditor.UI.Dialogs;
using TabularEditor.UIServices;

namespace TabularEditor.UI
{
    public partial class UIController
    {
        public void File_Open(string fileName)
        {
            var oldFile = File_Current;
            var oldDirectory = File_Directory;
            var oldLastWrite = File_LastWrite;
            var oldSaveMode = File_SaveMode;
            var oldHandler = Handler;

            var cancel = false;

            using (new Hourglass())
            {
                try
                {
                    Handler = new TabularModelHandler(fileName, Preferences.Current.GetSettings());

                    if(Handler.SourceType == ModelSourceType.Pbit)
                    {
                        switch(Handler.PowerBIGovernance.GovernanceMode)
                        {
                            case TOMWrapper.PowerBI.PowerBIGovernanceMode.ReadOnly:
                                MessageBox.Show("Editing a Power BI Template (.pbit) file that does not use the Enhanced Model Metadata (V3) format is not allowed, unless you enable Experimental Power BI Features under File > Preferences.\n\nTabular Editor will still load the file in read-only mode.", "Power BI Template Read-only", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                break;
                            case TOMWrapper.PowerBI.PowerBIGovernanceMode.V3Restricted:
                                break;
                            case TOMWrapper.PowerBI.PowerBIGovernanceMode.Unrestricted:
                                if (MessageBox.Show("Experimental Power BI features is enabled. You can edit any object and property of this Power BI Template file but be aware that many types of changes ARE NOT CURRENTLY SUPPORTED by Microsoft.\n\nKeep a backup of your .pbit file and proceed at your own risk.", "Power BI Template Warning", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning) == DialogResult.Cancel)
                                    cancel = true;
                                break;
                        }
                    }

                    if (!cancel)
                    {
                        TabularModelHandler.Cleanup();
                        if (oldHandler != null && File_SaveMode == ModelSourceType.Database) oldHandler.OnExternalChange -= Handler_OnExternalChange;
                        File_Current = Handler.Source;
                        File_Directory = FileSystemHelper.DirectoryFromPath(Handler.Source);
                        File_SaveMode = Handler.SourceType;

                        // TODO: Use a FileSystemWatcher to watch for changes to the currently loaded file(s)
                        // and handle them appropriately. For now, we just store the LastWriteDate of the loaded
                        // file, to ensure that we don't accidentally overwrite newer changes when saving.
                        File_LastWrite = File_SaveMode == ModelSourceType.Folder ? GetLastDirChange(File_Current) : File.GetLastWriteTime(File_Current);

                        LoadTabularModelToUI();
                        RecentFiles.Add(fileName);
                        RecentFiles.Save();
                        UI.FormMain.PopulateRecentFilesList();
                    }
                }
                catch (Exception ex)
                {
                    cancel = true;
                    MessageBox.Show(ex.Message, "Error loading Model from disk", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }

            if(cancel)
            {
                Handler = oldHandler;
                File_Current = oldFile;
                File_Directory = oldDirectory;
                File_SaveMode = oldSaveMode;
                File_LastWrite = oldLastWrite;
            }
        }

        public void File_New()
        {
            var newModelDialog = new NewModelDialog();
            if (newModelDialog.ShowDialog() == DialogResult.Cancel) return;

            TabularModelHandler.Cleanup();
            if (Handler != null && File_SaveMode == ModelSourceType.Database) Handler.OnExternalChange -= Handler_OnExternalChange;
            Handler = new TabularModelHandler(newModelDialog.CompatibilityLevel, Preferences.Current.GetSettings(), newModelDialog.PbiDatasetModel);
            File_Current = null;
            File_Directory = null;
            File_SaveMode = Handler.SourceType;

            LoadTabularModelToUI();
        }

        private void FileNewCompatibilityLevel_Click(object sender, EventArgs e)
        {
            throw new NotImplementedException();
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

        public string File_Current { get; private set; }
        public string File_Directory { get; private set; } = null;

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

        private ModelSourceType _file_SaveMode;
        public ModelSourceType File_SaveMode
        {
            get { return _file_SaveMode; }
            private set
            {
                _file_SaveMode = value;

                var actSave = UI.FormMain.actSave;
                switch (_file_SaveMode)
                {
                    case TOMWrapper.ModelSourceType.Database:
                        actSave.Text = "&Save";
                        actSave.ToolTipText = "Saves the changes to the connected database (Ctrl+S)";
                        actSave.Image = Resources.SaveToDB;
                        break;
                    case TOMWrapper.ModelSourceType.File:
                        actSave.Text = "&Save";
                        actSave.ToolTipText = "Saves the changes back to the currently loaded .bim file (Ctrl+S)";
                        actSave.Image = Resources.SaveToFile;
                        break;
                    case TOMWrapper.ModelSourceType.Folder:
                        actSave.Text = "&Save";
                        actSave.ToolTipText = "Saves the changes back to the currently loaded model folder structure (Ctrl+S)";
                        actSave.Image = Resources.SaveFolderTree;
                        break;
                    case TOMWrapper.ModelSourceType.Pbit:
                        actSave.Text = "&Save";
                        actSave.ToolTipText = "Saves the changes back to the currently loaded Power BI Template (Ctrl+S)";
                        actSave.Image = Resources.SaveToPBI;
                        break;
                }
            }
        }
        public DateTime File_LastWrite { get; private set; }

        public void File_SaveAs()
        {
            ExpressionEditor_AcceptEdit();

            // Only show the "Use serialize options from annotations" checkbox when the current model has these annotations,
            // and not when switching between file/folder:
            var showSfa = Handler.HasSerializeOptions;

            // If the model is currently loaded from a database or a folder structure, use the default "Model.bim" as a file
            // name. Otherwise, use the name of the current file:
            var defaultFileName = (Handler.SourceType == ModelSourceType.Database || Handler.SourceType == ModelSourceType.Folder) ? "Model.bim" : File_Current;

            // We only allow saving as a Power BI Template, if the current model was loaded from a template:
            var allowPbit = Handler.SourceType == ModelSourceType.Pbit;

            // This flag indicates whether we're currently connected to a database:
            var connectedToDatabase = Handler.SourceType == ModelSourceType.Database;

            // This flag indicates whether the "Current File" pointer should be set to the new file location, which
            // is the typical behaviour of Windows applications when choosing "Save As...". However, when connected
            // to a database, we do not want to do this, as users will probably want to keep working on the existing
            // connection.
            var changeFilePointer = !connectedToDatabase;

            // This flag indicates whether we should reset the undo-checkpoint after saving the model.
            // The purpose of resetting the checkpoint is to visually indicate to the user, that no
            // changes have been made to the model since the last save. We do this, only when changing
            // the "Current File" pointer:
            var resetCheckPoint = changeFilePointer;

            // This flag indicates whether the SerializationOptions annotations on the currently loaded
            // model will be restored to its original value, after the model is saved (possibly using
            // different serialization options, depending on the other arguments of the Save() method).
            // We should always restore when connected to a database, as we don't want our serialization
            // options to be saved to the database - only to the file/folder that we're currently saving to.
            var restoreSerializationOptions = connectedToDatabase;

            // The serialization options to use when saving (unless users check the "Use serialization settings from annotations" checkbox):
            var serializationOptions = Preferences.Current.GetSerializeOptions();

            using (var dialog = SaveAsDialog.CreateFileDialog(showSfa, defaultFileName, allowPbit))
            {
                var res = dialog.ShowDialog();

                if (res == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    using (new Hourglass())
                    {
                        UI.StatusLabel.Text = "Saving...";

                        // Save as a Power BI Template, if that's the type of file chosen in the dialog:
                        var saveFormat = dialog.FileType == "pbit" ? SaveFormat.PowerBiTemplate : SaveFormat.ModelSchemaOnly;

                        var fileName = dialog.FileName;
                        if (dialog.FileType == "pbit" && !fileName.EndsWith(".pbit")) fileName += ".pbit";
                        else if (dialog.FileType == "bim" && !fileName.EndsWith(".bim")) fileName += ".bim";

                        try
                        {
                            Handler.Save(fileName,
                                saveFormat,
                                serializationOptions,
                                dialog.UseSerializationFromAnnotations,
                                resetCheckPoint,
                                restoreSerializationOptions);

                            RecentFiles.Add(fileName);
                            RecentFiles.Save();
                            UI.FormMain.PopulateRecentFilesList();

                            // If not connected to a database, change the current working file:
                            if (changeFilePointer)
                            {
                                File_Current = fileName;
                                File_Directory = FileSystemHelper.DirectoryFromPath(File_Current);
                                File_LastWrite = File.GetLastWriteTime(File_Current);
                                File_SaveMode = dialog.FileType == "pbit" ? ModelSourceType.Pbit : ModelSourceType.File;
                            }
                        }
                        catch (Exception e)
                        {
                            HandleError("Could not save metadata to file", e);
                        }

                        UpdateUIText();
                    }
                }
            }
        }

        public void File_SaveAs_ToFolder()
        {
            ExpressionEditor_AcceptEdit();

            // Save as a Folder structure:
            var saveFormat = SaveFormat.TabularEditorFolder;

            // This flag indicates whether we're currently connected to a database:
            var connectedToDatabase = Handler.SourceType == ModelSourceType.Database;

            // This flag indicates whether the "Current File" pointer should be set to the new file location, which
            // is the typical behaviour of Windows applications when choosing "Save As...". However, when connected
            // to a database, we do not want to do this, as users will probably want to keep working on the existing
            // connection.
            var changeFilePointer = !connectedToDatabase;

            // This flag indicates whether we should reset the undo-checkpoint after saving the model.
            // The purpose of resetting the checkpoint is to visually indicate to the user, that no
            // changes have been made to the model since the last save. We do this, only when changing
            // the "Current File" pointer:
            var resetCheckPoint = changeFilePointer;

            // This flag indicates whether the SerializationOptions annotations on the currently loaded
            // model will be restored to its original value, after the model is saved (possibly using
            // different serialization options, depending on the other arguments of the Save() method).
            // We should always restore when connected to a database, as we don't want our serialization
            // options to be saved to the database - only to the file/folder that we're currently saving to.
            var restoreSerializationOptions = connectedToDatabase;

            // The serialization options to use when saving (unless users check the "Use serialization settings from annotations" checkbox):
            var serializationOptions = Preferences.Current.GetSerializeOptions();

            // Only show the "Use serialize options from annotations" checkbox when the current model has these annotations:
            var showSfa = Handler.HasSerializeOptions;

            using (var dialog = SaveAsDialog.CreateFolderDialog(showSfa))
            {
                var res = dialog.ShowDialog();
                if(res == CommonFileDialogResult.Ok && !string.IsNullOrWhiteSpace(dialog.FileName))
                {
                    using (new Hourglass())
                    {
                        UI.StatusLabel.Text = "Saving...";

                        try
                        {
                            var orgDbName = Handler.Database.Name;
                            var orgDbId = Handler.Database.ID;
                            if (LocalInstanceType == EmbeddedInstanceType.PowerBI && LocalInstanceName.StartsWith("localhost") && Guid.TryParse(orgDbName, out _))
                            {
                                Handler.Database.Name = LocalInstanceName.Split('.').Skip(1).FirstOrDefault()?.Replace(".","_") ?? Handler.Database.Name;
                                //Handler.Database.ID = Handler.Database.Name;
                            }
                            Handler.Save(dialog.FileName,
                                saveFormat,
                                serializationOptions,
                                dialog.UseSerializationFromAnnotations,
                                resetCheckPoint,
                                restoreSerializationOptions);
                            Handler.Database.Name = orgDbName;
                            Handler.Database.Name = orgDbId;

                            RecentFiles.Add(dialog.FileName);
                            RecentFiles.Save();
                            UI.FormMain.PopulateRecentFilesList();

                            // If working with a file, change the current file pointer:
                            if (changeFilePointer)
                            {
                                File_SaveMode = ModelSourceType.Folder;
                                File_Current = dialog.FileName;
                                File_Directory = FileSystemHelper.DirectoryFromPath(File_Current);
                                File_LastWrite = GetLastDirChange(File_Current);
                            }
                        }
                        catch  (Exception e)
                        {
                            HandleError("Could not save metadata to folder", e);
                        }
                        UpdateUIText();
                    }
                }
            }
        }

        public void HandleError(string operationTitle, Exception e)
        {
            if (e is SerializationException || 
                e is Microsoft.AnalysisServices.Tabular.TomInternalException)
                MessageBox.Show("Tabular Editor was unable to serialize the model to disk.\n\n" + e.Message, operationTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            else
                throw e;
        }

        public void Save()
        {
            ExpressionEditor_AcceptEdit();

            if (File_Current == null && File_SaveMode == ModelSourceType.File)
            {
                File_SaveAs();
                return;
            }

            UI.StatusLabel.Text = "Saving...";
            using (new Hourglass())
            {
                switch (File_SaveMode)
                {
                    case ModelSourceType.Database:
                        Database_Save();
                        break;

                    case ModelSourceType.Folder:
                        if (GetLastDirChange(File_Current, File_LastWrite) > File_LastWrite)
                        {
                            var mr = MessageBox.Show("Changes were made to the currently loaded folder structure after the model was loaded in Tabular Editor. Overwrite these changes?", "Overwriting folder structure changes", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                            if (mr == DialogResult.Cancel) break;
                        }
                        try
                        {
                            Handler.Save(File_Current, SaveFormat.TabularEditorFolder, null, true, true);
                            File_LastWrite = GetLastDirChange(File_Current);
                        }
                        catch (Exception e)
                        {
                            HandleError("Could not save metadata to folder", e);
                        }
                        break;

                    case ModelSourceType.File:
                    case ModelSourceType.Pbit:
                        if (File.GetLastWriteTime(File_Current) > File_LastWrite)
                        {
                            var mr = MessageBox.Show("Changes were made to the currently loaded file after the model was loaded in Tabular Editor. Overwrite these changes?", "Overwriting file changes", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning);
                            if (mr == DialogResult.Cancel) break;
                        }
                        try
                        {
                            if (File_SaveMode == ModelSourceType.Pbit)
                                Handler.Save(File_Current, SaveFormat.PowerBiTemplate, SerializeOptions.PowerBi, false, true);
                            else
                                Handler.Save(File_Current, SaveFormat.ModelSchemaOnly, null, true, true);
                            File_LastWrite = File.GetLastWriteTime(File_Current);
                        }
                        catch (Exception e)
                        {
                            HandleError("Could not save metadata to file", e);
                        }
                        break;
                }
                
                UpdateUIText();
            }
        }

        private DateTime GetLastDirChange(string path)
        {
            return GetLastDirChange(path, DateTime.MaxValue);
        }

        private DateTime GetLastDirChange(string path, DateTime anyAfter)
        {
            var dirs = Directory.EnumerateFiles(FileSystemHelper.DirectoryFromPath(path), "*.json", SearchOption.AllDirectories);
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
