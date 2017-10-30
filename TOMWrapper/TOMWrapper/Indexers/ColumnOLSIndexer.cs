extern alias json;

using json.Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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
    public sealed class ColumnOLSIndexer: GenericIndexer<ModelRole, TOM.MetadataPermission>
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

        protected override TOM.MetadataPermission GetValue(ModelRole role)
        {
            var tp = role.MetadataObject.TablePermissions.Find(Column.Table.Name);
            if (tp == null) return TOM.MetadataPermission.Default;

            return tp.ColumnPermissions.Find(Column.Name)?.MetadataPermission ?? TOM.MetadataPermission.Default;
        }

        protected override void SetValue(ModelRole role, TOM.MetadataPermission permission)
        {
            var tps = role.MetadataObject.TablePermissions;
            var tp = tps.Find(Column.Table.Name);
            var cp = tp?.ColumnPermissions?.Find(Column.Name);

            // Permission removed:
            if (permission == TOM.MetadataPermission.Default)
            {
                // Do nothing if same value or no CP defined
                if (cp == null || cp.MetadataPermission == permission) return;

                // Otherwise, remove CP:
                Column.Handler.UndoManager.Add(new UndoPropertyChangedAction(Column, "ObjectLevelSecurity", cp.MetadataPermission, permission, role.Name));
                tp.ColumnPermissions.Remove(cp);
            }
            else // Non-default permission assigned:
            {
                // Create TP if not exists:
                if (tp == null)
                {
                    tp = new TOM.TablePermission { Table = Column.Table.MetadataObject };
                    tps.Add(tp);
                }
                // Create CP if not exists:
                if (cp == null)
                {
                    cp = new TOM.ColumnPermission { Column = Column.MetadataObject };
                    tp.ColumnPermissions.Add(cp);
                }

                // Do nothing if same value
                if (cp.MetadataPermission == permission) return;

                // Otherwise, assign the new permission to CP:
                Column.Handler.UndoManager.Add(new UndoPropertyChangedAction(Column, "ObjectLevelSecurity", cp.MetadataPermission, permission, role.Name));
                cp.MetadataPermission = permission;
            }
        }
    }
}
