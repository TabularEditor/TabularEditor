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
    internal class ConnectionAddressPropertyCollectionEditor : RefreshGridCollectionEditor
    {
        public ConnectionAddressPropertyCollectionEditor() : base(typeof(Collection<ConnectionAddressProperty>))
        {

        }
        protected override object[] GetItems(object editValue)
        {
            if (editValue is object[] editValueResult)
            {
                return editValueResult;
            }
            else if (editValue is TOMWrapper.StructuredDataSource.AddressImpl adr)
            {
                this.adr = adr;
                this.lastAssigned = adr.Keys.Select(k => new ConnectionAddressProperty(k, adr[k].ToString())).ToArray();
                return lastAssigned;
            }
            else
                throw new NotSupportedException();
        }

        internal ConnectionAddressProperty[] lastAssigned;
        private TOMWrapper.StructuredDataSource.AddressImpl adr;

        protected override object SetItems(object editValue, object[] value)
        {
            if (Cancelled) return editValue;
            lastAssigned = value.Cast<ConnectionAddressProperty>().ToArray();
            return value;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if (!Cancelled && lastAssigned != null)
            {
                // Add new items / remove non-existing items:
                foreach (var adrProp in lastAssigned.Where(prop => prop.Value != null))
                {
                    adr[adrProp.Property] = adrProp.Value;
                }
                foreach (var adrKey in adr.Keys)
                {
                    if (!lastAssigned.Any(cp => cp.Property == adrKey))
                        adr[adrKey] = null;
                }
            }

            base.OnFormClosed(e);
        }

        protected override object CreateInstance(Type itemType)
        {
            return new ConnectionAddressProperty(this);
        }
    }

    internal class ConnectionAddressProperty
    {
        private readonly ConnectionAddressPropertyCollectionEditor owner;
        public ConnectionAddressProperty(ConnectionAddressPropertyCollectionEditor owner)
        {
            this.owner = owner;
        }
        public ConnectionAddressProperty(string property, string value)
        {
            this.property = property;
            this.Value = value;
        }

        private string property;
        [Category("Basic"), TypeConverter(typeof(ConnectionAddressPropertyConverter))]
        public string Property
        {
            get => this.property;
            set
            {
                if (owner.lastAssigned.Any(p => p.Property == value))
                    throw new ArgumentException("The ConnectionAddress collection already contains this property");
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


    class ConnectionAddressPropertyConverter : StringConverter
    {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var ConnectionAddressPropertyTypeInfo = typeof(Microsoft.AnalysisServices.Tabular.ConnectionAddressProperty);
            var fieldInfos = ConnectionAddressPropertyTypeInfo.GetFields(BindingFlags.Public | BindingFlags.Static);

            return new StandardValuesCollection(
                fieldInfos.Where(fi => fi.IsLiteral && !fi.IsInitOnly).Select(fi => fi.GetRawConstantValue().ToString()).ToList()
                );
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
    }
}
