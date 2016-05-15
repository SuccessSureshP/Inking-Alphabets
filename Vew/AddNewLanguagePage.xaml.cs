using GalaSoft.MvvmLight.Views;
using InkingAlphabets.ViewModel;
using Microsoft.Practices.ServiceLocation;
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

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace InkingAlphabets
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class AddNewLanguagePage : Page
    {
        public AddNewLanguagePage()
        {
            this.InitializeComponent();
            this.Loaded += AddNewLanguagePage_Loaded;
        }

        private void AddNewLanguagePage_Loaded(object sender, RoutedEventArgs e)
        {
            LanguageTextBox.Focus(FocusState.Keyboard);
        }

        private async void AppbarButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if(LanguageTextBox.Text.Trim().TrimStart().TrimEnd().Length == 0)
            {
                var dialog = ServiceLocator.Current.GetInstance<IDialogService>();
                await dialog.ShowMessage("Please specify Name of the new language", "Error");
                return;
            }      
            string alphabetsText;
            AlphabetsRichEditBox.Document.GetText(Windows.UI.Text.TextGetOptions.None, out alphabetsText);
            alphabetsText =  alphabetsText.Trim().TrimEnd().TrimStart();
            if (alphabetsText.Length == 0 || alphabetsText.Length == 1)
            {
                var dialog = ServiceLocator.Current.GetInstance<IDialogService>();
                await dialog.ShowMessage("Please specify alphabets of the new language","Error");
                return;
            }

            var viewModel = (AddNewLanguagePageViewModel)this.DataContext;
            await viewModel.AddNewLanguageAsync(LanguageTextBox.Text, alphabetsText);

            var shell = (Shell)Window.Current.Content;
            shell.ViewModel.SelectedTopItem = shell.ViewModel.TopItems.First(i => i.PageType == typeof(InkAlphabets));
        }
    }
}
