using MediatR;
internal sealed class TextBuildRequest : IRequest<Text>
{
    public IElement? Parent { get; init; }
    internal string? Source { get; init; }
}
internal sealed class TextBuildRequestHandler(IMediator mediator) : IRequestHandler<TextBuildRequest, Text>
{
    public async Task<Text> Handle(TextBuildRequest request, CancellationToken cancellationToken)
    {
        var built = request.Source;

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
        
        return new Text
        {
            Source = request.Source,
            Parent = request.Parent,
            Children = default,
            Built = built,
        };
    }
}
