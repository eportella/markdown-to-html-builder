using System.Text.RegularExpressions;
using MediatR;
internal sealed class CiteBuildRequest : IRequest<MatchCollection>
{
    internal string? Source { get; init; }
}
internal sealed class CiteBuildRequestHandler() : IRequestHandler<CiteBuildRequest, MatchCollection>
{
    const string PATTERN = @"^\[\^(?'CITE_INDEX'\d+)\]: +(?'CITE_CONTENT'.*)";
    static Regex Regex { get; }
    static CiteBuildRequestHandler()
    {
        Regex = new Regex(PATTERN, RegexOptions.Multiline);
    }
    public async Task<MatchCollection> Handle(CiteBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        return Regex.Matches(request.Source!);
    }
}
