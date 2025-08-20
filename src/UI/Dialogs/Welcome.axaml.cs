using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using DialogHostAvalonia;

namespace MercuryConverter.UI.Dialogs;

public partial class Welcome : Window
{
    public Welcome()
    {
        InitializeComponent();
    }

    private void ClickHandler(object sender, RoutedEventArgs args)
    {
        MainWindow.Instance!.Dialog.DialogContent = new DataScanning().Content;
    }
}