using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using InkingAlphabets.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Collections;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Xaml.Controls;

namespace InkingAlphabets.ViewModel
{
    public class InkingWordsViewModel : ViewModelBase
    {
        private string _pageTitle = string.Empty;
        private string _selectedPenColorName;
        private Color _selectedPenColor;
        IPropertySet _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings.Values;
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

        public InkingWordsViewModel()
        {
            PageTitle = "Inking Words";
            InkStream = new InMemoryRandomAccessStream();
            if (_localSettings.Keys.Contains("InkingWordsPenColor"))
                SelectedPenColorName = _localSettings["InkingWordsPenColor"].ToString();
            else
            {
                _localSettings["InkingWordsPenColor"] = SelectedPenColorName = "Green";
            }
        }

        private IRandomAccessStream _inkStream;

        public IRandomAccessStream InkStream
        {
            get
            {
                return _inkStream;
            }
            set
            {
                Set(ref _inkStream, value);
            }
        }
        public async Task UpdateSlateAsync(InkCanvas canvas)
        {
            _inkStream.Size = 0;
            IOutputStream outputStream = _inkStream.GetOutputStreamAt(0);
            await canvas.InkPresenter.StrokeContainer.SaveAsync(outputStream);
            await outputStream.FlushAsync();
        }

        public void ClearSlate()
        {
            _inkStream.Size = 0;         
        }


        public void LoadCachedInkingWordsData()
        {
            if (App.Current.Resources.Keys.Contains("CachedInkingWordsData"))
                InkStream = (IRandomAccessStream)App.Current.Resources["CachedInkingWordsData"];
        }
        public void CacheInkingSlateData()
        {
            App.Current.Resources["CachedInkingWordsData"] = InkStream;
            _localSettings["InkingWordsPenColor"] = SelectedPenColorName;
        }
    }
}
