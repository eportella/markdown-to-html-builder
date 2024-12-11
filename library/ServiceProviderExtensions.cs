using System;
using System.Runtime.CompilerServices;
using MediatR;
public static class IServiceProviderExtensions
{
    public static IServiceProvider ArgsConfigure(this IServiceProvider serviceProvider)
    {
        return serviceProvider
            .AddSingleton<InputBuildResponse>(serviceProvider=>{
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