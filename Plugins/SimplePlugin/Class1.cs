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
            registerCallback("MyPlugin\\Display Name", SayHello);
            registerCallback("MyPlugin\\Change Name", ChangeModelName);
        }

        public void SayHello()
        {
            MessageBox.Show("Hello from plugin!\n\nCurrently loaded model name:" + Handler.Model?.Name);

        }

        public void ChangeModelName()
        {
            Model.Name = "test";
        }
    }
}
