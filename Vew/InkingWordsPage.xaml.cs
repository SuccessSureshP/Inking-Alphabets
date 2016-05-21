using InkingAlphabets.UserControls;
using InkingAlphabets.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Streams;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Input.Inking;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace InkingAlphabets
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InkingWordsPage : Page
    {
        private InkingWordsViewModel viewModel;
        private Boolean _isInitialized;
        private readonly InkRecognizerContainer _inkRecognizerContainer;
        private InkDrawingAttributes _blackDrawingAttributes;
        public InkingWordsPage()
        {
            this.InitializeComponent();
            SlateCanvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse | Windows.UI.Core.CoreInputDeviceTypes.Pen | Windows.UI.Core.CoreInputDeviceTypes.Touch;
            this.Loaded += InkingWordsPage_Loaded;
            _inkRecognizerContainer = new InkRecognizerContainer();
        }

        private async void InkingWordsPage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized)
                InitializeGrid();
            _isInitialized = true;
            SlateCanvas.InkPresenter.StrokesCollected += InkPresenter_StrokesCollected; ;
            SlateCanvas.InkPresenter.StrokesErased += InkPresenter_StrokesErased;

            viewModel = (InkingWordsViewModel)this.DataContext;

            viewModel.LoadCachedInkingWordsData();
            if (viewModel.InkStream != null && viewModel.InkStream.Size > 0)
            {
                viewModel.InkStream.Seek(0);
                await SlateCanvas.InkPresenter.StrokeContainer.LoadAsync(viewModel.InkStream);
            }
            if (App.Current.Resources.Keys.Contains("InkingWord"))
                InputTextbox.Text = App.Current.Resources["InkingWord"].ToString();

            var selectedPen = SlateInkPenSelectorControl.Pens.FirstOrDefault(p => p.Name.Equals(viewModel.SelectedPenColorName));
            viewModel.SelectedPenColor = selectedPen.Pencolor;
            _blackDrawingAttributes = new InkDrawingAttributes() { Color = selectedPen.Pencolor, Size = new Size(10, 10) };
            SlateCanvas.InkPresenter.UpdateDefaultDrawingAttributes(_blackDrawingAttributes);
        }

        private async void InkPresenter_StrokesErased(InkPresenter sender, InkStrokesErasedEventArgs args)
        {
            await viewModel.UpdateSlateAsync(SlateCanvas);
        }

        private async void InkPresenter_StrokesCollected(InkPresenter sender, InkStrokesCollectedEventArgs args)
        {
            await viewModel.UpdateSlateAsync(SlateCanvas);
        }

        private void InitializeGrid()
        {
            for (int yPos = 20; yPos < SlateCanvas.ActualHeight; yPos += 20)
            {
                GridLinesContainer.Children.Add(new Line
                {
                    X1 = 0,
                    X2 = 1,
                    Y1 = 0,
                    Y2 = 0,
                    Stretch = Stretch.Fill,
                    Stroke = new SolidColorBrush(yPos % 140 == 0 ? Colors.Gray : Colors.LightGray),
                    StrokeThickness = 1,
                    Margin = new Thickness(0, yPos, 0, 0),
                    VerticalAlignment = VerticalAlignment.Top
                });
            }
            for (int xPos = 20; xPos < SlateCanvas.ActualWidth; xPos += 20)
            {
                GridLinesContainer.Children.Add(new Line
                {
                    X1 = 0,
                    X2 = 0,
                    Y1 = 0,
                    Y2 = 1,
                    Stretch = Stretch.Fill,
                    Stroke = new SolidColorBrush(xPos % 140 == 0 ? Colors.Gray : Colors.LightGray),
                    StrokeThickness = 1,
                    Margin = new Thickness(xPos, 0, 0, 0),
                    HorizontalAlignment = HorizontalAlignment.Left
                });
            }
        }
        private async void AppbarButtonClear_Click(object sender, RoutedEventArgs e)
        {
            SlateCanvas.InkPresenter.StrokeContainer.Clear();

            if (SlateCanvas.InkPresenter.StrokeContainer.GetStrokes().Any())
                await viewModel.UpdateSlateAsync(SlateCanvas);
            else
                viewModel.ClearSlate();
        }

        private void AppbarButtonPan_Checked(object sender, RoutedEventArgs e)
        {
            SlateCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen;
        }

        private void AppbarButtonPan_Unchecked(object sender, RoutedEventArgs e)
        {
            SlateCanvas.InkPresenter.InputDeviceTypes = CoreInputDeviceTypes.Mouse | CoreInputDeviceTypes.Pen | CoreInputDeviceTypes.Touch;
        }

        private void AppbarButtonEraser_Checked(object sender, RoutedEventArgs e)
        {
            SlateCanvas.InkPresenter.InputProcessingConfiguration.Mode = InkInputProcessingMode.Erasing;
        }

        private void AppbarButtonEraser_Unchecked(object sender, RoutedEventArgs e)
        {
            SlateCanvas.InkPresenter.InputProcessingConfiguration.Mode = InkInputProcessingMode.Inking;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //SystemNavigationManager.GetForCurrentView().BackRequested += Page_BackRequested;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            viewModel.CacheInkingSlateData();
            App.Current.Resources["InkingWord"] = InputTextbox.Text;
            //SystemNavigationManager.GetForCurrentView().BackRequested -= Page_BackRequested;
        }

        private void FontSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (SampleTextblock == null)
                return;           
            SampleTextblock.FontSize = FontSlider.Value;
        }

        private async void RecognizeButton_Click(object sender, RoutedEventArgs e)
        {
            var str = string.Empty;
            MessageDialog msgDialog = null;
            var currentStrokes =
     SlateCanvas.InkPresenter.StrokeContainer.GetStrokes();
            if (currentStrokes.Count > 0)
            {
                var recognitionResults = await _inkRecognizerContainer.RecognizeAsync(
                     SlateCanvas.InkPresenter.StrokeContainer,
                     InkRecognitionTarget.All);

                if (recognitionResults.Count > 0)
                {
                    // Display recognition result                   
                    foreach (var r in recognitionResults)
                    {
                        str += $"{r.GetTextCandidates()[0]} ";
                    }
                    msgDialog = new MessageDialog($"Recognized the text \"{str}\".");
                }
                else
                {
                    str = "No text recognized.";
                    msgDialog = new MessageDialog(str);
                }
            }
            else
            {
                str = "Write something to recognize.";
                msgDialog = new MessageDialog(str);
            }

            await msgDialog.ShowAsync();
        }

        private void SlateInkPenSelectorControl_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var pn = e.PropertyName;
            var selectedPen = ((InkPenSelectorControl)sender).SelectedPen;
            viewModel.SelectedPenColor = selectedPen.Pencolor;
            _blackDrawingAttributes = new InkDrawingAttributes() { Color = selectedPen.Pencolor, Size = new Size(10, 10) };
            SlateCanvas.InkPresenter.UpdateDefaultDrawingAttributes(_blackDrawingAttributes);
        }

        private void SlateInkPenSelectorControl_CloseClicked(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            PenColorAppBarButton.Flyout.Hide();
        }
    }
}
