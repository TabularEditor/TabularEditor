using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TabularEditor.PropertyGridUI;
using TabularEditor.TOMWrapper.Undo;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class StructuredDataSource
    {
        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if(propertyName == "Name")
            {
                Handler.UndoManager.BeginBatch("Update M expressions");

                // TODO: Very crude search-and-replace. We should lex/tokenize the M expressions instead, to be sure that we're doing this right.
                foreach (var namedExpr in base.Model.Expressions) namedExpr.Expression = namedExpr.Expression.Replace($"#\"{oldValue}\"", $"#\"{newValue}\"");
                foreach (var part in base.Model.AllPartitions.OfType<MPartition>()) part.Expression = part.Expression.Replace($"#\"{oldValue}\"", $"#\"{newValue}\"");

                Handler.UndoManager.EndBatch();
            }

            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }

        protected override void Init()
        {
            base.Init();
            if (MetadataObject.ConnectionDetails == null) MetadataObject.ConnectionDetails = new TOM.ConnectionDetails();
            if (MetadataObject.Credential == null) MetadataObject.Credential = new TOM.Credential();
        }

        [Category("Basic")]
        public string Protocol {
            get { return MetadataObject.ConnectionDetails.Protocol; }
            set { SetValue(Protocol, value, v => { MetadataObject.ConnectionDetails.Protocol = (string)v; }, "Protocol"); }
        }
        private bool ShouldSerializeProtocol() { return false; }

        [Category("Credentials")]
        public string Username
        {
            get { return MetadataObject.Credential.Username; }
            set { SetValue(Username, value, v => { MetadataObject.Credential.Username = (string)v; }, "Username"); }
        }
        private bool ShouldSerializeUsername() { return false; }

        [Category("Credentials"),PasswordPropertyText(true)]
        public string Password
        {
            get { return MetadataObject.Credential.Password; }
            set { SetValue(Password, value, v => { MetadataObject.Credential.Password = (string)v; }, "Password"); }
        }
        private bool ShouldSerializePassword() { return false; }

        [Category("Credentials")]
        public string PrivacySetting
        {
            get { return MetadataObject.Credential.PrivacySetting; }
            set { SetValue(PrivacySetting, value, v => { MetadataObject.Credential.PrivacySetting = (string)v; }, "PrivacySetting"); }
        }
        private bool ShouldSerializePrivacySetting() { return false; }

        [Category("Credentials")]
        public string AuthenticationKind
        {
            get { return MetadataObject.Credential.AuthenticationKind; }
            set { SetValue(AuthenticationKind, value, v => { MetadataObject.Credential.AuthenticationKind = (string)v; }, "AuthenticationKind"); }
        }
        private bool ShouldSerializeAuthenticationKind() { return false; }

        [Category("Credentials")]
        public bool EncryptConnection
        {
            get { return MetadataObject.Credential.EncryptConnection; }
            set { SetValue(EncryptConnection, value, v => { MetadataObject.Credential.EncryptConnection = (bool)v; }, "EncryptConnection"); }
        }
        private bool ShouldSerializeEncryptConnection() { return false; }

        [Category("Connection Details")]
        public string Account
        {
            get { return MetadataObject.ConnectionDetails.Address.Account; }
            set { SetValue(Account, value, v => { MetadataObject.ConnectionDetails.Address.Account = (string)v; }, "Account"); }
        }
        private bool ShouldSerializeAccount() { return false; }

        [Category("Connection Details"),DisplayName("Connection String")]
        public string ConnectionString
        {
            get { return MetadataObject.ConnectionDetails.Address.ConnectionString; }
            set { SetValue(ConnectionString, value, v => { MetadataObject.ConnectionDetails.Address.ConnectionString = (string)v; }, "ConnectionString"); }
        }
        private bool ShouldSerializeConnectionString() { return false; }

        [Category("Connection Details")]
        public string ContentType
        {
            get { return MetadataObject.ConnectionDetails.Address.ContentType; }
            set { SetValue(ContentType, value, v => { MetadataObject.ConnectionDetails.Address.ContentType = (string)v; }, "ContentType"); }
        }
        private bool ShouldSerializeContentType() { return false; }

        [Category("Connection Details")]
        public string Database
        {
            get { return MetadataObject.ConnectionDetails.Address.Database; }
            set { SetValue(Database, value, v => { MetadataObject.ConnectionDetails.Address.Database = (string)v; }, "Database"); }
        }
        private bool ShouldSerializeDatabase() { return false; }

        [Category("Connection Details")]
        public string Domain
        {
            get { return MetadataObject.ConnectionDetails.Address.Domain; }
            set { SetValue(Domain, value, v => { MetadataObject.ConnectionDetails.Address.Domain = (string)v; }, "Domain"); }
        }
        private bool ShouldSerializeDomain() { return false; }

        [Category("Connection Details")]
        public string EmailAddress
        {
            get { return MetadataObject.ConnectionDetails.Address.EmailAddress; }
            set { SetValue(EmailAddress, value, v => { MetadataObject.ConnectionDetails.Address.EmailAddress = (string)v; }, "EmailAddress"); }
        }
        private bool ShouldSerializeEmailAddress() { return false; }

        [Category("Connection Details")]
        public new string Model
        {
            get { return MetadataObject.ConnectionDetails.Address.Model; }
            set { SetValue(Model, value, v => { MetadataObject.ConnectionDetails.Address.Model = (string)v; }, "Model"); }
        }
        private bool ShouldSerializeModel() { return false; }

        [Category("Connection Details")]
        public string Object
        {
            get { return MetadataObject.ConnectionDetails.Address.Object; }
            set { SetValue(Object, value, v => { MetadataObject.ConnectionDetails.Address.Object = (string)v; }, "Object"); }
        }
        private bool ShouldSerializeObject() { return false; }

        [Category("Connection Details")]
        public string Path
        {
            get { return MetadataObject.ConnectionDetails.Address.Path; }
            set { SetValue(Path, value, v => { MetadataObject.ConnectionDetails.Address.Path = (string)v; }, "Path"); }
        }
        private bool ShouldSerializePath() { return false; }

        [Category("Connection Details")]
        public string Property
        {
            get { return MetadataObject.ConnectionDetails.Address.Property; }
            set { SetValue(Property, value, v => { MetadataObject.ConnectionDetails.Address.Property = (string)v; }, "Property"); }
        }
        private bool ShouldSerializeProperty() { return false; }

        [Category("Connection Details")]
        public string Resource
        {
            get { return MetadataObject.ConnectionDetails.Address.Resource; }
            set { SetValue(Resource, value, v => { MetadataObject.ConnectionDetails.Address.Resource = (string)v; }, "Resource"); }
        }
        private bool ShouldSerializeResource() { return false; }

        [Category("Connection Details")]
        public string Schema
        {
            get { return MetadataObject.ConnectionDetails.Address.Schema; }
            set { SetValue(Schema, value, v => { MetadataObject.ConnectionDetails.Address.Schema = (string)v; }, "Schema"); }
        }
        private bool ShouldSerializeSchema() { return false; }

        [Category("Connection Details")]
        public string Server
        {
            get { return MetadataObject.ConnectionDetails.Address.Server; }
            set { SetValue(Server, value, v => { MetadataObject.ConnectionDetails.Address.Server = (string)v; }, "Server"); }
        }
        private bool ShouldSerializeServer() { return false; }

        [Category("Connection Details")]
        public string Url
        {
            get { return MetadataObject.ConnectionDetails.Address.Url; }
            set { SetValue(Url, value, v => { MetadataObject.ConnectionDetails.Address.Url = (string)v; }, "Url"); }
        }
        private bool ShouldSerializeUrl() { return false; }

        [Category("Connection Details")]
        public string View
        {
            get { return MetadataObject.ConnectionDetails.Address.View; }
            set { SetValue(View, value, v => { MetadataObject.ConnectionDetails.Address.View = (string)v; }, "View"); }
        }
        private bool ShouldSerializeView() { return false; }
    }
    
}
