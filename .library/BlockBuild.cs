using System.Text.RegularExpressions;
using MediatR;
internal sealed class BlockBuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed class BlockBuildRequestHandler(IMediator mediator) : IRequestHandler<BlockBuildRequest, string?>
{
    static Regex RegexBlock { get; }

    static BlockBuildRequestHandler()
    {
        RegexBlock = new Regex($"({PBuildRequestHandler.PATTERN}|{H1BuildRequestHandler.PATTERN}|{H2BuildRequestHandler.PATTERN}|{H3BuildRequestHandler.PATTERN}|{H4BuildRequestHandler.PATTERN}|{H5BuildRequestHandler.PATTERN}|{H6BuildRequestHandler.PATTERN}|{BlockquoteBuildRequestHandler.PATTERN}|{UlOlBuildRequestHandler.PATTERN}|{CiteBuildRequestHandler.PATTERN})", RegexOptions.Multiline);
    }

    public async Task<string?> Handle(BlockBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return RegexBlock?.Replace(request.Source, (match) =>
        {
            if (!string.IsNullOrWhiteSpace(match.Groups["H1"].Value))
                return mediator.Send(new H1BuildRequest { Source = match.Groups["H1"].Value }, cancellationToken).Result;
            if (!string.IsNullOrWhiteSpace(match.Groups["H2"].Value))
                return mediator.Send(new H2BuildRequest { Source = match.Groups["H2"].Value }, cancellationToken).Result;
            if (!string.IsNullOrWhiteSpace(match.Groups["H3"].Value))
                return mediator.Send(new H3BuildRequest { Source = match.Groups["H3"].Value }, cancellationToken).Result;
            if (!string.IsNullOrWhiteSpace(match.Groups["H4"].Value))
                return mediator.Send(new H4BuildRequest { Source = match.Groups["H4"].Value }, cancellationToken).Result;
            if (!string.IsNullOrWhiteSpace(match.Groups["H5"].Value))
                return mediator.Send(new H5BuildRequest { Source = match.Groups["H5"].Value }, cancellationToken).Result;
            if (!string.IsNullOrWhiteSpace(match.Groups["H6"].Value))
                return mediator.Send(new H6BuildRequest { Source = match.Groups["H6"].Value }, cancellationToken).Result;
            if (!string.IsNullOrWhiteSpace(match.Groups["BLOCKQUOTE"].Value))
                return mediator.Send(new BlockquoteBuildRequest { Source = string.Join(string.Empty, match.Groups["BLOCKQUOTE"].Captures.Select(s => s.Value)) }, cancellationToken).Result;
            if (!string.IsNullOrWhiteSpace(match.Groups["UL_OL"].Value))
                return mediator.Send(new UlOlBuildRequest { Source = match.Groups["UL_OL"].Value }, cancellationToken).Result;
            if (match.Groups["P"].Value != string.Empty)
                return mediator.Send(new PBuildRequest { Source = match.Groups["P"].Value }, cancellationToken).Result;
            if (!string.IsNullOrWhiteSpace(match.Groups["CITE"].Value))
                return mediator.Send(new CiteBuildRequest { Source = match.Groups["CITE"].Value }, cancellationToken).Result;
            if (!string.IsNullOrWhiteSpace(match.Groups["INLINE"].Value))
                return mediator
                    .Send(new InlineBuildRequest { Source = match.Groups["INLINE"].Value }, cancellationToken).Result;

            if (match.Value == string.Empty)
                return match.Value;

            throw new InvalidOperationException($"build with {match.Value} invalid");
        });
    }
}
