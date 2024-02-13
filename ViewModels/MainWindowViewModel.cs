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
using CommunityToolkit.Mvvm.Input;

namespace Snowbreak_Rusifikator.ViewModels;

public partial class MainWindowViewModel : ViewModelBase
{
    #region [Properties]
    [ObservableProperty]
    private string selectPathTextBoxContent = string.Empty;

    [ObservableProperty]
    private string installRemoveButtonContent = string.Empty;

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
    private string status = string.Empty;
    #endregion

    public MainWindowViewModel()
    {
        Status = "Загрузка...";
        InstallRemoveButtonContent = @"/..\";
        Models.MainModel.BaseProgramConfig();
        IsTesterCheckboxChecked = Models.MainModel.isTester;
        if (Models.MainModel.programConfig.fileName != "")
        {
            InstallRemoveButtonContent = "Удалить перевод";
            IsCheckInstallUpdatesButtonEnabled = true;
        }
        else { InstallRemoveButtonContent = "Установить перевод"; }
        if (Models.MainModel.programConfig.gamePath != "")
        {
            IsInstallRemoveButtonEnabled = true;
            SelectPathTextBoxContent = Models.MainModel.programConfig.gamePath;
        }
        if (Models.MainModel.programConfig.launcherPath != "")
        {
            IsStartLauncherButtonEnabled = true;
        }
        Status = "Готово";
    }

    private void Testing()
    {
        
    }

    //{Binding SelectGameFolderCommand}
    //[RelayCommand]
    public async Task SelectGameFolder(IReadOnlyList<IStorageFolder> folder) // static public
    {
        await MainModel.GetGameFolder(folder);
        if (Models.MainModel.programConfig.gamePath != "")
        {
            IsInstallRemoveButtonEnabled = true;
            SelectPathTextBoxContent = Models.MainModel.programConfig.gamePath;
        }
        if (Models.MainModel.programConfig.launcherPath != "")
        {
            IsStartLauncherButtonEnabled = true;
        }
    }

    [RelayCommand]
    private async Task InstallRemove() 
    {
        IsInstallRemoveButtonEnabled = false;
        // проверка конфига на наличие установленой версии
        if (Models.MainModel.programConfig.fileName != "") 
        {
            // Remove
            Models.MainModel.RemoveFile();
            InstallRemoveButtonContent = "Установить перевод";
        } else {
            // Install
            Models.MainModel.StartUpdate();
            InstallRemoveButtonContent = "Удалить перевод";
            IsCheckInstallUpdatesButtonEnabled = true;
        }
        IsInstallRemoveButtonEnabled = true;
    }

    [RelayCommand]
    private async Task CheckInstallUpdates() 
    {
        IsCheckInstallUpdatesButtonEnabled = false;
        Models.MainModel.StartUpdate();
        IsCheckInstallUpdatesButtonEnabled = true;
    }
    
    [RelayCommand]
    private async Task StartLauncher() 
    {
        IsStartLauncherButtonEnabled = false;
        Models.MainModel.RunLauncher();
        IsStartLauncherButtonEnabled = true;
    }
    
    [RelayCommand]
    private async Task TesterCheckbox()
    {
        Status = "Изменение...";
        IsTesterCheckboxEnabled = false;
        Models.MainModel.isTester = IsTesterCheckboxChecked;
        await Models.MainModel.ChangeTesterState();
        Status = "Настройки сохранены";
        IsTesterCheckboxEnabled = true;
        await Task.Delay(500);
        Status = string.Empty;
    }
    //
    //string url = "https://boosty.to/casualgacha";
    //Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
}
