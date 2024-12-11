using MediatR;
using Microsoft.Extensions.DependencyInjection;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection ArgsAsInputAdd(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton(async serviceProvider => await serviceProvider
                .GetRequiredService<IMediator>()
                .Send(new InputBuildRequest
                {
                    Args = Environment.GetCommandLineArgs()
                },
                CancellationToken.None)
            );
    }
}