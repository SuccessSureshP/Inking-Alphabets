using InkingAlphabets.ViewModel;
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
    public sealed partial class SelectLanguagePage : Page
    {
        SelectLanguagePageViewModel viewModel;
        public SelectLanguagePage()
        {
            this.InitializeComponent();
            this.Loaded += SelectLanguagePage_Loaded;
            this.Unloaded += SelectLanguagePage_Unloaded;
        }

        private void SelectLanguagePage_Unloaded(object sender, RoutedEventArgs e)
        {
            RemoveHandlers();            
        }

        private void RemoveHandlers()
        {
            LanguagesGridView.SelectionChanged -= LanguagesGridView_SelectionChanged;
        }

        private async void SelectLanguagePage_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                viewModel = (SelectLanguagePageViewModel)this.DataContext;
                await viewModel.LoadLanguagesAsync();

                LanguagesGridView.SelectionChanged -= LanguagesGridView_SelectionChanged;
                LanguagesGridView.SelectionChanged += LanguagesGridView_SelectionChanged;
            }
            catch(Exception exp)
            {
                if (exp.Message.Equals("No available languages"))
                {
                    var msgDialog = new MessageDialog("No languages available.");                    
                    msgDialog.Commands.Add(new UICommand("Ok"));
                    await msgDialog.ShowAsync();
                }
            }
        }

        private void LanguagesGridView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LanguagesGridView.SelectedItem != null)
            {
                var shell = (Shell)Window.Current.Content;
                RemoveHandlers();
                shell.ViewModel.SelectedTopItem = shell.ViewModel.TopItems.First(i => i.PageType == typeof(InkAlphabets));
            }
        }

        private void AppbarButtonAddNew_Click(object sender, RoutedEventArgs e)
        {
            var shell = (Shell)Window.Current.Content;
            RemoveHandlers();
            shell.ViewModel.SelectedTopItem = shell.ViewModel.TopItems.First(i => i.PageType == typeof(AddNewLanguagePage));
        }
    }
}
