using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper.Undo;
using TabularEditor.TOMWrapper.Utils;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{

    public partial class TablePermission : IDaxDependantObject, IExpressionObject
    {
        public bool NoEffect
        {
            get
            {
                if(Handler.CompatibilityLevel >= 1400)
                {
                    if (
                        MetadataPermission != MetadataPermission.Default ||
                        ColumnPermissions.Any(cp => cp != MetadataPermission.Default)
                    ) return false;
                }
                if (!string.IsNullOrWhiteSpace(FilterExpression)) return false;
                return true;
            }
        }

        private DependsOnList _dependsOn = null;

        [Browsable(false)]
        public DependsOnList DependsOn
        {
            get
            {
                if (_dependsOn == null)
                    _dependsOn = new DependsOnList(this);
                return _dependsOn;
            }
        }

        [Browsable(true), Category("Metadata"),DisplayName("Table")]
        public string TableName => Table.Name;
        [Browsable(true), Category("Metadata"),DisplayName("Role")]
        public string RoleName => Role.Name;

        [Browsable(false)]
        public bool NeedsValidation => IsExpressionModified;
        string IExpressionObject.Expression { get => FilterExpression; set => FilterExpression = value; }
        string ITabularNamedObject.Name { get => Table.Name; set { } }

        int ITabularNamedObject.MetadataIndex => Role.MetadataObject.TablePermissions.IndexOf(MetadataObject);

        internal static TablePermission CreateFromMetadata(TOM.TablePermission metadataObject)
        {
            var obj = new TablePermission(metadataObject);
            obj.Init();
            return obj;
        }

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if (propertyName == Properties.EXPRESSION || propertyName == Properties.FILTEREXPRESSION)
            {
                FormulaFixup.BuildDependencyTree(this);
            }
        }

        protected override void Init()
        {
            if (!string.IsNullOrEmpty(ErrorMessage)) Handler._errors.Add(this);
            if (MetadataObject.Model != null) DelayedInit();
        }

        public void DelayedInit()
        {
            if (MetadataObject.Table == null)
                Delete();
            else if(ColumnPermissions == null)
                ColumnPermissions = new RoleColumnOLSIndexer(this);
        }

        [Browsable(true), DisplayName("OLS Column Permissions"), Category("Security")]
        public RoleColumnOLSIndexer ColumnPermissions { get; private set; }

        internal override bool IsBrowsable(string propertyName)
        {
            switch(propertyName)
            {
                case Properties.FILTEREXPRESSION:
                case Properties.ANNOTATIONS:
                case Properties.EXTENDEDPROPERTIES:
                case Properties.ERRORMESSAGE:
                case Properties.STATE:
                case Properties.METADATAPERMISSION:
                case Properties.COLUMNPERMISSIONS:
                case "TableName":
                case "RoleName":
                    return true;
                default:
                    return false;
            }
        }
    }

    public partial class TablePermissionCollection
    {
        public TablePermission this[Table table]
        {
            get
            {
                return this[table.Name];
            }
        }
    }
}
