using Microsoft.AnalysisServices.Tabular;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TabularEditor.PropertyGridUI.CollectionEditors
{
    // TODO: Consolidate this class and the DataSourceOptionsPropertyCollectionEditor
    internal class DataSourceOptionsPropertyCollectionEditor: RefreshGridCollectionEditor
    {
        public DataSourceOptionsPropertyCollectionEditor(): base(typeof(Collection<DataSourceOptionsProperty>))
        {

        }
        protected override object[] GetItems(object editValue)
        {
            if (editValue is object[] editValueResult)
            {
                return editValueResult;
            }
            else if (editValue is TOMWrapper.StructuredDataSource.DataSourceOptionsImpl opt)
            {
                this.opt = opt;
                this.lastAssigned = opt.Keys.Select(k => new DataSourceOptionsProperty(k, opt[k].ToString())).ToArray();
                return lastAssigned;
            }
            else
                throw new NotSupportedException();
        }

        internal DataSourceOptionsProperty[] lastAssigned;
        private TOMWrapper.StructuredDataSource.DataSourceOptionsImpl opt;
        
        protected override object SetItems(object editValue, object[] value)
        {
            if (Cancelled) return editValue;
            lastAssigned = value.Cast<DataSourceOptionsProperty>().ToArray();
            return value;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if(!Cancelled && lastAssigned != null)
            {
                // Add new items / remove non-existing items:
                foreach(var optProp in lastAssigned.Where(prop => prop.Value != null))
                {
                    opt[optProp.Property] = optProp.Value;
                }
                foreach(var optKey in opt.Keys)
                {
                    if (!lastAssigned.Any(cp => cp.Property == optKey))
                        opt[optKey] = null;
                }
            }

            base.OnFormClosed(e);
        }

        protected override object CreateInstance(Type itemType)
        {
            return new DataSourceOptionsProperty(this);
        }
    }

    internal class DataSourceOptionsProperty
    {
        private readonly DataSourceOptionsPropertyCollectionEditor owner;
        public DataSourceOptionsProperty(DataSourceOptionsPropertyCollectionEditor owner)
        {
            this.owner = owner;
        }
        public DataSourceOptionsProperty(string property, string value)
        {
            this.property = property;
            this.Value = value;
        }

        private string property;
        [Category("Basic")]
        public string Property
        {
            get => this.property;
            set
            {
                if (owner.lastAssigned.Any(p => p.Property == value))
                    throw new ArgumentException("The DataSourceOptions collection already contains this property");
                this.property = value;
            }
        }
        [Category("Basic")]
        public string Value { get; set; }
        public override string ToString()
        {
            return Property;
        }
    }
}
