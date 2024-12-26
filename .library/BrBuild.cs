using System.Text.RegularExpressions;
using MediatR;
internal sealed class BrBuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed partial class BrBuildRequestHandler() : IRequestHandler<BrBuildRequest, string?>
{
    const string PATTERN = @"(?'BR'\\(\r?\n))";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline)]
    private static partial Regex Regex();
    public async Task<string?> Handle(BrBuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;

        var target = await Regex().ReplaceAsync(
            request.Source,
            match => "<br />");

        return target;
    }
}
