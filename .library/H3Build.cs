using System.Text.RegularExpressions;
using MediatR;
internal sealed class H3BuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed partial class H3BuildRequestHandler(IMediator mediator) : IRequestHandler<H3BuildRequest, string?>
{
    const string PATTERN = @"^(?'H3'### *(?'H3_CONTENT'(?!#).*(\r?\n|)))";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline)]
    private static partial Regex Regex();
    public async Task<string?> Handle(H3BuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return Regex().Replace(request.Source, match =>
        {
            var children = mediator
                .Send(new InlineBuildRequest { Source = match.Groups["H3_CONTENT"].Value }, cancellationToken).Result;
            return $"<h3>{children}</h3>{Environment.NewLine}";
        });
    }
}
