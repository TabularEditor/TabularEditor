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
    public partial class ModelRole: IErrorMessageObject, ITabularObjectContainer
    {
        [Browsable(true), DisplayName("Row Level Security"), Category("Security")]
        public RoleRLSIndexer RowLevelSecurity { get; private set; }

        [Browsable(true), DisplayName("Table Permissions"), Category("Security")]
        public RoleOLSIndexer MetadataPermission { get; private set; }

        /// <summary>
        /// Specify domain/usernames of the members in this role. One member per line. DEPRECATED: Use the Members collection instead.
        /// </summary>
        [Browsable(false)]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        [Description("Specify domain/usernames of the members in this role. One member per line. DEPRECATED: Use the Members collection instead.")]
        [IntelliSense("Specify domain/usernames of the members in this role. One member per line. DEPRECATED: Use the Members collection instead.")]
        public string RoleMembers
        {
            get
            {
                return string.Join("\r\n", MetadataObject.Members.Select(m => m.MemberName));
            }
            set
            {
                if (MetadataObject.Members.Any(m => m is TOM.ExternalModelRoleMember))
                    throw new InvalidOperationException("This role uses External Role Members. To add External Role Members, please use the Members collection instead.");

                Handler.BeginUpdate("Set property 'Role Members'");
                Members.Clear();
                foreach(var member in value.Replace("\r", "").Split('\n'))
                {
                    WindowsModelRoleMember.CreateNew(this, member);
                }
                Handler.EndUpdate();
            }
        }

        /// <summary>
        /// Removes all members from this role.
        /// </summary>
        [IntelliSense("Removes all members from this role.")]
        public void ClearMembers()
        {
            Members.Clear();
        }

        /// <summary>
        /// Adds a Windows AD member to this role.
        /// </summary>
        [IntelliSense("Adds a Windows AD member to this role.")]
        public void AddWindowsMember(string memberName, string memberId = null)
        {
            var member = WindowsModelRoleMember.CreateNew(this, memberName);
            if (!string.IsNullOrEmpty(memberId)) member.MemberID = memberId;
        }

        /// <summary>
        /// Adds an Azure AD member to this role.
        /// </summary>
        [IntelliSense("Adds an Azure AD member to this role.")]
        public ExternalModelRoleMember AddExternalMember(string memberName)
        {
            return ExternalModelRoleMember.CreateNew(this, memberName, "AzureAD");
        }

        [Category("Metadata")]
        public string ErrorMessage
        {
            get
            {
                if(MetadataObject.TablePermissions.Any(tp => !string.IsNullOrEmpty(tp.ErrorMessage)))
                {
                    return string.Join("\n", MetadataObject.TablePermissions.Where(tp => !string.IsNullOrEmpty(tp.ErrorMessage)).Select(tp => "'" + tp.Table.Name + "' RLS: " + tp.ErrorMessage));
                }
                return null;
            }
        }

        internal override bool IsBrowsable(string propertyName)
        {
            switch (propertyName) {
                case "MetadataPermission": return Handler.CompatibilityLevel >= 1400;
                case Properties.TABLEPERMISSIONS: return false;
                default:  return true;
            }
        }

        protected override void Init()
        {
            foreach (var tp in this.TablePermissions.ToList()) tp.DelayedInit();

            RowLevelSecurity = new RoleRLSIndexer(this);
            if (Handler.CompatibilityLevel >= 1400) MetadataPermission = new RoleOLSIndexer(this);

            base.Init();
        }

        public IEnumerable<ITabularNamedObject> GetChildren()
        {
            return TablePermissions;
        }
    }
}
