using Microsoft.Extensions.DependencyInjection;
using MediatR;

var serviceCollection = new ServiceCollection();
serviceCollection
    .AddHttpClient()
    .AddTransient(typeof(IPipelineBehavior<,>), typeof(TimeElapsedPipelineBehavior<,>))
    .AddTransient(typeof(IStreamPipelineBehavior<,>), typeof(TimeElapsedStreamPipelineBehavior<,>))
    .AddMediatR(mediatorServiceConfiguration => mediatorServiceConfiguration.RegisterServicesFromAssemblyContaining<BuildRequest>());
var serviceProvider = serviceCollection.BuildServiceProvider();
var mediator = serviceProvider.GetRequiredService<IMediator>();
var sourcePath = Environment.GetCommandLineArgs()[1];
var targetPath = Environment.GetCommandLineArgs()[2];
var htmlFileName = Environment.GetCommandLineArgs()[3];
var repositoryOnwer = Environment.GetCommandLineArgs()[4];
var baseUrl = Environment.GetCommandLineArgs()[5];


if (sourcePath == default)
    return;
if (targetPath == default)
    return;

var title = (
    await mediator
        .Send(new GitHubRepositoryOwnerUserNameGetRequest
        {
            Name = repositoryOnwer
        },
        CancellationToken.None)
    ) ??
    repositoryOnwer;

var sourceDirectoryInfo = await mediator.Send(new DirectoryInfoGetRequest { Path = sourcePath });
await foreach (var source in mediator.CreateStream(new MarkdownFileInfoGetStreamRequest { DirectoryInfo = sourceDirectoryInfo }))
{
    var partial = source.FullName.Replace(source.Name, string.Empty).Replace(sourceDirectoryInfo.FullName, string.Empty);

    var target = await mediator
                .Send(new DirectoryInfoGetRequest
                {
                    Path = string.IsNullOrWhiteSpace(partial) ?
                        targetPath :
                        $"{targetPath}{Path.DirectorySeparatorChar}{partial}"
                });

    await mediator
        .Send(new MarkdownFileInfoBuildRequest
        {
            Title = title,
            Url = baseUrl,
            Source = source,
            Target = new FileInfo($"{target}{Path.DirectorySeparatorChar}{htmlFileName}"),
        });

}