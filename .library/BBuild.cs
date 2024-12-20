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
    const string B = @"(?'B'\*{2}(?'B_CONTENT'[^\*| ].+?)\*{2})";
    public async Task<BBuildResponse?> Handle(BBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return new BBuildResponse
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
            @$"({B})", (match) => $"<b>{match.Groups["B_CONTENT"].Value}</b>", 
            RegexOptions.Multiline);

        return target;
    }
}
