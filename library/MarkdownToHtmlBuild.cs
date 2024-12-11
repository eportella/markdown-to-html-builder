using MediatR;
public sealed class MarkdownToHtmlBuildRequest : IRequest
{
}
internal sealed class MarkdownToHtmlBuildRequestHandler(IMediator mediator, InputBuildResponse input) : IRequestHandler<MarkdownToHtmlBuildRequest>
{
    public async Task Handle(MarkdownToHtmlBuildRequest request, CancellationToken cancellationToken)
    {
        var project = input.BaseUrl!.AbsolutePath.TrimStart('/');
        var title = $"{(
                await mediator
                    .Send(new GitHubRepositoryOwnerUserNameGetRequest
                    {
                        Name = input.RepositoryOnwer
                    },
                    CancellationToken.None)
            ) ?? input.RepositoryOnwer}{(string.IsNullOrWhiteSpace(project) ? string.Empty : $" '{project}'")}"; 
            
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
