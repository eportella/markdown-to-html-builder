using MediatR;
internal sealed class MarkdownFileInfoBuildRequest : IRequest
{
    public string? Title { get; init; }
    public string? Url { get; init; }
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
                    Title = request.Title,
                    Url = request.Url,
                    Source = await mediator
                        .Send(new FileInfoTextReadRequest
                        {
                            FileInfo = request.Source
                        },
                        cancellationToken),
                },
                cancellationToken)
            )?
            .Target?
            .Built;

        if (!request.Target!.Directory!.Exists)
            request.Target.Directory.Create();

        var fileInfo = request.Target;
        using var fileStrem = fileInfo!.CreateText();
        await fileStrem.WriteAsync(built);
    }
}