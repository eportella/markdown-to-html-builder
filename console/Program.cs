using Microsoft.Extensions.DependencyInjection;
using MediatR;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("test")]

var serviceCollection = new ServiceCollection();
serviceCollection
    .AddTransient(typeof(IPipelineBehavior<,>), typeof(TimeElapsedPipelineBehavior<,>))
    .AddTransient(typeof(IStreamPipelineBehavior<,>), typeof(TimeElapsedStreamPipelineBehavior<,>))
    .AddMediatR(mediatorServiceConfiguration => mediatorServiceConfiguration.RegisterServicesFromAssemblyContaining<Program>());

var serviceProvider = serviceCollection.BuildServiceProvider();
var mediator = serviceProvider.GetRequiredService<IMediator>();
var sourceDirectoryInfo = await mediator.Send(new DirectoryInfoGetRequest { Path = Environment.GetCommandLineArgs()[1] });

if(sourceDirectoryInfo == default)
    return;

await foreach (var source in mediator.CreateStream(new MarkdownFileInfoGetStreamRequest { DirectoryInfo = sourceDirectoryInfo }))
{
    var partial = source.FullName.Replace(source.Name, string.Empty).Replace(sourceDirectoryInfo.FullName, string.Empty);

    var target = await mediator
                .Send(new DirectoryInfoGetRequest 
                { 
                    Path = string.IsNullOrWhiteSpace(partial) ? 
                        Environment.GetCommandLineArgs()[2] : 
                        $"{Environment.GetCommandLineArgs()[2]}{Path.DirectorySeparatorChar}{partial}" 
                });
    
    await mediator
        .Send(new MarkdownFileInfoBuildRequest
        {
            Source = source,
            Target = new FileInfo($"{target}{Path.DirectorySeparatorChar}{Environment.GetCommandLineArgs()[3]}"), 
        });
    
}
