extern alias json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using json.Newtonsoft.Json.Linq;
using TabularEditor.TOMWrapper;

namespace TabularEditor.Scripting
{
    public partial class ScriptOutputForm : Form
    {
        public static void Reset(bool canShowOutputForm = true)
        {
            DontShow = !canShowOutputForm;
        }
        private static ScriptOutputForm DormantForm;
        public bool ShowErrors;

        public static bool DontShow { get; set; }

        public ScriptOutputForm()
        {
            InitializeComponent();
        }

        public static void ShowObject(object value, string caption, bool errorObjects = false)
        {
            if (DormantForm != null && DormantForm.Visible) DormantForm.Close();
            DormantForm = new ScriptOutputForm();

            DormantForm.Text = caption;
            DormantForm.ShowErrors = errorObjects;
            if (DormantForm.ShowErrors)
            {
                DormantForm.chkDontShow.Visible = false;
                DormantForm.DataListView.Width = (DormantForm.Width * 3) / 5;
                DormantForm.DataListView.Columns[1].Name = "Error";
                DormantForm.DataListView.Columns[1].Width = 500;
            } else
            {
                DormantForm.chkDontShow.Visible = true;
                DormantForm.DataListView.Width = DormantForm.Width / 2;
                DormantForm.DataListView.Columns[1].Name = "Type";
                DormantForm.DataListView.Columns[1].Width = DormantForm.DataListView.Width - DormantForm.DataListView.Columns[0].Width - 5;
            }

            var testTabularObject = value as TabularObject;

            IEnumerable<ITabularNamedObject> testTabularCollection;
            if(value is IEnumerable<IDaxDependantObject>)
            {
                // Most IDaxDependantObjects are ITabularNamedObjects, with the exception of RLSFilterExpression.
                // So if we encounter an enumeration of IDaxDependantObjects lets convert it to an enumeration of
                // ITabularNamedObjects. If there are any RLSFilterExpressions in the enumeration, output their
                // corresponding roles instead.
                var v = (value as IEnumerable<IDaxDependantObject>);
                testTabularCollection = v.OfType<ITabularNamedObject>()
                    .Concat(v.OfType<TablePermission>().Select(rls => rls.Role));
            }
            else
                testTabularCollection = value as IEnumerable<ITabularNamedObject>;
            var testDictionary = value as IDictionary;
            var testList = value as IEnumerable<object>;

            DormantForm.btnCopy.Visible = false;
            if (value == null) ShowString("(Null)");
            else if (value is JValue) ShowString(value.ToString());
            else if (testTabularObject != null) ShowTabularObject(testTabularObject);
            else if (testTabularCollection != null) ShowTabularObjectCollection(testTabularCollection);
            else if (testDictionary != null) ShowObjectDictionary(testDictionary);
            else if (testList != null) ShowObjectCollection(testList);
            else if (value is DataTable dt) ShowDataTable(dt);
            else ShowString(value.ToString());

            if (errorObjects)
            {
                DormantForm.Show();
                DormantForm.BringToFront();
                DormantForm.Activate();
            }
            else
            {
                if(DormantForm.Visible) DormantForm.Visible = false;
                DormantForm.ShowDialog();
            }

            DontShow = DormantForm.chkDontShow.Checked;
        }

        private static void ShowTabularObject(TabularObject obj)
        {
            DormantForm.dataGridView.DataSource = null;
            DormantForm.dataGridView.Visible = false;
            DormantForm.DataProperties.Visible = true;
            DormantForm.DataListView.Visible = false;
            DormantForm.DataListView.VirtualListSize = 0;
            DormantForm.DataSplitter.Visible = false;
            DormantForm.DataTextBox.Visible = false;
            DormantForm.DataTextBox.Text = "";

            DormantForm.DataPropertyGrid.SelectedObject = obj;
        }

        private enum ListMode
        {
            TabularObjects,
            KeyValuePair,
            Others,
            DataTable
        }

        private List<object> objList;
        private ListMode Mode;

        private static void ShowCollection()
        {
            DormantForm.dataGridView.DataSource = null;
            DormantForm.dataGridView.Visible = false;
            DormantForm.DataProperties.Visible = true;
            DormantForm.DataListView.Visible = true;
            DormantForm.DataListView.VirtualListSize = DormantForm.objList.Count;
            DormantForm.DataTextBox.Visible = false;
            DormantForm.DataTextBox.Text = "";

            DormantForm.DataPropertyGrid.SelectedObject = null;
        }

        private static void ShowTabularObjectCollection(IEnumerable<ITabularNamedObject> obj)
        {
            DormantForm.Mode = ListMode.TabularObjects;
            DormantForm.objList = obj.ToList<object>();
            DormantForm.DataSplitter.Visible = true;
            DormantForm.DataPropertyGrid.Visible = true;
            DormantForm.DataListView.Dock = DockStyle.Left;
            DormantForm.DataListView.Columns[0].Text = "Name";

            ShowCollection();
        }

        private static void ShowObjectCollection(IEnumerable<object> obj)
        {
            DormantForm.Mode = ListMode.Others;
            DormantForm.objList = obj.ToList<object>();
            DormantForm.DataSplitter.Visible = false;
            DormantForm.DataPropertyGrid.Visible = false;
            DormantForm.DataListView.Dock = DockStyle.Fill;
            DormantForm.DataListView.Columns[0].Text = "Value";

            ShowCollection();
        }
        private static void ShowDataTable(DataTable dt)
        {
            DormantForm.Mode = ListMode.DataTable;

            DormantForm.DataProperties.Visible = false;
            DormantForm.DataListView.VirtualListSize = 0;
            DormantForm.DataTextBox.Visible = false;
            DormantForm.DataPropertyGrid.SelectedObject = null;
            DormantForm.btnCopy.Visible = true;

            DormantForm.dataGridView.Visible = true;
            DormantForm.dataGridView.DataSource = dt;
        }

        private static void ShowObjectDictionary(IDictionary obj)
        {
            DormantForm.Mode = ListMode.KeyValuePair;
            DormantForm.objList = obj.Keys.Cast<object>().Select(key => (object)new KeyValuePair<object, object>(key, obj[key])).ToList();
            DormantForm.DataSplitter.Visible = true;
            DormantForm.DataPropertyGrid.Visible = true;
            DormantForm.DataListView.Dock = DockStyle.Left;
            DormantForm.DataListView.Columns[0].Text = "Key";

            ShowCollection();
        }

        private static void ShowString(string value)
        {
            DormantForm.dataGridView.DataSource = null;
            DormantForm.dataGridView.Visible = false;
            DormantForm.DataProperties.Visible = false;
            DormantForm.DataListView.VirtualListSize = 0;
            DormantForm.DataTextBox.Visible = true;
            DormantForm.DataTextBox.Text = value;

            DormantForm.DataPropertyGrid.SelectedObject = null;

            DormantForm.btnCopy.Visible = true;
        }

        private void DataListView_RetrieveVirtualItem(object sender, RetrieveVirtualItemEventArgs e)
        {
            if (Mode == ListMode.TabularObjects)
            {
                var obj = objList[e.ItemIndex] as ITabularNamedObject;
                if (ShowErrors)
                {
                    e.Item = new ListViewItem(new[] { (obj as IDaxObject)?.DaxObjectFullName ?? obj.Name, (obj as IErrorMessageObject)?.ErrorMessage }, 15);
                }
                else
                {
                    e.Item = new ListViewItem(new[] { obj.Name, obj.GetTypeName() }, 15);
                }
            } else if (Mode == ListMode.KeyValuePair) {
                var obj = (KeyValuePair < object, object> )objList[e.ItemIndex];
                e.Item = new ListViewItem(new[] { obj.Key.ToString(), obj.Value?.GetType()?.Name });
            }
            else if (Mode == ListMode.Others)
            {
                var obj = objList[e.ItemIndex];
                e.Item = new ListViewItem(new[] { obj?.ToString(), obj?.GetType()?.Name }, 15);
            }
        }

        private void DataListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (DataListView.SelectedIndices.Count == 1 && Mode == ListMode.TabularObjects)
                DataPropertyGrid.SelectedObject = objList[DataListView.SelectedIndices[0]];
            else if (DataListView.SelectedIndices.Count == 1 && Mode == ListMode.KeyValuePair)
            {
                var obj = (KeyValuePair<object, object>)objList[DataListView.SelectedIndices[0]];
                if (obj.Value != null && obj.Value.GetType().IsPrimitive)
                    DataPropertyGrid.SelectedObject = obj;
                else
                    DataPropertyGrid.SelectedObject = obj.Value;
            }
            else
                DataPropertyGrid.SelectedObject = null;
        }

        private void DataListView_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            var item = DataListView.GetItemAt(e.X, e.Y);
            if (item == null) return;

            var obj = objList[item.Index] as TabularNamedObject;
            if(obj != null)
            {
                UI.UIController.Current.Goto(obj);
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            DormantForm.Close();
        }

        private void btnCopy_Click(object sender, EventArgs e)
        {
            if(Mode == ListMode.DataTable)
            {
                var dt = dataGridView.DataSource as DataTable;
                var headers = string.Join("\t", dt.Columns.OfType<System.Data.DataColumn>().Select(dc => dc.ColumnName).ToArray());
                var data = string.Join("\n", dt.Rows.OfType<System.Data.DataRow>().Select(dr => string.Join("\t", dr.ItemArray)));
                Clipboard.SetText(headers + "\n" + data);
            }
            else
                Clipboard.SetText(DataTextBox.Text);
        }
    }
}
