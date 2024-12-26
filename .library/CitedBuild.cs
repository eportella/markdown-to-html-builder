using System.Text.RegularExpressions;
using MediatR;
internal sealed class CitedBuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed partial class CitedBuildRequestHandler() : IRequestHandler<CitedBuildRequest, string?>
{
    const string PATTERN = @$"\[\^(?'CITED_INDEX'\d+)\]";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline)]
    private static partial Regex Regex();
    public async Task<string?> Handle(CitedBuildRequest request, CancellationToken cancellationToken)
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

        return target;
    }
}
