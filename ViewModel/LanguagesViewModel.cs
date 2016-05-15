using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using InkingAlphabets.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InkingAlphabets.ViewModel
{
    public class LanguagesViewModel : ViewModelBase
    {
        public LanguagesViewModel(
         ILanguagesDataService languagesDataService,
         INavigationService navigationService)
        {
            _languagesDataService = languagesDataService;
            _navigationService = navigationService;            
        }

        private readonly ILanguagesDataService _languagesDataService;
        private readonly INavigationService _navigationService;

        private ObservableCollection<Language> _languages;

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
    }
}
