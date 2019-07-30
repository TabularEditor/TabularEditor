using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.PropertyGridUI
{
    internal class RoleMemberCollectionEditor: RefreshGridCollectionEditor
    {
        public RoleMemberCollectionEditor(Type type): base(type)
        {

        }

        class MemberTypeDelegator : TypeDelegator
        {
            public MemberTypeDelegator(Type delegatingType)
                : base(delegatingType)
            {
            }

            public override string Name
            {
                get
                {
                    return this.typeImpl == typeof(WindowsModelRoleMember) ? "Windows AD Member" : "Azure AD Member";
                }
            }
        }

        protected override Type[] CreateNewItemTypes()
        {
            return new[] {
                new MemberTypeDelegator( typeof(WindowsModelRoleMember)),
                new MemberTypeDelegator( typeof(ExternalModelRoleMember)) };
        }

        object[] items;

        protected override object SetItems(object editValue, object[] value)
        {
            items = value;
            return editValue;
            //return base.SetItems(editValue, value);
        }

        protected override object[] GetItems(object editValue)
        {
            return base.GetItems(editValue);
        }

        protected override object CreateInstance(Type itemType)
        {
            if (itemType.UnderlyingSystemType == typeof(WindowsModelRoleMember))
                return WindowsModelRoleMember.CreateUnassigned();
            else
                return ExternalModelRoleMember.CreateUnassigned();
        }        

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if(!Cancelled && items != null)
            {
                var modelRole = Context.Instance as ModelRole;

                // Members to add:
                foreach(ModelRoleMember item in items.Except(modelRole.Members))
                {
                    if (string.IsNullOrEmpty(item.MemberID) && string.IsNullOrEmpty(item.MemberName)) continue;
                    modelRole.Members.Add(item);
                }

                // Members to remove:
                foreach(ModelRoleMember item in modelRole.Members.Except(items).ToList())
                {
                    item.Delete();
                }
            }
            base.OnFormClosed(e);
        }
        protected override string GetDisplayText(object value)
        {
            var mrm = value as ModelRoleMember;
            return !string.IsNullOrEmpty(mrm.MemberName) ? mrm.MemberName : !string.IsNullOrEmpty(mrm.MemberID) ? mrm.MemberID
                : "< New member >";
        }
    }
}
