using System.Text.RegularExpressions;
using MediatR;
internal sealed class CiteBuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed class CiteBuildRequestHandler(IMediator mediator) : IRequestHandler<CiteBuildRequest, string?>
{
    internal const string PATTERN = @"^(?'CITE'\[\^(?'CITE_INDEX'\d+)\]: +(?'CITE_CONTENT'.*))";
    static Regex Regex { get; }
    static CiteBuildRequestHandler()
    {
        Regex = new Regex(PATTERN, RegexOptions.Multiline);
    }
    public async Task<string?> Handle(CiteBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return Regex.Replace(request.Source, match =>
        {
            var index = match.Groups["CITE_INDEX"].Value;
            var children = mediator
                    .Send(new InlineBuildRequest { Source = match.Groups["CITE_CONTENT"].Value }, cancellationToken).Result;
                return @$"<cite id=""cite-{index}""><a href=""#cited-{index}"">({index})</a>. {children}</cite>";
        });
    }
}
