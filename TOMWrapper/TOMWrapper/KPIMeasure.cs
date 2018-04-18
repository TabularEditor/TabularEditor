using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper.Utils;

namespace TabularEditor.TOMWrapper
{
    public enum KPIMeasureType
    {
        Goal,
        Status,
        Trend
    }

    public class KPIMeasure : IDaxObject, IDaxDependantObject, IDynamicPropertyObject
    {

        public KPI Kpi { get; private set; }
        public Measure Measure => Kpi.Measure;
        public KPIMeasureType Type { get; private set; }
        internal KPIMeasure(KPI kpi, KPIMeasureType type)
        {
            Kpi = kpi;
            Type = type;
        }

        public string DaxObjectFullName => DaxObjectName;

        public string DaxObjectName => $"[_{Measure.Name} {Type}]";

        public string DaxTableName => Measure.DaxTableName;

        [DisplayName("Format String")]
        public string FormatString
        {
            get
            {
                return Type == KPIMeasureType.Goal ? Kpi.TargetFormatString : null;
            }
            set
            {
                if (Type == KPIMeasureType.Goal) Kpi.TargetFormatString = value;
            }
        }

        [DisplayName("Graphic")]
        public string Graphic
        {
            get
            {
                switch (Type)
                {
                    case KPIMeasureType.Status: return Kpi.StatusGraphic;
                    case KPIMeasureType.Trend: return Kpi.TrendGraphic;
                    default: return null;
                }
            }
            set
            {
                switch (Type)
                {
                    case KPIMeasureType.Status: Kpi.StatusGraphic = value; break;
                    case KPIMeasureType.Trend: Kpi.TrendGraphic = value; break;
                }
            }
        }

        [DisplayName("Expression")]
        public string Expression
        {
            get
            {
                switch (Type)
                {
                    case KPIMeasureType.Status: return Kpi.StatusExpression;
                    case KPIMeasureType.Trend: return Kpi.TrendExpression;
                    case KPIMeasureType.Goal: return Kpi.TargetExpression;
                    default: return null;
                }
            }
            set
            {
                switch (Type)
                {
                    case KPIMeasureType.Status: Kpi.StatusExpression = value; break;
                    case KPIMeasureType.Trend: Kpi.TrendExpression = value; break;
                    case KPIMeasureType.Goal: Kpi.TargetExpression = value; break;
                }
            }
        }

        [DisplayName("Description")]
        public string Description
        {
            get
            {
                switch (Type)
                {
                    case KPIMeasureType.Status: return Kpi.StatusDescription;
                    case KPIMeasureType.Trend: return Kpi.TrendDescription;
                    case KPIMeasureType.Goal: return Kpi.TargetDescription;
                    default: return null;
                }
            }
            set
            {
                switch (Type)
                {
                    case KPIMeasureType.Status: Kpi.StatusDescription = value; break;
                    case KPIMeasureType.Trend: Kpi.TrendDescription = value; break;
                    case KPIMeasureType.Goal: Kpi.TargetDescription = value; break;
                }
            }
        }

        private DependsOnList _dependsOn;

        public DependsOnList DependsOn
        {
            get
            {
                if (_dependsOn == null)
                    _dependsOn = new DependsOnList(this);
                return _dependsOn;
            }
        }

        public bool IsRemoved => Kpi.IsRemoved;

        public int MetadataIndex => Measure.MetadataIndex;

        public Model Model => Kpi.Model;

        public string Name
        {
            get { return Type.ToString(); }
            set { }
        }

        public ObjectType ObjectType => ObjectType.KPIMeasure;

        public ReferencedByList ReferencedBy { get; } = new ReferencedByList();

        public event PropertyChangedEventHandler PropertyChanged;

        public bool CanDelete() { return false; }

        public bool CanDelete(out string message) { message = ""; return false; }

        public void Delete() { }

        public bool Browsable(string propertyName)
        {
            switch (propertyName)
            {
                case "FormatString": return Type == KPIMeasureType.Goal;
                case "Graphic": return Type == KPIMeasureType.Status || Type == KPIMeasureType.Trend;
                case "Description":
                case "Expression":
                    return true;
                default:
                    return false;
            }
        }

        public bool Editable(string propertyName)
        {
            return true;
        }
    }
}
