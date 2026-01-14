namespace BiografWeb.Domain.Extensions;

public static class StringExtensions
{
    /// <summary>
    // Custom delegate #1: Evaluates a string rule.
    /// </summary>
    public static bool CheckString(string input, Func<string, bool> rule) => rule(input);

    /// <summary>
    /// Custom delegate #2: Transforms a single character.
    /// </summary>
    public delegate char CharTransformer(char c);

    /// <summary>
    /// Predicate that checks if the first non-space character is uppercase.
    /// </summary>
    public static bool IsFirstUpper(string s)
        => !string.IsNullOrWhiteSpace(s) && char.IsUpper(s.Trim()[0]);

    /// <summary>
    /// Ensures the first character of the string is uppercase.
    /// Trims leading/trailing whitespace before applying the rule.
    /// Returns an empty string if input is null/whitespace.
    /// </summary>
    public static string EnsureCapitalizedFirst(this string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        var trimmed = input.Trim();

        if (trimmed.Length == 0) return string.Empty;

        // Use both the predicate and the delegate-based consumer
        if (CheckString(trimmed, IsFirstUpper)) return trimmed;

        // Leverage the second delegate (CharTransformer) to perform the capitalization
        return TransformFirstChar(trimmed, static c => char.ToUpperInvariant(c));
    }

    /// <summary>
    /// Applies a provided CharTransformer to the first character of the (trimmed) string.
    /// Returns empty string if input is null/whitespace.
    /// </summary>
    public static string TransformFirstChar(this string? input, CharTransformer transformer)
    {
        if (string.IsNullOrWhiteSpace(input)) return string.Empty;

        var trimmed = input.Trim();

        if (trimmed.Length == 0) return string.Empty;

        var first = transformer(trimmed[0]);

        return trimmed.Length == 1 ? first.ToString() : first + trimmed[1..];
    }
}

