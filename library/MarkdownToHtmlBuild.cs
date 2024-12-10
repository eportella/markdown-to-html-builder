using MediatR;
public sealed class MarkdownToHtmlBuildRequest : IRequest
{
    public string[]? Args { internal get; init; }
}
internal sealed class MarkdownToHtmlBuildRequestHandler(IMediator mediator) : IRequestHandler<MarkdownToHtmlBuildRequest>
{

    public async Task Handle(MarkdownToHtmlBuildRequest request, CancellationToken cancellationToken)
    {
        var input = await mediator
            .Send(new InputBuildRequest
            {
                Args = request.Args
            },
            cancellationToken);

        var title = input.BaseUrl!.AbsolutePath == "/" ?
            (
                await mediator
                    .Send(new GitHubRepositoryOwnerUserNameGetRequest
                    {
                        Name = input.RepositoryOnwer
                    },
                    CancellationToken.None)
            ) ?? input.RepositoryOnwer :
            input.BaseUrl!.AbsolutePath.TrimStart('/');

        var sourceDirectoryInfo = await mediator
            .Send(new DirectoryInfoGetRequest
            {
                Path = input.SourcePath
            },
            cancellationToken);

        await foreach (var source in mediator
            .CreateStream(new MarkdownFileInfoGetStreamRequest
            {
                DirectoryInfo = sourceDirectoryInfo
            },
            cancellationToken)
        )
        {
            var sufix = source.FullName
                .Replace(source.Name, string.Empty)
                .Replace(sourceDirectoryInfo.FullName, string.Empty);

            var target = await mediator
                        .Send(new DirectoryInfoGetRequest
                        {
                            Path = string.IsNullOrWhiteSpace(sufix) ?
                                input.TargetPath :
                                $"{input.TargetPath}{Path.DirectorySeparatorChar}{sufix}"
                        },
                         cancellationToken);

            await mediator
                .Send(new MarkdownFileInfoBuildRequest
                {
                    Title = title,
                    Url = input.BaseUrl,
                    Source = source,
                    Target = new FileInfo($"{target}{Path.DirectorySeparatorChar}{input.TargetFileName}"),
                },
                cancellationToken);

        }
    }
}
