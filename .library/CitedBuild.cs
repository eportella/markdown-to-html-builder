using System.Text.RegularExpressions;
using MediatR;
internal sealed class CitedBuildRequest : IRequest<CitedBuildResponse?>
{
    internal string? Source { get; init; }
}
internal sealed class CitedBuildResponse
{
    internal string? Target { get; init; }
}
internal sealed partial class CitedBuildRequestHandler() : IRequestHandler<CitedBuildRequest, CitedBuildResponse?>
{
    const string PATTERN = @$"\[\^(?'CITED_INDEX'\d+)\]";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline)]
    private static partial Regex Regex();
    public async Task<CitedBuildResponse?> Handle(CitedBuildRequest request, CancellationToken cancellationToken)
    {
        
        if (request.Source == default)
            return default;

        var target = await Regex().ReplaceAsync(
            request.Source,
            match =>
            {
                var index = match.Groups["CITED_INDEX"].Value;
                return @$"<cite id=""cited-{index}""><a href=""#cite-{index}""><sup>({index})</sup></a></cite>";
            });

        return new CitedBuildResponse
        {
            Target = target,
        };
    }
}
