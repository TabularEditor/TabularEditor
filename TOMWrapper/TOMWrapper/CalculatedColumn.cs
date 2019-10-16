using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper.Utils;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class CalculatedColumn: IDaxDependantObject
    {
        private DependsOnList _dependsOn = null;

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

        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if (propertyName == Properties.EXPRESSION)
            {
                FormulaFixup.BuildDependencyTree(this);
            }

            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }

        protected override void Init()
        {
            if (!string.IsNullOrEmpty(ErrorMessage)) Table.AddError(this);
            base.Init();
        }


        [Browsable(false)]
        public bool NeedsValidation => IsExpressionModified;
    }
}
