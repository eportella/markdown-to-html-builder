using System.Text.RegularExpressions;
using MediatR;
internal sealed class BlockBuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed class BlockBuildRequestHandler(IMediator mediator) : IRequestHandler<BlockBuildRequest, string?>
{
    static Regex RegexBlock { get; }

    static BlockBuildRequestHandler()
    {
        RegexBlock = new Regex(@$"({PBuildRequestHandler.PATTERN}|{H1BuildRequestHandler.PATTERN}|{H2BuildRequestHandler.PATTERN}|{H3BuildRequestHandler.PATTERN}|{H4BuildRequestHandler.PATTERN}|{H5BuildRequestHandler.PATTERN}|{H6BuildRequestHandler.PATTERN}|{BlockquoteBuildRequestHandler.PATTERN}|{UlOlBuildRequestHandler.PATTERN}|{CiteBuildRequestHandler.PATTERN})", RegexOptions.Multiline);
    }

    public async Task<string?> Handle(BlockBuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;

        var target = request.Source;

        target = await mediator.Send(new PBuildRequest { Source = target }, cancellationToken);
        target = await mediator.Send(new H1BuildRequest { Source = target }, cancellationToken);
        target = await mediator.Send(new H2BuildRequest { Source = target }, cancellationToken);
        target = await mediator.Send(new H3BuildRequest { Source = target }, cancellationToken);
        target = await mediator.Send(new H4BuildRequest { Source = target }, cancellationToken);
        target = await mediator.Send(new H5BuildRequest { Source = target }, cancellationToken);
        target = await mediator.Send(new H6BuildRequest { Source = target }, cancellationToken);
        target = await mediator.Send(new BlockquoteBuildRequest { Source = target }, cancellationToken);
        target = await mediator.Send(new UlOlBuildRequest { Source = target }, cancellationToken);
        target = await mediator.Send(new CiteBuildRequest { Source = target }, cancellationToken);
        target = await mediator.Send(new InlineBuildRequest { Source = target }, cancellationToken);

        return target;
    }
}
