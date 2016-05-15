using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage.Streams;
using Windows.UI.Xaml.Controls;

namespace InkingAlphabets.Model
{
    public class Alphabet : ViewModelBase
    {
        public string AlphabetCharacter { get; set; }

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

        public bool InkingRight { get; set; }


        public async Task UpdateScreenAsync(InkCanvas canvas)
        {
            _inkStream.Size = 0;
            IOutputStream outputStream = _inkStream.GetOutputStreamAt(0);
            await canvas.InkPresenter.StrokeContainer.SaveAsync(outputStream);
            await outputStream.FlushAsync();
            
        }

        public void ClearScreen()
        {
            _inkStream.Size = 0;            
        }
    }
}
