using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Threading;
using UAssetAPI.PropertyTypes.Structs;
using UAssetAPI.UnrealTypes.EngineEnums;

namespace MercuryConverter.UI.Views;

public partial class Export : Panel
{
    public Export()
    {
        InitializeComponent();
        ShouldAudioConvertRadio.IsCheckedChanged += AudioConvertCheckChanged;
    }

    private void AudioConvertCheckChanged(object? sender, RoutedEventArgs args)
    {
        RadioButton btn = (RadioButton)args.Source!;
        AudioConvertFormat.IsEnabled = (bool)btn.IsChecked!;
    }
}