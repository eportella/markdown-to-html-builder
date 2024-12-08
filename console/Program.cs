using Microsoft.Extensions.DependencyInjection;
using MediatR;
using System.Runtime.CompilerServices;
using System.Net.Http.Headers;
using System.Net.Http.Json;
[assembly: InternalsVisibleTo("test")]

var serviceCollection = new ServiceCollection();
serviceCollection
    .AddHttpClient()
    .AddTransient(typeof(IPipelineBehavior<,>), typeof(TimeElapsedPipelineBehavior<,>))
    .AddTransient(typeof(IStreamPipelineBehavior<,>), typeof(TimeElapsedStreamPipelineBehavior<,>))
    .AddMediatR(mediatorServiceConfiguration => mediatorServiceConfiguration.RegisterServicesFromAssemblyContaining<Program>());
var serviceProvider = serviceCollection.BuildServiceProvider();
var mediator = serviceProvider.GetRequiredService<IMediator>();
var sourceDirectoryInfo = await mediator.Send(new DirectoryInfoGetRequest { Path = Environment.GetCommandLineArgs()[1] });

if (sourceDirectoryInfo == default)
    return;

using var client = serviceProvider
    .GetRequiredService<IHttpClientFactory>()
    .CreateClient();
client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("repository-owner", Environment.GetCommandLineArgs()[4]));

var owner = (
    await client
        .GetFromJsonAsync<User>(
            $"https://api.github.com/users/{Environment.GetCommandLineArgs()[4]}",
            CancellationToken.None
        )
    )?
    .Name ?? Environment.GetCommandLineArgs()[4];

await foreach (var source in mediator.CreateStream(new MarkdownFileInfoGetStreamRequest { DirectoryInfo = sourceDirectoryInfo }))
{
    var partial = source.FullName.Replace(source.Name, string.Empty).Replace(sourceDirectoryInfo.FullName, string.Empty);

    var target = await mediator
                .Send(new DirectoryInfoGetRequest
                {
                    Path = string.IsNullOrWhiteSpace(partial) ?
                        Environment.GetCommandLineArgs()[2] :
                        $"{Environment.GetCommandLineArgs()[2]}{Path.DirectorySeparatorChar}{partial}"
                });

    await mediator
        .Send(new MarkdownFileInfoBuildRequest
        {
            Title = owner,
            Url = Environment.GetCommandLineArgs()[5],
            Source = source,
            Target = new FileInfo($"{target}{Path.DirectorySeparatorChar}{Environment.GetCommandLineArgs()[3]}"),
        });

}

internal class User
{
    public string? Name { get; set; }
}