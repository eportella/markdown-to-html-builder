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
internal sealed partial class DelBuildRequestHandler() : IRequestHandler<DelBuildRequest, DelBuildResponse?>
{
    const string PATTERN = @"(?'DEL'\~{2}(?'DEL_CONTENT'[^\*| ].+?)\~{2})";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline)]
    private static partial Regex Regex();
    public async Task<DelBuildResponse?> Handle(DelBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        var target = Regex().Replace(
            request.Source,
            match => $"<del>{match.Groups["DEL_CONTENT"].Value}</del>");

        return new DelBuildResponse
        {
            Target = target,
        };
    }
}
