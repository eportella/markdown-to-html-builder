using MediatR;
public sealed class ProjectBuildRequest : IRequest<ProjectBuildResponse>, ITimmerElapsedLog
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
internal sealed class ProjectBuildRequestHandler(IMediator mediator, Task<InputBuildResponse> inputTask) : IRequestHandler<ProjectBuildRequest, ProjectBuildResponse>
{
    public async Task<ProjectBuildResponse> Handle(ProjectBuildRequest request, CancellationToken cancellationToken)
    {
        var input = await inputTask;
        var project = input.BaseUrl!.AbsolutePath.TrimStart('/');
        var ownerTitle = (
                await mediator
                    .Send(new GitHubRepositoryOwnerUserNameGetRequest
                    {
                        Name = input.RepositoryOnwer
                    },
                    CancellationToken.None)
            ) ?? input.RepositoryOnwer;

        return new ProjectBuildResponse
        {
            Title = string.IsNullOrWhiteSpace(project) ? ownerTitle : project,
            BaseUrl = input.BaseUrl,
            OwnerTitle = string.IsNullOrWhiteSpace(project) ? default : ownerTitle,
            OwnerBaseUrl = string.IsNullOrWhiteSpace(project) ? default : new Uri(input.BaseUrl!.GetLeftPart(UriPartial.Authority)),
        };
    }
}
