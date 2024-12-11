using Microsoft.Extensions.DependencyInjection;
using MediatR;

await new ServiceCollection()
    .AddHttpClient()
    .AddTransient(typeof(IPipelineBehavior<,>), typeof(TimeElapsedPipelineBehavior<,>))
    .AddTransient(typeof(IStreamPipelineBehavior<,>), typeof(TimeElapsedStreamPipelineBehavior<,>))
    .AddMediatR(mediatorServiceConfiguration => mediatorServiceConfiguration.RegisterServicesFromAssemblyContaining<MarkdownToHtmlBuildRequest>())
    .AddSingleton<InputBuildResponse>((serviceProvider)=>{
        return new InputBuildResponse
        {
            SourcePath = default,
            TargetPath = default,
            TargetFileName = default,
            RepositoryOnwer = default,
            BaseUrl = default,
        };
    })
    .BuildServiceProvider()
    .GetRequiredService<IMediator>()
    .Send(new MarkdownToHtmlBuildRequest
    {
        Args = Environment.GetCommandLineArgs(),
    }, CancellationToken.None);