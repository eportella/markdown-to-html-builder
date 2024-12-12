using MediatR;
internal sealed class CssThemeBuildRequest : IRequest
{
}
internal sealed class CssThemeBuildHandler(IMediator mediator, InputBuildResponse input) : IRequestHandler<CssThemeBuildRequest>
{
    public async Task Handle(CssThemeBuildRequest request, CancellationToken cancellationToken)
    {
        Console.WriteLine("-> theme.css entry!");
        var sourceDirectoryInfo = await mediator
            .Send(new DirectoryInfoGetRequest
            {
                Path = input.SourcePath
            },
            cancellationToken);

        var targetDirectoryInfo = await mediator
            .Send(new DirectoryInfoGetRequest
            {
                Path = input.TargetPath
            },
            cancellationToken);

        foreach (var item in sourceDirectoryInfo!.EnumerateFiles("*theme.css", new EnumerationOptions() { RecurseSubdirectories = true }))
        {
            Console.WriteLine("-> theme.css iterate!");
            if (!targetDirectoryInfo!.Exists)
                targetDirectoryInfo.Create();

            targetDirectoryInfo.Refresh();
            item.CopyTo($"{targetDirectoryInfo.FullName}{Path.DirectorySeparatorChar}{item.Name}");
        }

        await Task.Yield();
        Console.WriteLine("-> theme.css out!");
    }
}