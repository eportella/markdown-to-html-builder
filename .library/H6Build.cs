using System.Text.RegularExpressions;
using MediatR;
internal sealed class H6BuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed class H6BuildRequestHandler(IMediator mediator) : IRequestHandler<H6BuildRequest, string?>
{
    internal const string PATTERN = @"^(?'H6'# *(?'H6_CONTENT'(?!#).*(\r?\n|)))";
    static Regex Regex { get; }
    static H6BuildRequestHandler()
    {
        Regex = new Regex(PATTERN, RegexOptions.Multiline);
    }
    public async Task<string?> Handle(H6BuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return Regex.Replace(request.Source, match =>
        {
            var content = match.Groups["H6_CONTENT"].Value;
            var children = mediator
                .CreateStream(new InlineBuildRequest { Source = content }, cancellationToken)
                .ToBlockingEnumerable(cancellationToken);
            return $"<h6>{children.Build()}</h6>";
        });
    }
}
