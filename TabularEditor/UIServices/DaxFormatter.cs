/*
 * Parts of this code has been copied from the DAX Studio source code:
 * https://github.com/DaxStudio/DaxStudio
 * 
 * Please review their license here: 
 * https://github.com/DaxStudio/DaxStudio/blob/master/license.rtf
 */

extern alias json;

using json.Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using TabularEditor.TOMWrapper;
using TabularEditor.UIServices;

namespace TabularEditor.Dax
{
    public class DaxFormatterError
    {
        public int line;
        public int column;
        public string message;
    }

    public class ServerDatabaseInfo
    {
        public string ServerName { get; set; } // SHA-256 hash of server name
        public string ServerEdition { get; set; } // # Values: null, "Enterprise64", "Developer64", "Standard64"
        public string ServerType { get; set; } // Values: null, "SSAS", "PBI Desktop", "SSDT Workspace", "Tabular Editor"
        public string ServerMode { get; set; } // Values: null, "SharePoint", "Tabular"
        public string ServerLocation { get; set; } // Values: null, "OnPremise", "Azure"
        public string ServerVersion { get; set; } // Example: "14.0.800.192"
        public string DatabaseName { get; set; } // SHA-256 hash of database name
        public string DatabaseCompatibilityLevel { get; set; } // Values: 1200, 1400
    }

    public class DaxFormatterRequestSingle : DaxFormatterRequest
    {
        public string Dax { get; set; }

        public DaxFormatterRequestSingle(bool useSemicolonsAsSeparators, bool shortFormat, bool skipSpaceAfterFunctionName) : base(useSemicolonsAsSeparators, shortFormat, skipSpaceAfterFunctionName)
        {

        }
    }

    public class DaxFormatterRequestMulti : DaxFormatterRequest
    {
        public List<string> Dax { get; set; }

        public DaxFormatterRequestMulti(bool useSemicolonsAsSeparators, bool shortFormat, bool skipSpaceAfterFunctionName) : base(useSemicolonsAsSeparators, shortFormat, skipSpaceAfterFunctionName)
        {

        }
    }

    public class DaxFormatterRequest : ServerDatabaseInfo
    {
        public int? MaxLineLenght { get; set; }
        public bool? SkipSpaceAfterFunctionName { get; set; }
        public char ListSeparator { get; set; }
        public char DecimalSeparator { get; set; }
        public string CallerApp { get; set; }
        public string CallerVersion { get; set; }

        public DaxFormatterRequest()
        {
            this.ListSeparator = ',';
            this.DecimalSeparator = '.';

            // Save caller app and version
            var assemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName();
            this.CallerApp = assemblyName.Name;
            this.CallerVersion = assemblyName.Version.ToString();

            var telemetry = ModelTelemetry.Collect();
            if (telemetry != null) PopulateFromTelemetry(telemetry);
        }

        public DaxFormatterRequest(bool useSemicolonsAsSeparators, bool shortFormat, bool skipSpaceAfterFunctionName)
        {
            this.ListSeparator = useSemicolonsAsSeparators ? ';' : ',';
            this.DecimalSeparator = useSemicolonsAsSeparators ? ',' : '.';
            this.MaxLineLenght = shortFormat ? 1 : 0;
            this.SkipSpaceAfterFunctionName = skipSpaceAfterFunctionName;

            // Save caller app and version
            var assemblyName = System.Reflection.Assembly.GetEntryAssembly().GetName();
            this.CallerApp = assemblyName.Name;
            this.CallerVersion = assemblyName.Version.ToString();

            var telemetry = ModelTelemetry.Collect();
            if(telemetry != null) PopulateFromTelemetry(telemetry);
        }

        private void PopulateFromTelemetry(ModelTelemetry telemetry)
        {
            ServerName = telemetry.ServerName;
            ServerEdition = telemetry.ServerEdition;
            ServerType = telemetry.ServerType;
            ServerMode = telemetry.ServerMode;
            ServerLocation = telemetry.ServerLocation;
            ServerVersion = telemetry.ServerVersion;
            DatabaseName = telemetry.DatabaseName;
            DatabaseCompatibilityLevel = telemetry.DatabaseCompatibilityLevel;
        }
    }


    public class DaxFormatterResult
    {
        [JsonProperty(PropertyName = "formatted")]
        public string FormattedDax;
        public List<DaxFormatterError> errors;
    }

    public class DaxFormatterProxy : IDaxFormatterProxy
    {
        public static DaxFormatterProxy Instance = new DaxFormatterProxy();
        private DaxFormatterProxy()
        {
            // force the use of TLS 1.2
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        public const string DaxTextFormatUri = "https://www.daxformatter.com/api/daxformatter/daxtextformat";
        public const string DaxTextFormatMultiUri = "https://www.daxformatter.com/api/daxformatter/daxtextformatmulti";

        private string redirectUrl;  // cache the redirected URL
        private string redirectHost;

        public DaxFormatterResult FormatDax(string query, bool useSemicolonsAsSeparators, bool shortFormat, bool skipSpaceAfterFunctionName)
        {
            string output = CallDaxFormatterSingle(DaxTextFormatUri, query, useSemicolonsAsSeparators, shortFormat, skipSpaceAfterFunctionName);
            var res2 = new DaxFormatterResult();
            if (output.StartsWith("{"))
            {
                JsonConvert.PopulateObject(output, res2);
            } else
            {
                res2.FormattedDax = JsonConvert.DeserializeObject<string>(output);
            }
            return res2;
        }

        public List<DaxFormatterResult> FormatDaxMulti(List<string> dax, bool useSemicolonsAsSeparators, bool shortFormat, bool skipSpaceAfterFunctionName)
        {
            string output = CallDaxFormatterMulti(DaxTextFormatMultiUri, dax, useSemicolonsAsSeparators, shortFormat, skipSpaceAfterFunctionName);

            List<DaxFormatterResult> res;
            if (output.StartsWith("["))
            {
                res = JsonConvert.DeserializeObject<List<DaxFormatterResult>>(output);
            }
            else
            {
                res = new List<DaxFormatterResult>();
            }
            return res;
        }

        private string CallDaxFormatterSingle(string uri, string dax, bool useSemicolonsAsSeparators, bool shortFormat, bool skipSpaceAfterFunctionName)
        {
            try
            {
                PrimeConnection(uri);
                var originalUri = new Uri(uri);
                var actualUri = new UriBuilder(originalUri.Scheme, redirectHost, originalUri.Port, originalUri.PathAndQuery).ToString();

                var req = new DaxFormatterRequestSingle(useSemicolonsAsSeparators, shortFormat, skipSpaceAfterFunctionName)
                {
                    Dax = dax
                };

                var data = JsonConvert.SerializeObject(req, Formatting.Indented);

                var enc = System.Text.Encoding.UTF8;
                var data1 = enc.GetBytes(data);

                // this should allow DaxFormatter to work through http 1.0 proxies
                // see: http://stackoverflow.com/questions/566437/http-post-returns-the-error-417-expectation-failed-c
                //System.Net.ServicePointManager.Expect100Continue = false;

                var wr = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(actualUri);
                wr.Proxy = ProxyCache.GetProxy(actualUri);

                wr.Timeout = Preferences.Current.DaxFormatterRequestTimeout;
                wr.Method = "POST";
                wr.Accept = "application/json, text/javascript, */*; q=0.01";
                wr.Headers.Add("Accept-Encoding", "gzip,deflate");
                wr.Headers.Add("Accept-Language", "en-US,en;q=0.8");
                wr.ContentType = "application/json; charset=UTF-8";
                wr.AutomaticDecompression = DecompressionMethods.GZip;

                string output = "";
                using (var strm = wr.GetRequestStream())
                {
                    strm.Write(data1, 0, data1.Length);

                    using (var resp = wr.GetResponse())
                    {
                        //var outStrm = new System.IO.Compression.GZipStream(resp.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                        var outStrm = resp.GetResponseStream();
                        using (var reader = new System.IO.StreamReader(outStrm))
                        {
                            output = reader.ReadToEnd().Trim();
                        }
                    }
                }

                return output;
            }
            catch
            {
                throw;
            }
            finally
            {
            }
        }

        private string CallDaxFormatterMulti(string uri, List<string> dax, bool useSemicolonsAsSeparators, bool shortFormat, bool skipSpaceAfterFunctionName)
        {
            try
            {
                PrimeConnection(uri);
                var originalUri = new Uri(uri);
                var actualUri = new UriBuilder(originalUri.Scheme, redirectHost, originalUri.Port, originalUri.PathAndQuery).ToString();

                var req = new DaxFormatterRequestMulti(useSemicolonsAsSeparators, shortFormat, skipSpaceAfterFunctionName)
                {
                    Dax = dax
                };

                var data = JsonConvert.SerializeObject(req, Formatting.Indented);

                var enc = System.Text.Encoding.UTF8;
                var data1 = enc.GetBytes(data);

                // this should allow DaxFormatter to work through http 1.0 proxies
                // see: http://stackoverflow.com/questions/566437/http-post-returns-the-error-417-expectation-failed-c
                //System.Net.ServicePointManager.Expect100Continue = false;

                var wr = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(actualUri);
                wr.Proxy = ProxyCache.GetProxy(actualUri);

                wr.Timeout = Preferences.Current.DaxFormatterRequestTimeout;
                wr.Method = "POST";
                wr.Accept = "application/json, text/javascript, */*; q=0.01";
                wr.Headers.Add("Accept-Encoding", "gzip,deflate");
                wr.Headers.Add("Accept-Language", "en-US,en;q=0.8");
                wr.ContentType = "application/json; charset=UTF-8";
                wr.AutomaticDecompression = DecompressionMethods.GZip;

                string output = "";
                using (var strm = wr.GetRequestStream())
                {
                    strm.Write(data1, 0, data1.Length);

                    using (var resp = wr.GetResponse())
                    {
                        //var outStrm = new System.IO.Compression.GZipStream(resp.GetResponseStream(), System.IO.Compression.CompressionMode.Decompress);
                        var outStrm = resp.GetResponseStream();
                        using (var reader = new System.IO.StreamReader(outStrm))
                        {
                            output = reader.ReadToEnd().Trim();
                        }
                    }
                }

                return output;
            }
            catch
            {
                throw;
            }
            finally
            {
            }
        }

        private void PrimeConnection(string uri)
        {
            if (redirectHost == null)
            {
                // www.daxformatter.com redirects request to another site.  HttpWebRequest does redirect with GET.  It fails, since the web service works only with POST
                // The following 2 requests are doing manual POST re-direct
                //var webRequestFactory = IoC.Get<WebRequestFactory>();
                var redirectRequest = (HttpWebRequest)WebRequest.Create(uri);
                redirectRequest.Proxy = ProxyCache.GetProxy(uri);

                redirectRequest.AllowAutoRedirect = false;
                redirectRequest.Timeout = Preferences.Current.DaxFormatterRequestTimeout;
                try
                {
                    using (var netResponse = redirectRequest.GetResponse())
                    {
                        var redirectResponse = (HttpWebResponse)netResponse;
                        redirectUrl = redirectResponse.Headers["Location"];
                        var redirectUri = new Uri(redirectUrl);

                        // set the shared redirectHost variable
                        redirectHost = redirectUri.Host;
                    }
                }
                catch
                {
                    
                }
            }
        }
    }
}