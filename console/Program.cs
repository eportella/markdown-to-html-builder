using Microsoft.Extensions.DependencyInjection;
using MediatR;
var serviceCollection = new ServiceCollection();
serviceCollection
    .AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingPipelineBehavior<,>))
    .AddTransient(typeof(IStreamPipelineBehavior<,>), typeof(StreamLoggingPipelineBehavior<,>))
    .AddMediatR(mediatorServiceConfiguration => mediatorServiceConfiguration.RegisterServicesFromAssemblyContaining<Program>());

var serviceProvider = serviceCollection.BuildServiceProvider();
var mediator = serviceProvider.GetRequiredService<IMediator>();
var jekyllDirectoryInfo = await mediator.Send(new JekyllDirectoryInfoGetRequest());
await foreach (var cssFileInfo in mediator.CreateStream(new CssFileGetStreamRequest { DirectoryInfo = jekyllDirectoryInfo }))
    await mediator.Send(new CssMoveRequest 
    { 
        FileInfoSource = cssFileInfo,
        FileInfoTarget = new FileInfo(cssFileInfo!.FullName.Replace("/_jekyll/", "/_site/"))
    });

await foreach (var htmlFileInfo in mediator.CreateStream(new HtmlFileGetStreamRequest { DirectoryInfo = jekyllDirectoryInfo }))
    await mediator.Send(new BuildRequest 
    { 
        FileInfoSource = htmlFileInfo,
        FileInfoTarget = new FileInfo(htmlFileInfo!.FullName.Replace("/_jekyll/", "/_site/"))
    });

var rootDirectoryInfo = await mediator.Send(new RootDirectoryInfoGetRequest());
await foreach (var markdownFileInfo in mediator.CreateStream(new MarkdownFileInfoGetStreamRequest { DirectoryInfo = rootDirectoryInfo }))
{
    if(markdownFileInfo.FullName.Contains("/_jekyll/") || markdownFileInfo.FullName.Contains("/_site/"))
        continue;
    await mediator.Send(new LogRequest 
    { 
        FileInfo = markdownFileInfo,
    });
}
