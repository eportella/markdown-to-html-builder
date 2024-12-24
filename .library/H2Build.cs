using System.Text.RegularExpressions;
using MediatR;
internal sealed class H2BuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed class H2BuildRequestHandler(IMediator mediator) : IRequestHandler<H2BuildRequest, string?>
{
    internal const string PATTERN = @"^(?'H2'## *(?'H2_CONTENT'(?!#).*(\r?\n|)))";
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
            var content = match.Groups["H2_CONTENT"].Value;
            var children = mediator
                .CreateStream(new InlineBuildRequest { Source = content }, cancellationToken)
                .ToBlockingEnumerable(cancellationToken);
            return $"<h2>{children.Build()}</h2>";
        });
    }
}
