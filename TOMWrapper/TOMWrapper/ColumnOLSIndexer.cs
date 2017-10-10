extern alias json;

using json.Newtonsoft.Json;
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
    /// The ColumnOLSIndexer is used to browse all filters defined on one specific table, across
    /// all roles in the model. This is in contrast to the RoleOLSIndexer, which browses the
    /// filters across all tables for one specific role.
    /// </summary>
    [TypeConverter(typeof(IndexerConverter))]
    public class ColumnOLSIndexer : IEnumerable<TOM.MetadataPermission>, IExpandableIndexer
    {
        public string ToJson()
        {
            return JsonConvert.SerializeObject(Keys.Where(k => this[k] != TOM.MetadataPermission.Default).ToDictionary(k => k, k => this[k]));
        }

        public void Refresh()
        {
            OLSMap = Column.Model.Roles.ToDictionary(
                r => r,
                r =>
                {
                    var tps = r.MetadataObject.TablePermissions;
                    if (!tps.Contains(Column.Table.Name)) return TOM.MetadataPermission.Default;
                    var tp = tps[Column.Table.Name];
                    if (!tp.ColumnPermissions.Contains(Column.Name)) return TOM.MetadataPermission.Default;
                    var cp = tp.ColumnPermissions[Column.Name];
                    return cp.MetadataPermission;
                }
                );
        }

        public void CopyFrom(Dictionary<string, TOM.MetadataPermission> source)
        {
            Clear();
            foreach (var k in Keys)
            {
                if (source.ContainsKey(k)) this[k] = source[k];
            }
        }

        [IntelliSense("Copies all OLSs from another OLS collection.")]
        public void CopyFrom(ColumnOLSIndexer source)
        {
            foreach(var p in source.Keys)
            {
                var value = source[p];
                this[p] = value;
            }
        }

        [IntelliSense("Removes the OLS on this table from all roles.")]
        /// <summary>
        /// Removes the object from all OLSs.
        /// </summary>
        public void Clear()
        {
            Column.Handler.BeginUpdate("no OLSs");
            foreach (var key in Keys.ToList()) this[key] = TOM.MetadataPermission.Default;
            Column.Handler.EndUpdate();
        }

        private Dictionary<ModelRole, TOM.MetadataPermission> olsMap;
        protected Column Column;

        public string Summary
        {
            get
            {
                Refresh();
                return string.Format("OLS defined on {0} out of {1} roles", this.Count(p => p != TOM.MetadataPermission.Default), Column.Model.Roles.Count);
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return OLSMap.Keys.Select(k => k.Name);
            }
        }

        protected Dictionary<ModelRole, TOM.MetadataPermission> OLSMap
        {
            get
            {
                if (olsMap == null) Refresh();
                return olsMap;
            }

            set
            {
                olsMap = value;
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

        public ColumnOLSIndexer(Column column)
        {
            Column = column;
            Refresh();
        }

        public virtual TOM.MetadataPermission this[ModelRole role]
        {
            get {
                if (OLSMap == null) Refresh();
                if (!OLSMap.ContainsKey(role)) Refresh();
                return OLSMap[role];
            }
            set
            {
                var oldValue = this[role];
                if (value == oldValue) return;

                OLSMap[role] = value;
                SetOLS(role, value);
            }
        }

        protected void SetOLS(ModelRole role, TOM.MetadataPermission permission)
        {
            var tps = role.MetadataObject.TablePermissions;
            var tp = tps.Contains(Column.Table.Name) ? tps[Column.Table.Name] : null;
            if (tp == null && permission != TOM.MetadataPermission.Default) {
                tp = new TOM.TablePermission { Table = Column.Table.MetadataObject };
                tps.Add(tp);
            }
             
            var cp = tp.ColumnPermissions.Contains(Column.Name) ? tp.ColumnPermissions[Column.Name] : null;
            if (cp == null && permission != TOM.MetadataPermission.Default)
            {
                cp = new TOM.ColumnPermission { Column = Column.MetadataObject };
                tp.ColumnPermissions.Add(cp);
            }

            Column.Handler.UndoManager.Add(new UndoPropertyChangedAction(Column, "ObjectLevelSecurity", cp.MetadataPermission, permission, role.Name));
            if (permission != TOM.MetadataPermission.Default)
            {
                cp.MetadataPermission = permission;
            }
            else if (cp != null)
            {
                tp.ColumnPermissions.Remove(cp);
            }
        }

        public TOM.MetadataPermission this[string roleName]
        {
            get {
                return this[Column.Model.Roles[roleName]];
            }
            set
            {
                this[Column.Model.Roles[roleName]] = value;
            }
        }

        public IEnumerator<TOM.MetadataPermission> GetEnumerator()
        {
            return OLSMap.Values.GetEnumerator();
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
