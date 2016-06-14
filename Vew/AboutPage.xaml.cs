using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.ApplicationModel.Email;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace InkingAlphabets
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AboutPage : Page
    {
        public AboutPage()
        {
            this.InitializeComponent();
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
        }

        private async void SendFeedbackButton_Click(object sender, RoutedEventArgs e)
        {
            EmailMessage emailMessage = new EmailMessage();
            emailMessage.To.Add(new EmailRecipient("RedmondMSDev@outlook.com"));
            string messageSubject = "Hello Inking Alphabets team";
            emailMessage.Subject = messageSubject;
            emailMessage.Body = "Type your feedback here....";

            await EmailManager.ShowComposeNewEmailAsync(emailMessage);
        }

        private async void RateAndReviewtheAppButton_Click(object sender, RoutedEventArgs e)
        {
            //await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:reviewapp?appid=24118SureshPokkuluri.InkingAlphabets"));
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store:REVIEW?PFN==24118SureshPokkuluri.InkingAlphabets"));

        }

        private async void ShowappDetailsInStoreButton_Click(object sender, RoutedEventArgs e)
        {
            await Windows.System.Launcher.LaunchUriAsync(new Uri("ms-windows-store://pdp/?productid=9NBLGGH5HFV4"));
        }

        private void ShareAppbarButton_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }
        private async void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            var deferral = request.GetDeferral();
            try
            {

                var sourceFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
            var assetsFolder = await sourceFolder.GetFolderAsync(App.AssetsFolderName);
           
            var file = await assetsFolder.GetFileAsync("Wide310x150Logo.scale-200.png");

            List<IStorageItem> st_items = new List<IStorageItem>();
            st_items.Add(file);

            request.Data.SetStorageItems(st_items);
            request.Data.Properties.Title = "Check this cool Inking Alphabets App at https://www.microsoft.com/store/apps/9nblggh5hfv4";
            }
            catch (Exception ex)
            {
                MessageDialog msg = new MessageDialog("Something went wrong! Sorry. Please try again");
                await msg.ShowAsync();
                Microsoft.HockeyApp.HockeyClient.Current.TrackEvent($"Sharing the App link failed with exception :{ex.Message}");
            }
            finally
            {
                deferral.Complete();
            }

        }
    }
}
