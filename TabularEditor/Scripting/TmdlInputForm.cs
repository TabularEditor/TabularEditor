using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using Newtonsoft.Json.Linq;
using TabularEditor.TOMWrapper;

namespace TabularEditor.Scripting
{
    public partial class TmdlInputForm : Form
    {
        public TmdlInputForm()
        {
            InitializeComponent();
        }

        public bool Replace => chkReplace.Checked;

        public string Tmdl => txtTmdl.Text;
    }
}
