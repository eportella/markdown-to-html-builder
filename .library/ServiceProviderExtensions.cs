using MediatR;
using Microsoft.Extensions.DependencyInjection;
public static class IServiceCollectionExtensions
{
    public static IServiceCollection ArgsAsInputAdd(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton(serviceProvider => serviceProvider
                .GetRequiredService<IMediator>()
                .Send(
                    new InputBuildRequest
                    {
                        Args = Environment.GetCommandLineArgs()
                    },
                    CancellationToken.None
                )
                .Result
            );
    }
    public static IServiceCollection TitleAdd(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton(serviceProvider => serviceProvider
                .GetRequiredService<IMediator>()
                .Send(
                    new ProjectBuildRequest
                    {
                        Args = Environment.GetCommandLineArgs()
                    },
                    CancellationToken.None
                )
                .Result
            );
    }
}
