using System.Text.RegularExpressions;
using MediatR;
internal sealed class H3BuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed class H3BuildRequestHandler(IMediator mediator) : IRequestHandler<H3BuildRequest, string?>
{
    internal const string PATTERN = @"^(?'H3'### *(?'H3_CONTENT'(?!#).*(\r?\n|)))";
    static Regex Regex { get; }
    static H3BuildRequestHandler()
    {
        Regex = new Regex(PATTERN, RegexOptions.Multiline);
    }
    public async Task<string?> Handle(H3BuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return Regex.Replace(request.Source, match =>
        {
            var children = mediator
                .Send(new InlineBuildRequest { Source = match.Groups["H3_CONTENT"].Value }, cancellationToken).Result;
            return $"<h3>{children}</h3>{Environment.NewLine}";
        });
    }
}
