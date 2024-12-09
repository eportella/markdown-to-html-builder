using System.Net.Http.Headers;
using System.Net.Http.Json;
using MediatR;
internal sealed class GitHubRepositoryOwnerUserNameGetRequest : IRequest<string?>
{
    public string? Name { get; set; }
}
internal sealed class GitHubRepositoryOwnerUserNameGetRequestHandler(IHttpClientFactory factory) : IRequestHandler<GitHubRepositoryOwnerUserNameGetRequest, string?>
{

    public async Task<string?> Handle(GitHubRepositoryOwnerUserNameGetRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
            return request.Name;
            
        using var client = factory.CreateClient();
        client.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("repository-owner", request.Name));

        return (
            await client
                .GetFromJsonAsync<User>(
                    $"https://api.github.com/users/{request.Name}",
                    CancellationToken.None
                )
            )?
            .Name;
    }

    internal class User
    {
        public string? Name { get; set; }
    }
}