using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    public partial class DataSource
    {
        public IEnumerable<Table> UsedByTables
        {
            get { return Model.Tables.Where(t => t.Partitions.Any(p => p.DataSource == this)); }
        }

        public IEnumerable<Partition> UsedByPartitions
        {
            get { return Model.Tables.SelectMany(t => t.Partitions).Where(p => p.DataSource == this); }
        }
    }

    public partial class DataSourceCollection
    {
        /// <summary>
        /// Replaces any occurence of the specified "keyword" with the specified "replaceWith", in the
        /// ConnectionString property of all Provider Data Sources in the collection.
        /// </summary>
        public void SetPlaceholder(string keyword, string replaceWith)
        {
            this.OfType<ProviderDataSource>().ToList().ForEach(ds => ds.SetPlaceholder(keyword, replaceWith));
        }
    }
}
