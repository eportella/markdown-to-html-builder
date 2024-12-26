using System.Text.RegularExpressions;
using MediatR;
internal sealed class DelBuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed partial class DelBuildRequestHandler() : IRequestHandler<DelBuildRequest, string?>
{
    const string PATTERN = @"(?'DEL'\~{2}(?'DEL_CONTENT'[^\*| ].+?)\~{2})";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline)]
    private static partial Regex Regex();
    public async Task<string?> Handle(DelBuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;

        var target = await Regex().ReplaceAsync(
            request.Source,
            match => $"<del>{match.Groups["DEL_CONTENT"].Value}</del>");

        return target;
    }
}
