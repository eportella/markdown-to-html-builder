using MediatR;
internal sealed class HtmlBuildRequest : IRequest<string?>
{
    public string? Source { get; init; }
}

internal sealed class HtmlBuildRequestHandler(Task<ProjectBuildResponse> projectTask, IMediator mediator) : IRequestHandler<HtmlBuildRequest, string?>
{
    static HtmlBuildRequestHandler()
    {
    }
    public async Task<string?> Handle(HtmlBuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;

        var project = await projectTask;

        var bodyBuilt = @$"<body><h1><a href=""{project.BaseUrl}""/>{project.Title}</a></h1>{await mediator.Send(new BlockBuildRequest { Source = request.Source }, cancellationToken)}{(project.OwnerTitle != default && project.OwnerBaseUrl != default ? @$"<span class=""owner""><a href=""{project.OwnerBaseUrl}""/>{project.OwnerTitle}</a></span>" : string.Empty)}</body>";
        return $@"<!DOCTYPE html><html lang=""pt-BR""><head><title>{project.Title}</title><meta content=""text/html; charset=UTF-8;"" http-equiv=""Content-Type"" /><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""><meta name=""color-scheme"" content=""dark light""><link rel=""stylesheet"" href=""{project.BaseUrl!.ToString().TrimEnd('/')}/stylesheet.css""></style></head>{bodyBuilt}</html>";
   }
}
