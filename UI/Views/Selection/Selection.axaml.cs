using Microsoft.VisualBasic;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Threading.Tasks;
using Avalonia.Controls;
using MercuryConverter.Data;
using Avalonia;

namespace MercuryConverter.UI.Views;

public partial class Selection : Panel
{
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

        // DataContext = this;
    }
}