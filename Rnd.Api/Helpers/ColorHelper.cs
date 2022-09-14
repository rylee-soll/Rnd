﻿using System.Drawing;
using Dice;

namespace Rnd.Api.Helpers;

public static class ColorHelper
{
    public static readonly IReadOnlyCollection<Color> Default = new List<Color>
    {
        Color.Silver,
        Color.Brown,
        Color.Salmon,
        Color.RosyBrown,
        Color.Orange,
        Color.SandyBrown,
        Color.Gold,
        Color.Khaki,
        Color.GreenYellow,
        Color.ForestGreen,
        Color.Teal,
        Color.Lavender,
        Color.Aquamarine,
        Color.SkyBlue,
        Color.DodgerBlue,
        Color.Navy,
        Color.MediumSlateBlue,
        Color.Purple,
        Color.Orchid,
        Color.MediumVioletRed,
    };

    public static Color PickRandomDefault()
    {
        var random = (int) Roller.Roll($"1d{Default.Count}").Value;
        return Default.ElementAt(random);
    }

    public static string ToHex(this Color color)
    {
        return "#" + 
               color.R.ToString("X2") + 
               color.G.ToString("X2") + 
               color.B.ToString("X2") + 
               color.A.ToString("X2");
    }
}