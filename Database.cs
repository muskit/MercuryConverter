namespace MercuryConverter;

using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

public static class Database
{
    public static readonly ObservableCollection<Data.Song> Songs = new();
}