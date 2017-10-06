using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.TOMWrapper
{
    public partial class DataSource
    {
        [Browsable(false)]
        public IEnumerable<Table> UsedByTables
        {
            get { return Model.Tables.Where(t => t.Partitions.Any(p => p.DataSource == this)); }
        }

        [Browsable(false)]
        public IEnumerable<Partition> UsedByPartitions
        {
            get { return Model.AllPartitions.Where(p => p.DataSource == this); }
        }

        protected override bool AllowDelete(out string message)
        {
            if(UsedByPartitions.Any())
            {
                message = Messages.DataSourceInUse;
                return false;
            }
            return base.AllowDelete(out message);
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
