using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.PropertyGridUI
{
    public class PartitionCollectionEditor: RefreshGridCollectionEditor
    {
        public PartitionCollectionEditor(Type type) : base(type)
        {

        }

        protected override Type[] CreateNewItemTypes()
        {
            if (TabularModelHandler.Singleton.Model.Database.CompatibilityLevel >= 1400)
            {
                if (TabularModelHandler.Singleton.Model.DataSources.Any(ds => ds.Type == TOM.DataSourceType.Provider))
                    return new[] { typeof(Partition), typeof(MPartition) };
                else
                    return new[] { typeof(MPartition) };
            }
            else
                return base.CreateNewItemTypes();
        }
    }
}
