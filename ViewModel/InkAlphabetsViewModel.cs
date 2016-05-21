﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using InkingAlphabets.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
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
        IPropertySet _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings.Values;

        public InkAlphabetsViewModel(
          IAlphabetsDataService alphabetsDataService,
          ILanguagesDataService languagesDataService,
          INavigationService navigationService)
        {
            _alphabetsDataService = alphabetsDataService;
            _languagesDataService = languagesDataService;
            _navigationService = navigationService;

            if (_localSettings.Keys.Contains("InkingAlphabtsPenColor"))
                SelectedPenColorName = _localSettings["InkingAlphabtsPenColor"].ToString();
            else
            {
                _localSettings["InkingAlphabtsPenColor"] = SelectedPenColorName = "Blue";
            }


            if (_localSettings.Keys.Contains("InkingAlphabtsPenSize"))
                PenSize = int.Parse(_localSettings["InkingAlphabtsPenSize"].ToString());
            else
            {
                _localSettings["InkingAlphabtsPenSize"] = PenSize= 10;
            }

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
        }
    }
}
