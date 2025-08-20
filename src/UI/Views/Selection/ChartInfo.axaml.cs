using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;
using MercuryConverter.Data;
using SaturnData.Notation.Core;

namespace MercuryConverter.UI.Views;

public partial class ChartInfo : UserControl
{
    private static readonly IReadOnlyDictionary<Difficulty, (Color, Color)> COLORS = new Dictionary<Difficulty, (Color, Color)>(){
        {Difficulty.Normal, (new Color(0xff, 0x1b, 0x7c, 0xff), new Color(0xff, 0x3f, 0x66, 0xfa))},
        {Difficulty.Hard, (new Color(0xff, 0xFF, 0xC3, 0x00), new Color(0xff, 0xFF, 0xA0, 0x00))},
        {Difficulty.Expert, (new Color(0xff, 0xFF, 0x00, 0x84), new Color(0xff, 0xCF, 0x00, 0x5B))},
        {Difficulty.Inferno, (new Color(0xff, 0x40, 0x00, 0x43), new Color(0xff, 0x1F, 0x00, 0x20))}
    };

    public ChartInfo(Song song, Difficulty diff = Difficulty.Inferno)
    {
        InitializeComponent();

        var colLight = COLORS[diff].Item1;
        var colDark = COLORS[diff].Item2;
        ShapeBase.Fill = new SolidColorBrush(colLight);
        ShapeLeftRound.Fill = new SolidColorBrush(colLight);
        ShapeRightRound.Fill = new SolidColorBrush(colDark);
        ShapeDiagonal.Fill = new SolidColorBrush(colDark);

        var l = song.Levels[(int)diff]!;
        var level = l.Value.Item1;
        var designer = l.Value.Item2;
        LevelNoTxt.Text = $"{(int)level}{(level - (int)level >= 0.7f ? "+" : "")}";
        LevelNameTxt.Text = diff.ToString().ToUpper();
        DesignerTxt.Text = designer;
    }
}