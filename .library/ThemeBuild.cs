using System.Text.RegularExpressions;
using MediatR;
internal sealed class ThemeBuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed partial class ThemeBuildRequestHandler() : IRequestHandler<ThemeBuildRequest, string?>
{
    const string PATTERN = @"(?'THEME'\[!" + PATTERN_RULE + @"{1,2}\](?'THEME_CONTENT'\w+))";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline)]
    private static partial Regex Regex();
    const string PATTERN_RULE = @"(?'THEME_RULE'\.(?'THEME_LOCATION'(B|F|))(?'THEME_COLOR'(D|N|T|I|W|C|))(?'THEME_TONALITY'(0|1|2|3|4|5|6|7|8|9|)))";
    [GeneratedRegex(PATTERN_RULE, RegexOptions.Multiline)]
    private static partial Regex RegexRule();
    public async Task<string?> Handle(ThemeBuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;

        var target = await Regex()
            .ReplaceAsync(
                request.Source,
                match =>
                {
                    var themes = match
                        .Groups["THEME_RULE"]
                        .Captures
                        .Select(capture =>
                            RegexRule()
                                .Replace(
                                    capture.Value,
                                    match =>
                                    {
                                        var location = match.Groups["THEME_LOCATION"].Value;
                                        if (string.IsNullOrWhiteSpace(location))
                                            location = "F";
                                        var color = match.Groups["THEME_COLOR"].Value;
                                        if (string.IsNullOrWhiteSpace(color))
                                            color = "D";
                                        var tonality = match.Groups["THEME_TONALITY"].Value;
                                        if (string.IsNullOrWhiteSpace(tonality))
                                            tonality = "5";
                                        var content = match.Groups["THEME_CONTENT"].Value;

                                        return $"{color.ToLower()}-{location.ToLower()}-{tonality}";
                                    }
                                )
                        )
                        .ToArray();

                    return @$"<span class=""theme {string.Join(" ", themes)}"">{match.Groups["THEME_CONTENT"].Value}</span>";
                }
            );

        return target;
    }
}
