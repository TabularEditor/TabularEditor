using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TabularEditor.UI;

namespace TabularEditor.UIServices
{
    public static class UpdateService
    {
        public const string VERSION_MANIFEST_URL = "https://raw.githubusercontent.com/otykier/TabularEditor/master/TabularEditor/version.txt";
        public const string DOWNLOAD_UPDATE_URL = "https://github.com/otykier/TabularEditor/releases/latest";

        public static Version CurrentBuild { get; } = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version;
        public static Version AvailableBuild { get; private set; } = null;
        public static VersionCheckResult AvailableVersion { get; private set; } = VersionCheckResult.NoNewVersion;

        /// <summary>
        /// Checks online to see if an updated version is available.
        /// </summary>
        /// <param name="displayErrors">Set to true to display an error message in case the update check fails</param>
        /// <returns>True if a newer version of Tabular Editor is available, false otherwise</returns>
        public static VersionCheckResult Check(bool displayErrors = false)
        {
            using (new Hourglass())
            {
                AvailableVersion = InternalCheck(displayErrors);
            }
            return AvailableVersion;
        }

        private static VersionCheckResult InternalCheck(bool displayErrors)
        {
            try
            {
                var url = VERSION_MANIFEST_URL + "?q=" + Guid.NewGuid().ToString();
                var wr = WebRequest.CreateHttp(url);
                wr.Proxy = ProxyCache.GetProxy(url);
                wr.Timeout = 5000;

                using (var response = wr.GetResponse())
                {
                    using (var reader = new System.IO.StreamReader(response.GetResponseStream(), Encoding.UTF8))
                    {
                        var availableBuildString = reader.ReadToEnd();
                        AvailableBuild = Version.Parse(availableBuildString);

                        return CurrentBuild.DetermineUpdate(AvailableBuild);
                    }
                }
            }
            catch (Exception ex)
            {
                if(displayErrors) MessageBox.Show(ex.Message, "Unable to check for updated versions", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return VersionCheckResult.Unknown;
            }
        }

        public static void OpenDownloadPage()
        {
            System.Diagnostics.Process.Start(DOWNLOAD_UPDATE_URL);
        }
    }

    public enum VersionCheckResult
    {
        NoNewVersion,
        PatchAvailable,
        MinorAvailable,
        MajorAvailable,
        Unknown
    }

    public static class VersionCheckResultExtension
    {
        public static bool UpdateAvailable(this VersionCheckResult result, bool skipPatchUpdates = false)
        {
            switch(result)
            {
                case VersionCheckResult.PatchAvailable:
                    if (skipPatchUpdates)
                        return false;
                    else
                        return true;
                case VersionCheckResult.MinorAvailable:
                case VersionCheckResult.MajorAvailable:
                    return true;
                default:
                    return false;
            }
        }



        internal static VersionCheckResult DetermineUpdate(this Version current, Version available)
        {
            if (available.Major > current.Major)
            {
                return VersionCheckResult.MajorAvailable;
            }
            else if (available.Major == current.Major && available.Minor > current.Minor)
            {
                return VersionCheckResult.MinorAvailable;
            }
            else if (available > current)
            {
                return VersionCheckResult.PatchAvailable;
            }
            else
            {
                return VersionCheckResult.NoNewVersion;
            }
        }
    }
}
