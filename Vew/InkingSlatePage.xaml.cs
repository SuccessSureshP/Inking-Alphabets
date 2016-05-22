using InkingAlphabets.UserControls;
using InkingAlphabets.ViewModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
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
    public sealed partial class InkingSlatePage : Page
    {
        private InkingSlatePageViewModel viewModel;
        private InkDrawingAttributes _blackDrawingAttributes;
        private Boolean _isInitialized;
        public InkingSlatePage()
        {
            this.InitializeComponent();
            SlateCanvas.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse | Windows.UI.Core.CoreInputDeviceTypes.Pen | Windows.UI.Core.CoreInputDeviceTypes.Touch;
            
            this.Loaded += InkingSlatePage_Loaded;
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
        }

        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            //DataRequest request = args.Request;

            //StorageFile file = await ApplicationData.Current.LocalFolder.CreateFileAsync("Shared.gif", CreationCollisionOption.ReplaceExisting);
            //if (null != file)
            //{
            //    try
            //    {
            //        using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
            //        {
            //            await SlateCanvas.InkPresenter.StrokeContainer.SaveAsync(stream);
            //        }
            //    }
            //    catch
            //    {

            //    }
            //    //var randomAccessStreamReference = RandomAccessStreamReference.CreateFromFile(file);
            //    //List<IStorageItem> items = new List<IStorageItem>();
            //    //items.Add(file);
            //    request.Data.SetText("Sample");
            //}
            ////request.Data.SetText("Sample");
            //request.Data.Properties.Title = "I used InkAlphabets App to draw this.";
        }

        private async void InkingSlatePage_Loaded(object sender, RoutedEventArgs e)
        {
            if (!_isInitialized)
                InitializeGrid();
            _isInitialized = true;
            SlateCanvas.InkPresenter.StrokesCollected += InkPresenter_StrokesCollected; 
            SlateCanvas.InkPresenter.StrokesErased += InkPresenter_StrokesErased;

            viewModel = (InkingSlatePageViewModel)this.DataContext;

            viewModel.LoadCachedInkingSlateData();
            if (viewModel.InkStream != null && viewModel.InkStream.Size > 0)
            {
                viewModel.InkStream.Seek(0);
                await SlateCanvas.InkPresenter.StrokeContainer.LoadAsync(viewModel.InkStream);
            }

            ResetToInkPen();

            var selectedHighlighter = SlateHighliterPenSelectorControl.HighlighterPens.FirstOrDefault(p => p.Name.Equals(viewModel.SelectedHighlighterColorName));
            viewModel.SelectedHighlighterColor = selectedHighlighter.Pencolor;
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

        private async void AppbarButtonOpen_Click(object sender, RoutedEventArgs e)
        {
            var loadPicker = new Windows.Storage.Pickers.FileOpenPicker();
            loadPicker.FileTypeFilter.Add(".gif");
            StorageFile file = await loadPicker.PickSingleFileAsync();
            if (null != file)
            {
                using (var stream = await file.OpenSequentialReadAsync())
                {
                    try
                    {
                        await SlateCanvas.InkPresenter.StrokeContainer.LoadAsync(stream);

                    }
                    catch (Exception ex)
                    {
                        var msgDialog = new MessageDialog("Sorry. Something went wrong. Could not open your Inking file :( ");
                        msgDialog.Commands.Add(new UICommand("Ok"));                        
                        await msgDialog.ShowAsync();
                        Microsoft.HockeyApp.HockeyClient.Current.TrackEvent($"Opening Inking file failed with exception :{ex.Message}");
                    }
                }
            }
        }

        private async void AppbarButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (SlateCanvas.InkPresenter.StrokeContainer.GetStrokes().Count > 0)
            {
                var savePicker = new Windows.Storage.Pickers.FileSavePicker();
                savePicker.FileTypeChoices.Add("GIF with embedded ISF", new System.Collections.Generic.List<string> { ".gif" });
                StorageFile file = await savePicker.PickSaveFileAsync();
                if (null != file)
                {
                    try
                    {
                        using (IRandomAccessStream stream = await file.OpenAsync(FileAccessMode.ReadWrite))
                        {
                            await SlateCanvas.InkPresenter.StrokeContainer.SaveAsync(stream);
                            var msgDialog = new MessageDialog("All your Inking Saved! You want to start fresh again?");
                            msgDialog.Commands.Add(new UICommand("Yes",new UICommandInvokedHandler( (IUICommand c) =>  
                            {
                                SlateCanvas.InkPresenter.StrokeContainer.Clear();
                            })));
                            msgDialog.Commands.Add(new UICommand("No/Cancel"));
                            await msgDialog.ShowAsync();                            
                        }
                    }
                    catch(Exception exp)
                    {
                        var msgDialog = new MessageDialog("Sorry. Something went wrong. Could not save your Inking :( ");
                        msgDialog.Commands.Add(new UICommand("Ok"));
                        await msgDialog.ShowAsync();
                        Microsoft.HockeyApp.HockeyClient.Current.TrackEvent($"Inking Slate Save Failed with Exception:{exp.Message}");
                    }
                }
            }
            else
            {
                var msgDialog = new MessageDialog("You haven't started inking yet. Start Inking and then save!");                
                msgDialog.Commands.Add(new UICommand("Ok"));
                await msgDialog.ShowAsync();
            }
        }
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //SystemNavigationManager.GetForCurrentView().BackRequested += Page_BackRequested;
        }
        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            viewModel.CacheInkingSlateData();
            //SystemNavigationManager.GetForCurrentView().BackRequested -= Page_BackRequested;
        }

        protected override async void OnNavigatingFrom(NavigatingCancelEventArgs e)
        {
            //TODO: Implement this method if needed.
            //e.Cancel = true;         

            //if (SlateCanvas.InkPresenter.StrokeContainer.GetStrokes().Count > 0)
            //{
            //    var msgDialog = new MessageDialog("All your Un-saved Inkings will be lost. Do you want to continue navigating from this page?");
            //    msgDialog.Commands.Add(new UICommand("No", new UICommandInvokedHandler((IUICommand c) =>
            //    {
            //        e.Cancel = true;
            //        return;
            //    })));
            //    msgDialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler((IUICommand c) =>
            //    {
            //        base.OnNavigatingFrom(e);                    
            //    })));                
            //    await msgDialog.ShowAsync();
            //}            
        }


        private async void Page_BackRequested(object sender, BackRequestedEventArgs e)
        {
            //TODO: Implement this method if needed.
            //e.Handled = true;

            //var messageDialog = new MessageDialog("Do you want to go back?");

            //messageDialog.Commands.Add(new UICommand("Yes", null, true));
            //messageDialog.Commands.Add(new UICommand("No", null, false));

            //messageDialog.DefaultCommandIndex = 1;

            //var commandChosen = await messageDialog.ShowAsync();

            //if ((bool)commandChosen.Id)
            //{
            //    if (this.Frame.CanGoBack)
            //    {
            //        this.Frame.GoBack();
            //    }
            //}
        }

        private void ShareAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            DataTransferManager.ShowShareUI();
        }

        private void SlateInkPenSelectorControl_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var pn = e.PropertyName;
            var selectedPen = ((InkPenSelectorControl)sender).SelectedPen;
            viewModel.SelectedPenColor = selectedPen.Pencolor;
            _blackDrawingAttributes = new InkDrawingAttributes() { Color = selectedPen.Pencolor, Size = new Size(viewModel.PenSize, viewModel.PenSize) };
            SlateCanvas.InkPresenter.UpdateDefaultDrawingAttributes(_blackDrawingAttributes);
        }

        private void SlateInkPenSelectorControl_CloseClicked(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            PenColorAppBarButton.Flyout.Hide();
        }

        private void SlateHighliterPenSelectorControl_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var highlighterPenSelectorControl = ((HighlighterPenSelectorControl)sender);

            var selectedHighlighterPen = highlighterPenSelectorControl.SelectedHighlighterPen;
            viewModel.SelectedHighlighterColor = selectedHighlighterPen.Pencolor;

            _blackDrawingAttributes = new InkDrawingAttributes() { DrawAsHighlighter = true, Color = selectedHighlighterPen.Pencolor, Size = new Size(viewModel.HighlighterSize, viewModel.HighlighterSize) };
            SlateCanvas.InkPresenter.UpdateDefaultDrawingAttributes(_blackDrawingAttributes);
        }

        private void SlateHighliterPenSelectorControl_CloseClicked(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            HighliterAppBarButton.Flyout.Hide();
        }

        private void HighliterAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (AppbarButtonEraser.IsChecked != null && (bool)AppbarButtonEraser.IsChecked)
                AppbarButtonEraser.IsChecked = false;

            var selectedHighlighter =  SlateHighliterPenSelectorControl.HighlighterPens.FirstOrDefault(p => p.Name.Equals(viewModel.SelectedHighlighterColorName));
            viewModel.SelectedHighlighterColor = selectedHighlighter.Pencolor;

            _blackDrawingAttributes = new InkDrawingAttributes() { DrawAsHighlighter = true, Color = selectedHighlighter.Pencolor, Size = new Size(viewModel.HighlighterSize, viewModel.HighlighterSize) };
            SlateCanvas.InkPresenter.UpdateDefaultDrawingAttributes(_blackDrawingAttributes);
        }

        private void ResetToInkPen()
        {
            var selectedPen = SlateInkPenSelectorControl.Pens.FirstOrDefault(p => p.Name.Equals(viewModel.SelectedPenColorName));
            viewModel.SelectedPenColor = selectedPen.Pencolor;
            _blackDrawingAttributes = new InkDrawingAttributes() { Color = selectedPen.Pencolor, Size = new Size(viewModel.PenSize, viewModel.PenSize) };
            SlateCanvas.InkPresenter.UpdateDefaultDrawingAttributes(_blackDrawingAttributes);
        }

        private void PenColorAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            if (AppbarButtonEraser.IsChecked != null && (bool)AppbarButtonEraser.IsChecked)
                AppbarButtonEraser.IsChecked = false;
            ResetToInkPen();
        }
    }
}
