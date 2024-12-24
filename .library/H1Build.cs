using System.Text.RegularExpressions;
using MediatR;
internal sealed class H1BuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed partial class H1BuildRequestHandler(IMediator mediator) : IRequestHandler<H1BuildRequest, string?>
{
    const string PATTERN = @"^(?'H1'# *(?'H1_CONTENT'(?!#).*(\r?\n|)))";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline)]
    private static partial Regex Regex();
    public async Task<string?> Handle(H1BuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return Regex().Replace(request.Source, match =>
        {
            var children = mediator
                .Send(new InlineBuildRequest { Source = match.Groups["H1_CONTENT"].Value }, cancellationToken).Result;
            return $"<h1>{children}</h1>{Environment.NewLine}";
        });
    }
}
