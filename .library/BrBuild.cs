using System.Text.RegularExpressions;
using MediatR;
internal sealed class BrBuildRequest : IRequest<BrBuildResponse?>
{
    internal string? Source { get; init; }
}
internal sealed class BrBuildResponse
{
    internal string? Target { get; init; }
}
internal sealed partial class BrBuildRequestHandler() : IRequestHandler<BrBuildRequest, BrBuildResponse?>
{
    const string PATTERN = @"(?'BR'\\(\r?\n))";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline)]
    private static partial Regex Regex();
    public async Task<BrBuildResponse?> Handle(BrBuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;

        var target = await Regex().ReplaceAsync(
            request.Source,
            match => "<br />");

        return new BrBuildResponse
        {
            Target = target,
        };
    }
}
