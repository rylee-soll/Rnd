﻿using System.Text;

namespace Rnd.Api.Logic.Helpers;

public static class PathHelper
{
    public const string Separator = ".";

    public static string Combine(params string?[] items)
    {
        var sb = new StringBuilder();

        foreach (var item in items)
        {
            if (String.IsNullOrWhiteSpace(item)) continue;
            sb.Append(item);
        }

        return sb.ToString();
    }
    
    public static string GetName(string fullname)
    {
        var items = fullname.Split(Separator);
        return items.Last();
    }
    
    public static string? GetPath(string fullname)
    {
        var items = fullname.Split(Separator);
        if (items.Length <= 1) return null;
        return string.Join(Separator, items.SkipLast(1));
    }
}