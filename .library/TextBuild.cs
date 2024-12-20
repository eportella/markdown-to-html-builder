using MediatR;
internal sealed class TextBuildRequest : IRequest<TextBuildResponse?>
{
    internal string? Source { get; init; }
}
internal sealed class TextBuildResponse
{
    internal string? Target { get; init; }
}
internal sealed class TextBuildRequestHandler(IMediator mediator) : IRequestHandler<TextBuildRequest, TextBuildResponse?>
{
    public async Task<TextBuildResponse?> Handle(TextBuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;
        var target = request.Source;

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

        return new TextBuildResponse
        {
            Target = target
        };
    }
}
