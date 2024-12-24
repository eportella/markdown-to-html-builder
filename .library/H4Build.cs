using System.Text.RegularExpressions;
using MediatR;
internal sealed class H4BuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed class H4BuildRequestHandler(IMediator mediator) : IRequestHandler<H4BuildRequest, string?>
{
    internal const string PATTERN = @"^(?'H4'#### *(?'H4_CONTENT'(?!#).*(\r?\n|)))";
    static Regex Regex { get; }
    static H4BuildRequestHandler()
    {
        Regex = new Regex(PATTERN, RegexOptions.Multiline);
    }
    public async Task<string?> Handle(H4BuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return Regex.Replace(request.Source, match =>
        {
            var content = match.Groups["H4_CONTENT"].Value;
            var children = mediator
                .CreateStream(new InlineBuildRequest { Source = content }, cancellationToken)
                .ToBlockingEnumerable(cancellationToken);
            return $"<h4>{children.Build()}</h4>";
        });
    }
}
