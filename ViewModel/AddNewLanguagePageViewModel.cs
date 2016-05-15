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
    public class AddNewLanguagePageViewModel : ViewModelBase
    {
        private string _pageTitle = string.Empty;
        private readonly ILanguagesDataService _languagesDataService;
        private readonly IAlphabetsDataService _alphabetsDataService;
        
        private Language _selectedLanguage;

        public AddNewLanguagePageViewModel(
       ILanguagesDataService languagesDataService,
       IAlphabetsDataService alphabetsDataService)
        {
            _languagesDataService = languagesDataService;
            _alphabetsDataService = alphabetsDataService;
            PageTitle = "Add New Language";
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

        public async Task AddNewLanguageAsync(string languageName,string alphabets)
        {
            Language newLanguage = new Language();
            newLanguage.LanguageName = languageName;
            newLanguage.AlphabetsCount = alphabets.Split('\r').Count();
            newLanguage.FilePath = languageName + ".txt";
            await _languagesDataService.AddNewLanguageAsync(newLanguage);
            await _alphabetsDataService.SaveAlphabetsAsync(languageName, alphabets);

            Microsoft.HockeyApp.HockeyClient.Current.TrackEvent($"Language Added:  {languageName}");
        }

    }
}
