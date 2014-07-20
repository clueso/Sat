using Sat.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Sat
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        /// <summary>
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// NavigationHelper is used on each page to aid in navigation and 
        /// process lifetime management
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }


        public SettingsPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;
        }

        /// <summary>
        /// Populates the page with content passed during navigation. Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session. The state will be null the first time a page is visited.</param>
        private void navigationHelper_LoadState(object sender, LoadStateEventArgs e)
        {
        }

        /// <summary>
        /// Preserves state associated with this page in case the application is suspended or the
        /// page is discarded from the navigation cache.  Values must conform to the serialization
        /// requirements of <see cref="SuspensionManager.SessionState"/>.
        /// </summary>
        /// <param name="sender">The source of the event; typically <see cref="NavigationHelper"/></param>
        /// <param name="e">Event data that provides an empty dictionary to be populated with
        /// serializable state.</param>
        private void navigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// 
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="GridCS.Common.NavigationHelper.LoadState"/>
        /// and <see cref="GridCS.Common.NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            navigationHelper.OnNavigatedFrom(e);
        }

        #endregion

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (StationComboBox != null)
            {
                switch (StationComboBox.SelectedIndex)
                {
                    case 0://Seattle
                        GenericCodeClass.HomeStation = "west/wfo/sew";
                        break;
                    case 1://Vancouver
                        GenericCodeClass.HomeStation = "west/vanc";
                        break;
                    case 2://Billings
                        GenericCodeClass.HomeStation = "west/wfo/byz";
                        break;
                    case 3://Boise
                        GenericCodeClass.HomeStation = "west/wfo/boi";
                        break;
                    case 4://Elko
                        GenericCodeClass.HomeStation = "west/wfo/lkn";
                        break;
                    case 5://Eureka
                        GenericCodeClass.HomeStation = "west/wfo/eka";
                        break;
                    case 6://FlagStaff
                        GenericCodeClass.HomeStation = "west/wfo/fgz";
                        break;
                    case 7://Glasgow
                        GenericCodeClass.HomeStation = "west/wfo/ggw";
                        break;
                    case 8://Great Falls
                        GenericCodeClass.HomeStation = "west/wfo/tfx";
                        break;
                    case 9://Hanford/San Joaquin Valley
                        GenericCodeClass.HomeStation = "west/wfo/hnx";
                        break;
                    case 10://Las Vegas
                        GenericCodeClass.HomeStation = "west/wfo/vef";
                        break;
                    case 11://Los Angeles/Oxnard
                        GenericCodeClass.HomeStation = "west/wfo/lox";
                        break;
                    case 12://Medford
                        GenericCodeClass.HomeStation = "west/wfo/mfr";
                        break;
                    case 13://Missoula
                        GenericCodeClass.HomeStation = "west/wfo/mso";
                        break;
                    case 14://Pendleton
                        GenericCodeClass.HomeStation = "west/wfo/pdt";
                        break;
                    case 15://Phoenix
                        GenericCodeClass.HomeStation = "west/wfo/psr";
                        break;
                    case 16://Pocatello
                        GenericCodeClass.HomeStation = "west/wfo/pih";
                        break;
                    case 17://Portland
                        GenericCodeClass.HomeStation = "west/wfo/pqr";
                        break;
                    case 18://Reno
                        GenericCodeClass.HomeStation = "west/wfo/rev";
                        break;
                    case 19://Sacramento
                        GenericCodeClass.HomeStation = "west/wfo/sto";
                        break;
                    case 20://Salt Lake City
                        GenericCodeClass.HomeStation = "west/wfo/slc";
                        break;
                    case 21://San Diego
                        GenericCodeClass.HomeStation = "west/wfo/sgx";
                        break;
                    case 22://San Francisco Bay/Monterey
                        GenericCodeClass.HomeStation = "west/wfo/mtr";
                        break;
                    case 23://Spokane
                        GenericCodeClass.HomeStation = "west/wfo/otx";
                        break;
                    case 24://Tucson
                        GenericCodeClass.HomeStation = "west/wfo/twc";
                        break;
                    case 25:
                        GenericCodeClass.HomeStation = "flt/t7";
                        break;
                }
            }

            //GenericCodeClass.LoopInterval = ;
            //GenericCodeClass.DownloadInterval =;
            this.Frame.Navigate(typeof(MainPage));
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }

        private void StationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
        }
    }
}
