using System;
using System.Collections.Generic;
using System.Linq;
using SaturnData.Notation.Core;

namespace MercuryConverter.Data;

public static class Consts
{
    public static readonly IReadOnlyDictionary<int, string> CATEGORY_INDEX = new Dictionary<int, string>
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

    public static readonly IReadOnlyDictionary<uint, string> NUM_SOURCE = new Dictionary<uint, string>
    {
        { 1, string.Concat(new int[] {87, 65, 67, 67, 65}.Select(i => Convert.ToChar(i))) },
        { 2, string.Concat(new int[] {87, 65, 67, 67, 65, 32, 83}.Select(i => Convert.ToChar(i))) },
        { 3, string.Concat(new int[] {87, 65, 67, 67, 65, 32, 76, 73, 76, 89}.Select(i => Convert.ToChar(i))) },
        { 4, string.Concat(new int[] {87, 65, 67, 67, 65, 32, 76, 73, 76, 89, 32, 82}.Select(i => Convert.ToChar(i))) },
        { 5, string.Concat(new int[] {87, 65, 67, 67, 65, 32, 82, 101, 118, 101, 114, 115, 101}.Select(i => Convert.ToChar(i))) }
    };
    public static readonly IReadOnlyDictionary<string, uint> SOURCE_NUM = NUM_SOURCE.ToDictionary(p => p.Value, p => p.Key);

    public static readonly IReadOnlyDictionary<Difficulty, string> DIFF_LVL_KEY = new Dictionary<Difficulty, string>
    {
        {Difficulty.Normal, "DifficultyNormalLv"},
        {Difficulty.Hard, "DifficultyHardLv"},
        {Difficulty.Expert, "DifficultyExtremeLv"},
        {Difficulty.Inferno, "DifficultyInfernoLv"},
    };

    public static readonly IReadOnlyDictionary<Difficulty, string> DIFF_FILENAME_APPEND = new Dictionary<Difficulty, string>
    {
        {Difficulty.Normal, "00"},
        {Difficulty.Hard, "01"},
        {Difficulty.Expert, "02"},
        {Difficulty.Inferno, "03"},
    };

    public static readonly IReadOnlyDictionary<Difficulty, string> DIFF_CLEAR_KEY = new Dictionary<Difficulty, string>
    {
        {Difficulty.Normal, "ClearNormaRateNormal"},
        {Difficulty.Hard, "ClearNormaRateHard"},
        {Difficulty.Expert, "ClearNormaRateExtreme"},
        {Difficulty.Inferno, "ClearNormaRateInferno"},
    };

    public static readonly IReadOnlyDictionary<Difficulty, string> DIFF_DESIGNER_KEY = new Dictionary<Difficulty, string>()
    {
        {Difficulty.Normal, "NotesDesignerNormal"},
        {Difficulty.Hard, "NotesDesignerHard"},
        {Difficulty.Expert, "NotesDesignerExpert"},
        {Difficulty.Inferno, "NotesDesignerInferno"},
    };

    public static readonly IReadOnlyDictionary<Difficulty, string> DIFF_MOVIE_KEY = new Dictionary<Difficulty, string>()
    {
        {Difficulty.Normal, "MovieAssetName"},
        {Difficulty.Hard, "MovieAssetNameHard"},
        {Difficulty.Expert, "MovieAssetNameExpert"},
        {Difficulty.Inferno, "MovieAssetNameInferno"},
    };
}