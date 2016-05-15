using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using InkingAlphabets.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Controls;

namespace InkingAlphabets.ViewModel
{
    public class SelectLanguagePageViewModel : ViewModelBase
    {
        private string _pageTitle = string.Empty;
        private readonly ILanguagesDataService _languagesDataService;
        private readonly INavigationService _navigationService;

        private ObservableCollection<Language> _languages;
        private Language _selectedLanguage;
        

        public SelectLanguagePageViewModel(         
         ILanguagesDataService languagesDataService,
         INavigationService navigationService)
        {            
            _languagesDataService = languagesDataService;
            _navigationService = navigationService;         
        }

        public string PageTitle
        {
            get
            {
                return _pageTitle;
            }

            set
            {
                Set(ref _pageTitle, value);
            }
        }

        public ObservableCollection<Language> Languages
        {
            get
            {
                return _languages;
            }

            set
            {
                Set(ref _languages, value);
            }
        }

        public Language SelectedLanguage
        {
            get
            {
                return _selectedLanguage;
            }

            set
            {
                if (value != null)
                {
                    _languagesDataService.SelectNewLanguage(value);
                    Microsoft.HockeyApp.HockeyClient.Current.TrackEvent($"Selected Language:  {value.LanguageName}");
                    Set(ref _selectedLanguage, value);
                }
            }
        }

        public async Task LoadLanguagesAsync()
        {
            try
            {
                PageTitle = "Select a Language";
                Languages = await _languagesDataService.GetLanguagesAsync();
                if (Languages.Count == 0)
                    throw new Exception("No available languages");
                
                SelectedLanguage = await _languagesDataService.GetSelectedLanguageAsync();

               
            }
            catch
            {
                //LOG exception
                throw;
            }
        }



    }
}
