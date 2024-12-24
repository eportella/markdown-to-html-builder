using System.Text.RegularExpressions;
internal static class RegexExtensions
{
    internal static async Task<string> ReplaceAsync(this Regex regex, string input, Func<Match, Task<string?>> match)
    {
        var matches = regex.Matches(input);

        var result = input;
        
        int offset = 0;
        foreach (Match item in matches)
        {
            var replaced = await match(item) ?? string.Empty;
            result = result
                .Remove(item.Index + offset, item.Length)
                .Insert(item.Index + offset, replaced);

            offset += replaced.Length - item.Length;
        }
        return result;
    }

    internal static async Task<string> ReplaceAsync(this Regex regex, string input, Func<Match, string?> match)
    {
        await Task.Yield();
        var matches = regex.Matches(input);

        var result = input;
        
        int offset = 0;
        foreach (Match item in matches)
        {
            var replaced = match(item) ?? string.Empty;
            result = result
                .Remove(item.Index + offset, item.Length)
                .Insert(item.Index + offset, replaced);

            offset += replaced.Length - item.Length;
        }
        return result;
    }
}
