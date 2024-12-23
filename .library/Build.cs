using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using MediatR;
internal sealed class BuildRequest : IRequest<string?>
{
    public string? Source { get; init; }
}

internal sealed class BuildRequestHandler(ProjectBuildResponse project, IMediator mediator) : IRequestHandler<BuildRequest, string?>
{
    static BuildRequestHandler()
    {
    }
    public async Task<string?> Handle(BuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;

        var replaced = await mediator.Send(new BlockBuildRequest { Source = request.Source }, cancellationToken);

        var bodyBuilt = @$"<body><h1><a href=""{project.BaseUrl}""/>{project.Title}</a></h1>{replaced}{(project.OwnerTitle != default && project.OwnerBaseUrl != default ? @$"<span class=""owner""><a href=""{project.OwnerBaseUrl}""/>{project.OwnerTitle}</a></span>" : string.Empty)}</body>";
        return $@"<!DOCTYPE html><html lang=""pt-BR""><head><title>{project.Title}</title><meta content=""text/html; charset=UTF-8;"" http-equiv=""Content-Type"" /><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""><meta name=""color-scheme"" content=""dark light""><link rel=""stylesheet"" href=""{project.BaseUrl!.ToString().TrimEnd('/')}/stylesheet.css""></style></head>{bodyBuilt}</html>";

        
    }
}
