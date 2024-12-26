using MediatR;
public sealed class MarkdownToHtmlBuildRequest : IRequest, ITimmerElapsedLog
{
}
internal sealed class MarkdownToHtmlBuildRequestHandler(IMediator mediator, Task<InputBuildResponse> inputTask) : IRequestHandler<MarkdownToHtmlBuildRequest>
{
    public async Task Handle(MarkdownToHtmlBuildRequest request, CancellationToken cancellationToken)
    {
        var input = await inputTask;
        var sourceDirectoryInfo = await mediator
            .Send(new DirectoryInfoGetRequest
            {
                Path = input.SourcePath
            },
            cancellationToken);

        await mediator
            .Send(
                new CssThemeBuildRequest(),
                cancellationToken
            );

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
                    Source = source,
                    Target = new FileInfo($"{target}{Path.DirectorySeparatorChar}{input.TargetFileName}"),
                },
                cancellationToken);

        }
    }
}
