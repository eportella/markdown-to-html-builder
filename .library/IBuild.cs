using System.Text.RegularExpressions;
using MediatR;
internal sealed class IBuildRequest : IRequest<IBuildResponse?>
{
    internal string? Source { get; init; }
}
internal sealed class IBuildResponse
{
    internal string? Target { get; init; }
}
internal sealed partial class IBuildRequestHandler() : IRequestHandler<IBuildRequest, IBuildResponse?>
{
    const string PATTERN = @"(?'I'\*{1}(?'I_CONTENT'[^\*| ].+?)\*{1})";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline)]
    private static partial Regex Regex();
    public async Task<IBuildResponse?> Handle(IBuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;

        var target = await Regex().ReplaceAsync(
            request.Source,
            match => $"<i>{match.Groups["I_CONTENT"].Value}</i>");

        return new IBuildResponse
        {
            Target = target,
        };
    }
}
