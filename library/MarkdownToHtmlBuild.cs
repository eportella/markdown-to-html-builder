using MediatR;
public sealed class MarkdownToHtmlBuildRequest : IRequest
{
    public string? SourcePath { get; init; }
    public string? TargetPath { get; init; }
    public string? HtmlFileName { get; init; }
    public string? RepositoryOnwer { get; init; }
    public string? BaseUrl { get; init; }
}
internal sealed class MarkdownToHtmlBuildRequestHandler(IMediator mediator) : IRequestHandler<MarkdownToHtmlBuildRequest>
{

    public async Task Handle(MarkdownToHtmlBuildRequest request, CancellationToken cancellationToken)
    {
        if (request.SourcePath == default)
            return;
        if (request.TargetPath == default)
            return;
        if (request.HtmlFileName == default)
            return;

        var onwer = (
            await mediator
                .Send(new GitHubRepositoryOwnerUserNameGetRequest
                {
                    Name = request.RepositoryOnwer
                },
                CancellationToken.None)
            ) ??
            request.RepositoryOnwer;

        var sourceDirectoryInfo = await mediator.Send(new DirectoryInfoGetRequest { Path = request.SourcePath }, cancellationToken);
        await foreach (var source in mediator.CreateStream(new MarkdownFileInfoGetStreamRequest { DirectoryInfo = sourceDirectoryInfo }, cancellationToken))
        {
            var partial = source.FullName.Replace(source.Name, string.Empty).Replace(sourceDirectoryInfo.FullName, string.Empty);

            var target = await mediator
                        .Send(new DirectoryInfoGetRequest
                        {
                            Path = string.IsNullOrWhiteSpace(partial) ?
                                request.TargetPath :
                                $"{request.TargetPath}{Path.DirectorySeparatorChar}{partial}"
                        },
                         cancellationToken);

            await mediator
                .Send(new MarkdownFileInfoBuildRequest
                {
                    Title = onwer,
                    Url = request.BaseUrl,
                    Source = source,
                    Target = new FileInfo($"{target}{Path.DirectorySeparatorChar}{request.HtmlFileName}"),
                },
                cancellationToken);

        }
    }
}
