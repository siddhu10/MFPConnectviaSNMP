using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Composition;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;
using Windows.Storage;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Demo
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        ApplicationDataContainer localSettings;
        ApplicationDataContainer roamingSettings;
        ApplicationData applicationData = null;
        TypedEventHandler<ApplicationData, object> datachangedHandler = null;

        public MainPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            this.Loaded += MainPage_Loaded;
            applicationData = ApplicationData.Current;
            localSettings = applicationData.LocalSettings;
            roamingSettings = applicationData.RoamingSettings;
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            mfpIP.Focus(FocusState.Programmatic);
            //mfpImage.Source = new BitmapImage(new Uri("http://www.uniquecopiers.co.uk/wp-content/uploads/2016/08/Toshiba-e-studio2505AC.jpg", UriKind.Absolute));
            mfpFinder.RequestedTheme = GenericHelper.CurrentUITheme;
        }

        public static void SaveState()
        {
            //mfpIP.Text;
        }

        private void ShowPreviousConnectedPrinter()
        {
            ApplicationDataCompositeValue composite = (ApplicationDataCompositeValue)roamingSettings.Values[SNMPManager.settingName];

            /*if (localSettings.Values["IP"] != null)
                mfpIP.Text = localSettings.Values["IP"].ToString();
            else mfpIP.Text = "";*/

            if (composite != null)
            {
                String value = GenericHelper.NEW_LINE + GenericHelper.GetResourceString("IDS_PRN_INFO") + GenericHelper.NEW_LINE + GenericHelper.NEW_LINE +
                               GenericHelper.GetResourceString("IDS_PRN_NAME") + (String)composite[SNMPManager.settingName1] + GenericHelper.NEW_LINE +
                               GenericHelper.GetResourceString("IDS_FAX") + (String)composite[SNMPManager.settingName6] + GenericHelper.NEW_LINE +
                               GenericHelper.GetResourceString("IDS_MODEL") + (String)composite[SNMPManager.settingName2] + GenericHelper.NEW_LINE +
                               GenericHelper.GetResourceString("IDS_BRAND") + (String)composite[SNMPManager.settingName3] + GenericHelper.NEW_LINE +
                               GenericHelper.GetResourceString("IDS_MAC") + (String)composite[SNMPManager.settingName7] + GenericHelper.NEW_LINE +
                               GenericHelper.GetResourceString("IDS_SNO") + (String)composite[SNMPManager.settingName4] + GenericHelper.NEW_LINE +
                               GenericHelper.GetResourceString("IDS_LANG") + (String)composite[SNMPManager.settingName5];

                mfpInfo.Text = value;
            }
            else
            {
                mfpInfo.Text = "";
            }
        }

        private void OnThemeButtonClick(object sender, RoutedEventArgs e)
        {
            mfpFinder.RequestedTheme = mfpFinder.RequestedTheme == ElementTheme.Dark ? ElementTheme.Light : ElementTheme.Dark;
            GenericHelper.CurrentUITheme = mfpFinder.RequestedTheme;
        }

        private void mfpIP_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                if (true == GenericHelper.ValidateIP(mfpIP.Text))
                {
                    this.Frame.Navigate(typeof(DetailPage), mfpIP.Text);

                    localSettings.Values["IP"] = mfpIP.Text;
                }
                else
                {
                    localSettings.Values["IP"] = "";
                    mfpIP.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
                }
            }
        }

        private void Find_MFP(object sender, RoutedEventArgs e)
        {
            if (Window.Current.Content is Frame rootFrame)
            {
                if (true == GenericHelper.ValidateIP(mfpIP.Text))
                {
                    this.Frame.Navigate(typeof(DetailPage), mfpIP.Text);
                    localSettings.Values["IP"] = mfpIP.Text;
                }
                else
                {
                    localSettings.Values["IP"] = "";
                    mfpIP.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Red);
                }
            }
        }

        private void Clear_IP(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(mfpIP.Text))
            {
                mfpIP.Text = String.Empty;
            }
        }

        private async void mfpImage_DoubleTapped(object sender, DoubleTappedRoutedEventArgs e)
        {
            ContentDialog chkDialog = GenericHelper.GetDialog();
            await chkDialog.ShowAsync();
        }
        
        async void DataChangedHandler(ApplicationData appData, object o)
        {
            await this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
             {
                 ShowPreviousConnectedPrinter();
             });
        }

        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            //datachangeevent handler
            datachangedHandler = new TypedEventHandler<ApplicationData, object>(DataChangedHandler);
            applicationData.DataChanged += datachangedHandler;
			
			mfpIP.BorderBrush = new SolidColorBrush(Windows.UI.Colors.Blue);
            DemoRuntimeComponent.CalculatorWrapper cw = new DemoRuntimeComponent.CalculatorWrapper();
            int ans = cw.Add(10, 10);
            ContentDialog dlg = GenericHelper.GetDialog();
            dlg.Content = "Answer of ( 10 + 10 ) from DLL is " + ans;
            await dlg.ShowAsync();
            ShowPreviousConnectedPrinter();
        }
    }
}
