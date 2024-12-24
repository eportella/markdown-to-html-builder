using System.Text.RegularExpressions;
using MediatR;
internal sealed class InlineBuildRequest : IRequest<string?>
{
    internal string? Source { get; init; }
}
internal sealed class InlineBuildRequestHandler(IMediator mediator) : IRequestHandler<InlineBuildRequest, string?>
{
    const string PATTERN = @"^(?'INLINE'((.*(\r?\n|))*))";
    static Regex Regex { get; }

    static InlineBuildRequestHandler()
    {
        Regex = new Regex(PATTERN, RegexOptions.Multiline);
    }

    public Task<string?> Handle(InlineBuildRequest request, CancellationToken cancellationToken)
    {
        return Task.Run(() =>
        {
            if (request.Source == default)
                return default;

            return Regex.Replace(request.Source, match =>
            {
                var target = match.Groups["INLINE"].Value;

                target = mediator.Send(new BrBuildRequest { Source = target }, cancellationToken).Result?.Target;
                target = mediator.Send(new BIBuildRequest { Source = target }, cancellationToken).Result?.Target;
                target = mediator.Send(new BBuildRequest { Source = target }, cancellationToken).Result?.Target;
                target = mediator.Send(new IBuildRequest { Source = target }, cancellationToken).Result?.Target;
                target = mediator.Send(new DelBuildRequest { Source = target }, cancellationToken).Result?.Target;
                target = mediator.Send(new AgeCalcBuildRequest { Source = target }, cancellationToken).Result?.Target;
                target = mediator.Send(new ABuildRequest { Source = target }, cancellationToken).Result?.Target;
                target = mediator.Send(new SvgBuildRequest { Source = target }, cancellationToken).Result?.Target;
                target = mediator.Send(new CitedBuildRequest { Source = target }, cancellationToken).Result?.Target;
                target = mediator.Send(new ThemeBuildRequest { Source = target }, cancellationToken).Result?.Target;

                return target;
            });
        });
    }
}
