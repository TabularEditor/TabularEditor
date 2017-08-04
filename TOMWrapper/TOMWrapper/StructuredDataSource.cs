using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;
using TabularEditor.UndoFramework;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
#if CL1400
    public partial class StructuredDataSource
    {
        protected override void Init()
        {
            base.Init();
            if (MetadataObject.ConnectionDetails == null) MetadataObject.ConnectionDetails = new TOM.ConnectionDetails();
        }
        private void SetValue(string org, string value, Action<string> setter, string propertyName)
        {
            var oldValue = org;
            if (oldValue == value) return;
            bool undoable = true;
            bool cancel = false;
            OnPropertyChanging(propertyName, value, ref undoable, ref cancel);
            if (cancel) return;
            setter(value);
            if (undoable) Handler.UndoManager.Add(new UndoPropertyChangedAction(this, propertyName, oldValue, value));
            OnPropertyChanged(propertyName, oldValue, value);
        }

        [Category("Basic")]
        public string Protocol {
            get { return MetadataObject.ConnectionDetails.Protocol; }
            set { SetValue(Protocol, value, v => { MetadataObject.ConnectionDetails.Protocol = v; }, "Protocol"); }
        }

        [Category("Connection Details")]
        public string Account
        {
            get { return MetadataObject.ConnectionDetails.Address.Account; }
            set { SetValue(Account, value, v => { MetadataObject.ConnectionDetails.Address.Account = v; }, "Account"); }
        }

        [Category("Connection Details"),DisplayName("Connection String")]
        public string ConnectionString
        {
            get { return MetadataObject.ConnectionDetails.Address.ConnectionString; }
            set { SetValue(ConnectionString, value, v => { MetadataObject.ConnectionDetails.Address.ConnectionString = v; }, "ConnectionString"); }
        }

        [Category("Connection Details")]
        public string ContentType
        {
            get { return MetadataObject.ConnectionDetails.Address.ContentType; }
            set { SetValue(ContentType, value, v => { MetadataObject.ConnectionDetails.Address.ContentType = v; }, "ContentType"); }
        }

        [Category("Connection Details")]
        public string Database
        {
            get { return MetadataObject.ConnectionDetails.Address.Database; }
            set { SetValue(Database, value, v => { MetadataObject.ConnectionDetails.Address.Database = v; }, "Database"); }
        }

        [Category("Connection Details")]
        public string Domain
        {
            get { return MetadataObject.ConnectionDetails.Address.Domain; }
            set { SetValue(Domain, value, v => { MetadataObject.ConnectionDetails.Address.Domain = v; }, "Domain"); }
        }

        [Category("Connection Details")]
        public string EmailAddress
        {
            get { return MetadataObject.ConnectionDetails.Address.EmailAddress; }
            set { SetValue(EmailAddress, value, v => { MetadataObject.ConnectionDetails.Address.EmailAddress = v; }, "EmailAddress"); }
        }

        [Category("Connection Details")]
        public string Model
        {
            get { return MetadataObject.ConnectionDetails.Address.Model; }
            set { SetValue(Model, value, v => { MetadataObject.ConnectionDetails.Address.Model = v; }, "Model"); }
        }

        [Category("Connection Details")]
        public string Object
        {
            get { return MetadataObject.ConnectionDetails.Address.Object; }
            set { SetValue(Object, value, v => { MetadataObject.ConnectionDetails.Address.Object = v; }, "Object"); }
        }

        [Category("Connection Details")]
        public string Path
        {
            get { return MetadataObject.ConnectionDetails.Address.Path; }
            set { SetValue(Path, value, v => { MetadataObject.ConnectionDetails.Address.Path = v; }, "Path"); }
        }

        [Category("Connection Details")]
        public string Property
        {
            get { return MetadataObject.ConnectionDetails.Address.Property; }
            set { SetValue(Property, value, v => { MetadataObject.ConnectionDetails.Address.Property = v; }, "Property"); }
        }

        [Category("Connection Details")]
        public string Resource
        {
            get { return MetadataObject.ConnectionDetails.Address.Resource; }
            set { SetValue(Resource, value, v => { MetadataObject.ConnectionDetails.Address.Resource = v; }, "Resource"); }
        }

        [Category("Connection Details")]
        public string Schema
        {
            get { return MetadataObject.ConnectionDetails.Address.Schema; }
            set { SetValue(Schema, value, v => { MetadataObject.ConnectionDetails.Address.Schema = v; }, "Schema"); }
        }

        [Category("Connection Details")]
        public string Server
        {
            get { return MetadataObject.ConnectionDetails.Address.Server; }
            set { SetValue(Server, value, v => { MetadataObject.ConnectionDetails.Address.Server = v; }, "Server"); }
        }

        [Category("Connection Details")]
        public string Url
        {
            get { return MetadataObject.ConnectionDetails.Address.Url; }
            set { SetValue(Url, value, v => { MetadataObject.ConnectionDetails.Address.Url = v; }, "Url"); }
        }

        [Category("Connection Details")]
        public string View
        {
            get { return MetadataObject.ConnectionDetails.Address.View; }
            set { SetValue(View, value, v => { MetadataObject.ConnectionDetails.Address.View = v; }, "View"); }
        }
    }

#endif
}
