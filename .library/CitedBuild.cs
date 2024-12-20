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
    const string CITED = @$"\[\^(?'CITED_INDEX'\d+)\]";
    public async Task<CitedBuildResponse?> Handle(CitedBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return new CitedBuildResponse
        {
            Target = Build(request.Source),
        };
    }

    private string? Build(string? source)
    {
        if (source == default)
            return source;

        return Regex.Replace(
            source, 
            $"({CITED})", 
            match =>
            {
                var index = match.Groups["CITED_INDEX"].Value;
                return @$"<cite id=""cited-{index}""><a href=""#cite-{index}""><sup>({index})</sup></a></cite>";
            }, 
            RegexOptions.Multiline);
    }
}
