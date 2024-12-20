using System.Text.RegularExpressions;
using MediatR;
internal sealed class DelBuildRequest : IRequest<DelBuildResponse?>
{
    internal string? Source { get; init; }
}
internal sealed class DelBuildResponse
{
    internal string? Target { get; init; }
}
internal sealed class DelBuildRequestHandler() : IRequestHandler<DelBuildRequest, DelBuildResponse?>
{
    const string DEL = @"(?'DEL'\~{2}(?'DEL_CONTENT'[^\*| ].+?)\~{2})";
    public async Task<DelBuildResponse?> Handle(DelBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return new DelBuildResponse
        {
            Target = Build(request.Source),
        };
    }

    private string? Build(string? source)
    {
        if (source == default)
            return source;

        var target = source;

        target = Regex.Replace(
            target, 
            @$"({DEL})", 
            (match) =>
            {
                return $"<del>{match.Groups["DEL_CONTENT"].Value}</del>";
            }, 
            RegexOptions.Multiline);

        return target;
    }
}
