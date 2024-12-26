using System.Text.RegularExpressions;
using MediatR;
internal sealed class ABuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed partial class ABuildRequestHandler(Task<ProjectBuildResponse> project) : IRequestHandler<ABuildRequest, string?>
{
    const string PATTERN = @"(?'A'\[(?!(\^|!))(?'A_CONTENT'.*?)\]\((?'A_HREF'.*?)(?'A_HREF_SUFIX'readme.md.*?|)\))";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline | RegexOptions.IgnoreCase)]
    private static partial Regex Regex();
    public async Task<string?> Handle(ABuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;

        var target = await Regex().ReplaceAsync(
            request.Source,
            async match =>
            {
                var href = new Uri(match.Groups["A_HREF"].Value);

                if (string.IsNullOrWhiteSpace(href.Host))
                    return $@"<a href=""{(await project).BaseUrl!.AbsoluteUri.TrimEnd('/')}/{href.LocalPath.TrimStart('/')}"">{match.Groups["A_CONTENT"].Value}</a>";

                return $@"<a href=""{href}{match.Groups["A_HREF_SUFIX"].Value}"">{match.Groups["A_CONTENT"].Value}</a>";
            });

        return target;
    }
}
