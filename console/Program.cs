using Microsoft.Extensions.DependencyInjection;
using MediatR;

await new ServiceCollection()
    .AddHttpClient()
    .AddTransient(typeof(IPipelineBehavior<,>), typeof(TimeElapsedPipelineBehavior<,>))
    .AddTransient(typeof(IStreamPipelineBehavior<,>), typeof(TimeElapsedStreamPipelineBehavior<,>))
    .AddMediatR(mediatorServiceConfiguration => mediatorServiceConfiguration.RegisterServicesFromAssemblyContaining<BuildRequest>())
    .BuildServiceProvider()
    .GetRequiredService<IMediator>()
    .Send(new MarkdownToHtmlBuildRequest
    {
        SourcePath = Environment.GetCommandLineArgs()[1],
        TargetPath = Environment.GetCommandLineArgs()[2],
        HtmlFileName = Environment.GetCommandLineArgs()[3],
        RepositoryOnwer = Environment.GetCommandLineArgs()[4],
        BaseUrl = Environment.GetCommandLineArgs()[5],
    }, CancellationToken.None);