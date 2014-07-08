using System;
using Windows.Web.Http;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Graphics.Imaging;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

static class GenericCodeClass
{
    public static void GetListOfURLs(List<string> FileNames, int NoOfFiles)
    {
            DateTime CurrDateTime = DateTime.Now.ToUniversalTime();
            DateTime StartOfYearDate = new DateTime(CurrDateTime.Year-1, 12, 31);
            TimeSpan NoOfDays = CurrDateTime.Subtract(StartOfYearDate);
            int i;
         

            FileNames.Clear();

            if (CurrDateTime.Minute < 30)
                CurrDateTime = CurrDateTime.AddMinutes(0.0 - CurrDateTime.Minute);
            else
                CurrDateTime = CurrDateTime.AddMinutes(30.0 - CurrDateTime.Minute);

            for (i = 0; i < NoOfFiles; i++)
            {
                FileNames.Add(CurrDateTime.Year.ToString() + NoOfDays.Days.ToString() + "_" + CurrDateTime.Hour.ToString("D2") + CurrDateTime.Minute.ToString("D2") + "vis.jpg");
                CurrDateTime = CurrDateTime.AddMinutes(-30.0);
                NoOfDays = CurrDateTime.Subtract(StartOfYearDate);
            }

            FileNames.Reverse();
    }

    public static async void DownloadFiles(List<string> Filenames, int NoOfFiles)
    {
        string URLPath = "http://www.ssd.noaa.gov/goes/west/wfo/sew/img/";
        string FilePath;
        //var URI = new Uri("http://www.ssd.noaa.gov/goes/west/wfo/sew/img/2014186_2030vis.jpg");
        int i;
        StorageFolder ImageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);


        for (i = 0; i < NoOfFiles; i++)
        {
            FilePath = ImageFolder.Path + Filenames[i];

            //if (!File.Exists(FilePath))
            //{
                try
                {
                    GetFileUsingHttp(URLPath + Filenames[i], ImageFolder, Filenames[i]);
                }
                catch
                {
                    Filenames[i] = "Error";
                }
            //}
        }

        //Filenames.RemoveAll(IsError);
    }

    public static async void GetFileUsingHttp(string URL, StorageFolder Folder, string FileName)
    {
        HttpClient Client = new HttpClient();
        var URI = new Uri(URL);
        HttpResponseMessage message = await Client.GetAsync(URI);

        StorageFile sampleFile = await Folder.CreateFileAsync(FileName, CreationCollisionOption.ReplaceExisting);// this line throws an exception
        var FileBuffer = await message.Content.ReadAsBufferAsync();

        await FileIO.WriteBufferAsync(sampleFile, FileBuffer);
    }

    public static async Task<BitmapImage> GetBitmapImage(string FileName)
    {
        StorageFolder ImageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
        StorageFile ImageFile = await ImageFolder.CreateFileAsync(FileName, CreationCollisionOption.OpenIfExists);

        BitmapImage Image = new BitmapImage();

        Image = await LoadImage(ImageFile);

        return Image;
    }

    private static async Task<BitmapImage> LoadImage(StorageFile file)
    {
        BitmapImage bitmapImage = new BitmapImage();
        FileRandomAccessStream stream = (FileRandomAccessStream)await file.OpenAsync(FileAccessMode.Read);

        bitmapImage.SetSource(stream);

        return bitmapImage;

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

    public static async void OverlayFiles(string Basefile, string OverlayFile)
    {
        StorageFolder ImageFolder = await ApplicationData.Current.LocalFolder.CreateFolderAsync("Images", CreationCollisionOption.OpenIfExists);
        StorageFile Base = await ImageFolder.CreateFileAsync(Basefile, CreationCollisionOption.OpenIfExists);
        StorageFile Overlay = await ImageFolder.CreateFileAsync(OverlayFile, CreationCollisionOption.OpenIfExists);
        var BaseImageStream = await Base.OpenAsync(Windows.Storage.FileAccessMode.ReadWrite);
        var OverlayImageStream = await Overlay.OpenAsync(Windows.Storage.FileAccessMode.Read);
        var BaseImageDecoder = await BitmapDecoder.CreateAsync(BaseImageStream);
        var OverlayImageDecoder = await BitmapDecoder.CreateAsync(OverlayImageStream);
        var BasePixelData = await BaseImageDecoder.GetPixelDataAsync();
        var OverlayPixelData = await OverlayImageDecoder.GetPixelDataAsync();
        var BasePixel = BasePixelData.DetachPixelData();
        var OverlayPixel = OverlayPixelData.DetachPixelData();
        int i, j;
        uint height = BaseImageDecoder.PixelHeight, width = BaseImageDecoder.PixelWidth;

        for (i = 0; i < height; i++)
        {
            for (j = 0; j < width; j++)
            {
                if (OverlayPixel[(i * width + j) * 4 + 0] > 0)
                {
                    BasePixel[(i * width + j) * 4 + 0] = OverlayPixel[(i * width + j) * 4 + 0];
                    BasePixel[(i * width + j) * 4 + 1] = OverlayPixel[(i * width + j) * 4 + 1];
                    BasePixel[(i * width + j) * 4 + 2] = OverlayPixel[(i * width + j) * 4 + 2];
                    BasePixel[(i * width + j) * 4 + 3] = OverlayPixel[(i * width + j) * 4 + 3];
                }
            }
        }

        //InMemoryRandomAccessStream mem = new InMemoryRandomAccessStream();
        var encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.JpegEncoderId, BaseImageStream);
        encoder.SetPixelData(BitmapPixelFormat.Bgra8, BitmapAlphaMode.Straight, width, height, BaseImageDecoder.DpiX, BaseImageDecoder.DpiY, BasePixel);
        await encoder.FlushAsync();
    }

    public static bool IsError(string s)
    {
        return s.Equals("Error");
    }
}
