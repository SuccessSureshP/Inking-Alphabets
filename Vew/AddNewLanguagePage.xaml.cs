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
                var msg = new MessageDialog("Please specify Name of the new language");
                LanguageTextBox.Focus(FocusState.Keyboard);
                await msg.ShowAsync();
                return;
            }      
            string alphabetsText;
            AlphabetsRichEditBox.Document.GetText(Windows.UI.Text.TextGetOptions.None, out alphabetsText);
            alphabetsText = alphabetsText.Trim().TrimEnd().TrimStart();//.Replace(" ", "");
            if (alphabetsText.Length == 0)
            {                
                var msg = new MessageDialog("Please specify alphabets of the new language");
                AlphabetsRichEditBox.Focus(FocusState.Keyboard);
                await msg.ShowAsync();
                return;
            }

            var msgDialog = new MessageDialog($@"Make sure you have at least one space between alphabets. You can enter alphabets in different lines. Is everything fine?");
            msgDialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(async (IUICommand c) =>
            {
                var viewModel = (AddNewLanguagePageViewModel)this.DataContext;
                await viewModel.AddNewLanguageAsync(LanguageTextBox.Text, alphabetsText);

                var shell = (Shell)Window.Current.Content;
                shell.ViewModel.SelectedTopItem = shell.ViewModel.TopItems.First(i => i.PageType == typeof(InkAlphabets));
            })));
            msgDialog.Commands.Add(new UICommand("No, Let me edit"));
            msgDialog.DefaultCommandIndex = 0;
            await msgDialog.ShowAsync();
        }
    }
}
