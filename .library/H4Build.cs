using System.Text.RegularExpressions;
using MediatR;
internal sealed class H4BuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed partial class H4BuildRequestHandler(IMediator mediator) : IRequestHandler<H4BuildRequest, string?>
{
    const string PATTERN = @"^(?'H4'#### *(?'H4_CONTENT'(?!#).*(\r?\n|)))";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline)]
    private static partial Regex Regex();
    public async Task<string?> Handle(H4BuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return Regex().Replace(request.Source, match =>
        {
            var children = mediator
                .Send(new InlineBuildRequest { Source = match.Groups["H4_CONTENT"].Value }, cancellationToken).Result;
            return $"<h4>{children}</h4>{Environment.NewLine}";
        });
    }
}
