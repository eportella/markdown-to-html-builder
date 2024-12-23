using System.Text.RegularExpressions;
using MediatR;
internal sealed class BBuildRequest : IRequest<BBuildResponse?>
{
    internal string? Source { get; init; }
}
internal sealed class BBuildResponse
{
    internal string? Target { get; init; }
}
internal sealed class BBuildRequestHandler() : IRequestHandler<BBuildRequest, BBuildResponse?>
{
    const string PATTERN = @"(?'B'\*{2}(?'B_CONTENT'[^\*| ].+?)\*{2})";
    static Regex Regex { get; }
    static BBuildRequestHandler()
    {
        Regex = new Regex(PATTERN, RegexOptions.Multiline);
    }
    public async Task<BBuildResponse?> Handle(BBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        var target = Regex.Replace(
            request.Source,
            match => $"<b>{match.Groups["B_CONTENT"].Value}</b>");

        return new BBuildResponse
        {
            Target = target,
        };
    }
}
