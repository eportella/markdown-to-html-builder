using MediatR;
internal sealed class MarkdownFileInfoBuildRequest : IRequest
{
    
    public FileInfo? Source { get; init; }
    public FileInfo? Target { get; init; }
}
internal sealed class MarkdownFileInfoBuildRequesttHandler(IMediator mediator) : IRequestHandler<MarkdownFileInfoBuildRequest>
{
    public async Task Handle(MarkdownFileInfoBuildRequest request, CancellationToken cancellationToken)
    {
        var built = (
            await mediator
                .Send(new BuildRequest
                {
                    Source = await mediator
                        .Send(new FileInfoTextReadRequest
                        {
                            FileInfo = request.Source
                        },
                        cancellationToken),
                },
                cancellationToken)
            );

        if (!request.Target!.Directory!.Exists)
            request.Target.Directory.Create();

        var fileInfo = request.Target;
        using var fileStrem = fileInfo!.CreateText();
        await fileStrem.WriteAsync(built);
    }
}