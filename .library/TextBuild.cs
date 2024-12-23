using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using MediatR;
internal sealed class TextBuildRequest : IStreamRequest<Text>
{
    internal string? Source { get; init; }
}
internal sealed class TextBuildRequestHandler(IMediator mediator) : IStreamRequestHandler<TextBuildRequest, Text>
{
    const string TEXT = @"^(?'TEXT'((.*(\r?\n|))*))";
    static Regex RegexText { get; }

    static TextBuildRequestHandler()
    {
        RegexText = new Regex(TEXT, RegexOptions.Multiline);
    }

    public async IAsyncEnumerable<Text> Handle(TextBuildRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (request.Source == default)
            yield break;

        foreach (Match match in RegexText.Matches(request.Source))
        {
            var content = match.Groups["TEXT"].Value;
            if (!string.IsNullOrWhiteSpace(content))
            {
                var built = content;

                built = (await mediator.Send(new BrBuildRequest { Source = built }, cancellationToken))?.Target;
                built = (await mediator.Send(new BIBuildRequest { Source = built }, cancellationToken))?.Target;
                built = (await mediator.Send(new BBuildRequest { Source = built }, cancellationToken))?.Target;
                built = (await mediator.Send(new IBuildRequest { Source = built }, cancellationToken))?.Target;
                built = (await mediator.Send(new DelBuildRequest { Source = built }, cancellationToken))?.Target;
                built = (await mediator.Send(new AgeCalcBuildRequest { Source = built }, cancellationToken))?.Target;
                built = (await mediator.Send(new ABuildRequest { Source = built }, cancellationToken))?.Target;
                built = (await mediator.Send(new SvgBuildRequest { Source = built }, cancellationToken))?.Target;
                built = (await mediator.Send(new CitedBuildRequest { Source = built }, cancellationToken))?.Target;
                built = (await mediator.Send(new ThemeBuildRequest { Source = built }, cancellationToken))?.Target;

                yield return new Text
                {
                    Built = built,
                };
                continue;
            }
            var debug = string.Empty;
        }
    }
}
