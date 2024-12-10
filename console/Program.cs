using Microsoft.Extensions.DependencyInjection;
using MediatR;

await new ServiceCollection()
    .AddHttpClient()
    .AddTransient(typeof(IPipelineBehavior<,>), typeof(TimeElapsedPipelineBehavior<,>))
    .AddTransient(typeof(IStreamPipelineBehavior<,>), typeof(TimeElapsedStreamPipelineBehavior<,>))
    .AddMediatR(mediatorServiceConfiguration => mediatorServiceConfiguration.RegisterServicesFromAssemblyContaining<MarkdownToHtmlBuildRequest>())
    .BuildServiceProvider()
    .GetRequiredService<IMediator>()
    .Send(new MarkdownToHtmlBuildRequest
    {
        Args = Environment.GetCommandLineArgs().Skip(1).ToArray(),
    }, CancellationToken.None);