using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.UI.Dialogs;

namespace TabularEditor.UI
{
    public partial class UIController
    {
        private SaveFileDialog Translation_SaveDlg = new SaveFileDialog() { Filter = "JSON Translation|*.json", DefaultExt = "json", FileName = "default.json" };
        private ImportTranslationsForm Translation_OpenDlg = new ImportTranslationsForm();

        public void Translations_ExportAll()
        {
            var res = Translation_SaveDlg.ShowDialog();
            if (res == DialogResult.Cancel) return;

            var fileName = Translation_SaveDlg.FileName;
            (new FileInfo(fileName)).Directory.Create();
            File.WriteAllText(fileName, Handler.ScriptTranslations(Handler.Model.Cultures));
        }
        public void Translations_ExportSelected()
        {
            var res = Translation_SaveDlg.ShowDialog();
            if (res == DialogResult.Cancel) return;

            var fileName = Translation_SaveDlg.FileName;
            File.WriteAllText(fileName, Handler.ScriptTranslations(Selection.Cultures));
        }

        public void Translations_Import()
        {
            var res = Translation_OpenDlg.ShowDialog();
            if (res == DialogResult.Cancel) return;

            if (!File.Exists(Translation_OpenDlg.JsonFile)) return;
            var json = File.ReadAllText(Translation_OpenDlg.JsonFile);

            var result = Handler.ImportTranslations(json, Translation_OpenDlg.OverwriteExisting, Translation_OpenDlg.IgnoreInvalid);
            if (!result) MessageBox.Show("Invalid objects encountered while applying translations from file.", "Invalid objects in translation file", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
