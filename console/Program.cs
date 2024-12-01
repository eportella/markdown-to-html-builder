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

await foreach (var markdownFileInfo in mediator.CreateStream(new MarkdownFileInfoGetStreamRequest { DirectoryInfo = sourceDirectoryInfo }))
{
    var content = await mediator.Send(new StringGetdRequest { FileInfo = markdownFileInfo });
    content = await mediator.Send(new HtmlStringBuildRequest { String = content });
    
    var t = sourceDirectoryInfo.FullName.Replace(sourceDirectoryInfo.Name, string.Empty).Replace(Environment.GetCommandLineArgs()[1], string.Empty);
    var targetDirectoryInfo = await mediator
        .Send(new DirectoryInfoGetRequest 
        { 
            Path = string.IsNullOrWhiteSpace(t) ? 
                Environment.GetCommandLineArgs()[2] : 
                $"{Environment.GetCommandLineArgs()[2]}{Path.PathSeparator}{t}" 
        });

    if (!targetDirectoryInfo!.Exists)
        targetDirectoryInfo.Create();
    var fileInfo = new FileInfo($"{targetDirectoryInfo}{Path.DirectorySeparatorChar}{Environment.GetCommandLineArgs()[3]}");
    using var fileStrem = fileInfo!.CreateText();
    await fileStrem.WriteAsync(content);
    Console.WriteLine(fileInfo.FullName);
}
