using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using TabularEditor.PropertyGridUI;
using TabularEditor.PropertyGridUI.CollectionEditors;
using TabularEditor.TOMWrapper.Undo;
using TOM = Microsoft.AnalysisServices.Tabular;

namespace TabularEditor.TOMWrapper
{
    public partial class StructuredDataSource
    {
        protected override void OnPropertyChanged(string propertyName, object oldValue, object newValue)
        {
            if (propertyName == "Name")
            {
                Handler.UndoManager.BeginBatch("Update M expressions");

                // TODO: Very crude search-and-replace. We should lex/tokenize the M expressions instead, to be sure that we're doing this right.
                foreach (var namedExpr in base.Model.Expressions) if (namedExpr.Expression != null) namedExpr.Expression = namedExpr.Expression.Replace($"#\"{oldValue}\"", $"#\"{newValue}\"");
                foreach (var part in base.Model.AllPartitions.OfType<MPartition>()) if (part.Expression != null) part.Expression = part.Expression.Replace($"#\"{oldValue}\"", $"#\"{newValue}\"");

                Handler.UndoManager.EndBatch();
            }

            base.OnPropertyChanged(propertyName, oldValue, newValue);
        }

        protected override void Init()
        {
            base.Init();
            if (MetadataObject.ConnectionDetails == null) MetadataObject.ConnectionDetails = new TOM.ConnectionDetails();
            if (MetadataObject.Credential == null) MetadataObject.Credential = new TOM.Credential();
            if (MetadataObject.Options == null) MetadataObject.Options = new TOM.DataSourceOptions();
            this.Credential = new CredentialImpl(this);
            this.Options = new DataSourceOptionsImpl(this);
            this.Address = new AddressImpl(this);
        }

        [Category("Basic"), Description("Connection protocol"), IntelliSense("Connection protocol")]
        public string Protocol
        {
            get { return MetadataObject.ConnectionDetails.Protocol; }
            set { SetValue(Protocol, value, v => { MetadataObject.ConnectionDetails.Protocol = (string)v; }); }
        }
        private bool ShouldSerializeProtocol() { return false; }

        [Category("Options"), Editor(typeof(DataSourceOptionsPropertyCollectionEditor), typeof(UITypeEditor)), Description("Protocol-specific options used to connect the data source")]
        [IntelliSense("Protocol-specific options used to connect the data source")]
        public DataSourceOptionsImpl Options { get; private set; }

        [Category("Credential"), Editor(typeof(CredentialPropertyCollectionEditor), typeof(UITypeEditor)), Description("Protocol-specific options used to authenticate the connection")]
        [IntelliSense("Protocol-specific options used to authenticate the connection")]
        public CredentialImpl Credential { get; private set; }

        [Category("Connection Details"), Editor(typeof(ConnectionAddressPropertyCollectionEditor), typeof(UITypeEditor)), Description("Protocol-specific address options for this connection")]
        [IntelliSense("Protocol-specific options used to authenticate the connection")]
        public AddressImpl Address { get; private set; }

        [Category("Credential"), Description("Username property or null if it's not set"), IntelliSense("Username property or null if it's not set")]
        public string Username
        {
            get { return MetadataObject.Credential.Username; }
            set { SetValue(Username, value, v => { MetadataObject.Credential.Username = (string)v; }); }
        }
        private bool ShouldSerializeUsername() { return false; }

        [Category("Credential"), Description("Password property or null if it's not set"), IntelliSense("Password property or null if it's not set"), PasswordPropertyText(true)]
        public string Password
        {
            get { return MetadataObject.Credential.Password; }
            set { SetValue(Password, value, v => { MetadataObject.Credential.Password = (string)v; }); }
        }
        private bool ShouldSerializePassword() { return false; }

        [Category("Credential"), Description("Privacy setting from PrivacyClass or null if it's not present in the Credential property. Missing value is equivalent to the value of PrivacyClass.None."), IntelliSense("Privacy setting from PrivacyClass or null if it's not present in the Credential property. Missing value is equivalent to the value of PrivacyClass.None.")]
        public string PrivacySetting
        {
            get { return MetadataObject.Credential.PrivacySetting; }
            set { SetValue(PrivacySetting, value, v => { MetadataObject.Credential.PrivacySetting = (string)v; }); }
        }
        private bool ShouldSerializePrivacySetting() { return false; }

        [Category("Credential"), Description("Authentication kind, usually one of the AuthenticationKind constants"), IntelliSense("Authentication kind, usually one of the AuthenticationKind constants")]
        public string AuthenticationKind
        {
            get { return MetadataObject.Credential.AuthenticationKind; }
            set { SetValue(AuthenticationKind, value, v => { MetadataObject.Credential.AuthenticationKind = (string)v; }); }
        }
        private bool ShouldSerializeAuthenticationKind() { return false; }

        [Category("Credential"), Description("Whether connection must be encrypted"), IntelliSense("Whether connection must be encrypted")]
        public bool EncryptConnection
        {
            get { return MetadataObject.Credential.EncryptConnection; }
            set { SetValue(EncryptConnection, value, v => { MetadataObject.Credential.EncryptConnection = (bool)v; }); }
        }
        private bool ShouldSerializeEncryptConnection() { return false; }

        [Category("Connection Details"), Description("Account (Connection Address proprety)"), IntelliSense("Account (Connection Address proprety)")]
        public string Account
        {
            get { return MetadataObject.ConnectionDetails.Address.Account; }
            set { SetValue(Account, value, v => { MetadataObject.ConnectionDetails.Address.Account = (string)v; }); }
        }
        private bool ShouldSerializeAccount() { return false; }

        [Category("Connection Details"), Description("Connection string (Connection Address property)"), IntelliSense("Connection string (Connection Address property)"), DisplayName("Connection String")]
        public string ConnectionString
        {
            get { return MetadataObject.ConnectionDetails.Address.ConnectionString; }
            set { SetValue(ConnectionString, value, v => { MetadataObject.ConnectionDetails.Address.ConnectionString = (string)v; }); }
        }
        private bool ShouldSerializeConnectionString() { return false; }

        [Category("Connection Details"), Description("Content type (Connection Address property)"), IntelliSense("Content type (Connection Address property)")]
        public string ContentType
        {
            get { return MetadataObject.ConnectionDetails.Address.ContentType; }
            set { SetValue(ContentType, value, v => { MetadataObject.ConnectionDetails.Address.ContentType = (string)v; }); }
        }

        [Category("Connection Details"), Description("Native query (typically a SQL statement) to be sent to the source"), IntelliSense("Native query (typically a SQL statement) to be sent to the source")]
        public string Query
        {
            get
            {
                var json = MetadataObject.ConnectionDetails?.ToJson();
                if (string.IsNullOrEmpty(json)) return null;
                return JObject.Parse(json)["query"]?.ToString(Newtonsoft.Json.Formatting.None);
            }
            set
            {
                SetValue(ContentType, value, v =>
                {
                    var json = MetadataObject.ConnectionDetails?.ToJson();
                    var jObj = string.IsNullOrEmpty(json) ? new JObject() : JObject.Parse(json);
                    jObj["query"] = (string)v; MetadataObject.ConnectionDetails.ParseJson(jObj.ToString());
                });
            }
        }
        private bool ShouldSerializeContentType() { return false; }

        [Category("Connection Details"), Description("Database name (Connection Address property)"), IntelliSense("Database name (Connection Address property)")]
        public string Database
        {
            get { return MetadataObject.ConnectionDetails.Address.Database; }
            set { SetValue(Database, value, v => { MetadataObject.ConnectionDetails.Address.Database = (string)v; }); }
        }
        private bool ShouldSerializeDatabase() { return false; }

        [Category("Connection Details"), Description("Domain (Connection Address property)"), IntelliSense("Domain (Connection Address property)")]
        public string Domain
        {
            get { return MetadataObject.ConnectionDetails.Address.Domain; }
            set { SetValue(Domain, value, v => { MetadataObject.ConnectionDetails.Address.Domain = (string)v; }); }
        }
        private bool ShouldSerializeDomain() { return false; }

        [Category("Connection Details"), Description("Email address (Connection Address property)"), IntelliSense("Email address (Connection Address property)")]
        public string EmailAddress
        {
            get { return MetadataObject.ConnectionDetails.Address.EmailAddress; }
            set { SetValue(EmailAddress, value, v => { MetadataObject.ConnectionDetails.Address.EmailAddress = (string)v; }); }
        }
        private bool ShouldSerializeEmailAddress() { return false; }

        [Category("Connection Details"), Description("Model name (Connection Address property)"), IntelliSense("Model name (Connection Address property)"), DisplayName("Model")]
        public string AddressModel
        {
            get { return MetadataObject.ConnectionDetails.Address.Model; }
            set { SetValue(AddressModel, value, v => { MetadataObject.ConnectionDetails.Address.Model = (string)v; }); }
        }
        private bool ShouldSerializeModel() { return false; }

        [Category("Connection Details"), Description("Object (Connection Address property)"), IntelliSense("Object (Connection Address property)")]
        public string Object
        {
            get { return MetadataObject.ConnectionDetails.Address.Object; }
            set { SetValue(Object, value, v => { MetadataObject.ConnectionDetails.Address.Object = (string)v; }); }
        }
        private bool ShouldSerializeObject() { return false; }

        [Category("Connection Details"), Description("Path (Connection Address property)"), IntelliSense("Path (Connection Address property)")]
        public string Path
        {
            get { return MetadataObject.ConnectionDetails.Address.Path; }
            set { SetValue(Path, value, v => { MetadataObject.ConnectionDetails.Address.Path = (string)v; }); }
        }
        private bool ShouldSerializePath() { return false; }

        [Category("Connection Details"), Description("Property (Connection Address property)"), IntelliSense("Property (Connection Address property)")]
        public string Property
        {
            get { return MetadataObject.ConnectionDetails.Address.Property; }
            set { SetValue(Property, value, v => { MetadataObject.ConnectionDetails.Address.Property = (string)v; }); }
        }
        private bool ShouldSerializeProperty() { return false; }

        [Category("Connection Details"), Description("Resource (Connection Address property)"), IntelliSense("Resource (Connection Address property)")]
        public string Resource
        {
            get { return MetadataObject.ConnectionDetails.Address.Resource; }
            set { SetValue(Resource, value, v => { MetadataObject.ConnectionDetails.Address.Resource = (string)v; }); }
        }
        private bool ShouldSerializeResource() { return false; }

        [Category("Connection Details"), Description("Schema name (Connection Address property)"), IntelliSense("Schema name (Connection Address property)")]
        public string Schema
        {
            get { return MetadataObject.ConnectionDetails.Address.Schema; }
            set { SetValue(Schema, value, v => { MetadataObject.ConnectionDetails.Address.Schema = (string)v; }); }
        }
        private bool ShouldSerializeSchema() { return false; }

        [Category("Connection Details"), Description("Server address (Connection Address property)"), IntelliSense("Server address (Connection Address property)")]
        public string Server
        {
            get { return MetadataObject.ConnectionDetails.Address.Server; }
            set { SetValue(Server, value, v => { MetadataObject.ConnectionDetails.Address.Server = (string)v; }); }
        }
        private bool ShouldSerializeServer() { return false; }

        [Category("Connection Details"), Description("Url (Connection Address property)"), IntelliSense("Url (Connection Address property)")]
        public string Url
        {
            get { return MetadataObject.ConnectionDetails.Address.Url; }
            set { SetValue(Url, value, v => { MetadataObject.ConnectionDetails.Address.Url = (string)v; }); }
        }
        private bool ShouldSerializeUrl() { return false; }

        [Category("Connection Details"), Description("View (Connection Address property)"), IntelliSense("View (Connection Address property)")]
        public string View
        {
            get { return MetadataObject.ConnectionDetails.Address.View; }
            set { SetValue(View, value, v => { MetadataObject.ConnectionDetails.Address.View = (string)v; }); }
        }
        private bool ShouldSerializeView() { return false; }

        [TypeConverter(typeof(IndexerConverter))]
        public class CredentialImpl : IExpandableIndexer
        {
            private readonly StructuredDataSource dataSource;
            private TOM.Credential TomCredential => dataSource.MetadataObject.Credential;
            public CredentialImpl(StructuredDataSource dataSource)
            {
                this.dataSource = dataSource;
            }

            public string Summary => "(Click to edit)";

            public IEnumerable<string> Keys
            {
                get
                {
                    var json = TomCredential.ToString();
                    if (!string.IsNullOrEmpty(json))
                        return JObject.Parse(json).Properties().Select(p => p.Name);
                    else return Enumerable.Empty<string>();
                }
            }

            bool IExpandableIndexer.EnableMultiLine => false;

            public bool EnableMultiLine(string key) => false;

            public object this[string index]
            {
                get => TomCredential[index];
                set
                {
                    if (TomCredential[index] == value) return;
                    var oldValue = TomCredential[index];
                    if (index == nameof(TOM.Credential.EncryptConnection))
                    {
                        if (bool.TryParse(value as string, out bool result))
                        {
                            TomCredential[index] = result;
                            dataSource.Handler.UndoManager.Add(new UndoCredentialAction(dataSource, index, value as string, oldValue as string));
                            dataSource.OnPropertyChanged(index, oldValue, value);
                        }
                    }
                    else
                    {
                        TomCredential[index] = value;
                        dataSource.Handler.UndoManager.Add(new UndoCredentialAction(dataSource, index, value as string, oldValue as string));
                        dataSource.OnPropertyChanged(index, oldValue, value);
                    }
                }
            }


            public string GetDisplayName(string key)
            {
                return key;
            }
        }

        [TypeConverter(typeof(IndexerConverter))]
        public class DataSourceOptionsImpl : IExpandableIndexer
        {
            private readonly StructuredDataSource dataSource;
            private TOM.DataSourceOptions TomOptions => dataSource.MetadataObject.Options;
            public DataSourceOptionsImpl(StructuredDataSource dataSource)
            {
                this.dataSource = dataSource;
            }

            public string Summary => "(Click to edit)";

            public IEnumerable<string> Keys
            {
                get
                {
                    var json = TomOptions.ToString();
                    if (!string.IsNullOrEmpty(json))
                        return JObject.Parse(json).Properties().Select(p => p.Name);
                    else return Enumerable.Empty<string>();
                }
            }

            bool IExpandableIndexer.EnableMultiLine => false;

            public bool EnableMultiLine(string key) => false;

            public object this[string index]
            {
                get
                {
                    var json = TomOptions?.ToJson();
                    if (string.IsNullOrEmpty(json)) return null;
                    return JObject.Parse(json)[index]?.ToString(Newtonsoft.Json.Formatting.None);
                }
                set
                {
                    var oldValue = this[index];
                    if (oldValue == value) return;

                    var json = TomOptions?.ToJson();
                    var jOptions = string.IsNullOrEmpty(json) ? new JObject() : JObject.Parse(json);
                    jOptions[index] = JToken.Parse(value.ToString());
                    TomOptions.ParseJson(jOptions.ToString());
                    dataSource.Handler.UndoManager.Add(new UndoDataSourceOptionsAction(dataSource, index, value as string, oldValue as string));
                }
            }

            public string GetDisplayName(string key)
            {
                return key;
            }
        }

        [TypeConverter(typeof(IndexerConverter))]
        public class AddressOptionsImpl : IExpandableIndexer
        {
            private readonly JObject connectionDetails;
            private readonly StructuredDataSource dataSource;

            public AddressOptionsImpl(StructuredDataSource dataSource)
            {
                this.dataSource = dataSource;
                this.connectionDetails = JObject.Parse(dataSource.MetadataObject.ConnectionDetails.ToJson());
            }

            public object this[string index]
            {
                get => connectionDetails["address"]?["options"]?[index]?.ToString();
                set
                {
                    if (connectionDetails["address"] == null)
                        connectionDetails["address"] = new JObject();
                    if (connectionDetails["address"]["options"] == null)
                        connectionDetails["address"]["options"] = new JObject();

                    connectionDetails["address"][index] = value.ToString();
                    dataSource.MetadataObject.ConnectionDetails.ParseJson(connectionDetails.ToString());
                }
            }

            public string Summary => "(Click to edit)";

            public IEnumerable<string> Keys => connectionDetails["address"]?["options"] is JObject options ? options.Properties().Select(p => p.Name) : Enumerable.Empty<string>();

            bool IExpandableIndexer.EnableMultiLine => false;

            public bool EnableMultiLine(string key) => false;

            public string GetDisplayName(string key)
            {
                return key;
            }
        }

        [TypeConverter(typeof(IndexerConverter))]
        public class AddressImpl : IExpandableIndexer
        {
            private readonly StructuredDataSource dataSource;
            private TOM.ConnectionAddress TomAddress => dataSource.MetadataObject.ConnectionDetails.Address;
            public AddressImpl(StructuredDataSource dataSource)
            {
                this.dataSource = dataSource;
            }

            public string Summary => "(Click to edit)";

            public IEnumerable<string> Keys
            {
                get
                {
                    var json = dataSource.MetadataObject.ConnectionDetails.ToJson();
                    if (!string.IsNullOrEmpty(json))
                        return (JObject.Parse(json)["address"] as JObject)?.Properties().Select(p => p.Name) ?? Enumerable.Empty<string>();
                    else return Enumerable.Empty<string>();
                }
            }

            bool IExpandableIndexer.EnableMultiLine => true;

            public bool EnableMultiLine(string index)
            {
                return index == "options";
            }

            public object this[string index]
            {
                get
                {
                    // For some reason, "options" throws a System.NotImplementedException in Microsoft.AnalysisServices.Tabular.CustomJsonPropertyHelper
                    if (index == "options")
                    {
                        return JObject.Parse(dataSource.MetadataObject.ConnectionDetails.ToJson())["address"]?["options"]?.ToString();
                    }
                    else
                        return TomAddress[index];
                }
                set
                {
                    // TODO: Governance? OnPropertyChanging / OnPropertyChanged calls?

                    var oldValue = this[index];
                    if (oldValue == value) return;
                    if (index == "options")
                    {
                        var jObj = JObject.Parse(dataSource.MetadataObject.ConnectionDetails.ToJson());
                        if (string.IsNullOrEmpty(value?.ToString()))
                        {
                            (jObj["address"] as JObject)?.Remove("options");
                        }
                        else
                        {
                            jObj["address"]["options"] = JObject.Parse(value.ToString());
                        }
                        dataSource.MetadataObject.ConnectionDetails.ParseJson(jObj.ToString());
                    }
                    else
                        TomAddress[index] = value;

                    dataSource.Handler.UndoManager.Add(new UndoConnectionAddressAction(dataSource, index, value as string, oldValue as string));
                }
            }


            public string GetDisplayName(string key)
            {
                return key;
            }
        }
    }

}
