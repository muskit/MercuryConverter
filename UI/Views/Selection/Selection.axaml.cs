using Microsoft.VisualBasic;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Avalonia.Controls;
using MercuryConverter.Data;
using Avalonia;
using Avalonia.Threading;
using Avalonia.Media;
using System.IO;
using Avalonia.Media.Imaging;

namespace MercuryConverter.UI.Views;

public partial class Selection : Panel
{
    public Selection()
    {
        InitializeComponent();
        ListingTable.CellPointerPressed += OnCellClicked;
        ListingTable.SelectionMode = DataGridSelectionMode.Extended;

        foreach (var (k, v) in Consts.NUM_SOURCE)
        {
            FilterSourceContainer.Children.Add(
                new CheckBox
                {
                    Name = $"FilterSourceCheckbox{k}",
                    Content = v,
                }
            );
        }
        foreach (var (k, v) in Consts.CATEGORY_INDEX)
        {
            if (k == -1)
                continue;

            FilterCategoryContainer.Children.Add(
                new CheckBox
                {
                    Name = $"FilterCategoryCheckbox{k}",
                    Content = v,
                }
            );
        }
    }

    private void OnCellClicked(object? sender, DataGridCellPointerPressedEventArgs e)
    {
        Console.WriteLine($"{e.PointerPressedEventArgs.KeyModifiers} - {e.Cell}");

        if (e.Row.DataContext is Song song)
        {
            Console.WriteLine($"{song.Id}: {song.Artist} - {song.Name}");
            Dispatcher.UIThread.Post(() =>
            {
                if (song.Jacket != null)
                {
                    var file = File.OpenRead(song.Jacket);
                    InfoImageJacket.Source = new Bitmap(file);
                }
                InfoNameText.Text = song.Name;
                InfoArtistText.Text = song.Artist;
                InfoSourceText.Text = song.SourceName;
            });
        }
    }
}