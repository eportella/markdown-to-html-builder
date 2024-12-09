using MediatR;
public sealed class DirectoryInfoGetRequest : IRequest<DirectoryInfo?>
{
    public string? Path { get; set; }
}
internal sealed class DirectoryInfoGetRequestHandler : IRequestHandler<DirectoryInfoGetRequest, DirectoryInfo?>
{
    public async Task<DirectoryInfo?> Handle(DirectoryInfoGetRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        return new DirectoryInfo(request.Path!);
    }
}