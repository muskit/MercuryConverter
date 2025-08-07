using System;
using System.Collections.Specialized;
using Avalonia.Controls;

namespace MercuryConverter.Views;

public partial class Selection : Panel
{
    public Selection()
    {
        InitializeComponent();
        Database.Songs.CollectionChanged += OnSongsChg;
    }

    private void OnSongsChg(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Console.WriteLine("Songs collection changed!");

        if (e.NewItems != null)
        {
            Console.WriteLine("Added...");
            foreach (Data.Song added in e.NewItems)
            {
                Console.WriteLine($"[{added.id}] {added.artist} - {added.name}");
            }
        }
        if (e.OldItems != null)
        {
            Console.WriteLine("Removed...");
            foreach (Data.Song rem in e.OldItems)
            {
                Console.WriteLine($"[{rem.id}] {rem.artist} - {rem.name}");
            }
        } 
    }
}