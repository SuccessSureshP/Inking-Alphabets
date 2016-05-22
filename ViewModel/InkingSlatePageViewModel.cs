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
    public class InkingSlatePageViewModel : ViewModelBase
    {
        private string _pageTitle = string.Empty;
        private string _selectedPenColorName;
        private Color _selectedPenColor;        
        IPropertySet _localSettings = Windows.Storage.ApplicationData.Current.LocalSettings.Values;
        private int _penSize;

        private string _selectedHighlighterColorName;
        private Color _selectedHighlighterColor;
        private int _highlighterSize;

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

        public int PenSize
        {
            get
            {
                return _penSize;
            }

            set
            {
                _penSize = value;
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

        public InkingSlatePageViewModel()
        {
            PageTitle = "Inking Slate";
            InkStream = new InMemoryRandomAccessStream();

            SelectedPenColorName = Common.GetLocalSettingValue("InkingSlatePenColor").ToString();
            PenSize = int.Parse(Common.GetLocalSettingValue("InkingSlatePenSize").ToString());

            SelectedHighlighterColorName = Common.GetLocalSettingValue("InkingSlateHighlighterColor").ToString();
            HighlighterSize = int.Parse(Common.GetLocalSettingValue("InkingSlateHighlighterSize").ToString());

        }

        public void LoadCachedInkingSlateData()
        {
            
            if (App.Current.Resources.Keys.Contains("CachedInkingSlateData"))
                    InkStream =  (IRandomAccessStream) App.Current.Resources["CachedInkingSlateData"];
        }
        public void CacheInkingSlateData()
        {
            
            App.Current.Resources["CachedInkingSlateData"] =  InkStream;
            _localSettings["InkingSlatePenColor"] = SelectedPenColorName;
            _localSettings["InkingSlatePenSize"] = PenSize;

            _localSettings["InkingSlateHighlighterColor"] = SelectedHighlighterColorName;
            _localSettings["InkingSlateHighlighterSize"] = HighlighterSize;
        }    
    }
}
