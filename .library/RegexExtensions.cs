using System.Text.RegularExpressions;
internal static class RegexExtensions
{
    internal static async Task<string> ReplaceAsync(this Regex regex, string input, Func<Match, Task<string?>> match)
    {
        var result = input;
        var offset = 0;
        foreach (Match matched in regex.Matches(input))
        {
            var replaced = await match(matched) ?? string.Empty;
            var start = matched.Index + offset;
            var buffer = replaced.Length;
            var end = matched.Length;
            result = result
                .Remove(start, end)
                .Insert(start, replaced);

            offset += buffer - end;
        }
        return result;
    }

    internal static async Task<string> ReplaceAsync(this Regex regex, string input, Func<Match, string?> match)
    {
        await Task.Yield();

        var result = input;
        var offset = 0;
        foreach (Match matched in regex.Matches(input))
        {
            var replaced = match(matched) ?? string.Empty;
            var start = matched.Index + offset;
            var buffer = replaced.Length;
            var end = matched.Length;
            result = result
                .Remove(start, end)
                .Insert(start, replaced);

            offset += buffer - end;
        }
        return result;
    }
}
