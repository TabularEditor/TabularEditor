using System;
using System.Linq;
using System.Collections.Generic;
using TabularEditor.PropertyGridUI;
using TOM = Microsoft.AnalysisServices.Tabular;
using System.Diagnostics;

namespace TabularEditor.TOMWrapper
{
    public partial class Partition: IDynamicPropertyObject
    {
        public string Query
        {
            get
            {
                return (MetadataObject.Source as TOM.QueryPartitionSource)?.Query;
            }
        }

        public string Source
        {
            get
            {
                return (MetadataObject.Source as TOM.QueryPartitionSource)?.DataSource.Name;
            }
        }

        public string Expression
        {
            get
            {
                return MetadataObject.SourceType == TOM.PartitionSourceType.Calculated ?
                    (MetadataObject.Source as TOM.CalculatedPartitionSource)?.Expression :
                    (MetadataObject.Source as TOM.MPartitionSource)?.Expression;
            }
        }

        public bool Browsable(string propertyName)
        {
            switch(propertyName)
            {
                case "Query":
                    return SourceType == TOM.PartitionSourceType.Query;
                case "Expression":
                    return SourceType == TOM.PartitionSourceType.Calculated;
                case "Mode":
                case "Description":
                case "Name":
                    return true;
                default:
                    return false;
            }
        }

        public bool Editable(string propertyName)
        {
            return false;
        }
    }
}
