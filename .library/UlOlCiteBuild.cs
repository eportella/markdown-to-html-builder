using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using MediatR;
internal sealed class UlOlBuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed partial class UlOlBuildRequestHandler(IMediator mediator) : IRequestHandler<UlOlBuildRequest, string?>
{
    const string PATTERN = @"^(?'UL_OL'(((?'UL'-)|(?'OL'\d+\.)) *.+(\r?\n|))( *((-)|(\d+\.)) *.+(\r?\n|))*(\r?\n|))";
    const string UL_OL_INNER = @"^(((.+?\r?\n))(?'UL_OL'( *((-)|(\d+\.)) *.+(\r?\n|))*(\r?\n|)))";
    const string LI = @"^(-|\d+\.) *(?'LI'(.*(\r?\n|)+(?!(-|\d+\.)))+(\r?\n|))";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline)]
    private static partial Regex Regex();
    [GeneratedRegex(UL_OL_INNER, RegexOptions.Multiline)]
    private static partial Regex RegexOlUlInner();
    [GeneratedRegex(LI, RegexOptions.Multiline)]
    private static partial Regex RegexLi();
    [GeneratedRegex("^    ", RegexOptions.Multiline)]
    private static partial Regex RegexInner();
    public async Task<string?> Handle(UlOlBuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;

        return await Regex().ReplaceAsync(request.Source, async match =>
        {
            if (!string.IsNullOrWhiteSpace(match.Groups["UL"].Value))
                return $"<ul>{await RegexLi().ReplaceAsync(match.Value, async match => await Replace(match, cancellationToken))}</ul>{Environment.NewLine}";

            if (!string.IsNullOrWhiteSpace(match.Groups["OL"].Value))
                return $"<ol>{await RegexLi().ReplaceAsync(match.Value, async match => await Replace(match, cancellationToken))}</ol>{Environment.NewLine}";

            return match.Value;
        });
    }

    private async Task<string> Replace(Match match, CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(match.Groups["UL_OL"].Value))
            return (await mediator.Send(new UlOlBuildRequest { Source = match.Groups["UL_OL"].Value }, cancellationToken)) ?? string.Empty;

        if (!string.IsNullOrWhiteSpace(match.Groups["LI"].Value))
        {
            var children = Build(match.Groups["LI"].Value, cancellationToken)
                .ToBlockingEnumerable(cancellationToken);
            return $"<li>{string.Join(string.Empty, children)}</li>{Environment.NewLine}";
        }

        throw new InvalidOperationException($"build with {match.Value} invalid");
    }

    private async IAsyncEnumerable<string?> Build(string? source, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (source == default)
            yield break;

        foreach (Group group in RegexOlUlInner()
            .Matches(source)
            .Select(m => m.Groups["UL_OL"])
            .Where(g => g.Success && !string.IsNullOrWhiteSpace(g.Value)))
        {
            source = source
                .Replace(
                    group.Value,
                    await Regex()
                        .ReplaceAsync(
                            RegexInner()
                                .Replace(
                                    group.Value,
                                    string.Empty
                                ),
                                async match => await Replace(match, cancellationToken)
                        )
                );
        }

        yield return await mediator.Send(new InlineBuildRequest { Source = source }, cancellationToken);

    }
}
