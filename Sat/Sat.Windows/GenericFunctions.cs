using System;
using Windows.Web.Http;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text.RegularExpressions;

static class GenericCodeClass
{
    private static TimeSpan LoopTimerInterval = new TimeSpan(0,0,0,0,500); //Loop timer interval in seconds
    private static TimeSpan DownloadTimerInterval = new TimeSpan(0,15,0); //Download time interval in minutes
    private static string HomeStationURL = "http://www.ssd.noaa.gov/goes/west/wfo/sew/img/";
    private static string HomeStationString ="Seattle";
    private static bool IsHomeStationChanged = false;
    private static bool IsECLightningDataSelected = false;
    private static HttpClient Client;
    private static HttpResponseMessage Message;
    private static int DownloadPeriod = 3;
    public static List<string> ExistingFiles = new List<string>();
    public static bool IsLoopPaused = false;
    public static bool IsAppResuming = false;

    //Provide access to private property specifying Loop timer Interval
    public static TimeSpan LoopInterval
    {
        get { return LoopTimerInterval; }
        set { LoopTimerInterval = value; }
    }

    //Provide access to private property specifying Download timer Interval
    public static TimeSpan DownloadInterval
    {
        get { return DownloadTimerInterval; }
        set { DownloadTimerInterval = value; }
    }

    public static int FileDownloadPeriod
    {
        get { return DownloadPeriod; }
        set { DownloadPeriod = value; }
    }

    //Provide access to private property specifying whether home station has changed
    public static bool HomeStationChanged
    {
        get { return IsHomeStationChanged; }
        set { IsHomeStationChanged = value; }
    }

    //Provide access to private property specifying Home Weather Station
    public static string HomeStation
    {
        get { return HomeStationURL; }
        set 
        {
            if (HomeStationURL != value)
                IsHomeStationChanged = true;

            HomeStationURL = value;
        }
    }

    public static string HomeStationName
    {
        get { return HomeStationString; }
        set { HomeStationString = value;}
    }

    //Provide access to private property specifying whether home station has changed
    public static bool LightningDataSelected
    {
        get { return IsECLightningDataSelected; }
        set { IsECLightningDataSelected = value; }
    }

    public static DateTime GetDateTimeFromFile(string Filename)
    {
        DateTime LocalDateTime;

        string Time;
        string Year;
        string Day;
        string Month;

        if(Filename.EndsWith(".png"))	//Extract date from Env. Canada lightning data image file name
        {
            Time = Filename.Substring(12, 4);
            Year = Filename.Substring(4,4);
            Day =  Filename.Substring(10,2);
            Month = Filename.Substring(8,2);
            LocalDateTime = new DateTime(Convert.ToInt32(Year), Convert.ToInt32(Month), Convert.ToInt32(Day), Convert.ToInt32(Time.Substring(0, 2)), Convert.ToInt32(Time.Substring(2, 2)), 0);
        }
        else	//Extract date from NOAA satellite data image file name
        {
            Time = Filename.Substring(8, 4);
            Year = Filename.Substring(0,4);
            Day =  Filename.Substring(4,3);
            LocalDateTime = new DateTime(Convert.ToInt32(Year) - 1, 12, 31, Convert.ToInt32(Time.Substring(0, 2)), Convert.ToInt32(Time.Substring(2, 2)), 0);
            LocalDateTime = LocalDateTime.AddDays(Convert.ToDouble(Day)).ToLocalTime();
        }
        
        
        return LocalDateTime;
    }

    public static async Task GetListOfLatestFiles(List<string> FileNames)
    {
        var URI = new Uri(HomeStationURL);
        string StartDateTimeString;
        int i;
        Regex RegExp;

        ExistingFiles.Clear();

        if(IsHomeStationChanged == false)
        {
            foreach (string str in FileNames)
            {
                ExistingFiles.Add(str);
            }
        }
        
        FileNames.Clear();

        if (LightningDataSelected == true)
        {
            GenericCodeClass.GetWeatherDataURLs(FileNames, 6);
            return;
        }

        if (Client == null)
            Client = new HttpClient();

        var HttpClientTask = Client.GetAsync(URI);

        //string RegExpString = ">\\s*";
        DateTime CurrDateTime = DateTime.Now.ToUniversalTime();
        DateTime StartOfYearDate = new DateTime(CurrDateTime.Year - 1, 12, 31);
        DateTime StartDateTime = CurrDateTime.Subtract(new TimeSpan(DownloadPeriod, 0, 0));    //Subtract 3 hours from the Current Time
        TimeSpan NoOfDays = CurrDateTime.Subtract(StartOfYearDate);

        //if (StartDateTime.Year != CurrDateTime.Year)
        //    RegExpString = RegExpString + "(" + CurrDateTime.Year.ToString() + "|" + StartDateTime.Year.ToString() + ")";
        //else
        //    RegExpString = RegExpString + CurrDateTime.Year.ToString();

        //if (StartDateTime.Day != CurrDateTime.Day)
        //{
        //    RegExpString = RegExpString + "(" + NoOfDays.Days.ToString() + "|";
        //    NoOfDays = StartDateTime.Subtract(StartOfYearDate);
        //    RegExpString = RegExpString + NoOfDays.Days.ToString() + ")_(";
        //}
        //else
        //    RegExpString = RegExpString + NoOfDays.Days.ToString() + "_(";

        //if (StartDateTime.Hour > CurrDateTime.Hour)  //When the start and current time are on either side of midnight
        //{
        //    for (i = StartDateTime.Hour; i <= 23; i++)
        //        RegExpString = RegExpString + i.ToString("D2") + "|";

        //    for (i = 0; i < CurrDateTime.Hour; i++)
        //        RegExpString = RegExpString + i.ToString("D2") + "|";
        //}
        //else
        //{
        //    for (i = StartDateTime.Hour; i < CurrDateTime.Hour; i++)
        //        RegExpString = RegExpString + i.ToString("D2") + "|";
        //}

        //RegExpString = RegExpString + CurrDateTime.Hour.ToString("D2") + ")[0-9][0-9]vis.jpg\\s*<";

        StartDateTimeString = StartDateTime.Year.ToString() + StartDateTime.Subtract(StartOfYearDate).Days.ToString()
            + "_" + StartDateTime.Hour.ToString("D2") + StartDateTime.Minute.ToString("D2") + "vis.jpg";
        
        FileNames.Add(StartDateTimeString);
        RegExp = new Regex(">[0-9]+_[0-9]+vis.jpg<");

        try
        {
            //Message = await Client.GetAsync(URI);
            Message = await HttpClientTask;
        }
        catch (Exception e)
        {
            FileNames.Remove(StartDateTimeString);
            return;
        }


        if (Message.IsSuccessStatusCode)
        {
            int MessageLength = Message.Content.ToString().Length;
            MatchCollection Matches = RegExp.Matches(Message.Content.ToString());//.Substring(MessageLength/2));
            int Location;
            
            if (Matches.Count > 0)
            {
                foreach (Match match in Matches)
                {
                    if (match.Success)
                    {
                        string tmp = match.ToString();	//The regular expression matches the "<" and ">" signs around the filename. These signs have to be removed before adding the filename to the list
                        FileNames.Add(tmp.Substring(1, tmp.Length - 2));
                    }
                }
                FileNames.Sort();
                Location = FileNames.IndexOf(StartDateTimeString);
                FileNames.RemoveRange(0, Location + 1);
            }
        }
        else
        {
            //return some sort of error code?
        }
    }

    public static void GetWeatherDataURLs(List<string> FileNames, int NoOfFiles)
    {
        DateTime CurrDateTime = DateTime.Now.ToUniversalTime();
        int i;

        //No need to save previous files as that is done in the function GetLatestFiles()

        CurrDateTime = CurrDateTime.AddMinutes(-CurrDateTime.Minute % 10);

        //if (CurrDateTime.Minute < 30)
        //    CurrDateTime = CurrDateTime.AddMinutes(0.0 - CurrDateTime.Minute);
        //else
        //    CurrDateTime = CurrDateTime.AddMinutes(30.0 - CurrDateTime.Minute);

        for (i = 0; i < NoOfFiles; i++)
        {
            //FileNames.Add(CurrDateTime.Year.ToString() + NoOfDays.Days.ToString() + "_" + CurrDateTime.Hour.ToString("D2") + CurrDateTime.Minute.ToString("D2") + "vis.jpg");
            FileNames.Add("PAC_" + CurrDateTime.Year.ToString() + CurrDateTime.Month.ToString("D2") + CurrDateTime.Day.ToString("D2") + CurrDateTime.Hour.ToString("D2") + CurrDateTime.Minute.ToString("D2") + ".png");
            //CurrDateTime = CurrDateTime.AddMinutes(-30.0);
            //NoOfDays = CurrDateTime.Subtract(StartOfYearDate);
            CurrDateTime = CurrDateTime.AddMinutes(-10);
        }

        FileNames.Reverse();
    }

    //public static async Task DownloadFiles(StorageFolder ImageFolder, List<string> Filenames, int NoOfFiles)
    //{
    //    //string URLPath = "http://weather.gc.ca/data/lightning_images/";
    //    //string URLPath = "http://www.ssd.noaa.gov/goes/" + HomeWeatherStation + "/img/";
    //    int i;
    //    int RetCode; //Error code to check whether file was downloaded successfully
    //    //Task<int>[] TaskArray = new Task<int>[Filenames.Count];
        
    //    if(ImageFolder == null)
    //        ImageFolder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
        
    //    //Get list of files currently in the local data folder
    //    //var FileList = await ImageFolder.GetFilesAsync();

    //    for (i = 0; i < NoOfFiles; i++)
    //    {
    //        //Check whether the file already exists
    //        //if (FileExists(FileList, Filenames[i]) && IsHomeStationChanged == false)
    //        //    continue;

    //        if (ExistingFiles.Contains(Filenames[i].ToString()) && IsHomeStationChanged == false)
    //            continue;

    //        RetCode = await GetFileUsingHttp(HomeStationURL + Filenames[i], ImageFolder, Filenames[i]);
    //        //TaskArray[i] = GetFileUsingHttp(URLPath + Filenames[i], ImageFolder, Filenames[i]);

    //        if (RetCode == -1)
    //        {
    //            Filenames[i] = "Error.jpg";
    //        }
    //    }

    //    //for (i = 0; i < NoOfFiles; i++)
    //    //{
    //    //    if(TaskArray[i] != null)
    //    //    {
    //    //        RetCode = await TaskArray[i];

    //    //        if (RetCode == -1)
    //    //        {
    //    //            Filenames[i] = "Error.jpg";
    //    //        }
    //    //    }            
    //    //}
        
    //    Filenames.RemoveAll(IsError);
    //    IsHomeStationChanged = false;
    //}

    public static async Task<int> GetFileUsingHttp(string URL, StorageFolder Folder, string FileName)
    {
        var URI = new Uri(URL);
        StorageFile sampleFile;
                
        if (Client == null)
            Client = new HttpClient();

        //var DownloadFileTask = (System.Threading.Tasks.Task<Windows.Web.Http.HttpResponseMessage>)Client.GetAsync(URI); //Make this call asynchronous and crreate disk file in the mean time?
        //Message = await DownloadFileTask;
        Message = await Client.GetAsync(URI);

        if (Message.IsSuccessStatusCode)
        {
            sampleFile = await Folder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);// this line throws an exception
            var FileBuffer = await Message.Content.ReadAsBufferAsync();
            await FileIO.WriteBufferAsync(sampleFile, FileBuffer);
            return 0; //Return code to show an image was successfully downloaded.
        }
        else
        {
            return -1; //Error code to show image was not downloaded successfully.
        }
    }

    public static bool FileExists(IReadOnlyList<StorageFile> FileList, string FileName)
    {
        int i;

        for (i = 0; i < FileList.Count; i++)
            if (FileName.Equals(FileList[i].Name))
                return true;

        return false;
    }

    public static async Task<BitmapImage> GetBitmapImage(StorageFolder ImageFolder, string FileName)
    {
        if (ImageFolder == null)
        {
            ImageFolder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
        }

        StorageFile ImageFile = await ImageFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);
        BitmapImage Image;

        Image = await LoadBitmapImage(ImageFile);
                
        return Image;
    }

    private static async Task<BitmapImage> LoadBitmapImage(StorageFile file)
    {
        BitmapImage Image = new BitmapImage();
        FileRandomAccessStream stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);

        Image.SetSource(stream);

        return Image;

    }

    public static async Task<WriteableBitmap> GetWriteableBitmap(StorageFolder ImageFolder, string FileName)
    {
        StorageFile ImageFile;
        WriteableBitmap ImageBitmap;
        //BitmapImage Image = new BitmapImage();

        if(ImageFolder == null)
        {
             ImageFolder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
        }

        ImageFile = await ImageFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);
        ImageBitmap = await LoadWriteableBitmap(ImageFile);
               
        return ImageBitmap;
    }

    private static async Task<WriteableBitmap> LoadWriteableBitmap(StorageFile file)
    {
        WriteableBitmap ImageBitmap;
        //BitmapImage Image = new BitmapImage();
        FileRandomAccessStream stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);

        //Image.SetSource(stream);

        ImageBitmap = new WriteableBitmap(720, 480);//Image.PixelWidth,Image.PixelHeight);
        //stream.Position = 0;
        ImageBitmap.SetSource(stream);

        return ImageBitmap;

    }

    public static async Task DeleteAllFiles(StorageFolder ImageFolder)
    {
        StorageFile File;
        int i;
        
        if (ImageFolder == null)
            ImageFolder = await ApplicationData.Current.TemporaryFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);

        //Get list of files currently in the local data folder
        var FileList = await ImageFolder.GetFilesAsync();

        for (i = 0; i < FileList.Count; i++)
        {
            File = await ImageFolder.GetFileAsync(FileList[i].Name);
            await File.DeleteAsync();
        }
    }

    //public static void GetFileUsingHttp(string URL, string StorePath)
    //{
    //    HttpWebRequest Client = (HttpWebRequest) WebRequest.CreateHttp(URL);
    //    HttpWebResponse ClientResp = (HttpWebResponse) Client.GetResponse();
    //    BinaryReader ClientRespStream = new BinaryReader(ClientResp.GetResponseStream());
    //    MemoryStream Image = new MemoryStream();
    //    byte[] buffer;
    //    FileStream ImgFile = new FileStream(StorePath, FileMode.Create);

    //    buffer = ClientRespStream.ReadBytes(1024);

    //    while (buffer.Length > 0)
    //    {
    //        Image.Write(buffer, 0, buffer.Length);
    //        buffer = ClientRespStream.ReadBytes(1024);
    //    }

    //    byte[] lnFile = new byte[(int)Image.Length];
    //    Image.Position = 0;
    //    Image.Read(lnFile, 0, lnFile.Length);
    //    Image.Close();
    //    Image.Close();

    //    ImgFile.Write(lnFile, 0, lnFile.Length);
    //    ImgFile.Close();
    //}

    //public static void Cleanup()
    //{
    //    DirectoryInfo di = new DirectoryInfo("../Images");
        
    //    foreach (FileInfo file in di.GetFiles())
    //    {
    //        file.Delete();
    //    }
    //}

    //public static async void OverlayFiles(StorageFolder ImgFolder, string Basefile, string OverlayFile)
    //{
    //    //StorageFolder ImageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
    //    //if (ImageFolder == null)
    //    //{
    //    //    ImageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
    //    //}
    //    StorageFolder ImageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
    //    StorageFile Base = await ImageFolder.CreateFileAsync(Basefile, CreationCollisionOption.OpenIfExists);
    //    StorageFile Overlay = await ImageFolder.CreateFileAsync(OverlayFile, CreationCollisionOption.OpenIfExists);
    //    var BaseImageStream = await Base.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
    //    var OverlayImageStream = await Overlay.OpenAsync(Windows.Storage.FileAccessMode.Read);
    //    var BaseImageDecoder = await BitmapDecoder.CreateAsync(BaseImageStream);
    //    var OverlayImageDecoder = await BitmapDecoder.CreateAsync(OverlayImageStream);
    //    var BasePixelData = await BaseImageDecoder.GetPixelDataAsync();
    //    var OverlayPixelData = await OverlayImageDecoder.GetPixelDataAsync();
    //    var BasePixel = BasePixelData.DetachPixelData();
    //    var OverlayPixel = OverlayPixelData.DetachPixelData();
    //    int i, j;
    //    uint height = BaseImageDecoder.PixelHeight, width = BaseImageDecoder.PixelWidth;

    //    for (i = 0; i < height; i++)
    //    {
    //        for (j = 0; j < width; j++)
    //        {
    //            if (OverlayPixel[(i * width + j) * 4 + 0] > 250)
    //            {
    //                BasePixel[(i * width + j) * 4 + 0] = OverlayPixel[(i * width + j) * 4 + 0];
    //                BasePixel[(i * width + j) * 4 + 1] = OverlayPixel[(i * width + j) * 4 + 1];
    //                BasePixel[(i * width + j) * 4 + 2] = OverlayPixel[(i * width + j) * 4 + 2];
    //                BasePixel[(i * width + j) * 4 + 3] = OverlayPixel[(i * width + j) * 4 + 3];
    //            }
    //        }
    //    }

    //    //InMemoryRandomAccessStream mem = new InMemoryRandomAccessStream();
    //    var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, BaseImageStream);
    //    encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, width, height, BaseImageDecoder.DpiX, BaseImageDecoder.DpiY, BasePixel);
    //    await encoder.FlushAsync();
    //}

    //public static async void OverlayFileInImage(StorageFolder ImageFolder, WriteableBitmap CurrentImage, string OverlayFile)
    //{
    //    if (ImageFolder == null)
    //    {
    //        ImageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
    //    }

    //    StorageFile Overlay = await ImageFolder.CreateFileAsync(OverlayFile, CreationCollisionOption.OpenIfExists);
    //    var OverlayImageStream = await Overlay.OpenAsync(Windows.Storage.FileAccessMode.Read);
    //    var OverlayImageDecoder = await BitmapDecoder.CreateAsync(OverlayImageStream);
    //    var OverlayPixelData = await OverlayImageDecoder.GetPixelDataAsync();
    //    var OverlayPixel = OverlayPixelData.DetachPixelData();
    //    int i, j;
    //    int height = CurrentImage.PixelHeight, width = CurrentImage.PixelWidth;
    //    var BaseImageStream = CurrentImage.PixelBuffer.AsStream();  //Ashwin: this can be null if an image is not found/downloaded
    //    Byte[] BasePixel = new Byte[4 * width * height];

    //    BaseImageStream.Read(BasePixel, 0, BasePixel.Length);

    //    for (i = 0; i < height; i++)
    //    {
    //        for (j = 0; j < width; j++)
    //        {
    //            if (OverlayPixel[(i * width + j) * 4 + 0] > 250)
    //            {
    //                BasePixel[(i * width + j) * 4 + 0] = OverlayPixel[(i * width + j) * 4 + 0];
    //                BasePixel[(i * width + j) * 4 + 1] = OverlayPixel[(i * width + j) * 4 + 1];
    //                BasePixel[(i * width + j) * 4 + 2] = OverlayPixel[(i * width + j) * 4 + 2];
    //                BasePixel[(i * width + j) * 4 + 3] = OverlayPixel[(i * width + j) * 4 + 3];
    //            }
    //        }
    //    }

    //    BaseImageStream.Position = 0;
    //    BaseImageStream.Write(BasePixel, 0, BasePixel.Length);
    //}

    public static bool IsError(string s)
    {
        return s.Equals("Error.png");
    }
}
