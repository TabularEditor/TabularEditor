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
    internal class CredentialPropertyCollectionEditor: RefreshGridCollectionEditor
    {
        public CredentialPropertyCollectionEditor(): base(typeof(Collection<CredentialProperty>))
        {

        }
        protected override object[] GetItems(object editValue)
        {
            if (editValue is object[] editValueResult)
            {
                return editValueResult;
            }
            else if (editValue is TOMWrapper.StructuredDataSource.CredentialImpl cred)
            {
                this.cred = cred;
                this.lastAssigned = cred.Keys.Select(k => new CredentialProperty(k, cred[k].ToString())).ToArray();
                return lastAssigned;
            }
            else
                throw new NotSupportedException();
        }

        internal CredentialProperty[] lastAssigned;
        private TOMWrapper.StructuredDataSource.CredentialImpl cred;
        
        protected override object SetItems(object editValue, object[] value)
        {
            if (Cancelled) return editValue;
            lastAssigned = value.Cast<CredentialProperty>().ToArray();
            return value;
        }

        protected override void OnFormClosed(FormClosedEventArgs e)
        {
            if(!Cancelled && lastAssigned != null)
            {
                // Add new items / remove non-existing items:
                foreach(var credProp in lastAssigned.Where(prop => prop.Value != null))
                {
                    cred[credProp.Property] = credProp.Value;
                }
                foreach(var credKey in cred.Keys)
                {
                    if (!lastAssigned.Any(cp => cp.Property == credKey))
                        cred[credKey] = null;
                }
            }

            base.OnFormClosed(e);
        }

        protected override object CreateInstance(Type itemType)
        {
            return new CredentialProperty(this);
        }
    }

    internal class CredentialProperty
    {
        private readonly CredentialPropertyCollectionEditor owner;
        public CredentialProperty(CredentialPropertyCollectionEditor owner)
        {
            this.owner = owner;
        }
        public CredentialProperty(string property, string value)
        {
            this.property = property;
            this.Value = value;
        }

        private string property;
        [Category("Basic"), TypeConverter(typeof(CredentialPropertyConverter))]
        public string Property
        {
            get => this.property;
            set
            {
                if (owner.lastAssigned.Any(p => p.Property == value))
                    throw new ArgumentException("The credential collection already contains this property");
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


    class CredentialPropertyConverter : StringConverter
    {
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var credentialPropertyTypeInfo = typeof(Microsoft.AnalysisServices.Tabular.CredentialProperty);
            var fieldInfos = credentialPropertyTypeInfo.GetFields(BindingFlags.Public | BindingFlags.Static);

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
