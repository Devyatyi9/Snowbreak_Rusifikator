using Avalonia.Platform.Storage;
using ReactiveUI;
using Avalonia.Controls;
using System.Diagnostics;
using System.Reactive;
using System;
using static Snowbreak_Rusifikator.IConfigs;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Reactive.Linq;
using Snowbreak_Rusifikator.Models;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia;
using Snowbreak_Rusifikator.Views;

namespace Snowbreak_Rusifikator.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    public IStorageProvider? provider;

    //var window = TopLevel.GetTopLevel(Views.MainWindow);
    private string _message = string.Empty;
    private string _testmessage = string.Empty;
    private string _output = Models.MainModel.isTester.ToString();
    //private string _output = "Waiting...";

    public string Message
    {
        get => _message;
        set => this.RaiseAndSetIfChanged(ref _message, value);
    }

    public string Output
    {
        get => _output;
        set => this.RaiseAndSetIfChanged(ref _output, value);
    }

    public ReactiveCommand<Unit, Unit> ExampleCommand { get; }
    public ReactiveCommand<Unit, Unit> SelectFolderDialog {  get; }

    public MainWindowViewModel()
    {
        IObservable<bool> isValidObservable = this.WhenAnyValue(
            x => x.Message,
            x => !string.IsNullOrWhiteSpace(x) && x.Length > 7);
        ExampleCommand = ReactiveCommand.Create(PerformAction,
                                                isValidObservable);
        //SelectFolderDialog = ReactiveCommand.Create(SelectGameFlder);
    }

    private void PerformAction()
    {
        Output = $"The action was called. {_message}";
        Message = String.Empty;
    }
    //

    public string TestMessage
    {
        get => _testmessage;
        set => this.RaiseAndSetIfChanged(ref _testmessage, value);
    }

    public void ButtonClose_Command()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.IsActive is not { } parent)
            throw new NullReferenceException("Missing MainWindow instance.");
        desktop.MainWindow?.Close();
    }
    public void ButtonMinimize_Command()
    {
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.IsActive is not { } window)
            throw new NullReferenceException("Missing MainWindow instance.");
        desktop.MainWindow.WindowState = WindowState.Minimized;
    }

    public async Task SelectGameFolder_Command()
    {
        // See IoCFileOps project for an example of how to accomplish this.
        if (Application.Current?.ApplicationLifetime is not IClassicDesktopStyleApplicationLifetime desktop ||
            desktop.MainWindow?.StorageProvider is not { } provider)
            throw new NullReferenceException("Missing StorageProvider instance.");
        IReadOnlyList<IStorageFolder> folder = await provider.OpenFolderPickerAsync(new FolderPickerOpenOptions { Title = "Выберите папку игры", AllowMultiple = false });
        if (folder.Count == 1)
        {
            await MainModel.GetGameFolder(folder);
        }
    }


#pragma warning disable CA1822 // Mark members as static

#pragma warning restore CA1822 // Mark members as static
    //Models.MainModel MainModel { get; set; }
}
