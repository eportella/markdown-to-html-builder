using MediatR;
public sealed class ProjectBuildRequest : IRequest<ProjectBuildResponse>
{
    public string[]? Args { internal get; init; }
}
internal sealed class ProjectBuildResponse
{
    public string? Title { get; init; }
    public Uri? BaseUrl { get; init; }
    public string? OwnerTitle { get; init; }
    public Uri? OwnerBaseUrl { get; init; }
}
internal sealed class ProjectBuildRequestHandler(IMediator mediator, Task<InputBuildResponse> input) : IRequestHandler<ProjectBuildRequest, ProjectBuildResponse>
{
    public async Task<ProjectBuildResponse> Handle(ProjectBuildRequest request, CancellationToken cancellationToken)
    {
        var project = (await input).BaseUrl!.AbsolutePath.TrimStart('/');
        var ownerTitle = (
                await mediator
                    .Send(new GitHubRepositoryOwnerUserNameGetRequest
                    {
                        Name = (await input).RepositoryOnwer
                    },
                    CancellationToken.None)
            ) ?? (await input).RepositoryOnwer;

        return new ProjectBuildResponse
        {
            Title = string.IsNullOrWhiteSpace(project) ? ownerTitle : project,
            BaseUrl = (await input).BaseUrl,
            OwnerTitle = string.IsNullOrWhiteSpace(project) ? default : ownerTitle,
            OwnerBaseUrl = string.IsNullOrWhiteSpace(project) ? default : new Uri((await input).BaseUrl!.GetLeftPart(UriPartial.Authority)),
        };
    }
}
