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
        private static String ChosenSatelliteType;
        private static XMLParserClass ProvincialCityXML = new XMLParserClass("ProvinceCities.xml");
        private static XMLParserClass CityCodeXML = new XMLParserClass("CityCodes.xml");

        public OptionsPage()
        {
            this.InitializeComponent();            
        }

        private void ProvinceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProvinceComboBox != null)
            {
                PopulateStationBox(ProvinceComboBox.SelectedIndex, ProvinceComboBox.Items[ProvinceComboBox.SelectedIndex].ToString(), false);
                SetOptions();
            }
        }

        private void SetOptions()
        {
            bool IsPolarSelected = ProvinceComboBox.Items[ProvinceComboBox.SelectedIndex].Equals("Polar Imagery (no loop)");
            bool AreProvincesSelected = ProvinceComboBox.Items[ProvinceComboBox.SelectedIndex].Equals("British Columbia") ||
                                        ProvinceComboBox.Items[ProvinceComboBox.SelectedIndex].Equals("Ontario") ||
                                        ProvinceComboBox.Items[ProvinceComboBox.SelectedIndex].Equals("New Brunswick");
            bool IsRegionalSelected = ProvinceComboBox.Items[ProvinceComboBox.SelectedIndex].Equals("Regional Imagery");
		    bool IsNEPacSelected = StationComboBox.Items[StationComboBox.SelectedIndex].Equals("North East Pacific");

            //RGB
            ProductRadioButton3.IsEnabled = !GenericCodeClass.CanadaSelected || AreProvincesSelected;

            //Rainbow
            ProductRadioButton2.IsEnabled = !GenericCodeClass.CanadaSelected || AreProvincesSelected;
            //Aviation = !GenericCodeClass.CanadaSelected;
            ProductRadioButton1.IsChecked = (bool)ProductRadioButton1.IsChecked || (!(bool)ProductRadioButton4.IsChecked && !ProductRadioButton2.IsEnabled);

            DurationRadioButton1.IsEnabled = !IsPolarSelected;  //3h
            DurationRadioButton2.IsEnabled = !IsPolarSelected && !IsNEPacSelected;  //6h
            DurationRadioButton3.IsEnabled = !IsPolarSelected;  //Latest
            DurationRadioButton3.IsChecked = (bool)DurationRadioButton3.IsChecked || !DurationRadioButton1.IsEnabled;
            
            LoopTimerRadioButton1.IsEnabled = !IsPolarSelected;
            LoopTimerRadioButton2.IsEnabled = !IsPolarSelected;
            LoopTimerRadioButton3.IsEnabled = !IsPolarSelected;

            if (GenericCodeClass.CanadaSelected)
                ProductRadioButton2.Content = "Rainbow";
            else
                ProductRadioButton2.Content = "Aviation";

        }

        private void OptionsPage_BackClick(object sender, BackClickEventArgs e)
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

        private void OptionsPage_Unloaded(object sender, RoutedEventArgs e)
        {
            bool IsPolarSelected = ProvinceComboBox.Items[ProvinceComboBox.SelectedIndex].Equals("Polar Imagery");
            bool IsNEPacSelected = StationComboBox.Items[StationComboBox.SelectedIndex].Equals("North East Pacific");
            bool IsNWTerritoriesSelected = StationComboBox.Items[StationComboBox.SelectedIndex].Equals("Northwest Territories/Nunavut");
            ChosenCityName = StationComboBox.Items[StationComboBox.SelectedIndex].ToString();

            //multi-URL: change codes depending on stations selected.
            if (ProductRadioButton1.IsChecked == true)
            {
                if (IsNEPacSelected)
                    ChosenSatelliteType = "alir";
                else if (IsPolarSelected && IsNWTerritoriesSelected)
                    ChosenSatelliteType = "ir";
                else if (IsPolarSelected && !IsNWTerritoriesSelected)
                    ChosenSatelliteType = "03";
                else
                    ChosenSatelliteType = "ir4";
            }
            else if (ProductRadioButton2.IsChecked == true)
            {
                if (GenericCodeClass.CanadaSelected)
                    ChosenSatelliteType = "rb";
                else
                    ChosenSatelliteType = "avn";
            }
            else if (ProductRadioButton3.IsChecked == true)
            {
                ChosenSatelliteType = "rgb";
            }
            else if (ProductRadioButton4.IsChecked == true)
            {
                if (IsNEPacSelected)
                    ChosenSatelliteType = "alvs";
                else if (StationComboBox.Items[StationComboBox.SelectedIndex].Equals("Eastern Canada"))
                    ChosenSatelliteType = "visible";
                else if (IsPolarSelected)
                    ChosenSatelliteType = "nir";
                else
                    ChosenSatelliteType = "vis";
            }

            GenericCodeClass.HomeStationChanged = !ChosenCityName.Equals(GenericCodeClass.HomeStationName)
                                                    || !ChosenSatelliteType.Equals(GenericCodeClass.SatelliteTypeString);

            if (GenericCodeClass.HomeStationChanged)
            {
                string HomeStation;

                if (StationComboBox != null)
                {
                    GenericCodeClass.HomeStationCodeString = CityCodeXML.GetCityCode(StationComboBox.Items[StationComboBox.SelectedIndex].ToString()); //Change this to ChosenCityCode?
                    HomeStation = CityCodeXML.GetHomeURL(StationComboBox.Items[StationComboBox.SelectedIndex].ToString()); //Change this to ChosenCityCode?
                    GenericCodeClass.HomeStation = HomeStation.Replace("{SC}", GenericCodeClass.HomeStationCodeString);
                }

                GenericCodeClass.HomeStationName = StationComboBox.Items[StationComboBox.SelectedIndex].ToString();
                GenericCodeClass.HomeProvinceName = ProvinceComboBox.Items[ProvinceComboBox.SelectedIndex].ToString();
                GenericCodeClass.SatelliteTypeString = ChosenSatelliteType;
            }
            //Better to check for existing download intervals before setting new times?
            if (DurationRadioButton1.IsChecked == true)
                GenericCodeClass.FileDownloadPeriod = 3;
            else if (DurationRadioButton2.IsChecked == true)
                GenericCodeClass.FileDownloadPeriod = 6;
            else if (DurationRadioButton3.IsChecked == true)
                GenericCodeClass.FileDownloadPeriod = 1;
            
            if (LoopTimerRadioButton1.IsChecked == true)
                GenericCodeClass.LoopInterval = new TimeSpan(0, 0, 0, 0, 100);
            else if (LoopTimerRadioButton2.IsChecked == true)
                GenericCodeClass.LoopInterval = new TimeSpan(0, 0, 0, 0, 500);
            else
                GenericCodeClass.LoopInterval = new TimeSpan(0, 0, 0, 0, 1000);
            
            if (SettingsChanged != null)
                SettingsChanged(this, EventArgs.Empty);
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            switch (GenericCodeClass.FileDownloadPeriod)
            {
                case 1:
                    DurationRadioButton3.IsChecked = true;
                    break;
                case 3:
                    DurationRadioButton1.IsChecked = true;
                    break;
                case 6:
                    DurationRadioButton2.IsChecked = true;
                    break;
            }


            switch (GenericCodeClass.LoopInterval.Milliseconds)
            {
                case 0:
                    if (GenericCodeClass.LoopInterval.Seconds == 1)
                        LoopTimerRadioButton3.IsChecked = true;
                    break;
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

            switch (GenericCodeClass.SatelliteTypeString)
            {
                case "ir4":
		        case "alir":
		        case "1070":
		        case "03":
                    ProductRadioButton1.IsChecked = true;
                    break;
                case "rb":
		        case "avn":	
                    ProductRadioButton2.IsChecked = true;
                    break;
                case "rgb":
                    ProductRadioButton3.IsChecked = true;
                    break;
                case "vis":
		        case "alvs":
		        case "visible":
		        case "nir":	
                    ProductRadioButton4.IsChecked = true;
                    break;
            }

            CountryRadioButton1.IsChecked = GenericCodeClass.CanadaSelected;
            CountryRadioButton2.IsChecked = !GenericCodeClass.CanadaSelected;

            if (GenericCodeClass.CanadaSelected)
            {
                ProvincialCityXML.SetSourceFile("ProvinceCities.xml");
                CityCodeXML.SetSourceFile("CityCodes.xml");
            }
            else
            {
                ProvincialCityXML.SetSourceFile("USStateCities.xml");
                CityCodeXML.SetSourceFile("USCityCodes.xml");
            }

            PopulateProvinceBox(true);
            //ProvinceComboBox.SelectedItem = GenericCodeClass.HomeProvinceName;
            PopulateStationBox(ProvinceComboBox.SelectedIndex, ProvinceComboBox.Items[ProvinceComboBox.SelectedIndex].ToString(), true);
            SetOptions();
            //StationComboBox.SelectedItem = GenericCodeClass.HomeStationName;

            CountryRadioButton1.Checked += CountryRadioButton_CheckedHandler;
            CountryRadioButton2.Checked += CountryRadioButton_CheckedHandler;
            //ProvinceComboBox.SelectionChanged += ProvinceComboBox_SelectionChanged;
        }

        private void PopulateStationBox(int ProvinceBoxIndex, string ProvinceName, bool UseHomeStationValue)
        {

            if (StationComboBox != null)
            {
                List<string> CityNames = new List<string>();

                if (ProvinceName.Contains('&'))
                    ProvinceName = ProvinceName.Substring(0, 12);

                ProvincialCityXML.ReadCitiesInProvince(ProvinceName, CityNames);
                if (StationComboBox != null)
                {
                    StationComboBox.Items.Clear();
                    foreach (string City in CityNames)
                    {
                        StationComboBox.Items.Add(City);
                    }
                }

                if (UseHomeStationValue)
                    StationComboBox.SelectedItem = GenericCodeClass.HomeStationName;
                else
                    StationComboBox.SelectedIndex = 0;
            }
        }

        private void CountryRadioButton_CheckedHandler(object sender, RoutedEventArgs e)
        {
            if (sender == CountryRadioButton1)
            {
                if (GenericCodeClass.CanadaSelected)
                    return;
                ProvincialCityXML.SetSourceFile("ProvinceCities.xml");
                CityCodeXML.SetSourceFile("CityCodes.xml");
                GenericCodeClass.CanadaSelected = true;
            }
            else if (sender == CountryRadioButton2)
            {
                if (!GenericCodeClass.CanadaSelected)
                    return;
                ProvincialCityXML.SetSourceFile("USStateCities.xml");
                CityCodeXML.SetSourceFile("USCityCodes.xml");
                GenericCodeClass.CanadaSelected = false;
            }


            PopulateProvinceBox(false);

        }

        private void PopulateProvinceBox(bool UseHomeStationVlaue)
        {
            List<string> ProvinceList = ProvincialCityXML.ReadProvinceList();

            if (ProvinceComboBox != null)
            {
                ProvinceComboBox.SelectionChanged -= ProvinceComboBox_SelectionChanged;
                ProvinceComboBox.Items.Clear();

                foreach (string str in ProvinceList)
                    ProvinceComboBox.Items.Add(str);

                ProvinceComboBox.SelectionChanged += ProvinceComboBox_SelectionChanged;
                if (UseHomeStationVlaue)
                    ProvinceComboBox.SelectedItem = GenericCodeClass.HomeProvinceName;
                else
                    ProvinceComboBox.SelectedIndex = 0;

                //PopulateStationBox(ProvinceComboBox.SelectedIndex, ProvinceComboBox.Items[ProvinceComboBox.SelectedIndex].ToString());
            }

        }
    }
}
