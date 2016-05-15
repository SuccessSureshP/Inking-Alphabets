using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using GalaSoft.MvvmLight.Messaging;
using GalaSoft.MvvmLight.Threading;
using Windows.UI.Core;
using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;

namespace InkingAlphabets
{
    sealed partial class App
    {
        public static string LocalDbFolderName = "DB";
        public static string LocalDbName = "InkingAlphabets.Sqlite";
        public static string AlphabetsByLanguageFolderName = "AlphabetsByLanguage";

        public static string InkAlphabetsPageTitle = "Learning Alphabets";
        public static string SelectLanguagePageTitle = "Select a Language";
        public static string AddNewLanguagePageTitle = "Add New Language";
        public static string InkingSlatePageTitle = "Inking Slate";
        public static string InkingWordsPageTitle = "Inking Words";
        public static string AboutPageTitle = "About";
        public static string SettingsPageTitle = "Settings";




        public App()
        {
            InitializeComponent();
            Suspending += OnSuspending;
            Microsoft.HockeyApp.HockeyClient.Current.Configure("1d8fffb59eb3443294748ec215a2c18f");            
        }

        /// <summary>
        /// Invoked when the application is launched normally by the end user.  Other entry points
        /// will be used such as when the application is launched to open a specific file.
        /// </summary>
        /// <param name="e">Details about the launch request and process.</param>
        protected override void OnLaunched(LaunchActivatedEventArgs e)
        {

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                DebugSettings.EnableFrameRateCounter = true;
            }
#endif

            var shell = Window.Current.Content as Shell;

            // Do not repeat app initialization when the Window already has content,
            // just ensure that the window is active
            if (shell == null)
            {
                // Create a Frame to act as the navigation context and navigate to the first page
                shell = new Shell();

                // hook-up shell root frame navigation events
                shell.RootFrame.NavigationFailed += OnNavigationFailed;
                shell.RootFrame.Navigated += OnNavigated;

                if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
                {
                    //TODO: Load state from previously suspended application
                }

                // set the Shell as content
                Window.Current.Content = shell;

                // listen for back button clicks (both soft- and hardware)
                SystemNavigationManager.GetForCurrentView().BackRequested += OnBackRequested;

                if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                {
                    HardwareButtons.BackPressed += OnBackPressed;
                }

                UpdateBackButtonVisibility();
            }
           
            // Ensure the current window is active
            Window.Current.Activate();       
        }

        // handle hardware back button press
        void OnBackPressed(object sender, BackPressedEventArgs e)
        {
            var shell = (Shell)Window.Current.Content;
            if (shell.RootFrame.CanGoBack)
            {
                e.Handled = true;
                shell.RootFrame.GoBack();
            }
        }

        // handle software back button press
        void OnBackRequested(object sender, BackRequestedEventArgs e)
        {
            var shell = (Shell)Window.Current.Content;
            if (shell.RootFrame.CanGoBack)
            {
                e.Handled = true;
                shell.RootFrame.GoBack();
            }
        }

        void OnNavigated(object sender, NavigationEventArgs e)
        {
            UpdateBackButtonVisibility();
        }
        private void UpdateBackButtonVisibility()
        {
            var shell = (Shell)Window.Current.Content;

            var visibility = AppViewBackButtonVisibility.Collapsed;
            if (shell.RootFrame.CanGoBack)
            {
                visibility = AppViewBackButtonVisibility.Visible;
            }

            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = visibility;
        }

        private void HandleNotificationMessage(NotificationMessageAction<string> message)
        {
            message.Execute("Success (from App.xaml.cs)!");
        }

        /// <summary>
        /// Invoked when Navigation to a certain page fails
        /// </summary>
        /// <param name="sender">The Frame which failed navigation</param>
        /// <param name="e">Details about the navigation failure</param>
        void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
        }

        /// <summary>
        /// Invoked when application execution is being suspended.  Application state is saved
        /// without knowing whether the application will be terminated or resumed with the contents
        /// of memory still intact.
        /// </summary>
        /// <param name="sender">The source of the suspend request.</param>
        /// <param name="e">Details about the suspend request.</param>
        private void OnSuspending(object sender, SuspendingEventArgs e)
        {
            var deferral = e.SuspendingOperation.GetDeferral();
            //TODO: Save application state and stop any background activity
            deferral.Complete();
        }
    }
}
