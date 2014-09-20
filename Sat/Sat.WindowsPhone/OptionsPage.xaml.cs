﻿using Sat.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkID=390556

namespace Sat
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class OptionsPage : Page
    {
        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();

        public OptionsPage()
        {
            this.InitializeComponent();

            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += this.NavigationHelper_LoadState;
            this.navigationHelper.SaveState += this.NavigationHelper_SaveState;
        }

        /// <summary>
        /// Gets the <see cref="NavigationHelper"/> associated with this <see cref="Page"/>.
        /// </summary>
        public NavigationHelper NavigationHelper
        {
            get { return this.navigationHelper; }
        }

        /// <summary>
        /// Gets the view model for this <see cref="Page"/>.
        /// This can be changed to a strongly typed view model.
        /// </summary>
        public ObservableDictionary DefaultViewModel
        {
            get { return this.defaultViewModel; }
        }

        /// <summary>
        /// Populates the page with content passed during navigation.  Any saved state is also
        /// provided when recreating a page from a prior session.
        /// </summary>
        /// <param name="sender">
        /// The source of the event; typically <see cref="NavigationHelper"/>
        /// </param>
        /// <param name="e">Event data that provides both the navigation parameter passed to
        /// <see cref="Frame.Navigate(Type, Object)"/> when this page was initially requested and
        /// a dictionary of state preserved by this page during an earlier
        /// session.  The state will be null the first time a page is visited.</param>
        private void NavigationHelper_LoadState(object sender, LoadStateEventArgs e)
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
        private void NavigationHelper_SaveState(object sender, SaveStateEventArgs e)
        {
        }

        #region NavigationHelper registration

        /// <summary>
        /// The methods provided in this section are simply used to allow
        /// NavigationHelper to respond to the page's navigation methods.
        /// <para>
        /// Page specific logic should be placed in event handlers for the  
        /// <see cref="NavigationHelper.LoadState"/>
        /// and <see cref="NavigationHelper.SaveState"/>.
        /// The navigation parameter is available in the LoadState method 
        /// in addition to page state preserved during an earlier session.
        /// </para>
        /// </summary>
        /// <param name="e">Provides data for navigation methods and event
        /// handlers that cannot cancel the navigation request.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            this.navigationHelper.OnNavigatedFrom(e);
        }

        #endregion
        private void StationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StationComboBox != null)
            {
                GenericCodeClass.LightningDataSelected = false;
                switch (StationComboBox.SelectedIndex)
                {
                    case 0://Seattle
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/sew/img/";
                        break;
                    case 1://Vancouver
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/vanc/img/";
                        break;
                    case 2://Billings
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/byz/img/";
                        break;
                    case 3://Boise
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/boi/img/";
                        break;
                    case 4://Elko
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/lkn/img/";
                        break;
                    case 5://Eureka
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/eka/img/";
                        break;
                    case 6://FlagStaff
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/fgz/img/";
                        break;
                    case 7://Glasgow
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/ggw/img/";
                        break;
                    case 8://Great Falls
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/tfx/img/";
                        break;
                    case 9://Hanford/San Joaquin Valley
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/hnx/img/";
                        break;
                    case 10://Las Vegas
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/vef/img/";
                        break;
                    case 11://Los Angeles/Oxnard
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/lox/img/";
                        break;
                    case 12://Medford
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/mfr/img/";
                        break;
                    case 13://Missoula
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/mso/img/";
                        break;
                    case 14://Pendleton
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/pdt/img/";
                        break;
                    case 15://Phoenix
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/psr/img/";
                        break;
                    case 16://Pocatello
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/pih/img/";
                        break;
                    case 17://Portland
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/pqr/img/";
                        break;
                    case 18://Reno
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/rev/img/";
                        break;
                    case 19://Sacramento
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/sto/img/";
                        break;
                    case 20://Salt Lake City
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/slc/img/";
                        break;
                    case 21://San Diego
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/sgx/img/";
                        break;
                    case 22://San Francisco Bay/Monterey
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/mtr/img/";
                        break;
                    case 23://Spokane
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/otx/img/";
                        break;
                    case 24://Tucson
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/west/wfo/twc/img/";
                        break;
                    case 25:
                        GenericCodeClass.HomeStation = "http://www.ssd.noaa.gov/goes/flt/t7/img/";
                        break;
                    case 26:
                        GenericCodeClass.HomeStation = "http://weather.gc.ca/data/lightning_images/";
                        GenericCodeClass.LightningDataSelected = true;
                        break;
                }
            }
        }
    }
}