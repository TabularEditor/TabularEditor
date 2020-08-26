using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularEditor.UI.Extensions
{
    public class DoubleBufferedGridView: DataGridView
    {
        public DoubleBufferedGridView() { DoubleBuffered = true; }
    }
}
