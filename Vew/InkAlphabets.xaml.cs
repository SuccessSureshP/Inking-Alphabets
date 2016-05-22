using InkingAlphabets.Model;
using InkingAlphabets.UserControls;
using InkingAlphabets.ViewModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Input.Inking;
using Windows.UI.Popups;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace InkingAlphabets
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class InkAlphabets : Page
    {
        InkAlphabetsViewModel viewModel;
        private Boolean _ignoreCanvas = false;
        private InkDrawingAttributes _blackDrawingAttributes = new InkDrawingAttributes() { Color = Colors.Blue, Size = new Size(20, 20) };

        public InkAlphabets()
        {
            this.InitializeComponent();
            this.Loaded += InkAlphabets_Loaded;
            this.Unloaded += InkAlphabets_Unloaded;

            InkCanvas1.InkPresenter.InputDeviceTypes = Windows.UI.Core.CoreInputDeviceTypes.Mouse | Windows.UI.Core.CoreInputDeviceTypes.Pen | Windows.UI.Core.CoreInputDeviceTypes.Touch;
            //InkCanvas1.InkPresenter.UpdateDefaultDrawingAttributes(_blackDrawingAttributes);
    }

        private async void  InkAlphabets_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {

                //ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.FullScreen;

                if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
                {
                    await Windows.UI.ViewManagement.StatusBar.GetForCurrentView().HideAsync();
                }

                viewModel = (InkAlphabetsViewModel)this.DataContext;
                await viewModel.LoadPageData();

                InkCanvas1.InkPresenter.StrokesCollected += InkPresenter_StrokesCollected;
                InkCanvas1.InkPresenter.StrokesErased += InkPresenter_StrokesErased;
                //Disabling flip view and canvas for 10 seconds to make all letters to load properly. 
                DataLoadingProgressRingControl.IsActive = true;
                AlphabetslistView.IsEnabled = false;
                InkCanvas1.Visibility = Visibility.Collapsed;
                await Task.Delay(1000);              
                DataLoadingProgressRingControl.IsActive = false;
                AlphabetslistView.IsEnabled = true;
                InkCanvas1.Visibility = Visibility.Visible;
            }
            catch (Exception exp)
            {
                if (exp.Message.Equals("No available languages"))
                {
                    var msgDialog = new MessageDialog("No languages available/selected.");
                    msgDialog.Commands.Add(new UICommand("Ok"));
                    await msgDialog.ShowAsync();
                }
            }

            ResetToInkPen();

            var selectedHighlighter = AlphabetsHighliterPenSelectorControl.HighlighterPens.FirstOrDefault(p => p.Name.Equals(viewModel.SelectedHighlighterColorName));
            viewModel.SelectedHighlighterColor = selectedHighlighter.Pencolor;

            
        }
        private void InkAlphabets_Unloaded(object sender, RoutedEventArgs e)
        {
            InkCanvas1.InkPresenter.StrokesCollected -= InkPresenter_StrokesCollected;
            InkCanvas1.InkPresenter.StrokesErased -= InkPresenter_StrokesErased;
        }

        private void InkPresenter_StrokesErased(Windows.UI.Input.Inking.InkPresenter sender, Windows.UI.Input.Inking.InkStrokesErasedEventArgs args)
        {
            if (_ignoreCanvas)
                return;
        }

        private async void InkPresenter_StrokesCollected(Windows.UI.Input.Inking.InkPresenter sender, Windows.UI.Input.Inking.InkStrokesCollectedEventArgs args)
        {
            if (_ignoreCanvas)
                return;
            await viewModel.UpdateAlphabetStream(InkCanvas1);
        }
     
        private async void AlphabetslistView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateAppBarNavButtons();
            if (viewModel == null)
                return;
            _ignoreCanvas = true;
            InkCanvas1.InkPresenter.StrokeContainer.Clear();
            if(viewModel.CurrentAlphabet != null && viewModel.CurrentAlphabet.InkStream != null &&  viewModel.CurrentAlphabet.InkStream.Size > 0)
            {
                viewModel.CurrentAlphabet.InkStream.Seek(0);
                await InkCanvas1.InkPresenter.StrokeContainer.LoadAsync(viewModel.CurrentAlphabet.InkStream);
            }
            if (AlphabetslistView != null && viewModel != null && viewModel.CurrentAlphabet != null)
            {
                var AlphabetTextboxes = AlphabetslistView.GetChildrenOfType<TextBlock>().Where(x => x.Name == "AlphabeetTextbox").ToList();
                if (AlphabetTextboxes.Count != 0)
                {
                    var AlphabetTextbox = AlphabetTextboxes.Find(a => a.Text.Equals(viewModel.CurrentAlphabet.AlphabetCharacter)); //Find textbox which has same character that is appearning no the screen
                    if (AlphabetTextbox != null)
                    {
                        FontSlider.ValueChanged -= FontSlider_ValueChanged;
                        FontSlider.Value = AlphabetTextbox.FontSize; //Setting the new letter size to Slider. Syncing both.
                        FontSlider.ValueChanged += FontSlider_ValueChanged;
                    }
                }
            }
            _ignoreCanvas = false;
        }

        private async void AppbarButtonClear_Click(object sender, RoutedEventArgs e)
        {
            InkCanvas1.InkPresenter.StrokeContainer.Clear();

            if (InkCanvas1.InkPresenter.StrokeContainer.GetStrokes().Any())
                await viewModel.CurrentAlphabet.UpdateScreenAsync(InkCanvas1);
            else
                viewModel.CurrentAlphabet.ClearScreen();
        }

        private void AppbarButtonPrevious_Click(object sender, RoutedEventArgs e)
        {
            if (AlphabetslistView.SelectedIndex >= 1)
            {
                AlphabetslistView.SelectedIndex = AlphabetslistView.SelectedIndex - 1;                
                UpdateAppBarNavButtons();
            }
        }

        private void AppbarButtonNext_Click(object sender, RoutedEventArgs e)
        {
            if (AlphabetslistView.SelectedIndex <= AlphabetslistView.Items.Count - 2)
            {
                AlphabetslistView.SelectedIndex = AlphabetslistView.SelectedIndex + 1;                
                UpdateAppBarNavButtons();
            }
        }

        private void UpdateAppBarNavButtons()
        {
            if (AlphabetslistView.SelectedIndex == 0)
            {
                AppbarButtonPrevious.IsEnabled = false;
                GotoStartSecondaryAppbarButton.IsEnabled = false;
                GotoStartAppbarButton.IsEnabled = false;
            }
            else
            {
                AppbarButtonPrevious.IsEnabled = true;
                GotoStartSecondaryAppbarButton.IsEnabled = true;
                GotoStartAppbarButton.IsEnabled = true;
            }


            if (AlphabetslistView.SelectedIndex == AlphabetslistView.Items.Count - 1)
            {
                AppbarButtonNext.IsEnabled = false;
                GotoEndSecondaryAppbarButton.IsEnabled = false;
                GotoEndAppbarButton.IsEnabled = false;

            }
            else
            {
                AppbarButtonNext.IsEnabled = true;
                GotoEndSecondaryAppbarButton.IsEnabled = true;
                GotoEndAppbarButton.IsEnabled = true;
            }
        }

        //private void AppbarButtonMakeSmall_Click(object sender, RoutedEventArgs e)
        //{
        //    var currentAlphabetCharacter = ((Alphabet)AlphabetslistView.SelectedItem).AlphabetCharacter;

        //    var AlphabetTextboxes = AlphabetslistView.GetChildrenOfType<TextBlock>().Where(x => x.Name == "AlphabeetTextbox").ToList();

        //    var AlphabetTextbox = AlphabetTextboxes.Find(a => a.Text.Equals(currentAlphabetCharacter)); //Find textbox which has same character that is appearning no the screen

        //    AlphabetTextbox.FontSize = AlphabetTextbox.FontSize - 25;

        //}

        //private void AppbarButtonMakeBig_Click(object sender, RoutedEventArgs e)
        //{
        //    var currentAlphabetCharacter = ((Alphabet) AlphabetslistView.SelectedItem).AlphabetCharacter;

        //    var AlphabetTextboxes = AlphabetslistView.GetChildrenOfType<TextBlock>().Where(x => x.Name == "AlphabeetTextbox").ToList();

        //    var AlphabetTextbox = AlphabetTextboxes.Find(a=>a.Text.Equals(currentAlphabetCharacter)); //Find textbox which has same character that is appearning no the screen

        //    AlphabetTextbox.FontSize = AlphabetTextbox.FontSize + 25;            
        //}

        private void FontSlider_ValueChanged(object sender, RangeBaseValueChangedEventArgs e)
        {
            if (AlphabetslistView == null)
                return;
            var currentAlphabetCharacter = ((Alphabet)AlphabetslistView.SelectedItem).AlphabetCharacter;

            var AlphabetTextboxes = AlphabetslistView.GetChildrenOfType<TextBlock>().Where(x => x.Name == "AlphabeetTextbox").ToList();

            var AlphabetTextbox = AlphabetTextboxes.Find(a => a.Text.Equals(currentAlphabetCharacter)); //Find textbox which has same character that is appearning no the screen

            AlphabetTextbox.FontSize = FontSlider.Value;
        }

        private async void DeleteLanguageButton_Click(object sender, RoutedEventArgs e)
        {   
            var msgDialog = new MessageDialog($@"Are you sure, you want to delete this language ""{viewModel.WelcomeTitle}"" completely?");
            msgDialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(async (IUICommand c) =>
            {
                var statu = await viewModel.DeleteLanguage();

                if (statu == true)
                {
                    var shell = (Shell)Window.Current.Content;
                    shell.ViewModel.SelectedTopItem = shell.ViewModel.TopItems.First(i => i.PageType == typeof(SelectLanguagePage));
                }
                else
                {
                    var msgDialog1 = new MessageDialog($"Something went wrong. Please contact us for support.");
                    msgDialog1.Commands.Add(new UICommand("Ok"));
                    await msgDialog1.ShowAsync();
                    Microsoft.HockeyApp.HockeyClient.Current.TrackEvent($"Deleting Language Failed");
                }

            })));
            msgDialog.Commands.Add(new UICommand("No/Cancel"));
            msgDialog.DefaultCommandIndex = 1;
            await msgDialog.ShowAsync();
        }

        private void GotoStartButton_Click(object sender, RoutedEventArgs e)
        {
            if (AlphabetslistView.SelectedIndex != 0)
            {
                AlphabetslistView.SelectedIndex = 0;
                UpdateAppBarNavButtons();
            }
        }

        private void GotoEndButton_Click(object sender, RoutedEventArgs e)
        {
            if (AlphabetslistView.SelectedIndex <= AlphabetslistView.Items.Count - 2)
            {
                AlphabetslistView.SelectedIndex = AlphabetslistView.Items.Count - 1;
                UpdateAppBarNavButtons();
            }
        }

        private void AlphabetsInkPenSelectorControl_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var inkPenSelectorControl = ((InkPenSelectorControl)sender);

            var selectedPen = inkPenSelectorControl.SelectedPen;
            viewModel.SelectedPenColor = selectedPen.Pencolor;
            _blackDrawingAttributes = new InkDrawingAttributes() { Color = selectedPen.Pencolor, Size = new Size(viewModel.PenSize, viewModel.PenSize) };
            InkCanvas1.InkPresenter.UpdateDefaultDrawingAttributes(_blackDrawingAttributes);
        }

        private void PenColorAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            ResetToInkPen();
        }

        private void ResetToInkPen()
        {
            var selectedPen = AlphabetsInkPenSelectorControl.Pens.FirstOrDefault(p => p.Name.Equals(viewModel.SelectedPenColorName));
            viewModel.SelectedPenColor = selectedPen.Pencolor;

            _blackDrawingAttributes = new InkDrawingAttributes() { Color = selectedPen.Pencolor, Size = new Size(viewModel.PenSize, viewModel.PenSize) };
            InkCanvas1.InkPresenter.UpdateDefaultDrawingAttributes(_blackDrawingAttributes);
        }

        private void AlphabetsInkPenSelectorControl_CloseClicked(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            PenColorAppBarButton.Flyout.Hide();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            viewModel.CacheInkingAplhabetsPageData();
        }

        private void AlphabetsHighliterPenSelectorControl_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var highlighterPenSelectorControl = ((HighlighterPenSelectorControl)sender);

            var selectedHighlighterPen = highlighterPenSelectorControl.SelectedHighlighterPen;
            viewModel.SelectedHighlighterColor = selectedHighlighterPen.Pencolor;

            _blackDrawingAttributes = new InkDrawingAttributes() {  DrawAsHighlighter=true, Color = selectedHighlighterPen.Pencolor, Size = new Size(viewModel.HighlighterSize, viewModel.HighlighterSize) };
            InkCanvas1.InkPresenter.UpdateDefaultDrawingAttributes(_blackDrawingAttributes);
        }

        private void AlphabetsHighliterPenSelectorControl_CloseClicked(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            HighliterAppBarButton.Flyout.Hide();
        }

        private void HighliterAppBarButton_Click(object sender, RoutedEventArgs e)
        {
            var selectedHighlighter = AlphabetsHighliterPenSelectorControl.HighlighterPens.FirstOrDefault(p => p.Name.Equals(viewModel.SelectedHighlighterColorName));
            viewModel.SelectedHighlighterColor = selectedHighlighter.Pencolor;

            _blackDrawingAttributes = new InkDrawingAttributes() { DrawAsHighlighter = true, Color = selectedHighlighter.Pencolor, Size = new Size(viewModel.HighlighterSize, viewModel.HighlighterSize) };
            InkCanvas1.InkPresenter.UpdateDefaultDrawingAttributes(_blackDrawingAttributes);
        }
    }

}
