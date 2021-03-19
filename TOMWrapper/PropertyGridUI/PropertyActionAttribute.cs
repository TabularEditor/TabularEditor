using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;

namespace TabularEditor.PropertyGridUI
{
    public class PropertyActionAttribute: Attribute
    {
        private readonly string[] actions;

        public PropertyActionAttribute(params string[] actions)
        {
            this.actions = actions;
        }

        public List<PropertyAction> GetPropertyActions(object baseObject, Type declaringType)
        {
            var result = new List<PropertyAction>();
            for (int i = 0; i < actions.Length; i++)
            {
                var actionMethod = actions[i];

                var executeMethod = declaringType.GetMethod(actionMethod, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                if (executeMethod == null || executeMethod.GetParameters().Length != 0) continue;

                var displayName = executeMethod.GetCustomAttribute<DisplayNameAttribute>();
                var actionName = displayName != null ? displayName.DisplayName : actionMethod.SplitCamelCase();

                Action actionExecute = () => executeMethod.Invoke(baseObject, new object[] { });
                
                var enabledMethod = declaringType.GetMethod($"Can{actionMethod}", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                Func<bool> actionEnabled;
                if (enabledMethod != null && enabledMethod.ReturnType == typeof(bool) && enabledMethod.GetParameters().Length == 0)
                    actionEnabled = () => (bool)enabledMethod.Invoke(baseObject, new object[] { });
                else
                    actionEnabled = () => true;

                result.Add(new PropertyAction
                {
                    Name = actionName,
                    Execute = actionExecute,
                    Enabled = actionEnabled
                });
            }
            return result;
        }
    }
}
