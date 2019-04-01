using Windows.Graphics.Display;
using Windows.UI.Xaml;
using Windows.System.Profile;
//using Windows.ApplicationModel.Resources.Core;
//using static Demo.DeviceInformation;


namespace Demo
{
    /*public class DeviceInformation
    {
        public enum Orientations
        {
            Portrait,
            Landscape
        }

        public static Orientations Orientation => DisplayInformation.GetForCurrentView().CurrentOrientation.ToString().Contains("Landscape") ? Orientations.Landscape : Orientations.Portrait;

        public static DisplayInformation DisplayInformation => DisplayInformation.GetForCurrentView();
    }*/

    public class OrientationTrigger : StateTriggerBase
    {
        public enum Orientations
        {
            Portrait,
            Landscape
        }

        public OrientationTrigger()
        {
            //DeviceInformation.DisplayInformation.OrientationChanged += (s, e) => SetTrigger();
            Window.Current.SizeChanged += (s, e) => UpdateTrigger();
        }

        public Orientations Orientation
        {
            get => DisplayInformation.GetForCurrentView().CurrentOrientation.ToString().Contains("Landscape") ? Orientations.Landscape : Orientations.Portrait;
            set
            {
                UpdateTrigger();
            }
        }

        /* public static bool IsMobile
         {
             get
             {
                 var qual = ResourceContext.GetForCurrentView().QualifierValues;
                 return (qual.ContainsKey("DeviceFamily") && qual["DeviceFamily"] == "Mobile");
             }
         }

         public Orientations Orientation
         {
             get { return (Orientations)GetValue(OrientationProperty); }
             set { SetValue(OrientationProperty, value); }
         }

         public static readonly DependencyProperty OrientationProperty = DependencyProperty.Register(nameof(Orientation), typeof(Orientations), 
                                                                                                     typeof(OrientationTrigger), new PropertyMetadata(Orientations.Landscape));

         private void SetTrigger()
         {   
             SetActive(IsMobile && Orientation == DeviceInformation.Orientation);
         }*/

        private void UpdateTrigger()
        {
            // The trigger will be activated if the current device family is Windows.Mobile
            // and the UserInteractionMode matches the interaction mode value in XAML.
            SetActive(Orientation == Orientations.Landscape && AnalyticsInfo.VersionInfo.DeviceFamily == "Windows.Mobile");
        }
    }
}
