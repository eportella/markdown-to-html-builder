using System.Text.RegularExpressions;
using MediatR;
internal sealed class BIBuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed partial class BIBuildRequestHandler() : IRequestHandler<BIBuildRequest, string?>
{
    const string PATTERN = @"(?'BI'\*{3}(?'BI_CONTENT'[^\*| ].+?)\*{3})";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline)]
    private static partial Regex Regex();
    public async Task<string?> Handle(BIBuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;
        
        var target = await Regex().ReplaceAsync(
            request.Source,
            match => $"<b><i>{match.Groups["BI_CONTENT"].Value}</i></b>");

        return target;
    }
}
