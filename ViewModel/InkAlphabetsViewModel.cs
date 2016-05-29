using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using InkingAlphabets.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI;
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
        private Language _selectedLanguage;
        private int _penSize;        

        private ObservableCollection<Alphabet> _alphabets;

        private string _selectedPenColorName;
        private Color _selectedPenColor;
        private string _selectedHighlighterColorName;
        private Color _selectedHighlighterColor;
        private int _highlighterSize;
        IPropertySet _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings.Values;

        public InkAlphabetsViewModel(
          IAlphabetsDataService alphabetsDataService,
          ILanguagesDataService languagesDataService,
          INavigationService navigationService)
        {
            _alphabetsDataService = alphabetsDataService;
            _languagesDataService = languagesDataService;
            _navigationService = navigationService;

            SelectedPenColorName = Common.GetLocalSettingValue("InkingAlphabtsPenColor").ToString();
            PenSize = int.Parse(Common.GetLocalSettingValue("InkingAlphabtsPenSize").ToString());

            SelectedHighlighterColorName = Common.GetLocalSettingValue("InkingAlphabtsHighlighterColor").ToString();
            HighlighterSize = int.Parse(Common.GetLocalSettingValue("InkingAlphabtsHighlighterSize").ToString());


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
        public Color SelectedPenColor
        {
            get
            {
                return _selectedPenColor;
            }

            set
            {
                Set(ref _selectedPenColor, value);
            }
        }

        public string SelectedPenColorName
        {
            get
            {
                return _selectedPenColorName;
            }

            set
            {
                Set(ref _selectedPenColorName, value);
            }
        }

        public int PenSize
        {
            get
            {
                return _penSize;
            }

            set
            {
                Set(ref _penSize, value);
            }
        }


        public Color SelectedHighlighterColor
        {
            get
            {
                return _selectedHighlighterColor;
            }

            set
            {
                Set(ref _selectedHighlighterColor, value);
            }
        }

        public string SelectedHighlighterColorName
        {
            get
            {
                return _selectedHighlighterColorName;
            }

            set
            {
                Set(ref _selectedHighlighterColorName, value);
            }
        }

        public int HighlighterSize
        {
            get
            {
                return _highlighterSize;
            }

            set
            {
                Set(ref _highlighterSize, value);
            }
        }

        public async Task LoadPageData()
        {
            try
            {
                _selectedLanguage = await _languagesDataService.GetSelectedLanguageAsync();
                if (_selectedLanguage == null)
                    throw new Exception("No available languages");
                WelcomeTitle = _selectedLanguage.LanguageName;
                Alphabets = new ObservableCollection<Alphabet>(await _alphabetsDataService.GetAlphabets(_selectedLanguage.LanguageName));
                if(Alphabets.Count() >  0)
                    CurrentAlphabet = Alphabets[0];
                else
                {                   
                    throw new Exception("No Alphabets found!");
                }
            }
            catch
            {
                WelcomeTitle = string.Empty;
                Alphabets = null;
                //LOG exception
                throw;
            }
            IPropertySet _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings.Values;
            if(!_localSettings.Keys.Contains("SlateBackgroundImageStatus"))
                await CopySlateBackgroundImage();
        }

        private async Task CopySlateBackgroundImage()
        {
            try
            {
                var sourceFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;
                var destnationFolder = ApplicationData.Current.LocalFolder;
                await destnationFolder.CreateFolderAsync(App.AssetsFolderName, CreationCollisionOption.ReplaceExisting);
                File.Copy(Path.Combine(sourceFolder.Path, App.AssetsFolderName, "InkingSlateBackground.png"), Path.Combine(destnationFolder.Path, App.AssetsFolderName, "InkingSlateBackground.png"), true);
                _localSettings["SlateBackgroundImageStatus"] = "DefaultImageCopied";
            }
            catch (Exception exp)
            {
                Microsoft.HockeyApp.HockeyClient.Current.TrackEvent($"CopySlateBackgroundImage Failed with Exception:{exp.Message}");
                throw exp;
            }
        }

        public async Task UpdateAlphabetStream(InkCanvas canvas)
        {
            await CurrentAlphabet.UpdateScreenAsync(canvas);
        }

        public async Task<bool> DeleteLanguage()
        {
            bool deleteResult = false;
            try
            {
                if (_selectedLanguage != null)
                {
                    var alphabetsFileDeleted = await _alphabetsDataService.DeleteAlphabetsAsync(_selectedLanguage.LanguageName);
                    if (alphabetsFileDeleted)
                        deleteResult = await _languagesDataService.DeleteLanguageAsync(_selectedLanguage);
                }
            }
            catch (Exception exp)
            {
                Microsoft.HockeyApp.HockeyClient.Current.TrackEvent($"Deleting Language Failed with Exception:{exp.Message}");
            }

            return deleteResult;
        }

        public void CacheInkingAplhabetsPageData()
        {   
            _localSettings["InkingAlphabtsPenColor"] = SelectedPenColorName;
            _localSettings["InkingAlphabtsPenSize"] = PenSize;

            _localSettings["InkingAlphabtsHighlighterColor"] = SelectedHighlighterColorName;
            _localSettings["InkingAlphabtsHighlighterSize"] = HighlighterSize;
        }
    }
}
