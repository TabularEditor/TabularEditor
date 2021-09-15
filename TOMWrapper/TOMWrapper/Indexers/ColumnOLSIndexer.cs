using System;
using System.ComponentModel;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper.Undo;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    /// <summary>
    /// The ColumnOLSIndexer is used to browse all MetadataPermissions defined on one specific
    /// column, across all roles in the model. This is in contrast to the RoleOLSIndexer, 
    /// which browses the permissions across all tables for one specific role.
    /// </summary>
    [TypeConverter(typeof(IndexerConverter))]
    public sealed class ColumnOLSIndexer: GenericIndexer<ModelRole, MetadataPermission>
    {
        public readonly Column Column;

        internal ColumnOLSIndexer(Column column) : base(column)
        {
            if (Handler.CompatibilityLevel < 1400) throw new InvalidOperationException(Messages.CompatibilityError_ObjectLevelSecurity);
            Column = column;
        }

        protected override TabularObjectCollection<ModelRole> GetCollection()
        {
            return Model.Roles;
        }

        protected override MetadataPermission GetValue(ModelRole role)
        {
            if (role == null) return MetadataPermission.Default;
            var tp = role.MetadataObject.TablePermissions.Find(Column.Table.Name);
            if (tp == null) return MetadataPermission.Default;

            return (MetadataPermission) (tp.ColumnPermissions.Find(Column.Name)?.MetadataPermission ?? TOM.MetadataPermission.Default);
        }

        protected override void SetValue(ModelRole role, MetadataPermission permission)
        {
            var tp = role.TablePermissions.FindByName(Column.Table.Name);
            var cp = tp != null ? tp.MetadataObject.ColumnPermissions.Find(Column.Name) : null;

            // Permission removed:
            if (permission == MetadataPermission.Default)
            {
                // Do nothing if same value or no CP defined
                if (cp == null) return;

                // Otherwise, remove CP:
                Column.Handler.UndoManager.Add(new UndoPropertyChangedAction(Column, "ObjectLevelSecurity", cp.MetadataPermission, permission, role.Name));
                tp.MetadataObject.ColumnPermissions.Remove(cp);
            }
            else // Non-default permission assigned:
            {
                Handler.BeginUpdate("object level security");

                // Create TP if not exists:
                if (tp == null) tp = TablePermission.CreateFromMetadata(role, new TOM.TablePermission { Table = Column.Table.MetadataObject });

                // Create CP if not exists:
                if (cp == null)
                {
                    cp = new TOM.ColumnPermission { Column = Column.MetadataObject };
                    tp.MetadataObject.ColumnPermissions.Add(cp);
                }

                // Do nothing if same value
                if ((MetadataPermission)cp.MetadataPermission == permission) return;

                // Otherwise, assign the new permission to CP:
                Column.Handler.UndoManager.Add(new UndoPropertyChangedAction(Column, "ObjectLevelSecurity", cp.MetadataPermission, permission, role.Name));
                cp.MetadataPermission = (TOM.MetadataPermission)permission;

                Handler.EndUpdate();
            }
        }
    }
}
