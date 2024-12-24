using System.Text.RegularExpressions;
using MediatR;
internal sealed class H5BuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed partial class H5BuildRequestHandler(IMediator mediator) : IRequestHandler<H5BuildRequest, string?>
{
    const string PATTERN = @"^(?'H5'##### *(?'H5_CONTENT'(?!#).*(\r?\n|)))";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline)]
    private static partial Regex Regex();
    public async Task<string?> Handle(H5BuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;

        return await Regex().ReplaceAsync(request.Source, async match =>
        {
            var children = await mediator
                .Send(new InlineBuildRequest { Source = match.Groups["H5_CONTENT"].Value }, cancellationToken);
            return $"<h5>{children}</h5>{Environment.NewLine}";
        });
    }
}
