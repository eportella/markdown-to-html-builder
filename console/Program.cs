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
var sourceDirectoryInfo = await mediator.Send(new RootDirectoryInfoGetRequest{ Path = Environment.GetCommandLineArgs()[1]});

await foreach (var markdownFileInfo in mediator.CreateStream(new MarkdownFileInfoGetStreamRequest { DirectoryInfo = sourceDirectoryInfo }))
    await mediator.Send(new BuildRequest 
    { 
        FileInfoSource = markdownFileInfo,
        FileInfoTarget = new FileInfo(markdownFileInfo!.FullName.Replace("/_jekyll/", "/_site/"))
    });

var rootDirectoryInfo = await mediator.Send(new RootDirectoryInfoGetRequest());
await foreach (var markdownFileInfo in mediator.CreateStream(new MarkdownFileInfoGetStreamRequest { DirectoryInfo = rootDirectoryInfo }))
{
    var content = await mediator.Send(new StringGetdRequest { FileInfo = markdownFileInfo});
        Console.WriteLine(content);
        Console.WriteLine();
}
