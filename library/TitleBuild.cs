using MediatR;
public sealed class TitleBuildRequest : IRequest<TitleBuildResponse>
{
    public string[]? Args { internal get; init; }
}
internal sealed class TitleBuildResponse
{
    public string? Value { get; init; }
}
internal sealed class TitleBuildRequestHandler(IMediator mediator, InputBuildResponse input) : IRequestHandler<TitleBuildRequest, TitleBuildResponse>
{
    public async Task<TitleBuildResponse> Handle(TitleBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();

        var project = input.BaseUrl!.AbsolutePath.TrimStart('/');
        var title = $"{(
                await mediator
                    .Send(new GitHubRepositoryOwnerUserNameGetRequest
                    {
                        Name = input.RepositoryOnwer
                    },
                    CancellationToken.None)
            ) ?? input.RepositoryOnwer}{(string.IsNullOrWhiteSpace(project) ? string.Empty : $" '{project}'")}";

        return new TitleBuildResponse
        {
            Value = title
        };
    }
}
