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
internal sealed class CitedBuildRequestHandler() : IRequestHandler<CitedBuildRequest, CitedBuildResponse?>
{
    const string PATTERN = @$"\[\^(?'CITED_INDEX'\d+)\]";
    static Regex Regex { get; }
    static CitedBuildRequestHandler()
    {
        Regex = new Regex(PATTERN, RegexOptions.Multiline);
    }
    public async Task<CitedBuildResponse?> Handle(CitedBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        var target = Regex.Replace(
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
