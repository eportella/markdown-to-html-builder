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
internal sealed class IBuildRequestHandler() : IRequestHandler<IBuildRequest, IBuildResponse?>
{
    const string PATTERN = @"(?'I'\*{1}(?'I_CONTENT'[^\*| ].+?)\*{1})";
    public async Task<IBuildResponse?> Handle(IBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return new IBuildResponse
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
            $"({PATTERN})",
            match => $"<i>{match.Groups["I_CONTENT"].Value}</i>",
            RegexOptions.Multiline);
    }
}
