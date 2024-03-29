﻿using Avalonia.Platform.Storage;
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
using System.Threading;

namespace Snowbreak_Rusifikator.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    static readonly CancellationTokenSource s_cts = new CancellationTokenSource();
    public string Greeting => "Добро пожаловать в Авалонию!";
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

    public MainViewModel()
    {
#if !ANDROID
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
#endif
    }

    private async Task<Task> ChangeStatus(int time, string text = "")
    {
        if (text.Length > 0)
        { Status = text; }
        else { Status = Models.MainModel.programStatus; }
        await Task.Delay(time);
        Status = string.Empty;
        return Task.CompletedTask;
    }

    [RelayCommand]
    private async Task SelectGameFolder()
    {
        // See IoCFileOps project for an example of how to accomplish this.
        Status = "В процессе...";
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
            throw new NullReferenceException("Missing StorageProvider instance.");
        IReadOnlyList<IStorageFolder> folder = await provider.OpenFolderPickerAsync(new FolderPickerOpenOptions { Title = "Выберите папку игры", AllowMultiple = false });
        Status = string.Empty;
        if (folder.Count == 1)
        {
            Task localTask = await MainModel.GetGameFolder(folder);
            if (localTask.IsCompleted)
            {
                await ChangeStatus(1000);
                if (Models.MainModel.programConfig.gamePath != "")
                {
                    IsInstallRemoveButtonEnabled = true;
                    SelectPathTextBoxContent = Models.MainModel.programConfig.gamePath;
                }
                if (Models.MainModel.programConfig.fileName == "")
                {
                    InstallRemoveButtonContent = "Установить перевод";
                    IsCheckInstallUpdatesButtonEnabled = false;
                }
                if (Models.MainModel.programConfig.launcherPath == "")
                {
                    IsStartLauncherButtonEnabled = false;
                }
            }
            await Task.Delay(300);
            Status = string.Empty;
        }
    }

    [RelayCommand]
    private async Task InstallRemove()
    {
        IsInstallRemoveButtonEnabled = false;
        IsCheckInstallUpdatesButtonEnabled = false;
        Status = "В процессе...";
        Task? localTask = null;
        // проверка конфига на наличие установленой версии
        if (Models.MainModel.programConfig.fileName != "")
        {
            // Remove
            localTask = await Models.MainModel.RemoveFile();
            InstallRemoveButtonContent = "Установить перевод";
            if (localTask.IsCompleted)
            {
                ChangeStatus(1000);
                await Task.Delay(300);
                IsInstallRemoveButtonEnabled = true;
            }
        }
        else
        {
            // Install
            localTask = await Models.MainModel.StartUpdate(s_cts.Token);
            if (Models.MainModel.programConfig.fileName == "")
            {
                if (localTask.IsCompleted)
                {
                    ChangeStatus(2700);
                    await Task.Delay(300);
                    IsInstallRemoveButtonEnabled = true;
                }
            } else
            {
                InstallRemoveButtonContent = "Удалить перевод";
                if (localTask.IsCompleted)
                {
                    ChangeStatus(1000);
                    await Task.Delay(300);
                    IsCheckInstallUpdatesButtonEnabled = true;
                    IsInstallRemoveButtonEnabled = true;
                }
            }
        }
    }

    [RelayCommand]
    private async Task CheckInstallUpdates()
    {
        IsCheckInstallUpdatesButtonEnabled = false;
        IsInstallRemoveButtonEnabled = false;
        Status = "В процессе...";
        Task? localTask = await Models.MainModel.StartUpdate(s_cts.Token);
        if (localTask.IsCompleted)
        {
            await ChangeStatus(1700);
            IsCheckInstallUpdatesButtonEnabled = true;
            IsInstallRemoveButtonEnabled = true;
        }
        await Task.Delay(300);
    }

    [RelayCommand]
    private async Task StartLauncher()
    {
        IsStartLauncherButtonEnabled = false;
        Models.MainModel.RunLauncher();
        await Task.Delay(300);
        IsStartLauncherButtonEnabled = true;
        Environment.Exit(0);
    }

    [RelayCommand]
    private async Task TesterCheckbox()
    {
        Status = "Изменение...";
        IsTesterCheckboxEnabled = false;
        Models.MainModel.isTester = IsTesterCheckboxChecked;
        Task? localTask = null;
        localTask = await Models.MainModel.ChangeTesterState();
        if (localTask.IsCompleted)
        {
            await ChangeStatus(700);
            if ((Models.MainModel.programConfig.gamePath != "") && (IsCheckInstallUpdatesButtonEnabled == true))
            {
                if (localTask.IsCompleted)
                {
                    IsCheckInstallUpdatesButtonEnabled = false;
                    IsInstallRemoveButtonEnabled = false;
                    localTask = await Models.MainModel.StartUpdate(s_cts.Token);
                    if (localTask.IsCompleted)
                    {
                        await ChangeStatus(1000);
                        IsCheckInstallUpdatesButtonEnabled = true;
                        IsInstallRemoveButtonEnabled = true;
                    }
                }
            }
        }
        await Task.Delay(300);
        Status = string.Empty;
        IsTesterCheckboxEnabled = true;
    }
    //
    //string url = "https://boosty.to/casualgacha";
    //Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
}
