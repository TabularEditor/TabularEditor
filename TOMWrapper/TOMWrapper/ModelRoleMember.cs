using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper.PowerBI;
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

        /// <summary>
        /// Creates a new ExternalModelRoleMember and adds it to the parent ModelRole.
        /// Also creates the underlying metadataobject and adds it to the TOM tree.
        /// </summary>
        public static ExternalModelRoleMember CreateNew(ModelRole parent, string name, string identityProvider)
        {
            if (!parent.Handler.PowerBIGovernance.AllowCreate(typeof(ExternalModelRoleMember)))
            {
                throw new InvalidOperationException(string.Format(Messages.CannotCreatePowerBIObject, typeof(ExternalModelRoleMember).GetTypeName()));
            }

            var metadataObject = new TOM.ExternalModelRoleMember();
            metadataObject.IdentityProvider = identityProvider;
            metadataObject.MemberName = name;
            var obj = new ExternalModelRoleMember(metadataObject);

            parent.Members.Add(obj);

            obj.Init();

            return obj;
        }
    }

    public partial class ModelRoleMember
    {
        internal override bool IsBrowsable(string propertyName)
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
