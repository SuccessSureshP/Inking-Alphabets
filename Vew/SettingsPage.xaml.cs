using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI.Popups;
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
                RemoveInkingSlateBGButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                ChangeInkingSlateBGButton.Content = "Click to Change";
            }

            await LoadImageIntoThePage();
        }

        async Task LoadImageIntoThePage()
        {
            var LocalDataFolder = ApplicationData.Current.LocalFolder;            
            var assetsFolder = await LocalDataFolder.GetFolderAsync(App.AssetsFolderName);            
            var file = await assetsFolder.GetFileAsync(App.InkingSlateBackgroundFileName);
            var filestream = await file.OpenAsync(FileAccessMode.Read);

            var bitmapImage = new Windows.UI.Xaml.Media.Imaging.BitmapImage();
            await bitmapImage.SetSourceAsync(filestream);
            
            gbImage.Source = bitmapImage;
        }

        private async void ChangeInkingSlateBGButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {   
                var fileOpenPicker = new Windows.Storage.Pickers.FileOpenPicker();
                fileOpenPicker.FileTypeFilter.Add(".png");
                fileOpenPicker.FileTypeFilter.Add(".jpg");
                fileOpenPicker.FileTypeFilter.Add(".jpeg");
                fileOpenPicker.FileTypeFilter.Add(".bmp");

                //fileOpenPicker.ViewMode = Windows.Storage.Pickers.PickerViewMode.Thumbnail;
                StorageFile newbgFile = await fileOpenPicker.PickSingleFileAsync();

                var LocalDataFolder = ApplicationData.Current.LocalFolder;
                var destnationFolder = await LocalDataFolder.GetFolderAsync(App.AssetsFolderName);
                if (newbgFile != null)
                {   
                    await newbgFile.CopyAsync(destnationFolder, App.InkingSlateBackgroundFileName, NameCollisionOption.ReplaceExisting);

                    await LoadImageIntoThePage();
                    _localSettings["SlateBackgroundImageStatus"] = "DefaultImageReplaced";
                    ChangeInkingSlateBGButton.Content = "Click to Change";
                    RemoveInkingSlateBGButton.Visibility = Visibility.Visible;
                }
            }
            catch (Exception exp)
            {
                Microsoft.HockeyApp.HockeyClient.Current.TrackEvent($"ChangeInkingSlateBGButton_Click Failed with Exception:{exp.Message}");
                var msgDialog1 = new MessageDialog($"Something went wrong. Please contact us for support.");
                msgDialog1.Commands.Add(new UICommand("Ok"));
                await msgDialog1.ShowAsync();
            }
        }

        private async void RemoveInkingSlateBGButton_Click(object sender, RoutedEventArgs e)
        {
            await Common.CopyDefaultSlateBackgroundImage();
            ChangeInkingSlateBGButton.Content = "Click to Add";
            RemoveInkingSlateBGButton.Visibility = Visibility.Collapsed;
            await LoadImageIntoThePage();
        }
    }
}
