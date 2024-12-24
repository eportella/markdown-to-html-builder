using System.Runtime.CompilerServices;
using MediatR;
internal sealed class HtmlFileGetStreamRequest : IStreamRequest<FileInfo>
{
    public DirectoryInfo? DirectoryInfo { get; init; }
}
internal sealed class HtmlFileGetStreamHandler : IStreamRequestHandler<HtmlFileGetStreamRequest, FileInfo>
{
    public async IAsyncEnumerable<FileInfo> Handle(HtmlFileGetStreamRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        await Task.Yield();

        foreach (var item in request.DirectoryInfo!.EnumerateFiles("*.html", new EnumerationOptions() { RecurseSubdirectories = true }))
            yield return item;
    }
}