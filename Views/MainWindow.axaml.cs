using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;
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
    private void SelectPathButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        //OpenFileDialog
    }

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
        Models.MainModel.StartUpdate();
        installRemoveButton.IsEnabled = true;
    }

    private void CheckUpdatesButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
    }

    private void StartLauncherButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        //
    }
}