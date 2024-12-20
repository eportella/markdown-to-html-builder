using System.Text.RegularExpressions;
using MediatR;
internal sealed class BIBuildRequest : IRequest<BIBuildResponse?>
{
    internal string? Source { get; init; }
}
internal sealed class BIBuildResponse
{
    internal string? Target { get; init; }
}
internal sealed class BIBuildRequestHandler() : IRequestHandler<BIBuildRequest, BIBuildResponse?>
{
    const string PATTERN = @"(?'BI'\*{3}(?'BI_CONTENT'[^\*| ].+?)\*{3})";
    static Regex Regex { get; }
    static BIBuildRequestHandler()
    {
        Regex = new Regex(PATTERN, RegexOptions.Multiline);
    }
    public async Task<BIBuildResponse?> Handle(BIBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return new BIBuildResponse
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
            match => $"<b><i>{match.Groups["BI_CONTENT"].Value}</i></b>");
    }
}
