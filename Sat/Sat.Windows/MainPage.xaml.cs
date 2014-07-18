﻿using Sat.Common;
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
using Windows.Storage;
using Windows.UI.Xaml.Media.Imaging;
using System.Threading.Tasks;

// The Basic Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234237

namespace Sat
{
    /// <summary>
    /// A basic page that provides characteristics common to most applications.
    /// </summary>
    public sealed partial class MainPage : Page
    {

        private NavigationHelper navigationHelper;
        private ObservableDictionary defaultViewModel = new ObservableDictionary();
        List<string> Files = new List<string>();
        private static int CurrImgIndex = -1;
        //private static WriteableBitmap ImgSource;
        private StorageFolder ImageFolder;
        private DispatcherTimer LoopTimer;
        private DispatcherTimer DownloadTimer;
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


        public MainPage()
        {
            this.InitializeComponent();
            this.navigationHelper = new NavigationHelper(this);
            this.navigationHelper.LoadState += navigationHelper_LoadState;
            this.navigationHelper.SaveState += navigationHelper_SaveState;

            LoopTimer = new DispatcherTimer();
            DownloadTimer = new DispatcherTimer();
            LoopTimer.Tick += Timer_Handler;
            DownloadTimer.Tick += Timer_Handler;
            LoopTimer.Interval = new TimeSpan(0, 0, 1); //Create a timer that trigger every 1 s
            DownloadTimer.Interval = new TimeSpan(0, 30, 0); //Create a timer that triggers every 30 min
            
            DownloadFiles();
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
            //LoopTimer.Start();
            //DownloadTimer.Start();
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
            //LoopTimer.Stop();
            //DownloadTimer.Stop();
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

        private void QuitButton_Tapped(object sender, TappedRoutedEventArgs e)
        {
            App.Current.Exit();
        }

        private async void NextButton_Click(object sender, RoutedEventArgs e)
        {
            await ChangeImage(true);
        }

        private async void PrevButton_Click(object sender, RoutedEventArgs e)
        {
            //if (ImageFolder == null)
            //    ImageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);

            //if (CurrImgIndex != -1 && Files.Count != 0)
            //{
            //    CurrImgIndex = (CurrImgIndex + Files.Count - 1) % Files.Count;
            //    //ImgBox.Source = await GenericCodeClass.GetBitmapImage(ImageFolder, Files[CurrImgIndex]);
            //    ImgBox.Source = await GenericCodeClass.GetWriteableBitmap(ImageFolder, Files[CurrImgIndex]);
            //    //ImgBox.Source = await GenericCodeClass.GetBitmapImage(ImageFolder, "2014186_1700vis.jpg");
            //    //MapBox.ImageLocation = URLPath + Files[CurrImgIndex];
            //    StatusBox.Text = "Prev Button:" + "CurrImgIndex = " + CurrImgIndex.ToString() + "::" + Files[CurrImgIndex].ToString();
            //}
            //else
            //{
            //    ImgBox.Source = await GenericCodeClass.GetWriteableBitmap(ImageFolder, "Error.jpg");
            //}
            await ChangeImage(false);
        }

        private async void DownloadButton_Click(object sender, RoutedEventArgs e)
        {
            await DownloadFiles();    
        }

        private void QuitButton_Click(object sender, RoutedEventArgs e)
        {
            App.Current.Exit();
        }

        private async Task DownloadFiles()
        {
            //StorageFolder installedLocation = Windows.ApplicationModel.Package.Current.InstalledLocation;
            int i;

            if (ImageFolder == null)
                ImageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);

            Files.Clear(); //Get rid of the old list
            
            await GenericCodeClass.GetListOfLatestFiles(Files, 3);
            //GenericCodeClass.GetListOfURLs(Files, 6);
            
            for (i = 0; i < Files.Count; i++)
            {
                StatusBox.Text = string.Concat(StatusBox.Text, Files[i]);
                StatusBox.Text = string.Concat(StatusBox.Text, Environment.NewLine);
            }

            await GenericCodeClass.DownloadFiles(ImageFolder, Files, Files.Count);

            if (Files.Count > 1)
            {
                CurrImgIndex = 0;
                //ImgBox.Source = GenericCodeClass.GetBitmapImage(Files[CurrImgIndex]);
                //MapBox.ImageLocation = URLPath + Files[CurrImgIndex];
            }
            else
                CurrImgIndex = -1;
            //StatusBox.Text = "You clicked Download Button";
            DownloadTimer.Start();
            LoopTimer.Start();
        }

        private async Task ChangeImage(bool ShowNextImage)
        {
            //WriteableBitmap tmpBitmap;
            LoopTimer.Stop();   //Stop the loop timer to allow enough time to change image

            if (ImageFolder == null)
                ImageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);

            if (CurrImgIndex != -1 && Files.Count != 0)
            {
                if (ShowNextImage)
                {
                    CurrImgIndex = ++CurrImgIndex % Files.Count;
                }
                else
                {
                    CurrImgIndex = (CurrImgIndex + Files.Count - 1) % Files.Count;
                }
                
                ImgBox.Source = await GenericCodeClass.GetBitmapImage(ImageFolder, Files[CurrImgIndex]);
                //ImgBox.Source = await GenericCodeClass.GetWriteableBitmap(ImageFolder, Files[CurrImgIndex]);
                //ImgBox.Source = ImgSource;
                //ImgBox.Source = await GenericCodeClass.GetBitmapImage(ImageFolder, "2014186_1730vis.jpg");
                //MapBox.ImageLocation = URLPath + Files[CurrImgIndex];
                StatusBox.Text = "Next Button:" + "CurrImgIndex = " + CurrImgIndex.ToString() + "::" + Files[CurrImgIndex].ToString();
            }
            else
            {
                //ImgBox.Source = await GenericCodeClass.GetWriteableBitmap(ImageFolder, "Error.jpg");
                ImgBox.Source = await GenericCodeClass.GetBitmapImage(ImageFolder, "Error.jpg");
            }

            LoopTimer.Start();
            //tmpBitmap = (WriteableBitmap)ImgBox.Source;
            //GenericCodeClass.OverlayFileInImage(ImageFolder, tmpBitmap, "Overlay.jpg");
            //tmpBitmap.Invalidate();
            //GenericCodeClass.OverlayFiles(ImageFolder, "test.jpg", "CWA.gif");
        }

        private async void Timer_Handler(object sender, object e)
        {
            DispatcherTimer tmpTimer = (DispatcherTimer)sender;

            tmpTimer.Stop();
            if (tmpTimer == LoopTimer)
            {
                await ChangeImage(true);
            }
            else if (tmpTimer == DownloadTimer)
            {
                LoopTimer.Stop();
                await DownloadFiles();
                LoopTimer.Start();
            }
            tmpTimer.Start();
        }
    }
}
