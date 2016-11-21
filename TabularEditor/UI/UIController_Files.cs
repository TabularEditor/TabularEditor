using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;

namespace TabularEditor.UI
{
    public partial class UIController
    {
        public void File_Open(string fileName)
        {
            File_Current = fileName;
            Handler = new TabularModelHandler(fileName);
            LoadTabularModelToUI();
        }

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

        public void File_Open()
        {
            if (DiscardChangesCheck()) return;

            var oldFile = File_Current;
            var oldHandler = Handler;

            var res = UI.OpenBimDialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                try
                {
                    File_Open(UI.OpenBimDialog.FileName);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Error loading file", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    Handler = oldHandler;
                    File_Current = oldFile;
                }
            }
        }

        public void File_SaveAs()
        {
            var res = UI.SaveBimDialog.ShowDialog();
            if (res == DialogResult.OK)
            {
                Handler.SaveFile(UI.SaveBimDialog.FileName);
                if (File_Current != null) File_Current = UI.SaveBimDialog.FileName;
                UpdateUIText();
            }
        }

        public void Save()
        {
            if(Handler.IsConnected)
            {
                Database_Save();
            }
            else
            {
                Handler.SaveFile(File_Current);
            }
            UpdateUIText();
        }
    }
}
