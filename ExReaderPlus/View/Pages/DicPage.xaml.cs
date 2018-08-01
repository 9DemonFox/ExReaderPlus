﻿using ExReaderPlus.Manage;
using ExReaderPlus.View.Commands;
using ExReaderPlus.ViewModels;
using ExReaderPlus.WordsManager;
using System;
using System.Diagnostics;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ExReaderPlus.View.Pages {
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class DicPage : Page {

        DicPageViewModel _viewModel;

        private string _lastCommand;

        #region Methods

        protected override void OnNavigatingFrom(NavigatingCancelEventArgs e) {
            base.OnNavigatingFrom(e);
        }

        private async void _viewModel_CommandActions(object sender, CommandArgs args) {
            _viewModel.NewName = null;
            _viewModel.Tiptext = "请输入新建词库的名称";
            var res = await NewDialog.ShowAsync();
            _lastCommand = "NewDic";
        }


        private async void _viewModel_DicOpAction(object sender, CommandArgs args) {
            switch (args.command)
            {
                case "Open":
                    VisualStateManager.GoToState(this, "CompleteInfo", true);
                    break;
                case "Close":
                    VisualStateManager.GoToState(this, "BrifeInfo", true);
                    if (!VocabularySemantic.IsZoomedInViewActive)
                        VocabularySemantic.ToggleActiveView();
                    break;
                case "ReName":
                    _viewModel.NewName = null;
                    _viewModel.Tiptext = "请输入新的词库名称";
                    var res = await NewDialog.ShowAsync();
                    break;
                case "ReMove":
                    VisualStateManager.GoToState(this, "BrifeInfo", true);
                    break;
                 

            }
        }

        private void _viewModel_DialogActions(object sender, CommandArgs args) {
            if(args.parameter.Equals("YES"))
                switch (_lastCommand) {
                    case "NewDic":
                        CustomDicManage.AddACustomDictionary(_viewModel.NewName);
                        break;
                    default:break;
                }
            _lastCommand = null;
            NewDialog.Hide();
        }
        #endregion

        #region Private
        private void DicPage_Loaded(object sender, RoutedEventArgs e) {
            _viewModel = DataContext as DicPageViewModel;
            _viewModel.CommandActions += _viewModel_CommandActions;
            _viewModel.DialogActions += _viewModel_DialogActions;
            _viewModel.DicOpAction += _viewModel_DicOpAction;
            (App.Current.Resources["OverSettingService"] as OverSettingService).SetStateBarButtonFg((App.Current.Resources["DicPageFg"] as SolidColorBrush).Color);
        }

        private void DicPage_Unloaded(object sender, RoutedEventArgs e) {
            _viewModel.CommandActions -= _viewModel_CommandActions;
            _viewModel.DialogActions -= _viewModel_DialogActions;
            _viewModel.DicOpAction -= _viewModel_DicOpAction;
        }
        #endregion

        #region Constructor
        public DicPage() {
            InitializeComponent();
            Loaded += DicPage_Loaded;
            Unloaded += DicPage_Unloaded;
            ((DicPageViewModel)DataContext).window = Window.Current;
        }
        #endregion

        private void Page_DragOver(object sender, DragEventArgs e) {
            e.DragUIOverride.IsGlyphVisible = false;
        }
    }
}
