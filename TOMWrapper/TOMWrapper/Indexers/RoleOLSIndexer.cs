using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TabularEditor.PropertyGridUI;
using TabularEditor.UndoFramework;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{

    /// <summary>
    /// The RolePermissionIndexer is used to browse all metadata permissions across all
    /// tables in the model, for one specific role.
    /// </summary>
    [TypeConverter(typeof(IndexerConverter))]
    public class RoleOLSIndexer: GenericIndexer<Table, TOM.MetadataPermission>
    {
        public readonly ModelRole Role;

        public RoleOLSIndexer(ModelRole role) : base(role)
        {
            Role = role;
        }

        protected override TabularObjectCollection<Table> GetCollection()
        {
            return Model.Tables;
        }

        protected override TOM.MetadataPermission GetValue(Table table)
        {
            return Role.MetadataObject.TablePermissions.Find(table.Name)?.MetadataPermission ?? TOM.MetadataPermission.Default;
        }

        protected override void SetValue(Table table, TOM.MetadataPermission permission)
        {
            var tps = Role.MetadataObject.TablePermissions;
            var tp = tps.Find(table.Name);
            if (tp == null && permission == TOM.MetadataPermission.Default) return;
            if (tp == null)
            {
                tp = new TOM.TablePermission() { Table = table.MetadataObject };
                tps.Add(tp);
            }
            var oldValue = tp.MetadataPermission;
            tp.MetadataPermission = permission;

            Role.Handler.UndoManager.Add(new UndoPropertyChangedAction(Role, "MetadataPermission", oldValue, permission, table.Name));
        }
    }
}
