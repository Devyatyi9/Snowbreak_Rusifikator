using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection.PortableExecutable;
using System.Runtime.Versioning;

namespace Snowbreak_Rusifikator.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Models.MainModel.BaseProgramConfig();
        PointerPressed += (_, e) =>
        {
            if ((WindowState == WindowState.Normal) || (WindowState == WindowState.Maximized))
                BeginMoveDrag(e);
        };
    }
    // Text="{Binding $self}"


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
        IStorageProvider storage = StorageProvider;
        await Models.MainModel.SelectGameFlder(storage);
    }

    //MyTextInput.AddHandler(TextInputEvent, MyTextInput_InputHandler, RoutingStrategies.Tunnel);

    private async void TesterCheckbox_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
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