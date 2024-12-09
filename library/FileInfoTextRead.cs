using MediatR;
internal sealed class FileInfoTextReadRequest : IRequest<string?>
{
    public FileInfo? FileInfo { get; init; }
}
internal sealed class FileInfoTextReadRequestHandler : IRequestHandler<FileInfoTextReadRequest, string?>
{
    public async Task<string?> Handle(FileInfoTextReadRequest request, CancellationToken cancellationToken)
    {
        using var reader = request.FileInfo!.OpenText();
        return await reader.ReadToEndAsync();
    }
}
