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
    public class InkAlphabetsViewModel : ViewModelBase
    {
        private readonly IAlphabetsDataService _alphabetsDataService;
        private readonly ILanguagesDataService _languagesDataService;
        private readonly INavigationService _navigationService;
        private string _originalTitle;
        private string _welcomeTitle = string.Empty;
        private Alphabet _currentAlphabet;

        private ObservableCollection<Alphabet> _alphabets; 

        public InkAlphabetsViewModel(
          IAlphabetsDataService alphabetsDataService,
          ILanguagesDataService languagesDataService,
          INavigationService navigationService)
        {
            _alphabetsDataService = alphabetsDataService;
            _languagesDataService = languagesDataService;
            _navigationService = navigationService;           
        }

        public string WelcomeTitle
        {
            get
            {
                return _welcomeTitle;
            }

            set
            {
                Set(ref _welcomeTitle, value);
            }
        }

        public ObservableCollection<Alphabet> Alphabets
        {
            get
            {
                return _alphabets;
            }
            set
            {
                Set(ref _alphabets, value);
            }
        }

        public Alphabet CurrentAlphabet
        {
            get
            {
                return _currentAlphabet;
            }
            set
            {
                Set(ref _currentAlphabet, value);
            }
        }

        public async Task LoadPageData()
        {
            try
            {
                var selectedLanguage = await _languagesDataService.GetSelectedLanguageAsync();
                WelcomeTitle = selectedLanguage.LanguageName;
                Alphabets = new ObservableCollection<Alphabet>(await _alphabetsDataService.GetAlphabets(selectedLanguage.LanguageName));
                if(Alphabets.Count() >  0)
                    CurrentAlphabet = Alphabets[0];
                else
                {
                    throw new Exception("No Alphabets found!");
                }
            }
            catch
            {
                //LOG exception
                throw;
            }
        }

        public async Task UpdateAlphabetStream(InkCanvas canvas)
        {
            await CurrentAlphabet.UpdateScreenAsync(canvas);
        }
    }
}
