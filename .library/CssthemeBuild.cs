using MediatR;
internal sealed class CssThemeBuildRequest : IRequest
{
    public DirectoryInfo? SourceDirectoryInfo { get; init; }
    public DirectoryInfo? TargetDirectoryInfo { get; init; }
}
internal sealed class CssThemeBuildHandler : IRequestHandler<CssThemeBuildRequest>
{
    public async Task Handle(CssThemeBuildRequest request, CancellationToken cancellationToken)
    {
        foreach (var item in request.SourceDirectoryInfo!.EnumerateFiles("*theme.css", new EnumerationOptions() { RecurseSubdirectories = true }))
        {
            if (!request.TargetDirectoryInfo!.Exists)
                request.TargetDirectoryInfo.Create();

            request.TargetDirectoryInfo.Refresh();
            item.CopyTo($"{request.TargetDirectoryInfo.FullName}{Path.DirectorySeparatorChar}{item.Name}");
        }

        await Task.Yield();
    }
}