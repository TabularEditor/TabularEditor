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
        public static bool? UpdateAvailable { get; private set; } = false;

        /// <summary>
        /// Checks online to see if an updated version is available.
        /// </summary>
        /// <param name="displayErrors">Set to true to display an error message in case the update check fails</param>
        /// <returns>True if a newer version of Tabular Editor is available, false otherwise</returns>
        public static bool? Check(bool displayErrors = false)
        {
            using (new Hourglass())
            {
                UpdateAvailable = InternalCheck(displayErrors);
            }
            return UpdateAvailable;
        }

        private static bool? InternalCheck(bool displayErrors)
        {
            try
            {
                var cli = new WebClient();
                cli.Proxy = ProxyCache.GetProxy(VERSION_MANIFEST_URL);
                var availableBuildString = cli.DownloadString(VERSION_MANIFEST_URL + "?q=" + Guid.NewGuid().ToString());
                AvailableBuild = Version.Parse(availableBuildString);
                if (AvailableBuild > CurrentBuild)
                {
                    return true;
                }
            }
            catch (Exception ex)
            {
                if(displayErrors) MessageBox.Show(ex.Message, "Unable to check for updated versions", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return null;
            }
            return false;
        }

        public static void OpenDownloadPage()
        {
            System.Diagnostics.Process.Start(DOWNLOAD_UPDATE_URL);
        }
    }
}
