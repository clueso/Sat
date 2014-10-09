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

// The Settings Flyout item template is documented at http://go.microsoft.com/fwlink/?LinkId=273769

namespace Sat
{
    public sealed partial class OptionsPage : SettingsFlyout
    {
        public event EventHandler SettingsChanged;
        private static String ChosenCityName;
        private static String ChosenCityURL;

        public OptionsPage()
        {
            this.InitializeComponent();

            switch(GenericCodeClass.DownloadInterval.Hours)
            {
                case 3:
                    DurationRadioButton1.IsChecked = true;
                    break;
                case 6:
                    DurationRadioButton2.IsChecked = true;
                    break;
            }

            switch(GenericCodeClass.LoopInterval.Milliseconds)
            {
                case 500:
                    LoopTimerRadioButton2.IsChecked = true;
                    break;
                case 100:
                    LoopTimerRadioButton1.IsChecked = true;
                    break;
                case 1000:
                    LoopTimerRadioButton3.IsChecked = true;
                    break;
            }
            
            StationComboBox.SelectedItem = GenericCodeClass.HomeStationName;
        }

        private void StationComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (StationComboBox != null)
            {
                GenericCodeClass.LightningDataSelected = false;
                switch (StationComboBox.SelectedIndex)
                {
                    case 0://Seattle
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/sew/img/";
                        ChosenCityName = "Seattle";
                        break;
                    case 1://Vancouver
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/vanc/img/";
                        ChosenCityName = "Vancouver";
                        break;
                    case 2://Billings
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/byz/img/";
                        ChosenCityName = "Billings";
                        break;
                    case 3://Boise
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/boi/img/";
                        ChosenCityName = "Boise";
                        break;
                    case 4://Elko
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/lkn/img/";
                        ChosenCityName = "Elko";
                        break;
                    case 5://Eureka
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/eka/img/";
                        ChosenCityName = "Eureka";
                        break;
                    case 6://FlagStaff
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/fgz/img/";
                        ChosenCityName = "FlagStaff";
                        break;
                    case 7://Glasgow
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/ggw/img/";
                        ChosenCityName = "Glasgow";
                        break;
                    case 8://Great Falls
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/tfx/img/";
                        ChosenCityName = "Great Falls";
                        break;
                    case 9://Hanford/San Joaquin Valley
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/hnx/img/";
                        ChosenCityName = "Hanford, San Joaquin Valley";
                        break;
                    case 10://Las Vegas
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/vef/img/";
                        ChosenCityName = "Las Vegas";
                        break;
                    case 11://Los Angeles/Oxnard
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/lox/img/";
                        ChosenCityName = "Los Angeles/Oxnard";
                        break;
                    case 12://Medford
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/mfr/img/";
                        ChosenCityName = "Medford";
                        break;
                    case 13://Missoula
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/mso/img/";
                        ChosenCityName = "Missoula";
                        break;
                    case 14://Pendleton
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/pdt/img/";
                        ChosenCityName = "Pendleton";
                        break;
                    case 15://Phoenix
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/psr/img/";
                        ChosenCityName = "Phoenix";
                        break;
                    case 16://Pocatello
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/pih/img/";
                        ChosenCityName = "Pocatello";
                        break;
                    case 17://Portland
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/pqr/img/";
                        ChosenCityName = "Portland";
                        break;
                    case 18://Reno
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/rev/img/";
                        ChosenCityName = "Reno";
                        break;
                    case 19://Sacramento
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/sto/img/";
                        ChosenCityName = "Sacramento";
                        break;
                    case 20://Salt Lake City
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/slc/img/";
                        ChosenCityName = "Salt Lake City";
                        break;
                    case 21://San Diego
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/sgx/img/";
                        ChosenCityName = "San Diego";
                        break;
                    case 22://San Francisco Bay/Monterey
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/mtr/img/";
                        ChosenCityName = "San Francisco Bay/Monterey";
                        break;
                    case 23://Spokane
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/otx/img/";
                        ChosenCityName = "Spokane";
                        break;
                    case 24://Tucson
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/west/wfo/twc/img/";
                        ChosenCityName = "Tucson";
                        break;
                    case 25:
                        ChosenCityURL = "http://www.ssd.noaa.gov/goes/flt/t7/img/";
                        ChosenCityName = "Central BC";
                        break;
                    case 26:
                        ChosenCityURL = "http://weather.gc.ca/data/lightning_images/";
                        ChosenCityName = "EC Lightning Data";
                        GenericCodeClass.LightningDataSelected = true;
                        break;
                }
            }
        }

        private void OptionsPage_BackClick(object sender, BackClickEventArgs e)
        {
            if(ChosenCityName != null && ChosenCityName.Equals(GenericCodeClass.HomeStationName) == false)
            {
                GenericCodeClass.HomeStationName = ChosenCityName;
                GenericCodeClass.HomeStation = ChosenCityURL;   //check for null?
                GenericCodeClass.HomeStationChanged = true;
            }

            //Better to check for existing download intervals before setting new times?
            if (DurationRadioButton1.IsChecked == true)
                GenericCodeClass.FileDownloadPeriod = 3;
            else if(DurationRadioButton2.IsChecked == true)
                GenericCodeClass.FileDownloadPeriod = 6;
            

            if (LoopTimerRadioButton1.IsChecked == true)
                GenericCodeClass.LoopInterval = new TimeSpan(0, 0, 0, 0, 100);
            else if (LoopTimerRadioButton2.IsChecked == true)
                GenericCodeClass.LoopInterval = new TimeSpan(0, 0, 0, 0, 500);
            else
                GenericCodeClass.LoopInterval = new TimeSpan(0, 0, 0, 1, 0);
            

            if (SettingsChanged != null)
                SettingsChanged(this, EventArgs.Empty);
        }

        private void OptionsPage_LostFocus(object sender, RoutedEventArgs e)
        {
            
            
        }

        private void OptionsPage_Unloaded(object sender, RoutedEventArgs e)
        {
            if (ChosenCityName != null && ChosenCityName.Equals(GenericCodeClass.HomeStationName) == false)
            {
                GenericCodeClass.HomeStationName = ChosenCityName;
                GenericCodeClass.HomeStation = ChosenCityURL;   //check for null?
                GenericCodeClass.HomeStationChanged = true;
            }

            //Better to check for existing download intervals before setting new times?
            if (DurationRadioButton1.IsChecked == true)
                GenericCodeClass.FileDownloadPeriod = 3;
            else if (DurationRadioButton2.IsChecked == true)
                GenericCodeClass.FileDownloadPeriod = 6;


            if (LoopTimerRadioButton1.IsChecked == true)
                GenericCodeClass.LoopInterval = new TimeSpan(0, 0, 0, 0, 100);
            else if (LoopTimerRadioButton2.IsChecked == true)
                GenericCodeClass.LoopInterval = new TimeSpan(0, 0, 0, 0, 500);
            else
                GenericCodeClass.LoopInterval = new TimeSpan(0, 0, 0, 1, 0);


            if (SettingsChanged != null)
                SettingsChanged(this, EventArgs.Empty);
        }
    }
}
