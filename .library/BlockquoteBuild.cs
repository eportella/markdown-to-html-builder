using System.Text.RegularExpressions;
using MediatR;
internal sealed class BlockquoteBuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed partial class BlockquoteBuildRequestHandler(IMediator mediator) : IRequestHandler<BlockquoteBuildRequest, string?>
{
    const string PATTERN = @"^(?'BLOCKQUOTE'>(?'BLOCKQUOTE_CONTENT' *.*(\r?\n|)))+";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline)]
    private static partial Regex Regex();
    public async Task<string?> Handle(BlockquoteBuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;

        return await Regex().ReplaceAsync(request.Source, async match =>
        {
            var content = string.Join(string.Empty, match.Groups["BLOCKQUOTE_CONTENT"].Captures.Select(c => c.Value));
            var attribute = string.Empty;
            if (content.StartsWith("[!NOTE]"))
            {
                attribute = @" class=""note""";
            }
            else if (content.StartsWith("[!TIP]"))
            {
                attribute = @" class=""tip""";
            }
            else if (content.StartsWith("[!IMPORTANT]"))
            {
                attribute = @" class=""important""";
            }
            else if (content.StartsWith("[!WARNING]"))
            {
                attribute = @" class=""warning""";
            }
            else if (content.StartsWith("[!CAUTION]"))
            {
                attribute = @" class=""caution""";
            }
            var children = await mediator.Send(new BlockBuildRequest { Source = content }, cancellationToken);

            return $"<blockquote{attribute}>{children}</blockquote>{Environment.NewLine}";
        });
    }
}
