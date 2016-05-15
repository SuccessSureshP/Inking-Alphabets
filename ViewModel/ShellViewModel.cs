﻿using GalaSoft.MvvmLight;
using System;
using System.Linq;
using Intense.Presentation;
using System.Windows.Input;
using Windows.UI.Xaml;

namespace InkingAlphabets.ViewModel
{
    public class ShellViewModel : ViewModelBase
    {
        private NavigationItemCollection topItems = new NavigationItemCollection();
        private NavigationItem selectedTopItem;
        private NavigationItemCollection bottomItems = new NavigationItemCollection();
        private NavigationItem selectedBottomItem;
        private bool isSplitViewPaneOpen;

        public ShellViewModel()
        {
            this.ToggleSplitViewPaneCommand = new RelayCommand(() => this.IsSplitViewPaneOpen = !this.IsSplitViewPaneOpen);

            // open splitview pane in wide state
            this.IsSplitViewPaneOpen = IsWideState();

            TopItems.Add(new NavigationItem { Icon = "", DisplayName = App.InkAlphabetsPageTitle, PageType = typeof(InkAlphabets) });
            TopItems.Add(new NavigationItem { Icon = "", DisplayName = App.SelectLanguagePageTitle, PageType = typeof(SelectLanguagePage) });
            TopItems.Add(new NavigationItem { Icon = "", DisplayName = App.AddNewLanguagePageTitle, PageType = typeof(AddNewLanguagePage) });
            TopItems.Add(new NavigationItem { Icon = "", DisplayName = App.InkingSlatePageTitle, PageType = typeof(InkingSlatePage) });
            TopItems.Add(new NavigationItem { Icon = "", DisplayName = App.InkingWordsPageTitle, PageType = typeof(InkingWordsPage) });


            //BottomItems.Add(new NavigationItem { Icon = "", DisplayName = App.SettingsPageTitle, PageType = typeof(SettingsPage) });
            BottomItems.Add(new NavigationItem { Icon = "", DisplayName = App.AboutPageTitle, PageType = typeof(AboutPage) });

            SelectedItem = TopItems.First();
            AppTitle = "INKING ALPHABETS";
        }

        public string AppTitle
        { get; set; }

        public ICommand ToggleSplitViewPaneCommand { get; private set; }

        public bool IsSplitViewPaneOpen
        {
            get { return this.isSplitViewPaneOpen; }
            set { Set(ref this.isSplitViewPaneOpen, value); }
        }

        public NavigationItem SelectedTopItem
        {
            get { return this.selectedTopItem; }
            set
            {
                if (Set(ref this.selectedTopItem, value) && value != null)
                {
                    OnSelectedItemChanged(true);
                }
            }
        }

        public NavigationItem SelectedBottomItem
        {
            get { return this.selectedBottomItem; }
            set
            {
                if (Set(ref this.selectedBottomItem, value) && value != null)
                {
                    OnSelectedItemChanged(false);
                }
            }
        }

        public NavigationItem SelectedItem
        {
            get { return this.selectedTopItem ?? this.selectedBottomItem; }
            set
            {
                this.SelectedTopItem = this.topItems.FirstOrDefault(m => m == value);
                this.SelectedBottomItem = this.bottomItems.FirstOrDefault(m => m == value);
            }
        }

        public Type SelectedPageType
        {
            get
            {
                return this.SelectedItem?.PageType;
            }
            set
            {
                // select associated menu item
                this.SelectedTopItem = this.topItems.FirstOrDefault(m => m.PageType == value);
                this.SelectedBottomItem = this.bottomItems.FirstOrDefault(m => m.PageType == value);
            }
        }

        public NavigationItemCollection TopItems
        {
            get { return this.topItems; }
        }

        public NavigationItemCollection BottomItems
        {
            get { return this.bottomItems; }
        }

        private void OnSelectedItemChanged(bool top)
        {
            if (top)
            {
                this.SelectedBottomItem = null;
            }
            else
            {
                this.SelectedTopItem = null;
            }
            RaisePropertyChanged("SelectedItem");
            RaisePropertyChanged("SelectedPageType");

            // auto-close split view pane (only when not in widestate)
            if (!IsWideState())
            {
                this.IsSplitViewPaneOpen = false;
            }
        }

        // a helper determining whether we are in a wide window state
        // mvvm purists probably don't appreciate this approach
        private bool IsWideState()
        {
            return Window.Current.Bounds.Width >= 1024;
        }

    }
}