using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace Demo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DetailPage : Page, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        SNMPManager snmpMgr = null;
        
        public DetailPage()
        {
            this.InitializeComponent();
            this.Loaded += DetailPage_Loaded;
        }

        private bool _bIsActive = true;
        public bool IsProgressBarActive
        {
            get => _bIsActive;
            set
            {
               _bIsActive = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("IsProgressBarActive"));
                }
            }
        }

        private Visibility _bIsVisible = Visibility.Visible;
        public Visibility ShowProgressBar
        {
            get => _bIsVisible;
            set
            {
                _bIsVisible = value;
                if (PropertyChanged != null)
                {
                    PropertyChanged(this, new PropertyChangedEventArgs("ShowProgressBar"));
                }
            }
        }

        private void DetailPage_Loaded(object sender, RoutedEventArgs e)
        {
            //mfpImage.Source = new BitmapImage(new Uri("http://www.uniquecopiers.co.uk/wp-content/uploads/2016/08/Toshiba-e-studio2505AC.jpg", UriKind.Absolute));
            mfpDetails.RequestedTheme = GenericHelper.CurrentUITheme;
        }

        private void OnThemeButtonClick(object sender, RoutedEventArgs e)
        {
            mfpDetails.RequestedTheme = mfpDetails.RequestedTheme == ElementTheme.Dark ? ElementTheme.Light : ElementTheme.Dark;
            GenericHelper.CurrentUITheme = mfpDetails.RequestedTheme;
        }

        private async void mfpImg_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ContentDialog chkDialog = GenericHelper.GetDialog();
            await chkDialog.ShowAsync();
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                mfpInfo.Text = GenericHelper.GetResourceString("IDS_WAIT_MSG");
                SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
                //string strTemp = await GenericHelper.FetchMFPInfo();
                snmpMgr = SNMPManager.GetInstance();
                snmpMgr.ResetLocalSettings();
                await HTTPRequest.GetInstance().FetchEngineVersion(e.Parameter.ToString());
                bool bValid = await snmpMgr.FetchMFPInfo(e.Parameter.ToString());
                //Bindings.Update();
                await Task.Delay(3000);
                PostExecutionUpdate(bValid);
            }
            catch (Exception ex)
            {
                ContentDialog dlg = GenericHelper.GetDialog();
                dlg.Content = ex.Message;
                await dlg.ShowAsync();
            }
        }

        void PostExecutionUpdate(bool bValid)
        {
            ShowProgressBar = Visibility.Collapsed;
            IsProgressBarActive = false;
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            mfpInfo.Text = GenericHelper.GetResourceString("IDS_ERROR_MSG");
            mfpInfo.Foreground = new SolidColorBrush(Colors.Red);

            if (bValid)
            {
                mfpInfo.Text = string.Empty;
                Dictionary<string, string> dRes = snmpMgr.ResultList;
                mfpPrnValue.Text = dRes[SNMPManager.OID_NAME];
                mfpSysValue.Text = dRes[SNMPManager.OID_MODEL];
                brandValue.Text = dRes[SNMPManager.OID_BRAND];
                faxValue.Text = dRes[SNMPManager.OID_FAX_ENABLED];
                snoValue.Text = dRes[SNMPManager.OID_SERIAL_NO];
                langValue.Text = HTTPRequest.GetInstance().EngineVersion;// dRes[SNMPManager.OID_LANG];
                macValue.Text = dRes[SNMPManager.OID_MAC_ADDRESS];
            }
        }
    }
}
