using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace TabularEditor.UIServices
{
    public static class ProxyCache
    {
        private static Dictionary<string, IWebProxy> urlProxies = new Dictionary<string, IWebProxy>();
        public static IWebProxy GetProxy(string url)
        {
            if (urlProxies.TryGetValue(url, out IWebProxy proxy)) return proxy;

            proxy = CreateProxy(url);
            urlProxies.Add(url, proxy);
            return proxy;
        }

        private static bool ProxyRequiresCredentials(IWebProxy proxy)
        {
            if (proxy == null) return false;

            try
            {
                var wr = WebRequest.CreateHttp(UpdateService.VERSION_MANIFEST_URL);
                wr.Proxy = proxy;
                var resp = wr.GetResponse();

                return false;
            }
            catch (System.Net.WebException wex)
            {
                if (wex.Status == System.Net.WebExceptionStatus.ProtocolError || wex.Status == System.Net.WebExceptionStatus.NameResolutionFailure)
                {
                    return true;
                }
                return false;
            }
        }

        private static IWebProxy CreateProxy(string url)
        {
            if(Preferences.Current.ProxyUseSystem || string.IsNullOrWhiteSpace(Preferences.Current.ProxyAddress))
            {
                return GetSystemProxy();
            }
            else
            {
                try
                {
                    IWebProxy result;
                    result = new WebProxy(Preferences.Current.ProxyAddress);
                    result.Credentials = new NetworkCredential(Preferences.Current.ProxyUser, Preferences.Current.ProxyPasswordEncrypted.Decrypt());
                    return result;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error connecting to HTTP Proxy specified in File > Preferences > General: " + ex.Message);
                    return GetSystemProxy();
                }
            }
        }

        private static IWebProxy GetSystemProxy()
        {
            IWebProxy result;

            result = WebRequest.GetSystemWebProxy();
            if (ProxyRequiresCredentials(result))
            {
                result.Credentials = CredentialCache.DefaultCredentials;
                Console.WriteLine("Using System Proxy with default credentials");
            }
            else
            {
                Console.WriteLine("Using System Proxy without credentials");
            }
            return result;
        }

        public static void ClearProxyCache()
        {
            urlProxies.Clear();
        }
    }
}
