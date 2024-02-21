using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data;
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
        //Models.MainModel.BaseProgramConfig();
        PointerPressed += (_, e) =>
        {
            if ((WindowState == WindowState.Normal) || (WindowState == WindowState.Maximized))
                BeginMoveDrag(e);
        };
    }

    private void ButtonClose_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    { Close(); }

    private void ButtonMinimize_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    { WindowState = WindowState.Minimized; }
}