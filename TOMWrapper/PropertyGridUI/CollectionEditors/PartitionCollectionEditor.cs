using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.TOMWrapper;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.PropertyGridUI
{
    internal class PartitionCollectionEditor: ClonableObjectCollectionEditor<Partition>
    {
        public PartitionCollectionEditor(Type type) : base(type)
        {

        }

        protected override CollectionForm CreateCollectionForm()
        {
            // HACK: By setting the private "newItemTypes" field of the CollectionEditor to null, we force the CreateNewItemTypes() method to be called upon every launch of the form:
            var newItemTypesFieldInfo = typeof(CollectionEditor).GetField("newItemTypes", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            newItemTypesFieldInfo.SetValue(this, null);
            return base.CreateCollectionForm();
        }

        protected override Type[] CreateNewItemTypes()
        {
            var cl = TabularModelHandler.Singleton.CompatibilityLevel;
            if (cl >= 1400)
            {
                if (TabularModelHandler.Singleton.Model.DataSources.Any(ds => ds.Type == DataSourceType.Provider))
                    return cl >= 1561 
                        ? new[] { typeof(Partition), typeof(MPartition), typeof(EntityPartition) } 
                        : new[] { typeof(Partition), typeof(MPartition) };
                else
                    return cl >= 1561 
                        ? new[] { typeof(MPartition), typeof(EntityPartition) } 
                        : new[] { typeof(MPartition) };
            }
            else
                return base.CreateNewItemTypes();
        }

        protected override object SetItems(object editValue, object[] value)
        {
            var col = editValue as PartitionCollection;
            if (col.Parent is CalculatedTable && value.Length > 1) throw new InvalidOperationException("A calculated table cannot have more than 1 partition.");
            if (value.Length == 0) throw new InvalidOperationException("A table must always have at least 1 partition.");
            return base.SetItems(editValue, value);
        }

        Table table;

        protected override object[] GetItems(object editValue)
        {
            table = (editValue as PartitionCollection).Table;
            return base.GetItems(editValue);
        }

        protected override object CreateInstance(Type itemType)
        {
            if(itemType == typeof(MPartition)) {
                return MPartition.CreateNew(table);
            }
            if(itemType == typeof(Partition))
            {
                return Partition.CreateNew(table);
            }
            if (itemType == typeof(EntityPartition))
            {
                return EntityPartition.CreateNew(table);
            }
            return base.CreateInstance(itemType);
        }
    }
}
