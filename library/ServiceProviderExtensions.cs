using Microsoft.Extensions.DependencyInjection;
public static class IServiceProviderExtensions
{
    public static IServiceCollection ArgsConfigure(this IServiceCollection serviceCollection)
    {
        return serviceCollection
            .AddSingleton(sp =>
            {
                return new InputBuildResponse
                {
                    SourcePath = default,
                    TargetPath = default,
                    TargetFileName = default,
                    RepositoryOnwer = default,
                    BaseUrl = default,
                };
            });
    }
}