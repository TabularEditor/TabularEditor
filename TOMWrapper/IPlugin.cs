using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TabularEditor.TOMWrapper
{
    public interface ITabularEditorPlugin
    {
        void Init(TabularModelHandler handler);
        void RegisterActions(Action<string, Action> registerCallback);
    }
}
