using System.Text.RegularExpressions;
using MediatR;
internal sealed class H5BuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed class H5BuildRequestHandler(IMediator mediator) : IRequestHandler<H5BuildRequest, string?>
{
    internal const string PATTERN = @"^(?'H5'##### *(?'H5_CONTENT'(?!#).*(\r?\n|)))";
    static Regex Regex { get; }
    static H5BuildRequestHandler()
    {
        Regex = new Regex(PATTERN, RegexOptions.Multiline);
    }
    public async Task<string?> Handle(H5BuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return Regex.Replace(request.Source, match =>
        {
            var content = match.Groups["H5_CONTENT"].Value;
            var children = mediator
                .Send(new InlineBuildRequest { Source = content }, cancellationToken).Result;
            return $"<h5>{children}</h5>";
        });
    }
}
