using System.Text.RegularExpressions;
using MediatR;
internal sealed class H2BuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed class H2BuildRequestHandler(IMediator mediator) : IRequestHandler<H2BuildRequest, string?>
{
    const string PATTERN = @"^(?'H2'## *(?'H2_CONTENT'(?!#).*(\r?\n|)))";
    static Regex Regex { get; }
    static H2BuildRequestHandler()
    {
        Regex = new Regex(PATTERN, RegexOptions.Multiline);
    }
    public async Task<string?> Handle(H2BuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return Regex.Replace(request.Source, match =>
        {
            var children = mediator
                .Send(new InlineBuildRequest { Source = match.Groups["H2_CONTENT"].Value }, cancellationToken).Result;
            return $"<h2>{children}</h2>{Environment.NewLine}";
        });
    }
}
