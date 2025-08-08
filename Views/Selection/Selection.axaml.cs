using Microsoft.VisualBasic;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Avalonia.Controls;
using MercuryConverter.Data;
using Avalonia;

namespace MercuryConverter.Views;

public partial class Selection : Panel
{
    public static ObservableCollection<Song> SongCollection { get; } = new();
    public Selection()
    {
        InitializeComponent();
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

        SongCollection.CollectionChanged += OnSongsChg;
        DataContext = this;

        // placeholder data
        if (SongCollection.Count == 0)
        {
            SongCollection.Add(
                new Song { Id = "S00-000", Name = "A Name", Artist = "An Artist", Source = Consts.NUM_SOURCE[2] }
            );
            SongCollection.Add(
                new Song { Id = "S00-001", Name = "A Name", Artist = "An Artist", Source = Consts.NUM_SOURCE[3] }
            );
        }
    }

    private void OnSongsChg(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Console.WriteLine("Songs collection changed!");

        if (e.NewItems != null)
        {
            Console.WriteLine("Added...");
            foreach (Song added in e.NewItems)
            {
                Console.WriteLine($"[{added.Id}] {added.Artist} - {added.Name}");
            }
        }
        if (e.OldItems != null)
        {
            Console.WriteLine("Removed...");
            foreach (Song rem in e.OldItems)
            {
                Console.WriteLine($"[{rem.Id}] {rem.Artist} - {rem.Name}");
            }
        } 
    }
}