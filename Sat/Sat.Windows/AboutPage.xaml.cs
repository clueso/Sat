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
    public sealed partial class AboutPage : SettingsFlyout
    {
        public AboutPage()
        {
            this.InitializeComponent();
        }

        private void AboutPage_Loaded(object sender, RoutedEventArgs e)
        {
            AboutPageTextBlock.Text = "The Satellite Imagery app provides a quick source of advanced weather information for mobile users on the go.\n\n";
            AboutPageTextBlock.Text += "All data are from the National Oceanic and Atmospheric Administration (NOAA). The app makes no attempt to ensure the quality and accuracy of the data.\n\n";
            AboutPageTextBlock.Text += "The internet connection of the phone is used to access data. No information about the user or the phone is collected, stored, or shared.\n\n";
            AboutPageTextBlock.Text += "We would love to hear your feedback on the app. If you have any questions, comments, or suggestions, please send an email to xx@xx.com.";
        }
    }
}
