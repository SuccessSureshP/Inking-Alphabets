using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace InkingAlphabets
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class SettingsPage : Page
    {
        IPropertySet _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings.Values;
        public SettingsPage()
        {
            this.InitializeComponent();
            this.Loaded += SettingsPage_Loaded;
        }

        private async void SettingsPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (_localSettings["SlateBackgroundImageStatus"].Equals("DefaultImageCopied"))
            {
                ChangeInkingSlateBGButton.Content = "Click to Add";
            }
            else
            {
                ChangeInkingSlateBGButton.Content = "Click to Change";
            }

            var LocalDataFolder = ApplicationData.Current.LocalFolder;
            var assetsFolder = await LocalDataFolder.GetFolderAsync(App.AssetsFolderName);
            var file = await assetsFolder.GetFileAsync("InkingSlateBackground.png");
            var data = await FileIO.ReadBufferAsync(file);

            // create a stream from the file
            var ms = new InMemoryRandomAccessStream();
            var dw = new Windows.Storage.Streams.DataWriter(ms);
            dw.WriteBuffer(data);
            await dw.StoreAsync();
            ms.Seek(0);

            // find out how big the image is, don't need this if you already know
            var bm = new BitmapImage();
            bm.DecodePixelHeight = 480;
            bm.DecodePixelWidth = 480;

            await bm.SetSourceAsync(ms);

            // create a writable bitmap of the right size
            var wb = new WriteableBitmap(bm.PixelWidth, bm.PixelHeight);
            ms.Seek(0);

            // load the writable bitpamp from the stream
            await wb.SetSourceAsync(ms);

            gbImage.Source = wb;


        }

       
        private async void ChangeInkingSlateBGButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {   
                var fileOpenPicker = new Windows.Storage.Pickers.FileOpenPicker();
                fileOpenPicker.FileTypeFilter.Add(".png");
                fileOpenPicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                StorageFile newbgFile = await fileOpenPicker.PickSingleFileAsync();

                var LocalDataFolder = ApplicationData.Current.LocalFolder;
                var destnationFolder = await LocalDataFolder.GetFolderAsync(App.AssetsFolderName);
                if (newbgFile != null)
                {
                    await newbgFile.CopyAsync(destnationFolder, "InkingSlateBackground.png", NameCollisionOption.ReplaceExisting);
                }


                //var LocalDataFolder = ApplicationData.Current.LocalFolder;
                var assetsFolder = await LocalDataFolder.GetFolderAsync(App.AssetsFolderName);
                var file = await assetsFolder.GetFileAsync("InkingSlateBackground.png");
                var data = await FileIO.ReadBufferAsync(file);

                // create a stream from the file
                var ms = new InMemoryRandomAccessStream();
                var dw = new Windows.Storage.Streams.DataWriter(ms);
                dw.WriteBuffer(data);
                await dw.StoreAsync();
                ms.Seek(0);

                // find out how big the image is, don't need this if you already know
                var bm = new BitmapImage();
                bm.DecodePixelHeight = 480;
                bm.DecodePixelWidth = 480;

                await bm.SetSourceAsync(ms);

                // create a writable bitmap of the right size
                var wb = new WriteableBitmap(bm.PixelWidth, bm.PixelHeight);
                ms.Seek(0);

                // load the writable bitpamp from the stream
                await wb.SetSourceAsync(ms);

                gbImage.Source = wb;

                _localSettings["SlateBackgroundImageStatus"] = "DefaultImageReplaced";


            }
            catch (Exception exp)
            {
                Microsoft.HockeyApp.HockeyClient.Current.TrackEvent($"ChangeInkingSlateBGButton_Click Failed with Exception:{exp.Message}");
                throw exp;
            }
        }
    }
}
