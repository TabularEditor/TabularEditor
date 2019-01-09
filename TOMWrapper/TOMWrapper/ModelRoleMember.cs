using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class WindowsModelRoleMember
    {
        internal static WindowsModelRoleMember CreateUnassigned()
        {
            var obj = new WindowsModelRoleMember(new TOM.WindowsModelRoleMember());
            obj.Init();
            return obj;
        }
    }

    public partial class ExternalModelRoleMember
    {
        internal static ExternalModelRoleMember CreateUnassigned()
        {
            var obj = new ExternalModelRoleMember(new TOM.ExternalModelRoleMember {  IdentityProvider = "AzureAD" });
            obj.Init();
            return obj;
        }
    }

    public partial class ModelRoleMember
    {
        protected override bool IsBrowsable(string propertyName)
        {
            switch (propertyName)
            {
                case Properties.MEMBERNAME:
                case Properties.MEMBERID:
                case Properties.MEMBERTYPE:
                case Properties.OBJECTTYPE:
                case Properties.IDENTITYPROVIDER:
                case Properties.EXTENDEDPROPERTIES:
                case Properties.ANNOTATIONS:
                    return true;
                default:
                    return false;
            }
        }
    }
}
