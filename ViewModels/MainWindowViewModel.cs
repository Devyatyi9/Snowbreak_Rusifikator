using Avalonia.Platform.Storage;
using Avalonia.Controls;
using System.Diagnostics;
using System;
using static Snowbreak_Rusifikator.IConfigs;
using System.Collections.Generic;
using System.Threading.Tasks;
using Snowbreak_Rusifikator.Models;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using Snowbreak_Rusifikator.Views;
using System.Linq;
using Avalonia.Input;
using CommunityToolkit.Mvvm.ComponentModel;

namespace Snowbreak_Rusifikator.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    [ObservableProperty]
    private bool isInstallRemoveButtonEnabled = false;
    [ObservableProperty]
    private bool isCheckInstallUpdatesButtonEnabled = false;
    [ObservableProperty]
    private bool isStartLauncherButtonEnabled = false;

    [ObservableProperty]
    private bool isTesterCheckboxEnabled = true;
    [ObservableProperty]
    private bool isTesterCheckboxChecked = false;

    [ObservableProperty]
    private string status = "Статус";

    public MainWindowViewModel()
    {
        Models.MainModel.BaseProgramConfig();
        isTesterCheckboxChecked = Models.MainModel.isTester;
        //status = "Загрузка...";
    }

    //{Binding SelectGameFolderCommand}
    //[RelayCommand]
    private async Task SelectGameFolder()
    {
    }

    //{Binding TesterCheckboxCommand}
    //[RelayCommand]
    private async void TesterCheckbox()
    {
        //var topLevel = TopLevel.GetTopLevel(this);
        //testerCheckbox.IsEnabled = false;
        //Models.MainModel.isTester = (bool)testerCheckbox.IsChecked;
        //await Models.MainModel.ChangeTesterState();
        ////Models.MainModel.StartUpdate();
        //testerCheckbox.IsEnabled = true;
    }

    //
    
#pragma warning disable CA1822 // Mark members as static

#pragma warning restore CA1822 // Mark members as static
}
