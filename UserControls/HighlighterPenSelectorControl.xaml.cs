using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace InkingAlphabets.UserControls
{
    public sealed partial class HighlighterPenSelectorControl : UserControl
    {
        private List<ColorClass> _highlighterPens;
        private ColorClass _selectedHighlighterPen;
        private int _selectedHighlighterPenSize;

        public ColorClass SelectedHighlighterPen
        {
            get
            {
                return _selectedHighlighterPen;
            }
            set
            {
                _selectedHighlighterPen = value;
                if (_selectedHighlighterPen != null)
                    HighlighterPenColor = _selectedHighlighterPen.Name;
            }
        }




        public int SelectedHighlighterPenSize
        {
            get
            {
                return _selectedHighlighterPenSize;
            }
            set
            {
                _selectedHighlighterPenSize = value;
                HighlighterPenSize = _selectedHighlighterPenSize;
            }
        }

        public List<ColorClass> HighlighterPens
        {
            get
            {
                return _highlighterPens;
            }

            set
            {
                _highlighterPens = value;
            }
        }

        public static readonly DependencyProperty HighlighterPenColorProperty = DependencyProperty.Register("HighlighterPenColor", typeof(string), typeof(HighlighterPenSelectorControl), null);
        public string HighlighterPenColor
        {
            get
            {
                return (string)GetValue(HighlighterPenColorProperty);
            }
            set
            {
                SetValueDP(HighlighterPenColorProperty, value);
            }
        }


        public static readonly DependencyProperty HighlighterPenSizeProperty = DependencyProperty.Register("HighlighterPenSize", typeof(int), typeof(HighlighterPenSelectorControl), null);
        public int HighlighterPenSize
        {
            get
            {
                return (int)GetValue(HighlighterPenSizeProperty);
            }
            set
            {
                SetValueDP(HighlighterPenSizeProperty, value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void SetValueDP(DependencyProperty property, object valu, [System.Runtime.CompilerServices.CallerMemberName] string p = null)
        {
            SetValue(property, valu);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        public event PropertyChangedEventHandler CloseClicked;

        public HighlighterPenSelectorControl()
        {
            this.InitializeComponent();
            this.Loaded += HighlighterPenSelectorControl_Loaded;

            HighlighterPens = new List<ColorClass>();
            HighlighterPens.Add(new ColorClass() { Pencolor = Colors.Green, Name = "Green" });
            HighlighterPens.Add(new ColorClass() { Pencolor = Colors.Red, Name = "Red" });
            HighlighterPens.Add(new ColorClass() { Pencolor = Colors.Black, Name = "Black" });
            HighlighterPens.Add(new ColorClass() { Pencolor = Colors.Blue, Name = "Blue" });
            HighlighterPens.Add(new ColorClass() { Pencolor = Colors.Brown, Name = "Brown" });
            HighlighterPens.Add(new ColorClass() { Pencolor = Colors.Chocolate, Name = "Chocolate" });
            HighlighterPens.Add(new ColorClass() { Pencolor = Colors.DarkBlue, Name = "DarkBlue" });
            HighlighterPens.Add(new ColorClass() { Pencolor = Colors.DarkCyan, Name = "DarkCyan" });
            HighlighterPens.Add(new ColorClass() { Pencolor = Colors.DarkGoldenrod, Name = "DarkGoldenrod" });
            HighlighterPens.Add(new ColorClass() { Pencolor = Colors.Yellow, Name = "Yellow" });

            (this.Content as FrameworkElement).DataContext = this;


        }

        private void HighlighterPenSelectorControl_Loaded(object sender, RoutedEventArgs e)
        {

            if (ColorsListView.SelectedItem == null)
            {
                SelectedHighlighterPen = HighlighterPens.FirstOrDefault(p => p.Name.Equals(HighlighterPenColor));
                ColorsListView.SelectedItem = SelectedHighlighterPen;
            }

            HighlighterPenSizeSlider.Value = HighlighterPenSize;

        }

        private void HighlighterPenSelectionCloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseClicked?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
