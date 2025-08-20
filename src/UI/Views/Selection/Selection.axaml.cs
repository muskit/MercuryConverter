using System;
using System.Linq;
using Avalonia.Controls;
using MercuryConverter.Data;
using Avalonia.Threading;
using System.IO;
using Avalonia.Media.Imaging;
using SaturnData.Notation.Core;
using System.Collections.Generic;
using UAssetAPI.UnrealTypes.EngineEnums;

namespace MercuryConverter.UI.Views;

public partial class Selection : Panel
{
    public Selection()
    {
        InitializeComponent();
        ListingTable.CellPointerPressed += OnCellClicked;
        ListingTable.SelectionChanged += OnSelectionChange;
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

    /// <summary>
    /// Table cell right click handler.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void OnCellClicked(object? sender, DataGridCellPointerPressedEventArgs e)
    {
        var cell = e.Cell;
        var tb = (TextBlock)cell.Content!;

        Console.WriteLine($"{e.PointerPressedEventArgs.Properties.IsRightButtonPressed} - {e.Cell.Content}");
    }

    private void OnSelectionChange(object sender, SelectionChangedEventArgs e)
    {
        if (e.AddedItems.Count > 0)
        {
            var song = (Song) e.AddedItems[e.AddedItems.Count-1]!;

            Dispatcher.UIThread.Post(() =>
            {
                if (song.Jacket != null)
                {
                    var file = File.OpenRead(song.Jacket);
                    InfoImageJacket.Source = new Bitmap(file);
                }
                InfoId.Text = song.Id;
                InfoNameText.Text = song.Name;
                InfoArtistText.Text = song.Artist;
                InfoSourceText.Text = song.SourceName;

                ChartInfoGroup.Children.Clear();
                foreach (Difficulty diff in Enum.GetValues(typeof(Difficulty)))
                {
                    // skip non-canon difficulties
                    if (diff == Difficulty.None || diff == Difficulty.WorldsEnd) continue;

                    var idx = (int)diff;
                    var level = song.Levels[idx];
                    if (level == null) continue;

                    var ci = new ChartInfo(song, diff);
                    ChartInfoGroup.Children.Add(ci);
                }
            });
        }
    }
}