using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.UI.Dialogs
{
    public delegate void ValidationEventHandler(object sender, ValidationEventArgs e);

    public class ValidationEventArgs: EventArgs
    {
        public bool IsValid { get; private set; }
        public ValidationEventArgs(bool isValid)
        {
            IsValid = isValid;
        }
    }

}
