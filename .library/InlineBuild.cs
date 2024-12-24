using System.Text.RegularExpressions;
using MediatR;
internal sealed class InlineBuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed partial class InlineBuildRequestHandler(IMediator mediator) : IRequestHandler<InlineBuildRequest, string?>
{
    const string PATTERN = @"^(?'INLINE'((.*(\r?\n|))*))";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline)]
    private static partial Regex Regex();
    public async Task<string?> Handle(InlineBuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;

        return await Regex().ReplaceAsync(request.Source, async match =>
        {
            var target = match.Groups["INLINE"].Value;

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

            return target ?? string.Empty;
        });

    }
}
