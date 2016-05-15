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
    public class SettingsViewModel : ViewModelBase
    {
        private string _pageTitle = string.Empty;

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

        public SettingsViewModel()
        {
            PageTitle = "Settings";
        }
    }
}
