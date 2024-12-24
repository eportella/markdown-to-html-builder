using System.Text.RegularExpressions;
using MediatR;
internal sealed class PBuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed class PBuildRequestHandler(IMediator mediator) : IRequestHandler<PBuildRequest, string?>
{
    internal const string PATTERN = @"^(?'P'((?!(#|>| *-| *\d+\.|\[\^\d+\]:)).+(\r?\n|))+(\r?\n|))";
    static Regex Regex { get; }
    static PBuildRequestHandler()
    {
        Regex = new Regex(PATTERN, RegexOptions.Multiline);
    }
    public async Task<string?> Handle(PBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return Regex.Replace(request.Source, match =>
        {
            var children = mediator
                .CreateStream(new InlineBuildRequest { Source = match.Groups["P"].Value }, cancellationToken)
                .ToBlockingEnumerable(cancellationToken);
            return $"<p>{children.Build()}</p>";
        });
    }
}
