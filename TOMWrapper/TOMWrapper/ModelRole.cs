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
    public partial class ModelRole: IErrorMessageObject
    {
        [Browsable(true), DisplayName("Row Level Security"), Category("Security")]
        public RoleRLSIndexer RowLevelSecurity { get; private set; }

        [Browsable(true), DisplayName("Object Level Security"), Category("Security")]
        public RoleOLSIndexer MetadataPermission { get; private set; }

        internal override void RemoveReferences()
        {
            base.RemoveReferences();

            foreach(var fEx in RowLevelSecurity.FilterExpressions.Values)
            {
                fEx.DependsOn.Keys.ToList().ForEach(d => d.ReferencedBy.Remove(fEx));
                fEx.DependsOn.Clear();
            }
            RowLevelSecurity._filterExpressions.Clear();
        }
        /*
        [Category("Security"), DisplayName("Members")]
        [Description("Specify domain/usernames of the members in this role. One member per line.")]
        [Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
        public string RoleMembers
        {
            get
            {
                return string.Join("\r\n", MetadataObject.Members.Select(m => m.MemberName));
            }
            set
            {
                if (MetadataObject.Members.Any(m => m is TOM.ExternalModelRoleMember))
                    throw new InvalidOperationException("This role uses External Role Members. These role members are not supported in this version of Tabular Editor.");
                if (RoleMembers == value) return;

                Handler.UndoManager.Add(new Undo.UndoPropertyChangedAction(this, "RoleMembers", RoleMembers, value));
                MetadataObject.Members.Clear();
                foreach (var member in value.Replace("\r", "").Split('\n'))
                {
                    MetadataObject.Members.Add(new TOM.WindowsModelRoleMember() { MemberName = member });
                }
            }
        }
        */

        [Category("Basic")]
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

        protected override bool IsBrowsable(string propertyName)
        {
            switch (propertyName) {
                case "MetadataPermission": return Handler.CompatibilityLevel >= 1400;
                default:  return true;
            }
        }

        protected override void Init()
        {
            RowLevelSecurity = new RoleRLSIndexer(this);
            if (Handler.CompatibilityLevel >= 1400) MetadataPermission = new RoleOLSIndexer(this);

            base.Init();
        }
    }
}
