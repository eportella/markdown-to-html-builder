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
internal sealed class BrBuildRequestHandler() : IRequestHandler<BrBuildRequest, BrBuildResponse?>
{
    const string BR = @"(?'BR'\\(\r?\n))";
    public async Task<BrBuildResponse?> Handle(BrBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return new BrBuildResponse
        {
            Target = Build(request.Source),
        };
    }

    private string? Build(string? source)
    {
        if (source == default)
            return source;

        var target = source;

        target = Regex.Replace(
            target, 
            $"({BR})", 
            (match) => "<br />", 
            RegexOptions.Multiline);
        
        return target;
    }
}
