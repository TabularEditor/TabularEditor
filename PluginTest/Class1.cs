using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;

namespace PluginTest
{
    public class MyPlugin: ITabularEditorPlugin
    {
        TabularModelHandler Handler;
        Model Model;

        public void Init(TabularModelHandler handler)
        {
            Handler = handler;

            Model = handler.Model;
        }

        public void RegisterActions(Action<string, Action> registerCallback)
        {
            registerCallback("MyPlugin\\Display Name", Action1);
            registerCallback("MyPlugin\\Change Name", Action2);
        }

        public void Action1()
        {
            MessageBox.Show("Hello from plugin!\n\nCurrently loaded model name:" + Handler.Model?.Name);

        }

        public void Action2()
        {
            Model.Name = "test";
        }
    }
}
