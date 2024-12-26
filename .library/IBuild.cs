using System.Text.RegularExpressions;
using MediatR;
internal sealed class IBuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed partial class IBuildRequestHandler() : IRequestHandler<IBuildRequest, string?>
{
    const string PATTERN = @"(?'I'\*{1}(?'I_CONTENT'[^\*| ].+?)\*{1})";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline)]
    private static partial Regex Regex();
    public async Task<string?> Handle(IBuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;

        var target = await Regex().ReplaceAsync(
            request.Source,
            match => $"<i>{match.Groups["I_CONTENT"].Value}</i>");

        return target;
    }
}
