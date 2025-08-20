using System;
using System.Collections.Generic;
using System.Linq;
using SaturnData.Notation.Core;

namespace MercuryConverter.Data;

public static class Consts
{
    private static Dictionary<int, string> _CATEGORY_INDEX = new()
    {
        { -1, "Unknown"},
        { 0, "Anime/Pop"},
        { 1, "Vocaloid"},
        { 2, "Touhou Project"},
        { 3, "2.5D" },
        { 4, "Variety" },
        { 5, "Original" },
        { 6, "HARDCORE TANO*C" },
        { 7, "VTuber" },
    };
    public static readonly IReadOnlyDictionary<int, string> CATEGORY_INDEX = _CATEGORY_INDEX;

    private static Dictionary<uint, string> _NUM_SOURCE = new()
    {
        { 1, string.Concat(new int[] {87, 65, 67, 67, 65}.Select(i => Convert.ToChar(i))) },
        { 2, string.Concat(new int[] {87, 65, 67, 67, 65, 32, 83}.Select(i => Convert.ToChar(i))) },
        { 3, string.Concat(new int[] {87, 65, 67, 67, 65, 32, 76, 73, 76, 89}.Select(i => Convert.ToChar(i))) },
        { 4, string.Concat(new int[] {87, 65, 67, 67, 65, 32, 76, 73, 76, 89, 32, 82}.Select(i => Convert.ToChar(i))) },
        { 5, string.Concat(new int[] {87, 65, 67, 67, 65, 32, 82, 101, 118, 101, 114, 115, 101}.Select(i => Convert.ToChar(i))) }
    };
    public static readonly IReadOnlyDictionary<uint, string> NUM_SOURCE = _NUM_SOURCE;
    public static readonly IReadOnlyDictionary<string, uint> SOURCE_NUM = _NUM_SOURCE.ToDictionary(p => p.Value, p => p.Key);

    private static readonly Dictionary<Difficulty, string> _DIFF_LVL_KEY = new()
    {
        {Difficulty.Normal, "DifficultyNormalLv"},
        {Difficulty.Hard, "DifficultyHardLv"},
        {Difficulty.Expert, "DifficultyExtremeLv"},
        {Difficulty.Inferno, "DifficultyInfernoLv"},
    };
    public static readonly IReadOnlyDictionary<Difficulty, string> DIFF_LVL_KEY = _DIFF_LVL_KEY;

    private static readonly Dictionary<Difficulty, string> _DIFF_FILENAME_PREPEND = new()
    {
        {Difficulty.Normal, "00"},
        {Difficulty.Hard, "01"},
        {Difficulty.Expert, "02"},
        {Difficulty.Inferno, "03"},
    };
    public static readonly IReadOnlyDictionary<Difficulty, string> DIFF_FILENAME_PREPEND = _DIFF_FILENAME_PREPEND;

    private static readonly Dictionary<Difficulty, string> _DIFF_CLEAR_KEY = new()
    {
        {Difficulty.Normal, "ClearNormaRateNormal"},
        {Difficulty.Hard, "ClearNormaRateHard"},
        {Difficulty.Expert, "ClearNormaRateExtreme"},
        {Difficulty.Inferno, "ClearNormaRateInferno"},
    };
    public static readonly IReadOnlyDictionary<Difficulty, string> DIFF_CLEAR_KEY = _DIFF_CLEAR_KEY;

    public static readonly IReadOnlyDictionary<Difficulty, string> DIFF_DESIGNER_KEY = new Dictionary<Difficulty, string>()
    {
        {Difficulty.Normal, "NotesDesignerNormal"},
        {Difficulty.Hard, "NotesDesignerHard"},
        {Difficulty.Expert, "NotesDesignerExpert"},
        {Difficulty.Inferno, "NotesDesignerInferno"},
    };
}