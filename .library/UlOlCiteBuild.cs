using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using MediatR;
internal sealed class UlOlBuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed class UlOlBuildRequestHandler(IMediator mediator) : IRequestHandler<UlOlBuildRequest, string?>
{
    internal const string PATTERN = @"^(?'UL_OL'(((?'UL'-)|(?'OL'\d+\.)) *.+(\r?\n|))( *((-)|(\d+\.)) *.+(\r?\n|))*(\r?\n|))";
    const string UL_OL_INNER = @"^(((.+?\r?\n))(?'UL_OL'( *((-)|(\d+\.)) *.+(\r?\n|))*(\r?\n|)))";
    const string LI = @"^(-|\d+\.) *(?'LI'(.*(\r?\n|)+(?!(-|\d+\.)))+(\r?\n|))";
    static Regex Regex { get; }
    static Regex RegexOlUlInner { get; }
    static Regex RegexLi { get; }
    static UlOlBuildRequestHandler()
    {
        Regex = new Regex(PATTERN, RegexOptions.Multiline);
        RegexOlUlInner = new Regex(UL_OL_INNER, RegexOptions.Multiline);
        RegexLi = new Regex(LI, RegexOptions.Multiline);
    }
    public async Task<string?> Handle(UlOlBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return Regex.Replace(request.Source, match =>
        {
            if (!string.IsNullOrWhiteSpace(match.Groups["UL"].Value))
                return $"<ul>{RegexLi.Replace(match.Value, match => Replace(match, cancellationToken))}</ul>";

            if (!string.IsNullOrWhiteSpace(match.Groups["OL"].Value))
                return $"<ol>{RegexLi.Replace(match.Value, match => Replace(match, cancellationToken))}</ol>";

            return match.Value;
        });
    }

    private string Replace(Match match, CancellationToken cancellationToken)
    {

        {
            var content = match.Groups["UL_OL"].Value;
            if (!string.IsNullOrWhiteSpace(content))
            {
                return mediator.Send(new UlOlBuildRequest { Source = content }, cancellationToken).Result;
            }
        }

        {
            var content = match.Groups["LI"].Value;
            if (!string.IsNullOrWhiteSpace(content))
            {
                var children = Build(content, cancellationToken)
                    .ToBlockingEnumerable(cancellationToken);
                return $"<li>{children.Build()}</li>";
            }
        }

        
        throw new InvalidOperationException($"build with {match.Value} invalid");
    }

    private async IAsyncEnumerable<string?> Build(string? source, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (source == default)
            yield break;

        foreach (Group group in RegexOlUlInner
            .Matches(source)
            .Select(m => m.Groups["UL_OL"])
            .Where(g => g.Success && !string.IsNullOrWhiteSpace(g.Value)))
        {
            source = source
                .Replace(
                    group.Value,
                    Regex
                        .Replace(
                            Regex
                                .Replace(
                                    group.Value,
                                    "^    ",
                                    string.Empty,
                                    RegexOptions.Multiline
                                ),
                                match => Replace(match, cancellationToken)
                        )
                );
        }

        await foreach (var text in mediator.CreateStream(new InlineBuildRequest { Source = source }, cancellationToken))
            yield return text;
    }
}
