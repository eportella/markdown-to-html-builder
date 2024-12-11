using MediatR;
public sealed class TitleBuildRequest : IRequest<TitleInputBuildResponse>
{
    public string[]? Args { internal get; init; }
}
internal sealed class TitleInputBuildResponse
{
    public string? Value { get; init; }
}
internal sealed class TitleBuildRequestHandler(IMediator mediator, InputBuildResponse input) : IRequestHandler<TitleBuildRequest, TitleInputBuildResponse>
{
    public async Task<TitleInputBuildResponse> Handle(TitleBuildRequest request, CancellationToken cancellationToken)
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

        return new TitleInputBuildResponse
        {
            Value = title
        };
    }
}
