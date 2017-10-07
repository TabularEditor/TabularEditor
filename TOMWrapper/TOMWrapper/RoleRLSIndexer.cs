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
    /// The RoleRLSIndexer is used to browse all filters across all tables in the model, for
    /// one specific role. This is in contrast to the TableRLSIndexer, which browses the
    /// filters across all roles in the model, for one specific table.
    /// </summary>
    [TypeConverter(typeof(IndexerConverter))]
    public class RoleRLSIndexer : IEnumerable<string>, IExpandableIndexer
    {
        public void Refresh()
        {
            RLSMap = Role.Model.Tables.ToDictionary(
                t => t,
                t => Role.MetadataObject.TablePermissions.Contains(t.Name) ? Role.MetadataObject.TablePermissions[t.Name].FilterExpression : null
                );
        }

        [IntelliSense("Copies all RLSs from another RLS collection.")]
        public void CopyFrom(RoleRLSIndexer source)
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
            foreach (var key in Keys.ToList()) this[key] = "";
            Role.Handler.EndUpdate();
        }

        private Dictionary<Table, string> rlsMap;
        protected ModelRole Role;

        public string Summary
        {
            get
            {
                Refresh();
                return string.Format("RLS defined on {0} out of {1} tables", this.Count(p => !string.IsNullOrEmpty(p)), Role.Model.Tables.Count);
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return RLSMap.Keys.Select(k => k.Name);
            }
        }

        protected Dictionary<Table, string> RLSMap
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
                this[index] = (string)value;
            }
        }

        public RoleRLSIndexer(ModelRole role)
        {
            Role = role;
            Refresh();
        }

        public virtual string this[Table table]
        {
            get {
                if (RLSMap == null) Refresh();
                if (!RLSMap.ContainsKey(table)) Refresh();
                return RLSMap[table];
            }
            set
            {
                var oldValue = this[table];
                if (value == oldValue && !string.IsNullOrEmpty(value)) return;

                RLSMap[table] = value;
                SetRLS(table, value);
            }
        }

        protected void SetRLS(Table table, string filterExpression)
        {
            var tps = Role.MetadataObject.TablePermissions;
            var tp = tps.Contains(table.Name) ? tps[table.Name] : null;
            if (string.IsNullOrEmpty(filterExpression) && tp == null) return;

            if(string.IsNullOrEmpty(filterExpression) && tp != null)
            {
                Role.Handler.UndoManager.Add(new UndoPropertyChangedAction(Role, "RowLevelSecurity", tp.FilterExpression, null, table.Name));
                tps.Remove(tp);
                return;
            }
            if(!string.IsNullOrEmpty(filterExpression) && tp == null)
            {
                tp = new TOM.TablePermission() { Table = table.MetadataObject };
                tps.Add(tp);
            }
            var oldValue = tp.FilterExpression;
            tp.FilterExpression = filterExpression;

            Role.Handler.UndoManager.Add(new UndoPropertyChangedAction(Role, "RowLevelSecurity", oldValue, filterExpression, table.Name));
        }

        public string this[string tableName]
        {
            get {
                return this[Role.Model.Tables[tableName]];
            }
            set
            {
                this[Role.Model.Tables[tableName]] = value;
            }
        }

        public IEnumerator<string> GetEnumerator()
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
}
