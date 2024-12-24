using System.Text.RegularExpressions;
using MediatR;
internal sealed class H2BuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed partial class H2BuildRequestHandler(IMediator mediator) : IRequestHandler<H2BuildRequest, string?>
{
    const string PATTERN = @"^(?'H2'## *(?'H2_CONTENT'(?!#).*(\r?\n|)))";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline)]
    private static partial Regex Regex();
    public async Task<string?> Handle(H2BuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;

        return await Regex().ReplaceAsync(request.Source, async match =>
        {
            var children = await mediator
                .Send(new InlineBuildRequest { Source = match.Groups["H2_CONTENT"].Value }, cancellationToken);
            return $"<h2>{children}</h2>{Environment.NewLine}";
        });
    }
}
