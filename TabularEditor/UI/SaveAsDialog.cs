using Microsoft.WindowsAPICodePack.Dialogs;
using Microsoft.WindowsAPICodePack.Dialogs.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.UI
{
    public class SaveAsDialog: IDisposable
    {
        private CommonFileDialog dlg;
        private CommonFileDialogCheckBox sfaCheck = null;
        public string FileName => dlg.FileName;
        public bool UseSerializationFromAnnotations => sfaCheck != null && sfaCheck.IsChecked;

        public string FileType => dlg.Filters[dlg.SelectedFileTypeIndex - 1].Extensions.FirstOrDefault();

        public void Dispose()
        {
            dlg.Dispose();
        }

        private void AddSfaCheckbox()
        {
            sfaCheck = new CommonFileDialogCheckBox
            {
                Text = "Use serialization settings from annotations",
                IsChecked = true
            };
            dlg.Controls.Add(sfaCheck);
        }

        public static SaveAsDialog CreateFileDialog(bool showSfaCheckbox, string defaultFileName, bool allowPbit)
        {
            var result = new SaveAsDialog();
            result.dlg = new CommonSaveFileDialog();
            if (showSfaCheckbox) result.AddSfaCheckbox();
            result.dlg.DefaultFileName = defaultFileName;
            result.dlg.Filters.Clear();
            if (allowPbit) result.dlg.Filters.Add(new CommonFileDialogFilter("Power BI Template", "*.pbit"));
            result.dlg.Filters.Add(new CommonFileDialogFilter("Tabular Model Files", "*.bim"));
            result.dlg.Filters.Add(new CommonFileDialogFilter("All files", "*.*"));

            return result;
        }

        public static SaveAsDialog CreateFolderDialog(bool showSfaCheckbox)
        {
            var result = new SaveAsDialog();
            result.dlg = new CommonOpenFileDialog();
            if (showSfaCheckbox) result.AddSfaCheckbox();
            (result.dlg as CommonOpenFileDialog).IsFolderPicker = true;

            return result;
        }

        public CommonFileDialogResult ShowDialog()
        {
            return dlg.ShowDialog();
        }
    }
}
