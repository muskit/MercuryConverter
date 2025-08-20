using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

public class ObservableRangeCollection<T> : ObservableCollection<T>
{
    private bool _suppressNotification = false;

    protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
    {
        // Only raise the event if notifications are not suppressed
        if (!_suppressNotification)
        {
            base.OnCollectionChanged(e);
        }
    }

    public void AddRange(IEnumerable<T> collection)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        _suppressNotification = true; // Suppress notifications during bulk add

        foreach (var item in collection)
        {
            Items.Add(item); // Add items using the underlying IList<T>
        }

        _suppressNotification = false; // Re-enable notifications

        // Raise a single Reset event to notify about the bulk change
        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
    }
}