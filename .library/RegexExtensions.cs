using System.Text.RegularExpressions;
internal static class RegexExtensions
{
    internal static async Task<string> ReplaceAsync(this Regex regex, string input, Func<Match, Task<string?>> match)
    {
        var result = input;
        var offset = 0;
        foreach (var item in regex
            .Matches(input)
            .Select(async matched =>
            {
                var item = new
                {
                    Replaced = await match(matched) ?? string.Empty,
                    Start = matched.Index + offset,
                    End = matched.Length
                };
                offset += item.Replaced.Length - item.End;
                return item;
            }))
        {
            result = result
                .Remove((await item).Start, (await item).End)
                .Insert((await item).Start, (await item).Replaced);
        }
        return result;
    }

    internal static async Task<string> ReplaceAsync(this Regex regex, string input, Func<Match, string?> match)
    {
        await Task.Yield();

        var result = input;
        var offset = 0;
        foreach (var item in regex
            .Matches(input)
            .Select(matched =>
            {
                var item = new
                {
                    Replaced = match(matched) ?? string.Empty,
                    Start = matched.Index + offset,
                    End = matched.Length
                };
                offset += item.Replaced.Length - item.End;
                return item;
            }))
        {
            result = result
                .Remove(item.Start, item.End)
                .Insert(item.Start, item.Replaced);
        }
        return result;
    }
}
