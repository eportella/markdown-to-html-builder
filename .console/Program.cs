using Microsoft.Extensions.DependencyInjection;
using MediatR;

await new ServiceCollection()
    .AddHttpClient()
    .AddTransient<IPipelineBehavior<MarkdownToHtmlBuildRequest, object>, TimeElapsedPipelineBehavior<object>>()
    .AddTransient<IStreamPipelineBehavior<MarkdownToHtmlBuildRequest, object>, TimeElapsedStreamPipelineBehavior<object>>()
    .AddMediatR(mediatorServiceConfiguration => mediatorServiceConfiguration.RegisterServicesFromAssemblyContaining<MarkdownToHtmlBuildRequest>())
    .ArgsAsInputAdd()
    .TitleAdd()
    .BuildServiceProvider()
    .GetRequiredService<IMediator>()
    .Send(new MarkdownToHtmlBuildRequest(), CancellationToken.None);