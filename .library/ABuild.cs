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
internal sealed class ABuildRequestHandler(ProjectBuildResponse project) : IRequestHandler<ABuildRequest, ABuildResponse?>
{
    const string A = @"(?'A'\[(?!(\^|!))(?'A_CONTENT'.*?)\]\((?'A_HREF'.*?)(?'A_HREF_SUFIX'readme.md.*?|)\))";
    public async Task<ABuildResponse?> Handle(ABuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return new ABuildResponse
        {
            Target = Build(request.Source),
        };
    }

    private string? Build(string? source)
    {
        if (source == default)
            return source;

        var target = source;

        target = Regex.Replace(target, @$"({A})", (match) =>
        {
            var str = match.Groups["A_HREF"].Value;
            var href = new Uri(str);

            if (string.IsNullOrWhiteSpace(href.Host))
            {
                return $@"<a href=""{project.BaseUrl!.AbsoluteUri.TrimEnd('/')}/{href.LocalPath.TrimStart('/')}"">{match.Groups["A_CONTENT"].Value}</a>";
            }

            return $@"<a href=""{href}{match.Groups["A_HREF_SUFIX"].Value}"">{match.Groups["A_CONTENT"].Value}</a>";
        }, RegexOptions.Multiline | RegexOptions.IgnoreCase);

        return target;
    }
}
