using System.Text.RegularExpressions;
using MediatR;
internal sealed class ThemeBuildRequest : IRequest<ThemeBuildResponse?>
{
    internal string? Source { get; init; }
}
internal sealed class ThemeBuildResponse
{
    internal string? Target { get; init; }
}
internal sealed class ThemeBuildRequestHandler() : IRequestHandler<ThemeBuildRequest, ThemeBuildResponse?>
{
    const string THEME_RULE = @"(?'THEME_RULE'\.(?'THEME_LOCATION'(B|F|))(?'THEME_COLOR'(D|N|T|I|W|C|))(?'THEME_TONALITY'(0|1|2|3|4|5|6|7|8|9|)))";
    const string THEME = @"(?'THEME'\[!" + THEME_RULE + @"{1,2}\](?'THEME_CONTENT'\w+))";
    public async Task<ThemeBuildResponse?> Handle(ThemeBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return new ThemeBuildResponse
        {
            Target = Build(request.Source),
        };
    }

    private string? Build(string? source)
    {
        if (source == default)
            return source;

        return Regex.Replace(
            source,
            $"({THEME})",
            match =>
            {
                var themes = match
                    .Groups["THEME_RULE"]
                    .Captures
                    .Select(capture =>
                        Regex.Replace(capture.Value, THEME_RULE, match =>
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
                        })
                    )
                    .ToArray();

                return @$"<span class=""theme {string.Join(" ", themes)}"">{match.Groups["THEME_CONTENT"].Value}</span>";
            },
            RegexOptions.Multiline);
    }
}
