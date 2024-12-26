using System.Text.RegularExpressions;
using MediatR;
internal sealed class BBuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed partial class BBuildRequestHandler() : IRequestHandler<BBuildRequest, string?>
{
    const string PATTERN = @"(?'B'\*{2}(?'B_CONTENT'[^\*| ].+?)\*{2})";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline)]
    private static partial Regex Regex();
    public async Task<string?> Handle(BBuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;

        var target = await Regex().ReplaceAsync(
            request.Source,
            match => $"<b>{match.Groups["B_CONTENT"].Value}</b>");

        return target;
    }
}
