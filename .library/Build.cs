using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using MediatR;
internal sealed class BuildRequest : IRequest<BuildResponse>
{
    public string? Source { get; init; }
}
internal sealed class BuildResponse
{
    internal Html? Target { get; init; }
}
internal sealed class BuildRequestHandler(ProjectBuildResponse project, IMediator mediator) : IRequestHandler<BuildRequest, BuildResponse>
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
    static Regex RegexLi { get; }

    static BuildRequestHandler()
    {
        RegexLi = new Regex(LI, RegexOptions.Multiline);
    }
    public async Task<BuildResponse> Handle(BuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return new BuildResponse();

        return new BuildResponse
        {
            Target = Build(request, cancellationToken),
        };
    }

    private Html Build(BuildRequest request, CancellationToken cancellationToken)
    {
        var html = new Html
        {
            Source = request.Source,
            Parent = default,
        };
        html.Children = Build(html, request, cancellationToken).ToBlockingEnumerable(cancellationToken).ToArray();
        html.Built = $@"<!DOCTYPE html><html lang=""pt-BR""><head><title>{project.Title}</title><meta content=""text/html; charset=UTF-8;"" http-equiv=""Content-Type"" /><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""><meta name=""color-scheme"" content=""dark light""><link rel=""stylesheet"" href=""{project.BaseUrl!.ToString().TrimEnd('/')}/stylesheet.css""></style></head>{html.Children.Build()}</html>";
        return html;
    }

    private async IAsyncEnumerable<IElement> Build(Html html, BuildRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (request.Source == default)
            yield break;
        var body = new Body
        {
            Source = request.Source,
            Parent = html,
        };
        body.Children = Build(body, Regex.Matches(body.Source, @$"({P}|{H1}|{H2}|{H3}|{H4}|{H5}|{H6}|{BLOCKQUOTE}|{UL_OL})", RegexOptions.Multiline).Concat(await mediator.Send(new CiteBuildRequest { Source = body.Source }, cancellationToken)).ToArray(), cancellationToken)
            .ToBlockingEnumerable(cancellationToken)
            .ToArray();
        body.Built = @$"<body><h1><a href=""{project.BaseUrl}""/>{project.Title}</a></h1>{body.Children.Build()}{(project.OwnerTitle != default && project.OwnerBaseUrl != default ? @$"<span class=""owner""><a href=""{project.OwnerBaseUrl}""/>{project.OwnerTitle}</a></span>" : string.Empty)}</body>";
        yield return body;
    }
    private async IAsyncEnumerable<IElement> Build(LI? parent, string? source, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        if (source == default)
            yield break;

        var matches = Regex.Matches(source, UL_OL_INNER, RegexOptions.Multiline);
        foreach (Group group in matches.Select(m => m.Groups["UL_OL"]).Where(g => g.Success && !string.IsNullOrWhiteSpace(g.Value)))
        {
            var sourceInner = Regex
                .Replace(
                    group.Value,
                    "^    ",
                    string.Empty,
                    RegexOptions.Multiline
                );
            source = source
                .Replace(
                    group.Value,
                    Build(
                        parent,
                        Regex.Matches(
                                sourceInner,
                                UL_OL
                            )
                            .ToArray(),
                        cancellationToken
                    )
                    .ToBlockingEnumerable()?
                    .SingleOrDefault()?
                    .Built
                );
        }

        await foreach (var text in mediator.CreateStream(new TextBuildRequest { Parent = parent, Source = source }, cancellationToken))
            yield return text;
    }
    private async IAsyncEnumerable<IElement> Build(IElement? parent, Match[] matches, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (Match match in matches)
        {
            if (!string.IsNullOrWhiteSpace(match.Groups["H1"].Value))
            {
                var content = match.Groups["H1_CONTENT"].Value;
                var h1 = new H1
                {
                    Source = content,
                    Parent = parent,
                };
                h1.Children = mediator
                    .CreateStream(new TextBuildRequest { Parent = h1, Source = content }, cancellationToken)
                    .ToBlockingEnumerable(cancellationToken)
                    .ToArray();
                h1.Built = $"<h1>{h1.Children.Build()}</h1>";
                yield return h1;
                continue;
            }
            if (!string.IsNullOrWhiteSpace(match.Groups["H2"].Value))
            {
                var content = match.Groups["H2_CONTENT"].Value;
                var h2 = new H2
                {
                    Source = content,
                    Parent = parent,
                };
                h2.Children = mediator
                    .CreateStream(new TextBuildRequest { Parent = h2, Source = content }, cancellationToken)
                    .ToBlockingEnumerable(cancellationToken)
                    .ToArray();
                h2.Built = $"<h2>{h2.Children.Build()}</h2>";
                yield return h2;
                continue;
            }
            if (!string.IsNullOrWhiteSpace(match.Groups["H3"].Value))
            {
                var content = match.Groups["H3_CONTENT"].Value;
                var h3 = new H3
                {
                    Source = content,
                    Parent = parent,
                };
                h3.Children = mediator
                    .CreateStream(new TextBuildRequest { Parent = h3, Source = content }, cancellationToken)
                    .ToBlockingEnumerable(cancellationToken)
                    .ToArray();
                h3.Built = $"<h3>{h3.Children.Build()}</h3>";
                yield return h3;
                continue;
            }
            if (!string.IsNullOrWhiteSpace(match.Groups["H4"].Value))
            {
                var content = match.Groups["H4_CONTENT"].Value;
                var h4 = new H4
                {
                    Source = content,
                    Parent = parent,
                };
                h4.Children = mediator
                    .CreateStream(new TextBuildRequest { Parent = h4, Source = content }, cancellationToken)
                    .ToBlockingEnumerable(cancellationToken)
                    .ToArray();
                h4.Built = $"<h4>{h4.Children.Build()}</h4>";
                yield return h4;
                continue;
            }
            if (!string.IsNullOrWhiteSpace(match.Groups["H5"].Value))
            {
                var content = match.Groups["H5_CONTENT"].Value;
                var h5 = new H5
                {
                    Source = content,
                    Parent = parent,
                };
                h5.Children = mediator
                    .CreateStream(new TextBuildRequest { Parent = h5, Source = content }, cancellationToken)
                    .ToBlockingEnumerable(cancellationToken)
                    .ToArray();
                h5.Built = $"<h5>{h5.Children.Build()}</h5>";
                yield return h5;
                continue;
            }
            if (!string.IsNullOrWhiteSpace(match.Groups["H6"].Value))
            {
                var content = match.Groups["H6_CONTENT"].Value;
                var h6 = new H6
                {
                    Source = content,
                    Parent = parent,
                };
                h6.Children = mediator
                    .CreateStream(new TextBuildRequest { Parent = h6, Source = content }, cancellationToken)
                    .ToBlockingEnumerable(cancellationToken)
                    .ToArray();
                h6.Built = $"<h6>{h6.Children.Build()}</h6>";
                yield return h6;
                continue;
            }
            if (!string.IsNullOrWhiteSpace(match.Groups["BLOCKQUOTE"].Value))
            {
                var blockquote = new Blockquote
                {
                    Source = string.Join(string.Empty, match.Groups["BLOCKQUOTE_CONTENT"].Captures.Select(c => c.Value)),
                    Parent = parent,
                    Children = default,
                };

                var attribute = string.Empty;
                if (blockquote.Source.StartsWith("[!NOTE]"))
                {
                    attribute = @" class=""note""";
                }
                else if (blockquote.Source.StartsWith("[!TIP]"))
                {
                    attribute = @" class=""tip""";
                }
                else if (blockquote.Source.StartsWith("[!IMPORTANT]"))
                {
                    attribute = @" class=""important""";
                }
                else if (blockquote.Source.StartsWith("[!WARNING]"))
                {
                    attribute = @" class=""warning""";
                }
                else if (blockquote.Source.StartsWith("[!CAUTION]"))
                {
                    attribute = @" class=""caution""";
                }
                blockquote.Children = Build(blockquote, Regex.Matches(blockquote.Source, @$"({P}|{H1}|{H2}|{H3}|{H4}|{H5}|{H6}|{BLOCKQUOTE}|{UL_OL})", RegexOptions.Multiline).ToArray(), cancellationToken)
                    .ToBlockingEnumerable(cancellationToken)
                    .ToArray();
                blockquote.Built = $"<blockquote{attribute}>{blockquote.Children.Build()}</blockquote>";
                yield return blockquote;
                continue;
            }

            {
                var content = match.Groups["UL_OL"].Value;
                if (!string.IsNullOrWhiteSpace(content))
                {
                    if (!string.IsNullOrWhiteSpace(match.Groups["UL"].Value))
                    {
                        var ul = new Ul
                        {
                            Source = content,
                            Parent = parent,
                        };
                        ul.Children = Build(ul, RegexLi.Matches(content).ToArray(), cancellationToken)
                            .ToBlockingEnumerable(cancellationToken)
                            .ToArray();
                        ul.Built = $"<ul>{ul.Children.Build()}</ul>";
                        yield return ul;
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(match.Groups["OL"].Value))
                    {
                        var ol = new Ol
                        {
                            Source = content,
                            Parent = parent,
                        };
                        ol.Children = Build(ol, RegexLi.Matches(content).ToArray(), cancellationToken)
                            .ToBlockingEnumerable(cancellationToken)
                            .ToArray();
                        ol.Built = $"<ol>{ol.Children.Build()}</ol>";
                        yield return ol;
                        continue;
                    }
                }
            }

            {
                var content = match.Groups["LI"].Value;
                if (!string.IsNullOrWhiteSpace(content))
                {
                    var li = new LI
                    {
                        Source = content,
                        Parent = parent,
                    };
                    li.Children = Build(li, content, cancellationToken)
                        .ToBlockingEnumerable(cancellationToken)
                        .ToArray();
                    li.Built = $"<li>{li.Children.Build()}</li>";
                    yield return li;
                    continue;
                }
            }

            {
                var content = match.Groups["P"].Value;
                if (!string.IsNullOrWhiteSpace(content))
                {
                    var p = new P
                    {
                        Source = content,
                        Parent = parent,
                    };
                    p.Children = mediator
                        .CreateStream(new TextBuildRequest { Parent = p, Source = content }, cancellationToken)
                        .ToBlockingEnumerable(cancellationToken)
                        .ToArray();
                    p.Built = $"<p>{p.Children.Build()}</p>";
                    yield return p;
                    continue;
                }
            }
            {
                var index = match.Groups["CITE_INDEX"].Value;
                var content = match.Groups["CITE_CONTENT"].Value;
                if (!string.IsNullOrWhiteSpace(index))
                {
                    var cite = new Cite
                    {
                        Source = content,
                        Parent = parent,
                    };
                    cite.Children = mediator
                        .CreateStream(new TextBuildRequest { Parent = cite, Source = content }, cancellationToken)
                        .ToBlockingEnumerable(cancellationToken)
                        .ToArray();
                    cite.Built = @$"<cite id=""cite-{index}""><a href=""#cited-{index}"">({index})</a>. {cite.Children.Build()}</cite>";
                    yield return cite;
                    continue;
                }
            }
            {
                var content = match.Groups["TEXT"].Value;
                if (!string.IsNullOrWhiteSpace(content))
                {
                    await foreach (var text in mediator.CreateStream(new TextBuildRequest { Parent = parent, Source = content }, cancellationToken))
                        yield return text;
                    continue;
                }
            }
            var debug = string.Empty;
        }
    }
}
