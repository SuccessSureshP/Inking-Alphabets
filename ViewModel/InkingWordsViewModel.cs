using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Views;
using InkingAlphabets.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace InkingAlphabets.ViewModel
{
    public class InkingWordsViewModel : ViewModelBase
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

        public InkingWordsViewModel()
        {
            PageTitle = "Inking Words";
            InkStream = new InMemoryRandomAccessStream();
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
        }
    }
}
