using System.Text.RegularExpressions;
using MediatR;
internal sealed class CiteBuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed partial class CiteBuildRequestHandler(IMediator mediator) : IRequestHandler<CiteBuildRequest, string?>
{
    const string PATTERN = @"^(?'CITE'\[\^(?'CITE_INDEX'\d+)\]: +(?'CITE_CONTENT'.*))";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline)]
    private static partial Regex Regex();
    public async Task<string?> Handle(CiteBuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;

        return await Regex().ReplaceAsync(request.Source, async match =>
        {
            var index = match.Groups["CITE_INDEX"].Value;
            var children = await mediator
                    .Send(new InlineBuildRequest { Source = match.Groups["CITE_CONTENT"].Value }, cancellationToken);
                return @$"<cite id=""cite-{index}""><a href=""#cited-{index}"">({index})</a>. {children}</cite>{Environment.NewLine}";
        });
    }
}
