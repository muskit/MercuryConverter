using System;
using System.Collections.Generic;
using System.Linq;

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

    private static Dictionary<int, string> _NUM_SOURCE = new()
    {
        { 1, string.Concat(new int[] {87, 65, 67, 67, 65}.Select(i => Convert.ToChar(i))) },
        { 2, string.Concat(new int[] {87, 65, 67, 67, 65, 32, 83}.Select(i => Convert.ToChar(i))) },
        { 3, string.Concat(new int[] {87, 65, 67, 67, 65, 32, 76, 73, 76, 89}.Select(i => Convert.ToChar(i))) },
        { 4, string.Concat(new int[] {87, 65, 67, 67, 65, 32, 76, 73, 76, 89, 32, 82}.Select(i => Convert.ToChar(i))) },
        { 5, string.Concat(new int[] {87, 65, 67, 67, 65, 32, 82, 101, 118, 101, 114, 115, 101}.Select(i => Convert.ToChar(i))) }
    };
    public static readonly IReadOnlyDictionary<int, string> NUM_SOURCE = _NUM_SOURCE;
    public static readonly IReadOnlyDictionary<string, int> SOURCE_NUM = _NUM_SOURCE.ToDictionary(p => p.Value, p=>p.Key);

    private static string[] _DIFFICULTIES = {
        "Normal", "Hard", "Expert", "Inferno"
    };
    public static readonly IReadOnlyList<string> DIFFICULTIES = _DIFFICULTIES;
}