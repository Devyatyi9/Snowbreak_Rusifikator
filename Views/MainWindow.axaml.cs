using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using Snowbreak_Rusifikator.Models;
using Snowbreak_Rusifikator.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace Snowbreak_Rusifikator.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        PointerPressed += (_, e) =>
        {
            if ((WindowState == WindowState.Normal) || (WindowState == WindowState.Maximized))
                BeginMoveDrag(e);
        };
    }

    private void ButtonClose_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        Close();
    }

    private void ButtonMinimize_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        WindowState = WindowState.Minimized;
    }

    //private async void test(object? sender, Avalonia.Interactivity.RoutedEventArgs e) { }
    private async void SelectPathButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        IReadOnlyList<IStorageFolder> folder = await topLevel.StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions { Title = "Выберите папку игры", AllowMultiple = false });
        if (folder.Count == 1)
        {
            await MainModel.GetGameFolder(folder);
        }
    }
    //MyTextInput.AddHandler(TextInputEvent, MyTextInput_InputHandler, RoutingStrategies.Tunnel);

    private async void TesterCheckbox_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        //var topLevel = TopLevel.GetTopLevel(this);
        testerCheckbox.IsEnabled = false;
        Models.MainModel.isTester = (bool)testerCheckbox.IsChecked;
        await Models.MainModel.ChangeTesterState();
        //Models.MainModel.StartUpdate();
        testerCheckbox.IsEnabled = true;
    }

    private async void InstallRemoveButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        installRemoveButton.IsEnabled = false;
        //if проверка конфига на наличие установленой версии
        Models.MainModel.StartUpdate();
        // изменить имя на Удалить перевод
        installRemoveButton.IsEnabled = true;
    }

    private async void CheckInstallUpdatesButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        checkInstallUpdatesButton.IsEnabled = false;
        //
        checkInstallUpdatesButton.IsEnabled = true;
    }

    private async void StartLauncherButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        startLauncherButton.IsEnabled = false;
        Models.MainModel.RunLauncher();
        startLauncherButton.IsEnabled = true;
    }
}