using System;
using Avalonia.Controls;
using MercuryConverter.Data;
using Avalonia.Threading;
using System.IO;
using Avalonia.Media.Imaging;
using SaturnData.Notation.Core;
using System.Collections.Generic;

namespace MercuryConverter.UI.Views;

public partial class Selection : Panel
{
    public static ObservableRangeCollection<Song> Selections { get; } = new();

    private List<CheckBox> sourceCBs = new();
    private List<CheckBox> categoryCBs = new();

    public Selection()
    {
        InitializeComponent();
        DataContext = this;

        ListingTable.CellPointerPressed += OnCellClicked;
        ListingTable.SelectionChanged += OnSelectionChange;
        ListingTable.SelectionMode = DataGridSelectionMode.Extended;

        foreach (var (k, v) in Consts.NUM_SOURCE)
        {
            var cb = new CheckBox { Content = v };
            cb.IsCheckedChanged += OnFilterChange;
            sourceCBs.Add(cb);
            FilterSourceContainer.Children.Add(cb);
        }

        foreach (var (k, v) in Consts.CATEGORY_NAME)
        {
            if (k == -1)
                continue;

            var cb = new CheckBox { Content = v };
            cb.IsCheckedChanged += OnFilterChange;
            categoryCBs.Add(cb);
            FilterCategoryContainer.Children.Add(cb);
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

        // Console.WriteLine($"{e.PointerPressedEventArgs.Properties.IsRightButtonPressed} - {e.Cell.Content}");
    }

    private void OnSelectionChange(object? sender, SelectionChangedEventArgs e)
    {
        Selections.Clear();
        List<Song> sels = new();
        foreach (Song s in ListingTable.SelectedItems)
        {
            sels.Add(s);
        }
        Selections.AddRange(sels);

        if (ListingTable.SelectedItems.Count > 0)
        {
            Song song = e.AddedItems.Count > 0 ?
                (Song)e.AddedItems[e.AddedItems.Count - 1]! :
                (Song)ListingTable.SelectedItems[ListingTable.SelectedItems.Count - 1]!;

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

    private void OnSearchTextChange(object? _, TextChangedEventArgs args)
    {
        UpdateListing();
    }

    private void OnFilterChange(object? _, EventArgs __)
    {
        UpdateListing();
    }

    private void UpdateListing()
    {
        var filterOptions = new FilterOptions();

        foreach (var cb in sourceCBs)
        {
            if ((bool)cb.IsChecked!)
            {
                filterOptions.sources.Add(Consts.SOURCE_NUM[(string) cb.Content!]);
            }
        }

        foreach (var cb in categoryCBs)
        {
            if ((bool)cb.IsChecked!)
            {
                filterOptions.categories.Add(Consts.CATEGORY_INDEX[(string) cb.Content!]);
            }
        }

        var searchResults = Database.SearchAndFilter(SearchBox.Text!, filterOptions);
        Dispatcher.UIThread.Post(() => ListingTable.ItemsSource = searchResults);
    }
}