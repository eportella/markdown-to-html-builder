using System.Text.RegularExpressions;
using MediatR;
internal sealed class PBuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed partial class PBuildRequestHandler(IMediator mediator) : IRequestHandler<PBuildRequest, string?>
{
    const string PATTERN = @"^(?'P'((?!(#|>| *-| *\d+\.|\[\^\d+\]:)).+(\r?\n|))+(\r?\n|))";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline)]
    private static partial Regex Regex();
    
    public async Task<string?> Handle(PBuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;

        return await Regex().ReplaceAsync(request.Source, async match =>
        {
            var children = await mediator
                .Send(new InlineBuildRequest { Source = match.Groups["P"].Value }, cancellationToken);
            return $"<p>{children}</p>{Environment.NewLine}";
        });
    }
}
