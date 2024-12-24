using System.Text.RegularExpressions;
using MediatR;
internal sealed class H1BuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed class H1BuildRequestHandler(IMediator mediator) : IRequestHandler<H1BuildRequest, string?>
{
    internal const string PATTERN = @"^(?'H1'# *(?'H1_CONTENT'(?!#).*(\r?\n|)))";
    static Regex Regex { get; }
    static H1BuildRequestHandler()
    {
        Regex = new Regex(PATTERN, RegexOptions.Multiline);
    }
    public async Task<string?> Handle(H1BuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return Regex.Replace(request.Source, match =>
        {
            var content = match.Groups["H1_CONTENT"].Value;
            var children = mediator
                .CreateStream(new InlineBuildRequest { Source = content }, cancellationToken)
                .ToBlockingEnumerable(cancellationToken);
            return $"<h1>{children.Build()}</h1>";
        });
    }
}
