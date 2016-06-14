using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Views;
using Microsoft.Practices.ServiceLocation;
using InkingAlphabets.Model;

namespace InkingAlphabets.ViewModel
{
    public class ViewModelLocator
    {
        public const string SecondPageKey = "SecondPage";

        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            var nav = new NavigationService();
            nav.Configure(SecondPageKey, typeof(SecondPage));
            SimpleIoc.Default.Register<INavigationService>(() => nav);

            SimpleIoc.Default.Register<IDialogService, DialogService>();

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IAlphabetsDataService, Design.DesignAlphabetsDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IAlphabetsDataService, AlphabetsDataService>();
            }

            SimpleIoc.Default.Register<ILanguagesDataService, LanguagesDataService>();

            SimpleIoc.Default.Register<ShellViewModel>();            
            SimpleIoc.Default.Register<InkAlphabetsViewModel>();
            SimpleIoc.Default.Register<SelectLanguagePageViewModel>();
            SimpleIoc.Default.Register<AddNewLanguagePageViewModel>();
            SimpleIoc.Default.Register<InkingSlatePageViewModel>();
            SimpleIoc.Default.Register<SettingsViewModel>();
            SimpleIoc.Default.Register<InkingWordsViewModel>();            
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance",
            "CA1822:MarkMembersAsStatic",
            Justification = "This non-static member is needed for data binding purposes.")]        

        public InkAlphabetsViewModel InkAlphabetsViewModelInstance => ServiceLocator.Current.GetInstance<InkAlphabetsViewModel>();

        public SelectLanguagePageViewModel SelectLanguagePageViewModelInstance => ServiceLocator.Current.GetInstance<SelectLanguagePageViewModel>();
        public AddNewLanguagePageViewModel AddNewLanguagePageViewModelInstance => ServiceLocator.Current.GetInstance<AddNewLanguagePageViewModel>();

        public InkingSlatePageViewModel InkingSlatePageViewModelInstance => ServiceLocator.Current.GetInstance<InkingSlatePageViewModel>();

        public SettingsViewModel SettingsViewModelInstance => ServiceLocator.Current.GetInstance<SettingsViewModel>();

        public InkingWordsViewModel InkingWordsViewModelInstance => ServiceLocator.Current.GetInstance<InkingWordsViewModel>();
                
        public ShellViewModel ShellViewModelInstance => ServiceLocator.Current.GetInstance<ShellViewModel>();
    }
}
