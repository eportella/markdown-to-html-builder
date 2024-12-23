using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using MediatR;
internal sealed class InlineBuildRequest : IStreamRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed class InlineVerticalBuildRequestHandler(IMediator mediator) : IStreamRequestHandler<InlineBuildRequest, string?>
{
    const string PATTERN = @"^(?'INLINE'((.*(\r?\n|))*))";
    static Regex Regex { get; }

    static InlineVerticalBuildRequestHandler()
    {
        Regex = new Regex(PATTERN, RegexOptions.Multiline);
    }

    public async IAsyncEnumerable<string?> Handle(InlineBuildRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (request.Source == default)
            yield break;

        foreach (Match match in Regex.Matches(request.Source))
        {
            var target = match.Groups["INLINE"].Value;
            if (!string.IsNullOrWhiteSpace(target))
            {
                target = (await mediator.Send(new BrBuildRequest { Source = target }, cancellationToken))?.Target;
                target = (await mediator.Send(new BIBuildRequest { Source = target }, cancellationToken))?.Target;
                target = (await mediator.Send(new BBuildRequest { Source = target }, cancellationToken))?.Target;
                target = (await mediator.Send(new IBuildRequest { Source = target }, cancellationToken))?.Target;
                target = (await mediator.Send(new DelBuildRequest { Source = target }, cancellationToken))?.Target;
                target = (await mediator.Send(new AgeCalcBuildRequest { Source = target }, cancellationToken))?.Target;
                target = (await mediator.Send(new ABuildRequest { Source = target }, cancellationToken))?.Target;
                target = (await mediator.Send(new SvgBuildRequest { Source = target }, cancellationToken))?.Target;
                target = (await mediator.Send(new CitedBuildRequest { Source = target }, cancellationToken))?.Target;
                target = (await mediator.Send(new ThemeBuildRequest { Source = target }, cancellationToken))?.Target;

                yield return target;
                continue;
            }
            //throw new InvalidOperationException($"build with {target} invalid");
        }
    }
}
