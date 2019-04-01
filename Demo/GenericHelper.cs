
using System;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.ApplicationModel.Resources;
using System.Text.RegularExpressions;

namespace Demo
{
    class GenericHelper
    {
        private static ElementTheme _currentElementTheme;
        private static ResourceLoader loader = new Windows.ApplicationModel.Resources.ResourceLoader();
        public const string NEW_LINE = "\n";

        public static ElementTheme CurrentUITheme
        {
            get => _currentElementTheme;
            set => _currentElementTheme = value;
        }

        public async static Task<string> FetchMFPInfo()
        {
            string strTemp = string.Empty;
            for (int idx = 65; idx < 91; idx++)
            {
                strTemp += (char)idx;
                await Task.Delay(100);
            }
            return strTemp;
        }

        public static ContentDialog GetDialog()
        {
            ContentDialog chkDialog = new ContentDialog();
            try
            {
                chkDialog.Title = GetResourceString("IDS_CHKDLG_TITLE");
                chkDialog.Content = GetResourceString("IDS_CHKDLG_CONTENT");
                chkDialog.PrimaryButtonText = GetResourceString("IDS_CHKDLG_OK");
                chkDialog.CloseButtonText = GetResourceString("IDS_CHKDLG_CLOSE");
            }
            catch (Exception ex)
            {
                Console.WriteLine("GenericHelper :: GetDialog() :: Exception handled " + ex);
            }
            return chkDialog;
        }

        public static bool ValidateIP(string strIPAddress)
        {
            bool bValid = true;
            Regex regExpr = new Regex("\\b(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\\." +
                                        "(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\\." +
                                        "(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\\." +
                                        "(25[0-5]|2[0-4][0-9]|1[0-9][0-9]|[1-9][0-9]|[0-9])\\b");
            bValid = regExpr.IsMatch(strIPAddress);
            return bValid;
        }

        public static string GetResourceString(string strID)
        {
            string strValue = string.Empty;
            if ( loader != null )
            {
                strValue = loader.GetString(strID);
            }
            return strValue;
        }
    }
}