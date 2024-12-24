using System.Text.RegularExpressions;
using MediatR;
internal sealed class H6BuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed partial class H6BuildRequestHandler(IMediator mediator) : IRequestHandler<H6BuildRequest, string?>
{
    const string PATTERN = @"^(?'H6'###### *(?'H6_CONTENT'(?!#).*(\r?\n|)))";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline)]
    private static partial Regex Regex();
    public async Task<string?> Handle(H6BuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return Regex().Replace(request.Source, match =>
        {
            var children = mediator
                .Send(new InlineBuildRequest { Source = match.Groups["H6_CONTENT"].Value }, cancellationToken).Result;
            return $"<h6>{children}</h6>{Environment.NewLine}";
        });
    }
}
