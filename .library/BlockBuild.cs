using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using MediatR;
internal sealed class BlockBuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed class BlockBuildRequestHandler(IMediator mediator) : IRequestHandler<BlockBuildRequest, string?>
{
    const string P = @"^(?'P'((?!(#|>| *-| *\d+\.|\[\^\d+\]:)).+(\r?\n|))+(\r?\n|))";
    static Regex RegexBlock { get; }

    static BlockBuildRequestHandler()
    {
        RegexBlock = new Regex(@$"({P}|{H1BuildRequestHandler.PATTERN}|{H2BuildRequestHandler.PATTERN}|{H3BuildRequestHandler.PATTERN}|{H4BuildRequestHandler.PATTERN}|{H5BuildRequestHandler.PATTERN}|{H6BuildRequestHandler.PATTERN}|{BlockquoteBuildRequestHandler.PATTERN}|{UlOlBuildRequestHandler.PATTERN}|{CiteBuildRequestHandler.PATTERN})", RegexOptions.Multiline);
    }

    public Task<string?> Handle(BlockBuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return Task.FromResult(default(string?));

        return Task.FromResult(RegexBlock?.Replace(request.Source, (match) => Replace(match, cancellationToken)));
    }

    private string Replace(Match match, CancellationToken cancellationToken)
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

        {
            var content = match.Groups["P"].Value;
            if ((content ?? string.Empty) != string.Empty)
            {
                var children = mediator
                    .CreateStream(new InlineBuildRequest { Source = content }, cancellationToken)
                    .ToBlockingEnumerable(cancellationToken);
                return $"<p>{children.Build()}</p>";
            }
        }

        if (!string.IsNullOrWhiteSpace(match.Groups["CITE"].Value))
            return mediator.Send(new CiteBuildRequest { Source = match.Groups["CITE"].Value }, cancellationToken).Result;

        {
            var content = match.Groups["INLINE"].Value;
            if (!string.IsNullOrWhiteSpace(content))
                return mediator
                    .CreateStream(new InlineBuildRequest { Source = content }, cancellationToken)
                    .ToBlockingEnumerable(cancellationToken)
                    .Build()!;
        }
        if (match.Value == string.Empty)
            return match.Value;

        throw new InvalidOperationException($"build with {match.Value} invalid");
    }
}
