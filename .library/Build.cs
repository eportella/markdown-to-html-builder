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
    const string CITE = @"^\[\^(?'CITE_INDEX'\d+)\]: +(?'CITE_CONTENT'.*)";
    static Regex RegexLi { get; }
    static Regex RegexBody { get; }
    static Regex RegexBlockquote { get; }
    static Regex RegexOlUl { get; }
    static Regex RegexOlUlInner { get; }

    static BuildRequestHandler()
    {
        RegexLi = new Regex(LI, RegexOptions.Multiline);
        RegexBody = new Regex(@$"({P}|{H1}|{H2}|{H3}|{H4}|{H5}|{H6}|{BLOCKQUOTE}|{UL_OL}|{CITE})", RegexOptions.Multiline);
        RegexBlockquote = new Regex(@$"({P}|{H1}|{H2}|{H3}|{H4}|{H5}|{H6}|{BLOCKQUOTE}|{UL_OL})", RegexOptions.Multiline);
        RegexOlUl = new Regex(UL_OL);
        RegexOlUlInner = new Regex(UL_OL_INNER, RegexOptions.Multiline);
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
        var body = Build(
                html,
                request,
                cancellationToken
            );
        html.Built = $@"<!DOCTYPE html><html lang=""pt-BR""><head><title>{project.Title}</title><meta content=""text/html; charset=UTF-8;"" http-equiv=""Content-Type"" /><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""><meta name=""color-scheme"" content=""dark light""><link rel=""stylesheet"" href=""{project.BaseUrl!.ToString().TrimEnd('/')}/stylesheet.css""></style></head>{body?.Built}</html>";
        return html;
    }

    private Body? Build(Html html, BuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;
        var body = new Body
        {
            Source = request.Source,
            Parent = html,
        };
        var children = Build(
                body,
                RegexBody.Matches(body.Source),
                cancellationToken
            )
            .ToBlockingEnumerable(cancellationToken)
            .ToArray();
        body.Built = @$"<body><h1><a href=""{project.BaseUrl}""/>{project.Title}</a></h1>{children.Build()}{(project.OwnerTitle != default && project.OwnerBaseUrl != default ? @$"<span class=""owner""><a href=""{project.OwnerBaseUrl}""/>{project.OwnerTitle}</a></span>" : string.Empty)}</body>";
        return body;
    }
    private async IAsyncEnumerable<IElement> Build(LI? parent, string? source, [EnumeratorCancellation] CancellationToken cancellationToken)
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
                    Build(
                        parent,
                        RegexOlUl
                            .Matches(Regex
                            .Replace(
                                group.Value,
                                "^    ",
                                string.Empty,
                                RegexOptions.Multiline
                            )
                        ),
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
    private async IAsyncEnumerable<IElement> Build(IElement? parent, MatchCollection matches, [EnumeratorCancellation] CancellationToken cancellationToken)
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
                var children = mediator
                    .CreateStream(new TextBuildRequest { Parent = h1, Source = content }, cancellationToken)
                    .ToBlockingEnumerable(cancellationToken)
                    .ToArray();
                h1.Built = $"<h1>{children.Build()}</h1>";
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
                var children = mediator
                    .CreateStream(new TextBuildRequest { Parent = h2, Source = content }, cancellationToken)
                    .ToBlockingEnumerable(cancellationToken)
                    .ToArray();
                h2.Built = $"<h2>{children.Build()}</h2>";
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
                var children = mediator
                    .CreateStream(new TextBuildRequest { Parent = h3, Source = content }, cancellationToken)
                    .ToBlockingEnumerable(cancellationToken)
                    .ToArray();
                h3.Built = $"<h3>{children.Build()}</h3>";
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
                var children = mediator
                    .CreateStream(new TextBuildRequest { Parent = h4, Source = content }, cancellationToken)
                    .ToBlockingEnumerable(cancellationToken)
                    .ToArray();
                h4.Built = $"<h4>{children.Build()}</h4>";
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
                var children = mediator
                    .CreateStream(new TextBuildRequest { Parent = h5, Source = content }, cancellationToken)
                    .ToBlockingEnumerable(cancellationToken)
                    .ToArray();
                h5.Built = $"<h5>{children.Build()}</h5>";
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
                var children = mediator
                    .CreateStream(new TextBuildRequest { Parent = h6, Source = content }, cancellationToken)
                    .ToBlockingEnumerable(cancellationToken)
                    .ToArray();
                h6.Built = $"<h6>{children.Build()}</h6>";
                yield return h6;
                continue;
            }
            if (!string.IsNullOrWhiteSpace(match.Groups["BLOCKQUOTE"].Value))
            {
                var blockquote = new Blockquote
                {
                    Source = string.Join(string.Empty, match.Groups["BLOCKQUOTE_CONTENT"].Captures.Select(c => c.Value)),
                    Parent = parent,
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
                var children = Build(
                        blockquote,
                        RegexBlockquote.Matches(blockquote.Source),
                        cancellationToken
                    )
                    .ToBlockingEnumerable(cancellationToken)
                    .ToArray();
                blockquote.Built = $"<blockquote{attribute}>{children.Build()}</blockquote>";
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
                        var children = Build(ul, RegexLi.Matches(content), cancellationToken)
                            .ToBlockingEnumerable(cancellationToken)
                            .ToArray();
                        ul.Built = $"<ul>{children.Build()}</ul>";
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
                        var children = Build(ol, RegexLi.Matches(content), cancellationToken)
                            .ToBlockingEnumerable(cancellationToken)
                            .ToArray();
                        ol.Built = $"<ol>{children.Build()}</ol>";
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
                    var children = Build(li, content, cancellationToken)
                        .ToBlockingEnumerable(cancellationToken)
                        .ToArray();
                    li.Built = $"<li>{children.Build()}</li>";
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
                    var children = mediator
                        .CreateStream(new TextBuildRequest { Parent = p, Source = content }, cancellationToken)
                        .ToBlockingEnumerable(cancellationToken)
                        .ToArray();
                    p.Built = $"<p>{children.Build()}</p>";
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
                    var children = mediator
                        .CreateStream(new TextBuildRequest { Parent = cite, Source = content }, cancellationToken)
                        .ToBlockingEnumerable(cancellationToken)
                        .ToArray();
                    cite.Built = @$"<cite id=""cite-{index}""><a href=""#cited-{index}"">({index})</a>. {children.Build()}</cite>";
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
