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
        };
        if (request.Source == default)
            return html;

        var body = new Body
        {
            Source = request.Source,
        };

        var children = RegexBody.Replace(body.Source, (match) => { return Replace(match, cancellationToken).Result!; });

        body.Built = @$"<body><h1><a href=""{project.BaseUrl}""/>{project.Title}</a></h1>{children}{(project.OwnerTitle != default && project.OwnerBaseUrl != default ? @$"<span class=""owner""><a href=""{project.OwnerBaseUrl}""/>{project.OwnerTitle}</a></span>" : string.Empty)}</body>";
        html.Built = $@"<!DOCTYPE html><html lang=""pt-BR""><head><title>{project.Title}</title><meta content=""text/html; charset=UTF-8;"" http-equiv=""Content-Type"" /><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""><meta name=""color-scheme"" content=""dark light""><link rel=""stylesheet"" href=""{project.BaseUrl!.ToString().TrimEnd('/')}/stylesheet.css""></style></head>{body?.Built}</html>";
        return html;
    }
    private async IAsyncEnumerable<IElement> Build(string? source, [EnumeratorCancellation] CancellationToken cancellationToken)
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
                                match => Replace(match, cancellationToken).Result!
                        )
                );
        }

        await foreach (var text in mediator.CreateStream(new TextBuildRequest { Source = source }, cancellationToken))
            yield return text;
    }
    private async Task<string?> Replace(Match match, CancellationToken cancellationToken)
    {

        if (!string.IsNullOrWhiteSpace(match.Groups["H1"].Value))
        {
            var content = match.Groups["H1_CONTENT"].Value;
            var h1 = new H1
            {
                Source = content,
            };
            var children = mediator
                .CreateStream(new TextBuildRequest { Source = content }, cancellationToken)
                .ToBlockingEnumerable(cancellationToken)
                .ToArray();
            h1.Built = $"<h1>{children.Build()}</h1>";
            return h1.Built;
        }
        if (!string.IsNullOrWhiteSpace(match.Groups["H2"].Value))
        {
            var content = match.Groups["H2_CONTENT"].Value;
            var h2 = new H2
            {
                Source = content,
            };
            var children = mediator
                .CreateStream(new TextBuildRequest { Source = content }, cancellationToken)
                .ToBlockingEnumerable(cancellationToken)
                .ToArray();
            h2.Built = $"<h2>{children.Build()}</h2>";
            return h2.Built;
        }
        if (!string.IsNullOrWhiteSpace(match.Groups["H3"].Value))
        {
            var content = match.Groups["H3_CONTENT"].Value;
            var h3 = new H3
            {
                Source = content,
            };
            var children = mediator
                .CreateStream(new TextBuildRequest { Source = content }, cancellationToken)
                .ToBlockingEnumerable(cancellationToken)
                .ToArray();
            h3.Built = $"<h3>{children.Build()}</h3>";
            return h3.Built;
        }
        if (!string.IsNullOrWhiteSpace(match.Groups["H4"].Value))
        {
            var content = match.Groups["H4_CONTENT"].Value;
            var h4 = new H4
            {
                Source = content,
            };
            var children = mediator
                .CreateStream(new TextBuildRequest { Source = content }, cancellationToken)
                .ToBlockingEnumerable(cancellationToken)
                .ToArray();
            h4.Built = $"<h4>{children.Build()}</h4>";
            return h4.Built;
        }
        if (!string.IsNullOrWhiteSpace(match.Groups["H5"].Value))
        {
            var content = match.Groups["H5_CONTENT"].Value;
            var h5 = new H5
            {
                Source = content,
            };
            var children = mediator
                .CreateStream(new TextBuildRequest { Source = content }, cancellationToken)
                .ToBlockingEnumerable(cancellationToken)
                .ToArray();
            h5.Built = $"<h5>{children.Build()}</h5>";
            return h5.Built;
        }
        if (!string.IsNullOrWhiteSpace(match.Groups["H6"].Value))
        {
            var content = match.Groups["H6_CONTENT"].Value;
            var h6 = new H6
            {
                Source = content,
            };
            var children = mediator
                .CreateStream(new TextBuildRequest { Source = content }, cancellationToken)
                .ToBlockingEnumerable(cancellationToken)
                .ToArray();
            h6.Built = $"<h6>{children.Build()}</h6>";
            return h6.Built;
        }
        if (!string.IsNullOrWhiteSpace(match.Groups["BLOCKQUOTE"].Value))
        {
            var blockquote = new Blockquote
            {
                Source = string.Join(string.Empty, match.Groups["BLOCKQUOTE_CONTENT"].Captures.Select(c => c.Value)),
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
            var children = RegexBlockquote.Replace(blockquote.Source, match => Replace(match, cancellationToken).Result!);
            blockquote.Built = $"<blockquote{attribute}>{children}</blockquote>";
            return blockquote.Built;
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
                    };
                    var replaced = RegexLi.Replace(content, match => Replace(match, cancellationToken).Result!);

                    ul.Built = $"<ul>{replaced}</ul>";
                    return ul.Built;
                }

                if (!string.IsNullOrWhiteSpace(match.Groups["OL"].Value))
                {
                    var ol = new Ol
                    {
                        Source = content,
                    };
                    var children = RegexLi.Replace(content, match => Replace(match, cancellationToken).Result!);
                    ol.Built = $"<ol>{children}</ol>";
                    return ol.Built;
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
                };
                var children = Build(content, cancellationToken)
                    .ToBlockingEnumerable(cancellationToken)
                    .ToArray();
                li.Built = $"<li>{children.Build()}</li>";
                return li.Built;
            }
        }

        {
            var content = match.Groups["P"].Value;
            if (!string.IsNullOrWhiteSpace(content))
            {
                var p = new P
                {
                    Source = content,
                };
                var children = mediator
                    .CreateStream(new TextBuildRequest { Source = content }, cancellationToken)
                    .ToBlockingEnumerable(cancellationToken)
                    .ToArray();
                p.Built = $"<p>{children.Build()}</p>";
                return p.Built;
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
                };
                var children = mediator
                    .CreateStream(new TextBuildRequest { Source = content }, cancellationToken)
                    .ToBlockingEnumerable(cancellationToken)
                    .ToArray();
                cite.Built = @$"<cite id=""cite-{index}""><a href=""#cited-{index}"">({index})</a>. {children.Build()}</cite>";
                return cite.Built;
            }
        }
        {
            var content = match.Groups["TEXT"].Value;
            if (!string.IsNullOrWhiteSpace(content))
            {
                return string.Join(string.Empty, mediator.CreateStream(new TextBuildRequest { Source = content }, cancellationToken).ToBlockingEnumerable(cancellationToken).Select(text => text.Built));
            }
        }
        return match.Value;
    }
}
