using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using ReactiveUI;
using Snowbreak_Rusifikator.Models;
using Snowbreak_Rusifikator.ViewModels;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reactive;
using System.Reflection.PortableExecutable;
using System.Runtime.Versioning;
using System.Threading.Tasks;

namespace Snowbreak_Rusifikator.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        InitializeComponent();
        Models.MainModel.BaseProgramConfig();
        PointerPressed += (_, e) =>
        {
            if ((WindowState == WindowState.Normal) || (WindowState == WindowState.Maximized))
                BeginMoveDrag(e);
        };
    }
    // Text="{Binding $self}"


    //private async void test(object? sender, Avalonia.Interactivity.RoutedEventArgs e) { }
    private async void SelectPathButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        //var topLevel = TopLevel.GetTopLevel(control);
        //IStorageProvider storage = StorageProvider;
        //await Models.MainModel.SelectGameFolder(storage);
    }

    public async Task SelectGameFolderTest_Command()
    {
        //var topLevel = TopLevel.GetTopLevel(this);
        IReadOnlyList<IStorageFolder> folder = await StorageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions { Title = "Выберите папку игры", AllowMultiple = false });
        if (folder.Count == 1)
        {
            await MainModel.GetGameFolder(folder);
        }
    }

    //MyTextInput.AddHandler(TextInputEvent, MyTextInput_InputHandler, RoutingStrategies.Tunnel);

    private async void TesterCheckbox_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var topLevel = TopLevel.GetTopLevel(this);
        testerCheckbox.IsEnabled = false;
        Models.MainModel.isTester = (bool)testerCheckbox.IsChecked;
        await Models.MainModel.ChangeTesterState();
        //Models.MainModel.StartUpdate();
        testerCheckbox.IsEnabled = true;
    }

    private void InstallRemoveButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
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

    private void StartLauncherButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        startLauncherButton.IsEnabled = false;
        Models.MainModel.RunLauncher();
        startLauncherButton.IsEnabled = true;
    }
}