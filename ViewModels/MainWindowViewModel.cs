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
    #region [Properties]
    [ObservableProperty]
    private string installRemoveButtonContent = @"/..\";

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
    public string status = string.Empty;
    #endregion

    public MainWindowViewModel()
    {
        status = "Загрузка...";
        Models.MainModel.BaseProgramConfig();
        isTesterCheckboxChecked = Models.MainModel.isTester;
        if (Models.MainModel.programConfig.fileName != "")
        {
            installRemoveButtonContent = "Удалить перевод";
            isCheckInstallUpdatesButtonEnabled = true;
        } else { installRemoveButtonContent = "Установить перевод"; }
        if (Models.MainModel.programConfig.gamePath != "")
        {
            isInstallRemoveButtonEnabled = true;
        }
        if (Models.MainModel.programConfig.launcherPath != "")
        {
            isStartLauncherButtonEnabled = true;
        }
        status = "Готово";
    }

    //{Binding SelectGameFolderCommand}
    //[RelayCommand]
    private async Task SelectGameFolder()
    {
    }

    private async void InstallRemove() { }

    private async void CheckInstallUpdates() { }

    //{Binding StartLauncherCommand}
    //[RelayCommand]
    private async void StartLauncher() { }

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

}
