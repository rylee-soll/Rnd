﻿using System.Text.RegularExpressions;

namespace Rnd.Compiler.Lexer;

public class Lexeme
{
    private Lexeme(LexemeType type, string value, Lexeme? previous)
    {
        Type = type;
        Value = value;

        if (previous == null) return;
        
        Column = previous.Column + previous.Width;
        Previous = previous;
        previous.Next = this;
    }
    
    public int Column { get; }
    public int Width => Value.Length;
    
    public Lexeme? Previous { get; }
    public Lexeme? Next { get; set; }
    
    public LexemeType Type { get; }
    public string Value { get; }
    
    #region Parser

    public static List<Lexeme> Parse(string source)
    {
        var result = new List<Lexeme>();
        
        while (source != String.Empty)
        {
            result.Add(ParseNext(ref source, result.LastOrDefault()));
        }

        return result;
    }

    public static Lexeme ParseNext(ref string source, Lexeme? previous)
    {
        var type = ParseType(source);
        var value = Regex.Match(source, Patterns[type]).Value;
        source = Regex.Replace(source, Patterns[type], "").TrimStart();
        return new Lexeme(type, value, previous);
    }
    
    public static LexemeType ParseType(string source)
    {
        return Patterns.FirstOrDefault(p => Regex.IsMatch(source, p.Value)).Key;
    }

    public static Dictionary<LexemeType, string> Patterns => 
        Enum.GetValues<LexemeType>().Reverse()
            .ToDictionary(type => type, GetPattern);

    public static string GetPattern(LexemeType type)
    {
        return "^" + type switch
        {
            LexemeType.Unknown => ".",
            LexemeType.Literal => "\\{.*\\}|\".*\"|\\d+.?\\d*|true|false|\\[.*\\]",
            LexemeType.Multistring => "\"\"\"",
            LexemeType.Operator => ":|\\.",
            LexemeType.Identifier => "[A-Z]\\w*",
            LexemeType.Attribute => "[a-z]\\w*",
            LexemeType.Role => "var|const|exp|func|module",
            LexemeType.Type => "obj|str|int|float|bool|list",
            LexemeType.Accessor => "public|private|protected",
            LexemeType.Tabulation => "(?:  )+",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, "Unknown lexeme type")
        };
    }

    #endregion
}