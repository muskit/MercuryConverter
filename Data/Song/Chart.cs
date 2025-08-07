using System;

namespace MercuryConverter.Data;


public class Chart
{
    public required string audioId;
    public required string audioOffset;
    public required string audioPreviewTime;
    public required string audioPreviewDuration;
    public required string video;
    public required string designer;
    public required string clearRequirement;
    public required string diffLevel;
    public string diffString
    {
        get
        {
            var d = Convert.ToDouble(diffLevel);
            var i = (int)d;
            return $"{(int)d}{(d > i ? "+" : "")}";
        }
    }
}