using Newtonsoft.Json;
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
    /// The TableRLSIndexer is used to browse all filters defined on one specific table, across
    /// all roles in the model. This is in contrast to the RoleRLSIndexer, which browses the
    /// filters across all tables for one specific role.
    /// </summary>
    [TypeConverter(typeof(IndexerConverter))]
    public class TableRLSIndexer : IEnumerable<string>, IExpandableIndexer
    {
        public string ToJson()
        {
            return JsonConvert.SerializeObject(Keys.Where(k => !string.IsNullOrEmpty(this[k])).ToDictionary(k => k, k => this[k]));
        }

        public void Refresh()
        {
            RLSMap = Table.Model.Roles.ToDictionary(
                r => r,
                r => r.MetadataObject.TablePermissions.Contains(Table.Name) ? r.MetadataObject.TablePermissions[Table.Name].FilterExpression : null
                );
        }

        public void CopyFrom(Dictionary<string, string> source)
        {
            Clear();
            foreach(var r in Keys)
            {
                if (source.ContainsKey(r)) this[r] = source[r];
            }
        }

        [IntelliSense("Copies all RLSs from another RLS collection.")]
        public void CopyFrom(TableRLSIndexer source)
        {
            foreach(var p in source.Keys)
            {
                var value = source[p];
                this[p] = value;
            }
        }

        [IntelliSense("Removes the RLS on this table from all roles.")]
        /// <summary>
        /// Removes the object from all RLSs.
        /// </summary>
        public void Clear()
        {
            Table.Handler.BeginUpdate("no RLSs");
            foreach (var key in Keys.ToList()) this[key] = "";
            Table.Handler.EndUpdate();
        }

        private Dictionary<ModelRole, string> rlsMap;
        protected Table Table;

        public string Summary
        {
            get
            {
                Refresh();
                return string.Format("RLS defined on {0} out of {1} roles", this.Count(p => !string.IsNullOrEmpty(p)), Table.Model.Roles.Count);
            }
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return RLSMap.Keys.Select(k => k.Name);
            }
        }

        protected Dictionary<ModelRole, string> RLSMap
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

        public TableRLSIndexer(Table table)
        {
            Table = table;
            Refresh();
        }

        public virtual string this[ModelRole role]
        {
            get {
                if (RLSMap == null) Refresh();
                if (!RLSMap.ContainsKey(role)) Refresh();
                return RLSMap[role];
            }
            set
            {
                var oldValue = this[role];
                if (value == oldValue) return;

                RLSMap[role] = value;
                SetRLS(role, value);
            }
        }

        protected void SetRLS(ModelRole role, string filterExpression)
        {
            role.RowLevelSecurity[Table] = filterExpression;
        }

        public string this[string roleName]
        {
            get {
                return this[Table.Model.Roles[roleName]];
            }
            set
            {
                this[Table.Model.Roles[roleName]] = value;
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
