using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using TabularEditor.PropertyGridUI;
using TabularEditor.UndoFramework;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{

#if CL1400
    /// <summary>
    /// The RolePermissionIndexer is used to browse all metadata permissions across all
    /// tables in the model, for one specific role.
    /// </summary>
    [TypeConverter(typeof(IndexerConverter))]
    public class RoleOLSIndexer : IEnumerable<TOM.MetadataPermission>, IExpandableIndexer
    {

        public void Refresh()
        {
            RLSMap = Role.Model.Tables.ToDictionary(
                t => t,
                t => Role.MetadataObject.TablePermissions.Contains(t.Name) ? Role.MetadataObject.TablePermissions[t.Name].MetadataPermission : TOM.MetadataPermission.Default
                );
        }

        [IntelliSense("Copies all RLSs from another RLS collection.")]
        public void CopyFrom(RoleOLSIndexer source)
        {
            foreach(var p in source.Keys)
            {
                var value = source[p];
                this[p] = value;
            }
        }

        [IntelliSense("Removes the RLS on this role from all tables.")]
        /// <summary>
        /// Removes the object from all RLSs.
        /// </summary>
        public void Clear()
        {
            Role.Handler.BeginUpdate("no RLSs");
            foreach (var key in Keys.ToList()) this[key] = Microsoft.AnalysisServices.Tabular.MetadataPermission.Default;
            Role.Handler.EndUpdate();
        }

        private Dictionary<Table, TOM.MetadataPermission> rlsMap;
        protected ModelRole Role;

        public string Summary
        {
            get
            {
                Refresh();
                return string.Format("OLS defined on {0} out of {1} tables", this.Count(p => p != TOM.MetadataPermission.Default), Role.Model.Tables.Count);
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return RLSMap.Keys.Select(k => k.Name);
            }
        }

        protected Dictionary<Table, TOM.MetadataPermission> RLSMap
        {
            get
            {
                if (rlsMap == null) Refresh();
                return rlsMap;
            }

            set
            {
                rlsMap = value;
            }
        }

        object IExpandableIndexer.this[string index]
        {
            get
            {
                return this[index];
            }
            set
            {
                this[index] = (TOM.MetadataPermission)value;
            }
        }

        public RoleOLSIndexer(ModelRole role)
        {
            Role = role;
            Refresh();
        }

        public virtual TOM.MetadataPermission this[Table table]
        {
            get {
                if (RLSMap == null) Refresh();
                if (!RLSMap.ContainsKey(table)) Refresh();
                return RLSMap[table];
            }
            set
            {
                var oldValue = this[table];
                if (value == oldValue) return;

                RLSMap[table] = value;
                SetRLS(table, value);
            }
        }

        protected void SetRLS(Table table, TOM.MetadataPermission permission)
        {
            var tps = Role.MetadataObject.TablePermissions;
            var tp = tps.Contains(table.Name) ? tps[table.Name] : null;

            if(tp == null)
            {
                tp = new TOM.TablePermission() { Table = table.MetadataObject };
                tps.Add(tp);
            }
            var oldValue = tp.MetadataPermission;
            tp.MetadataPermission = permission;

            Role.Handler.UndoManager.Add(new UndoPropertyChangedAction(Role, "MetadataPermission", oldValue, permission, table.Name));
        }

        public TOM.MetadataPermission this[string tableName]
        {
            get {
                return this[Role.Model.Tables[tableName]];
            }
            set
            {
                this[Role.Model.Tables[tableName]] = value;
            }
        }

        public IEnumerator<TOM.MetadataPermission> GetEnumerator()
        {
            return RLSMap.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public string GetDisplayName(string key)
        {
            return key;
        }
    }

#endif
}
