using System.Text.RegularExpressions;
using MediatR;
internal sealed class ABuildRequest : IRequest<ABuildResponse?>
{
    internal string? Source { get; init; }
}
internal sealed class ABuildResponse
{
    internal string? Target { get; init; }
}
internal sealed partial class ABuildRequestHandler(ProjectBuildResponse project) : IRequestHandler<ABuildRequest, ABuildResponse?>
{
    const string PATTERN = @"(?'A'\[(?!(\^|!))(?'A_CONTENT'.*?)\]\((?'A_HREF'.*?)(?'A_HREF_SUFIX'readme.md.*?|)\))";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline | RegexOptions.IgnoreCase)]
    private static partial Regex Regex();
    public async Task<ABuildResponse?> Handle(ABuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;

        var target = await Regex().ReplaceAsync(
            request.Source,
            match =>
            {
                var href = new Uri(match.Groups["A_HREF"].Value);

                if (string.IsNullOrWhiteSpace(href.Host))
                    return $@"<a href=""{project.BaseUrl!.AbsoluteUri.TrimEnd('/')}/{href.LocalPath.TrimStart('/')}"">{match.Groups["A_CONTENT"].Value}</a>";

                return $@"<a href=""{href}{match.Groups["A_HREF_SUFIX"].Value}"">{match.Groups["A_CONTENT"].Value}</a>";
            });

        return new ABuildResponse
        {
            Target = target,
        };
    }
}
