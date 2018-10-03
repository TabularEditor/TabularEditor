using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper.Utils;

namespace TabularEditor.TOMWrapper
{
    public class RLSFilterExpression : IDaxDependantObject
    {
        public ModelRole Role;
        public Table Table;

        static internal RLSFilterExpression Get(ModelRole role, Table table)
        {
            RLSFilterExpression result;
            if(!role.RowLevelSecurity.FilterExpressions.TryGetValue(table, out result)) {
                result = new RLSFilterExpression(role, table);
                role.RowLevelSecurity._filterExpressions.Add(table, result);
            }
            return result;
        }

        private RLSFilterExpression(ModelRole role, Table table)
        {
            Role = role;
            Table = table;
        }

        private DependsOnList _dependsOn = null;

        public event PropertyChangedEventHandler PropertyChanged;

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

        public bool IsRemoved => false;

        public int MetadataIndex => -1;

        public Model Model => Table.Model;

        public ObjectType ObjectType => ObjectType.RLSFilterExpression;
    }
}
