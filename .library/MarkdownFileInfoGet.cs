using System.Runtime.CompilerServices;
using MediatR;
internal sealed class MarkdownFileInfoGetStreamRequest : IStreamRequest<FileInfo>
{
    public DirectoryInfo? DirectoryInfo { get; init; }
}
internal sealed class MarkdownFileInfoGetStreamHandler : IStreamRequestHandler<MarkdownFileInfoGetStreamRequest, FileInfo>
{
    public async IAsyncEnumerable<FileInfo> Handle(MarkdownFileInfoGetStreamRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await Task.Yield();
        
        foreach (var item in request.DirectoryInfo!.EnumerateFiles("*.md", new EnumerationOptions() { RecurseSubdirectories = true }))
            yield return item;

    }
}