using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

// The User Control item template is documented at http://go.microsoft.com/fwlink/?LinkId=234236

namespace InkingAlphabets.UserControls
{
    public sealed partial class InkPenSelectorControl : UserControl
    {
        private List<ColorClass> _pens;
        private ColorClass _selectedPen;
        private int _selectedPenSize;

        public ColorClass SelectedPen
        {
            get
            {
                return _selectedPen;
            }
            set
            {               
                _selectedPen = value;
                if(_selectedPen != null)
                    PenColor = _selectedPen.Name;             
            }
        }


        

        public int SelectedPenSize
        {
            get
            {
                return _selectedPenSize;
            }
            set
            {
                _selectedPenSize = value;
                 PenSize = _selectedPenSize;
            }
        }

        public List<ColorClass> Pens
        {
            get
            {
                return _pens;
            }

            set
            {
                _pens = value;
            }
        }

        public static readonly DependencyProperty PenColorProperty = DependencyProperty.Register("PenColor", typeof(string), typeof(InkPenSelectorControl), null);
        public string PenColor
        {
            get
            {
                return (string)GetValue(PenColorProperty);
            }
            set
            {                
                SetValueDP(PenColorProperty, value);
            }
        }


        public static readonly DependencyProperty PenSizeProperty = DependencyProperty.Register("PenSize", typeof(int), typeof(InkPenSelectorControl), null);
        public int PenSize
        {
            get
            {
                return (int)GetValue(PenSizeProperty);
            }
            set
            {
                SetValueDP(PenSizeProperty, value);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        void SetValueDP(DependencyProperty property, object valu,[System.Runtime.CompilerServices.CallerMemberName] string p = null)
        {
            SetValue(property, valu);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(p));
        }

        public event PropertyChangedEventHandler CloseClicked;

        public InkPenSelectorControl()
        {
            this.InitializeComponent();
            this.Loaded += InkPenSelectorControl_Loaded;

            Pens = new List<ColorClass>();
            Pens.Add(new ColorClass() { Pencolor = Colors.Green, Name = "Green" });
            Pens.Add(new ColorClass() { Pencolor = Colors.Red, Name = "Red" });
            Pens.Add(new ColorClass() { Pencolor = Colors.Black, Name = "Black" });
            Pens.Add(new ColorClass() { Pencolor = Colors.Blue, Name = "Blue" });
            Pens.Add(new ColorClass() { Pencolor = Colors.Brown, Name = "Brown" });
            Pens.Add(new ColorClass() { Pencolor = Colors.Chocolate, Name = "Chocolate" });
            Pens.Add(new ColorClass() { Pencolor = Colors.DarkBlue, Name = "DarkBlue" });
            Pens.Add(new ColorClass() { Pencolor = Colors.DarkCyan, Name = "DarkCyan" });
            Pens.Add(new ColorClass() { Pencolor = Colors.DarkGoldenrod, Name = "DarkGoldenrod" });

            (this.Content as FrameworkElement).DataContext = this;

            
        }

        private void InkPenSelectorControl_Loaded(object sender, RoutedEventArgs e)
        {

            if (ColorsListView.SelectedItem == null)
            {   
                SelectedPen = Pens.FirstOrDefault(p => p.Name.Equals(PenColor));
                ColorsListView.SelectedItem = SelectedPen;
            }

            PenSizeSlider.Value = PenSize;

        }

        private void PenSelectionCloseButton_Click(object sender, RoutedEventArgs e)
        {
            CloseClicked?.Invoke(this, new PropertyChangedEventArgs(null));
        }
    }
}
