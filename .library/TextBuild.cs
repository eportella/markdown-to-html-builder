using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using MediatR;
internal sealed class InlineBuildRequest : IStreamRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed class InlineVerticalBuildRequestHandler(IMediator mediator) : IStreamRequestHandler<InlineBuildRequest, string?>
{
    const string PATTERN = @"^(?'TEXT'((.*(\r?\n|))*))";
    static Regex RegexText { get; }

    static InlineVerticalBuildRequestHandler()
    {
        RegexText = new Regex(PATTERN, RegexOptions.Multiline);
    }

    public async IAsyncEnumerable<string?> Handle(InlineBuildRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (request.Source == default)
            yield break;

        foreach (Match match in RegexText.Matches(request.Source))
        {
            var content = match.Groups["TEXT"].Value;
            if (!string.IsNullOrWhiteSpace(content))
            {
                var built = content;

                built = (await mediator.Send(new BrBuildRequest { Source = built }, cancellationToken))?.Target;
                built = (await mediator.Send(new BIBuildRequest { Source = built }, cancellationToken))?.Target;
                built = (await mediator.Send(new BBuildRequest { Source = built }, cancellationToken))?.Target;
                built = (await mediator.Send(new IBuildRequest { Source = built }, cancellationToken))?.Target;
                built = (await mediator.Send(new DelBuildRequest { Source = built }, cancellationToken))?.Target;
                built = (await mediator.Send(new AgeCalcBuildRequest { Source = built }, cancellationToken))?.Target;
                built = (await mediator.Send(new ABuildRequest { Source = built }, cancellationToken))?.Target;
                built = (await mediator.Send(new SvgBuildRequest { Source = built }, cancellationToken))?.Target;
                built = (await mediator.Send(new CitedBuildRequest { Source = built }, cancellationToken))?.Target;
                built = (await mediator.Send(new ThemeBuildRequest { Source = built }, cancellationToken))?.Target;

                yield return built;
                continue;
            }
            var debug = string.Empty;
        }
    }
}
