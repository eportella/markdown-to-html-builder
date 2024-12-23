using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using MediatR;
internal sealed class BuildRequest : IRequest<string?>
{
    public string? Source { get; init; }
}

internal sealed class BuildRequestHandler(ProjectBuildResponse project, IMediator mediator) : IRequestHandler<BuildRequest, string?>
{
    const string P = @"^(?'P'((?!(#|>| *-| *\d+\.|\[\^\d+\]:)).+(\r?\n|))+(\r?\n|))";
    const string H1 = @"^(?'H1'# *(?'H1_CONTENT'(?!#).+(\r?\n|)))";
    const string H2 = @"^(?'H2'## *(?'H2_CONTENT'(?!#).+(\r?\n|)))";
    const string H3 = @"^(?'H3'### *(?'H3_CONTENT'(?!#).+(\r?\n|)))";
    const string H4 = @"^(?'H4'#### *(?'H4_CONTENT'(?!#).+(\r?\n|)))";
    const string H5 = @"^(?'H5'##### *(?'H5_CONTENT'(?!#).+(\r?\n|)))";
    const string H6 = @"^(?'H6'###### *(?'H6_CONTENT'(?!#).+(\r?\n|)))";
    const string BLOCKQUOTE = @"^(?'BLOCKQUOTE'>(?'BLOCKQUOTE_CONTENT' *.*(\r?\n|)))+";
    const string UL_OL = @"^(?'UL_OL'(((?'UL'-)|(?'OL'\d+\.)) *.+(\r?\n|))( *((-)|(\d+\.)) *.+(\r?\n|))*(\r?\n|))";
    const string UL_OL_INNER = @"^(((.+?\r?\n))(?'UL_OL'( *((-)|(\d+\.)) *.+(\r?\n|))*(\r?\n|)))";
    const string LI = @"^(-|\d+\.) *(?'LI'(.*(\r?\n|)+(?!(-|\d+\.)))+(\r?\n|))";
    const string CITE = @"^\[\^(?'CITE_INDEX'\d+)\]: +(?'CITE_CONTENT'.*)";
    static Regex RegexBlock { get; }
    static Regex RegexOlUl { get; }
    static Regex RegexOlUlInner { get; }
    static Regex RegexLi { get; }

    static BuildRequestHandler()
    {
        RegexBlock = new Regex(@$"({P}|{H1}|{H2}|{H3}|{H4}|{H5}|{H6}|{BLOCKQUOTE}|{UL_OL}|{CITE})", RegexOptions.Multiline);
        RegexOlUl = new Regex(UL_OL);
        RegexOlUlInner = new Regex(UL_OL_INNER, RegexOptions.Multiline);
        RegexLi = new Regex(LI, RegexOptions.Multiline);
    }
    public async Task<string?> Handle(BuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return Build(request, cancellationToken);
    }

    private string? Build(BuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;

        var replaced = RegexBlock.Replace(request.Source, (match) => Replace(match, cancellationToken));

        var bodyBuilt = @$"<body><h1><a href=""{project.BaseUrl}""/>{project.Title}</a></h1>{replaced}{(project.OwnerTitle != default && project.OwnerBaseUrl != default ? @$"<span class=""owner""><a href=""{project.OwnerBaseUrl}""/>{project.OwnerTitle}</a></span>" : string.Empty)}</body>";
        return $@"<!DOCTYPE html><html lang=""pt-BR""><head><title>{project.Title}</title><meta content=""text/html; charset=UTF-8;"" http-equiv=""Content-Type"" /><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""><meta name=""color-scheme"" content=""dark light""><link rel=""stylesheet"" href=""{project.BaseUrl!.ToString().TrimEnd('/')}/stylesheet.css""></style></head>{bodyBuilt}</html>";
    }
    private async IAsyncEnumerable<string?> Build(string? source, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (source == default)
            yield break;

        foreach (Group group in RegexOlUlInner
            .Matches(source)
            .Select(m => m.Groups["UL_OL"])
            .Where(g => g.Success && !string.IsNullOrWhiteSpace(g.Value)))
        {
            source = source
                .Replace(
                    group.Value,
                    RegexOlUl
                        .Replace(
                            Regex
                                .Replace(
                                    group.Value,
                                    "^    ",
                                    string.Empty,
                                    RegexOptions.Multiline
                                ),
                                match => Replace(match, cancellationToken)
                        )
                );
        }

        await foreach (var text in mediator.CreateStream(new InlineBuildRequest { Source = source }, cancellationToken))
            yield return text;
    }
    private string Replace(Match match, CancellationToken cancellationToken)
    {

        if (!string.IsNullOrWhiteSpace(match.Groups["H1"].Value))
        {
            var content = match.Groups["H1_CONTENT"].Value;
            var children = mediator
                .CreateStream(new InlineBuildRequest { Source = content }, cancellationToken)
                .ToBlockingEnumerable(cancellationToken);
            return $"<h1>{children.Build()}</h1>";
        }
        if (!string.IsNullOrWhiteSpace(match.Groups["H2"].Value))
        {
            var content = match.Groups["H2_CONTENT"].Value;

            var children = mediator
                .CreateStream(new InlineBuildRequest { Source = content }, cancellationToken)
                .ToBlockingEnumerable(cancellationToken);
            return $"<h2>{children.Build()}</h2>";
        }
        if (!string.IsNullOrWhiteSpace(match.Groups["H3"].Value))
        {
            var content = match.Groups["H3_CONTENT"].Value;
            var children = mediator
                .CreateStream(new InlineBuildRequest { Source = content }, cancellationToken)
                .ToBlockingEnumerable(cancellationToken);
            return $"<h3>{children.Build()}</h3>";
        }
        if (!string.IsNullOrWhiteSpace(match.Groups["H4"].Value))
        {
            var content = match.Groups["H4_CONTENT"].Value;

            var children = mediator
                .CreateStream(new InlineBuildRequest { Source = content }, cancellationToken)
                .ToBlockingEnumerable(cancellationToken);
            return $"<h4>{children.Build()}</h4>";
        }
        if (!string.IsNullOrWhiteSpace(match.Groups["H5"].Value))
        {
            var content = match.Groups["H5_CONTENT"].Value;
            var children = mediator
                .CreateStream(new InlineBuildRequest { Source = content }, cancellationToken)
                .ToBlockingEnumerable(cancellationToken);
            return $"<h5>{children.Build()}</h5>";
        }
        if (!string.IsNullOrWhiteSpace(match.Groups["H6"].Value))
        {
            var content = match.Groups["H6_CONTENT"].Value;
            var children = mediator
                .CreateStream(new InlineBuildRequest { Source = content }, cancellationToken)
                .ToBlockingEnumerable(cancellationToken);
            return $"<h6>{children.Build()}</h6>"; ;
        }
        if (!string.IsNullOrWhiteSpace(match.Groups["BLOCKQUOTE"].Value))
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
            var children = RegexBlock.Replace(content, match => Replace(match, cancellationToken));

            return $"<blockquote{attribute}>{children}</blockquote>";
        }

        {
            var content = match.Groups["UL_OL"].Value;
            if (!string.IsNullOrWhiteSpace(content))
            {
                if (!string.IsNullOrWhiteSpace(match.Groups["UL"].Value))
                    return $"<ul>{RegexLi.Replace(content, match => Replace(match, cancellationToken))}</ul>";

                if (!string.IsNullOrWhiteSpace(match.Groups["OL"].Value))
                    return $"<ol>{RegexLi.Replace(content, match => Replace(match, cancellationToken))}</ol>";

                return content;
            }
        }

        {
            var content = match.Groups["LI"].Value;
            if (!string.IsNullOrWhiteSpace(content))
            {
                var children = Build(content, cancellationToken)
                    .ToBlockingEnumerable(cancellationToken);
                return $"<li>{children.Build()}</li>";
            }
        }

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
        {
            var index = match.Groups["CITE_INDEX"].Value;
            var content = match.Groups["CITE_CONTENT"].Value;
            if (!string.IsNullOrWhiteSpace(index))
            {
                var children = mediator
                    .CreateStream(new InlineBuildRequest { Source = content }, cancellationToken)
                    .ToBlockingEnumerable(cancellationToken);
                return @$"<cite id=""cite-{index}""><a href=""#cited-{index}"">({index})</a>. {children.Build()}</cite>";
            }
        }
        {
            var content = match.Groups["INLINE"].Value;
            if (!string.IsNullOrWhiteSpace(content))
                return mediator
                    .CreateStream(new InlineBuildRequest { Source = content }, cancellationToken)
                    .ToBlockingEnumerable(cancellationToken)
                    .Build()!;
        }
        return match.Value;
    }
}
