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
    Console.WriteLine($"SOURCE-PATH: {sourceDirectoryInfo.FullName}");
    Console.WriteLine($"MD-PATH: {markdownFileInfo.FullName}");
    var t = markdownFileInfo.FullName.Replace(markdownFileInfo.Name, string.Empty).Replace(sourceDirectoryInfo.FullName, string.Empty);
    Console.WriteLine($"PART-PATH: {markdownFileInfo.FullName}");
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
    Console.WriteLine($"HTML-PATH: {fileInfo.FullName}");
}
