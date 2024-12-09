using System.Globalization;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MediatR;
internal sealed class BuildRequest : IRequest<BuildResponse>
{
    public string? Title { get; init; }
    public string? Source { get; init; }
    public string? Url { get; internal set; }
}
internal sealed class BuildResponse
{
    internal Html? Target { get; init; }
}
internal sealed class BuildRequestHandler : IRequestHandler<BuildRequest, BuildResponse>
{
    const string P = @"^(?'P'((?!(#|>| *-| *\d+\.|\[\^\d+\]:)).+(\r?\n|))+(\r?\n|))";
    const string H1 = @"^(?'H1'# *(?'H1_CONTENT'(?!#).+(\r?\n|)))";
    const string H2 = @"^(?'H2'## *(?'H2_CONTENT'(?!#).+(\r?\n|)))";
    const string H3 = @"^(?'H3'### *(?'H3_CONTENT'(?!#).+(\r?\n|)))";
    const string H4 = @"^(?'H4'#### *(?'H4_CONTENT'(?!#).+(\r?\n|)))";
    const string H5 = @"^(?'H5'##### *(?'H5_CONTENT'(?!#).+(\r?\n|)))";
    const string H6 = @"^(?'H6'###### *(?'H6_CONTENT'(?!#).+(\r?\n|)))";
    const string BLOCKQUOTE = @"^(?'BLOCKQUOTE'> *(?'BLOCKQUOTE_CONTENT'.*(\r?\n|)))+";
    const string UL_OL = @"^(?'UL_OL'(((?'UL'-)|(?'OL'\d+\.)) *.+(\r?\n|))( *((-)|(\d+\.)) *.+(\r?\n|))*(\r?\n|))";
    const string UL_OL_INNER = @"^(((.+?\r?\n))(?'UL_OL'( *((-)|(\d+\.)) *.+(\r?\n|))*(\r?\n|)))";
    const string LI = @"^(-|\d+\.) *(?'LI'(.*(\r?\n|)+(?!(-|\d+\.)))+(\r?\n|))";
    const string I = @"(?'I'\*{1}(?'I_CONTENT'[^\*| ].+?)\*{1})";
    const string B = @"(?'B'\*{2}(?'B_CONTENT'[^\*| ].+?)\*{2})";
    const string BI = @"(?'BI'\*{3}(?'BI_CONTENT'[^\*| ].+?)\*{3})";
    const string TEXT = @"^(?'TEXT'((.*(\r?\n|))*))";
    const string BR = @"(?'BR'\\(\r?\n))";
    const string AGE_CALC = @"(?'AGE_CALC'`\[age-calc\]:(?'AGE_CALC_CONTENT'[\d]{4}\-[\d]{2}\-[\d]{2})\`)";
    const string A = @"(?'A'\[(?!\^)(?'A_CONTENT'.*?)\]\((?'A_HREF'.*?)(readme.md|)\))";
    const string SVG_NOTE = @"(?'SVG_NOTE'\[!NOTE\])";
    const string SVG_TIP = @"(?'SVG_TIP'\[!TIP\])";
    const string SVG_IMPORTANT = @"(?'SVG_IMPORTANT'\[!IMPORTANT\])";
    const string SVG_WARNING = @"(?'SVG_WARNING'\[!WARNING\])";
    const string SVG_CAUTION = @"(?'SVG_CAUTION'\[!CAUTION\])";
    const string CITE = @"^\[\^(?'CITE_INDEX'\d+)\]: +(?'CITE_CONTENT'.*)";
    const string CITED = @$"\[\^(?'CITED_INDEX'\d+)\]";
    const string STYLE = @"
:root {
    --color-background-opacity: 0.1;
}
@media (prefers-color-scheme: dark) {
    :root {
        --color-note-a0: #121929;
        --color-note-a10: #132440;
        --color-note-a20: #172f55;
        --color-note-a30: #193d75;
        --color-note-a40: #1b4fa0;
        --color-note-a50: #1d61cb;
        --color-note-a60: #4387e0;
        --color-note-a70: #6eacf3;
        --color-note-a80: #98c7f8;
        --color-note-a90: #c0e0fa;

        --color-tip-a0: #152217;
        --color-tip-a10: #1a341e;
        --color-tip-a20: #214626;
        --color-tip-a30: #275e2f;
        --color-tip-a40: #307f3b;
        --color-tip-a50: #39a047;
        --color-tip-a60: #5bb564;
        --color-tip-a70: #86ca8a;
        --color-tip-a80: #b3dab4;
        --color-tip-a90: #dbe9db;

        --color-important-a0: #201b2b;
        --color-important-a10: #302743;
        --color-important-a20: #413458;
        --color-important-a30: #58437b;
        --color-important-a40: #7658a8;
        --color-important-a50: #946dd6;
        --color-important-a60: #b999e8;
        --color-important-a70: #dbc8f3;
        --color-important-a80: #f0e9f8;
        --color-important-a90: #f2ecfa;

        --color-warning-a0: #251e12;
        --color-warning-a10: #3a2d14;
        --color-warning-a20: #4d3c18;
        --color-warning-a30: #6a501a;
        --color-warning-a40: #906a1d;
        --color-warning-a50: #b68520;
        --color-warning-a60: #caa342;
        --color-warning-a70: #e0c36c;
        --color-warning-a80: #f0de9a;
        --color-warning-a90: #faf1c8;

        --color-caution-a0: #2b1616;
        --color-caution-a10: #431c1c;
        --color-caution-a20: #582624;
        --color-caution-a30: #7b2f2c;
        --color-caution-a40: #a83c36;
        --color-caution-a50: #d64841;
        --color-caution-a60: #e8736a;
        --color-caution-a70: #f3a095;
        --color-caution-a80: #f8c7bf;
        --color-caution-a90: #faece9;

        --color-surface-a0: #232323;
        --color-surface-a10: #363636;
        --color-surface-a20: #484848;
        --color-surface-a30: #626262;
        --color-surface-a40: #858585;
        --color-surface-a50: #a8a8a8;
        --color-surface-a60: #bcbcbc;
        --color-surface-a70: #d1d1d1;
        --color-surface-a80: #e2e2e2;
        --color-surface-a90: #f1f1f1;
    }
}

@media (prefers-color-scheme: light) {
    :root {
        --color-note-a0: #edf7ff;
        --color-note-a10: #c4e4ff;
        --color-note-a20: #9ccdff;
        --color-note-a30: #73b4ff;
        --color-note-a40: #4894f7;
        --color-note-a50: #1f6feb;
        --color-note-a60: #1052c4;
        --color-note-a70: #05389e;
        --color-note-a80: #002478;
        --color-note-a90: #001652;

        --color-tip-a0: #ebfaeb;
        --color-tip-a10: #dfeddf;
        --color-tip-a20: #b8e0b9;
        --color-tip-a30: #8cd490;
        --color-tip-a40: #63c76d;
        --color-tip-a50: #3fb950;
        --color-tip-a60: #2b943c;
        --color-tip-a70: #1a6e2b;
        --color-tip-a80: #0e471b;
        --color-tip-a90: #06210d;

        --color-important-a0: #f8f0ff;
        --color-important-a10: #f7f0ff;
        --color-important-a20: #f7f0ff;
        --color-important-a30: #e5d1ff;
        --color-important-a40: #cba8ff;
        --color-important-a50: #ab7df8;
        --color-important-a60: #845ed1;
        --color-important-a70: #6344ab;
        --color-important-a80: #452e85;
        --color-important-a90: #2f205e;

        --color-warning-a0: #fffdf0;
        --color-warning-a10: #fff6cc;
        --color-warning-a20: #f7e49e;
        --color-warning-a30: #ebcc71;
        --color-warning-a40: #deb347;
        --color-warning-a50: #d29922;
        --color-warning-a60: #ab7613;
        --color-warning-a70: #855508;
        --color-warning-a80: #5e3701;
        --color-warning-a90: #381f00;

        --color-caution-a0: #fff3f0;
        --color-caution-a10: #fff0ed;
        --color-caution-a20: #ffcdc4;
        --color-caution-a30: #ffa79c;
        --color-caution-a40: #ff7e73;
        --color-caution-a50: #f85149;
        --color-caution-a60: #d13532;
        --color-caution-a70: #ab2023;
        --color-caution-a80: #851318;
        --color-caution-a90: #5e0c13;

        --color-surface-a0: #ffffff;
        --color-surface-a10: #f5f5f5;
        --color-surface-a20: #e8e8e8;
        --color-surface-a30: #dbdbdb;
        --color-surface-a40: #cfcfcf;
        --color-surface-a50: #c2c2c2;
        --color-surface-a60: #9c9c9c;
        --color-surface-a70: #757575;
        --color-surface-a80: #4f4f4f;
        --color-surface-a90: #292929;
    }
}

html, body {
    font-family: Segoe UI, SegoeUI, Helvetica Neue, Helvetica, Arial, sans-serif;
}
html 
{
    display: flex; 
    justify-content: center;
    color: var(--color-surface-a90);
    background: var(--color-surface-a0);
} 
html > body 
{
    display: flex; 
    flex-direction: column; 
    max-width: 1024px; 
    flex: 1 1 auto;
}
html > body a {
    color: var(--color-note-a60);
}

h1
{
    border-bottom: solid 0.085em var(--color-surface-a10);
}
a
{
    text-decoration: none;
}
li::marker
{
    color: var(--color-surface-a50);
}
p, h1, h2, h3, h4, h5, h6, ul, ol
{
    margin-block-start: 0.2em;
    margin-block-end: 1em;
}
blockquote
{
    margin-block-start: 0.85em;
    margin-block-end: 0.85em;
    margin-inline-start: 0em;
    margin-inline-end: 0em;
    padding-inline-start: 1em;
    border-left: solid 0.25em var(--color-surface-a50);
    background-color: rgb(from var(--color-surface-a50) r g b / var(--color-background-opacity));
    color: var(--color-surface-a70);
}
blockquote > p
{
    margin-block-start: 0.2em;
    margin-block-end: 0.5em;
}
blockquote > p > span.icon {
    display: flex;
    flex-direction: row;
    align-items: center;
    margin-top: 0.85em;
    margin-bottom: 0.85em;
    column-gap: 0.5em;
}
blockquote.note
{
    border-left: solid 0.25em var(--color-note-a50);
    background-color: rgb(from var(--color-note-a50) r g b / var(--color-background-opacity));
}
blockquote.note > p > span.icon
{
    color: var(--color-note-a50);
}
blockquote.tip
{
    border-left: solid 0.25em var(--color-tip-a50);
    background-color: rgb(from var(--color-tip-a50) r g b / var(--color-background-opacity));
}
blockquote.tip > p > span.icon
{
    color: var(--color-tip-a50);
}
blockquote.important
{
    border-left: solid 0.25em var(--color-important-a50);
    background-color: rgb(from var(--color-important-a50) r g b / var(--color-background-opacity));
}
blockquote.important > p > span.icon
{
    color: var(--color-important-a50);
}
blockquote.warning
{
    border-left: solid 0.25em var(--color-warning-a50);
    background-color: rgb(from var(--color-warning-a50) r g b / var(--color-background-opacity));
}
blockquote.warning > p > span.icon
{
    color: var(--color-warning-a50);
}
blockquote.caution
{
    border-left: solid 0.25em var(--color-caution-a50);
    background-color: rgb(from var(--color-caution-a50) r g b / var(--color-background-opacity));
}
blockquote.caution > p > span.icon
{
    color: var(--color-caution-a50);
}
cite
{
    font-size: 0.85em;
    color: var(--color-surface-a70);
    vertical-align: super;
}
";
    public async Task<BuildResponse> Handle(BuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return new BuildResponse();

        return new BuildResponse
        {
            Target = Build(request),
        };
    }

    private Html Build(BuildRequest request)
    {
        var element = new Html
        {
            Title = request.Title,
            Source = request.Source,
            Parent = default,
        };
        element.Children = Build(element, request);
        element.Built = $@"<!DOCTYPE html><html lang=""pt-BR""><head><title>{element.Title}</title><meta content=""text/html; charset=UTF-8;"" http-equiv=""Content-Type"" /><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""><meta name=""color-scheme"" content=""dark light""><style>{STYLE}</style></head>{element.Children.Build()}</html>";
        return element;
    }

    private static IElement[] Build(Html html, BuildRequest request)
    {
        var body = new Body
        {
            Title = request.Title,
            Url = request.Url,
            Source = request.Source,
            Parent = html,
        };
        body.Children = Build(body, request.Source).ToArray();
        body.Built = @$"<body><h1><a href=""{body.Url}""/>{body.Title}</a></h1>{body.Children.Build()}</body>";
        return [body];
    }

    private static IEnumerable<IElement> Build(IElement? parent, string? source)
    {
        if (source == default)
            yield break;

        foreach (IElement element in Build(parent, Regex.Matches(source, @$"({P}|{H1}|{H2}|{H3}|{H4}|{H5}|{H6}|{BLOCKQUOTE}|{UL_OL}|{CITE})", RegexOptions.Multiline)))
            yield return element;
    }

    private static IEnumerable<IElement> Build(Blockquote? parent, string? source)
    {
        if (source == default)
            yield break;

        foreach (IElement element in Build(parent, Regex.Matches(source, @$"({P}|{H1}|{H2}|{H3}|{H4}|{H5}|{H6}|{BLOCKQUOTE}|{UL_OL})", RegexOptions.Multiline)))
            yield return element;
    }

    private static IEnumerable<IElement> Build(LI? parent, string? source)
    {
        if (source == default)
            yield break;

        var matches = Regex.Matches(source, @$"{UL_OL_INNER}", RegexOptions.Multiline);
        foreach (Group match in matches.Select(m => m.Groups["UL_OL"]).Where(g => g.Success && !string.IsNullOrWhiteSpace(g.Value)))
        {
            var sourceInner = Regex.Replace(match.Value, "^    ", string.Empty, RegexOptions.Multiline);
            source = source.Replace(match.Value, Build(parent, Regex.Matches(sourceInner, @$"({UL_OL})"))?.SingleOrDefault()?.Built);
        }

        foreach (IElement element in Build(parent, Regex.Matches(source, @$"({TEXT})", RegexOptions.Singleline)))
            yield return element;
    }

    private static IEnumerable<IElement> Build(Ul? parent, string? source)
    {
        if (source == default)
            yield break;

        foreach (IElement element in Build(parent, Regex.Matches(source, @$"({LI})", RegexOptions.Multiline)))
            yield return element;
    }

    private static IEnumerable<IElement> Build(Ol? parent, string? source)
    {
        if (source == default)
            yield break;

        foreach (IElement element in Build(parent, Regex.Matches(source, @$"({LI})", RegexOptions.Multiline)))
            yield return element;
    }

    private static IEnumerable<IElement> Build(H1? parent, string? source)
    {
        if (source == default)
            yield break;

        foreach (IElement element in Build(parent, Regex.Matches(source, @$"({TEXT})", RegexOptions.Multiline)))
            yield return element;
    }

    private static IEnumerable<IElement> Build(H2? parent, string? source)
    {
        if (source == default)
            yield break;

        foreach (IElement element in Build(parent, Regex.Matches(source, @$"({TEXT})", RegexOptions.Multiline)))
            yield return element;
    }

    private static IEnumerable<IElement> Build(H3? parent, string? source)
    {
        if (source == default)
            yield break;

        foreach (IElement element in Build(parent, Regex.Matches(source, @$"({TEXT})", RegexOptions.Multiline)))
            yield return element;
    }

    private static IEnumerable<IElement> Build(H4? parent, string? source)
    {
        if (source == default)
            yield break;

        foreach (IElement element in Build(parent, Regex.Matches(source, @$"({TEXT})", RegexOptions.Multiline)))
            yield return element;
    }

    private static IEnumerable<IElement> Build(H5? parent, string? source)
    {
        if (source == default)
            yield break;

        foreach (IElement element in Build(parent, Regex.Matches(source, @$"({TEXT})", RegexOptions.Multiline)))
            yield return element;
    }

    private static IEnumerable<IElement> Build(H6? parent, string? source)
    {
        if (source == default)
            yield break;

        foreach (IElement element in Build(parent, Regex.Matches(source, @$"({TEXT})", RegexOptions.Multiline)))
            yield return element;
    }

    private static IEnumerable<IElement> Build(P? parent, string? source)
    {
        if (source == default)
            yield break;

        foreach (IElement element in Build(parent, Regex.Matches(source, @$"({TEXT})", RegexOptions.Multiline)))
            yield return element;
    }

    private static IEnumerable<IElement> Build(Cite? parent, string? source)
    {
        if (source == default)
            yield break;

        foreach (IElement element in Build(parent, Regex.Matches(source, @$"({TEXT})", RegexOptions.Multiline)))
            yield return element;
    }

    private static string? Build(string? source)
    {
        if (source == default)
            return source;
        var target = source;

        target = Regex.Replace(target, @$"({BR})", (match) =>
        {
            return $"<br />";
        }, RegexOptions.Multiline);

        target = Regex.Replace(target, @$"({BI})", (match) =>
        {
            return $"<b><i>{match.Groups["BI_CONTENT"].Value}</i></b>";
        }, RegexOptions.Multiline);

        target = Regex.Replace(target, @$"({B})", (match) =>
        {
            return $"<b>{match.Groups["B_CONTENT"].Value}</b>";
        }, RegexOptions.Multiline);

        target = Regex.Replace(target, @$"({I})", (match) =>
        {
            return $"<i>{match.Groups["I_CONTENT"].Value}</i>";
        }, RegexOptions.Multiline);

        target = Regex.Replace(target, @$"({AGE_CALC})", (match) =>
        {
            return AgeCalculate(DateTime.ParseExact(match.Groups["AGE_CALC_CONTENT"].Value, "yyyy-mm-dd", CultureInfo.InvariantCulture)).ToString();
        }, RegexOptions.Multiline);

        target = Regex.Replace(target, @$"({A})", (match) =>
        {
            return $@"<a href=""{match.Groups["A_HREF"].Value}"">{match.Groups["A_CONTENT"].Value}</a>";
        }, RegexOptions.Multiline | RegexOptions.IgnoreCase);

        target = Regex.Replace(target, @$"({SVG_NOTE})", (match) =>
        {
            return $@"<span class=""icon"">{SvgBuild(
                "var(--color-note-a50)",
                "M0 8a8 8 0 1 1 16 0A8 8 0 0 1 0 8Zm8-6.5a6.5 6.5 0 1 0 0 13 6.5 6.5 0 0 0 0-13ZM6.5 7.75A.75.75 0 0 1 7.25 7h1a.75.75 0 0 1 .75.75v2.75h.25a.75.75 0 0 1 0 1.5h-2a.75.75 0 0 1 0-1.5h.25v-2h-.25a.75.75 0 0 1-.75-.75ZM8 6a1 1 0 1 1 0-2 1 1 0 0 1 0 2Z"
            )} <b>Nota</b></span>";
        }, RegexOptions.Multiline);

        target = Regex.Replace(target, @$"({SVG_TIP})", (match) =>
        {
            return $@"<span class=""icon"">{SvgBuild(
                "var(--color-tip-a50)",
                "M8 1.5c-2.363 0-4 1.69-4 3.75 0 .984.424 1.625.984 2.304l.214.253c.223.264.47.556.673.848.284.411.537.896.621 1.49a.75.75 0 0 1-1.484.211c-.04-.282-.163-.547-.37-.847a8.456 8.456 0 0 0-.542-.68c-.084-.1-.173-.205-.268-.32C3.201 7.75 2.5 6.766 2.5 5.25 2.5 2.31 4.863 0 8 0s5.5 2.31 5.5 5.25c0 1.516-.701 2.5-1.328 3.259-.095.115-.184.22-.268.319-.207.245-.383.453-.541.681-.208.3-.33.565-.37.847a.751.75 0 0 1-1.485-.212c.084-.593.337-1.078.621-1.489.203-.292.45-.584.673-.848.075-.088.147-.173.213-.253.561-.679.985-1.32.985-2.304 0-2.06-1.637-3.75-4-3.75ZM5.75 12h4.5a.75.75 0 0 1 0 1.5h-4.5a.75.75 0 0 1 0-1.5ZM6 15.25a.75.75 0 0 1 .75-.75h2.5a.75.75 0 0 1 0 1.5h-2.5a.75.75 0 0 1-.75-.75Z"
            )} <b>Dica</b></span>";
        }, RegexOptions.Multiline);

        target = Regex.Replace(target, @$"({SVG_IMPORTANT})", (match) =>
        {
            return $@"<span class=""icon"">{SvgBuild(
                "var(--color-important-a50)",
                "M0 1.75C0 .784.784 0 1.75 0h12.5C15.216 0 16 .784 16 1.75v9.5A1.75 1.75 0 0 1 14.25 13H8.06l-2.573 2.573A1.458 1.458 0 0 1 3 14.543V13H1.75A1.75 1.75 0 0 1 0 11.25Zm1.75-.25a.25.25 0 0 0-.25.25v9.5c0 .138.112.25.25.25h2a.75.75 0 0 1 .75.75v2.19l2.72-2.72a.749.749 0 0 1 .53-.22h6.5a.25.25 0 0 0 .25-.25v-9.5a.25.25 0 0 0-.25-.25Zm7 2.25v2.5a.75.75 0 0 1-1.5 0v-2.5a.75.75 0 0 1 1.5 0ZM9 9a1 1 0 1 1-2 0 1 1 0 0 1 2 0Z"
            )} <b>Importante</b></span>";
        }, RegexOptions.Multiline);

        target = Regex.Replace(target, @$"({SVG_WARNING})", (match) =>
        {
            return $@"<span class=""icon"">{SvgBuild(
                "var(--color-warning-a50)",
                "M6.457 1.047c.659-1.234 2.427-1.234 3.086 0l6.082 11.378A1.75 1.75 0 0 1 14.082 15H1.918a1.75 1.75 0 0 1-1.543-2.575Zm1.763.707a.25.25 0 0 0-.44 0L1.698 13.132a.25.25 0 0 0 .22.368h12.164a.25.25 0 0 0 .22-.368Zm.53 3.996v2.5a.75.75 0 0 1-1.5 0v-2.5a.75.75 0 0 1 1.5 0ZM9 11a1 1 0 1 1-2 0 1 1 0 0 1 2 0Z"
            )} <b>Aviso</b></span>";
        }, RegexOptions.Multiline);

        target = Regex.Replace(target, @$"({SVG_CAUTION})", (match) =>
        {
            return $@"<span class=""icon"">{SvgBuild(
                "var(--color-caution-a50)",
                "M4.47.22A.749.749 0 0 1 5 0h6c.199 0 .389.079.53.22l4.25 4.25c.141.14.22.331.22.53v6a.749.749 0 0 1-.22.53l-4.25 4.25A.749.749 0 0 1 11 16H5a.749.749 0 0 1-.53-.22L.22 11.53A.749.749 0 0 1 0 11V5c0-.199.079-.389.22-.53Zm.84 1.28L1.5 5.31v5.38l3.81 3.81h5.38l3.81-3.81V5.31L10.69 1.5ZM8 4a.75.75 0 0 1 .75.75v3.5a.75.75 0 0 1-1.5 0v-3.5A.75.75 0 0 1 8 4Zm0 8a1 1 0 1 1 0-2 1 1 0 0 1 0 2Z"
            )} <b>Cuidado</b></span>";
        }, RegexOptions.Multiline);

        target = Regex.Replace(target, @$"({CITED})", (match) =>
        {
            var index = match.Groups["CITED_INDEX"].Value;
            return @$"<cite id=""cited-{index}""><a href=""#cite-{index}"">({index})</a></cite>";
        }, RegexOptions.Multiline);

        return target;
    }
    private static int AgeCalculate(DateTime birthDate)
    {
        DateTime today = DateTime.Today;

        int age = today.Year - birthDate.Year;

        if (birthDate.Date > today.AddYears(-age).Date)
            return age - 1;

        return age;
    }
    static string SvgBuild(string color, string shape)
    {
        var namespaceUri = "http://www.w3.org/2000/svg";
        var svgElement = new XElement(XName.Get("svg", namespaceUri),
            new XAttribute("viewBox", "0 0 16 16"),
            new XAttribute("version", "1.1"),
            new XAttribute("width", "16"),
            new XAttribute("height", "16"),
            new XAttribute("aria-hidden", "true"),
            new XElement(XName.Get("path", namespaceUri),
                new XAttribute("d", shape),
                new XAttribute("style", $"fill:{color}")
            )
        );

        return svgElement.ToString(SaveOptions.DisableFormatting);
    }

    private static IEnumerable<IElement> Build(IElement? parent, MatchCollection matches)
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
                h1.Children = Build(h1, content).ToArray();
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
                h2.Children = Build(h2, content).ToArray();
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
                h3.Children = Build(h3, content).ToArray();
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
                h4.Children = Build(h4, content).ToArray();
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
                h5.Children = Build(h5, content).ToArray();
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
                h6.Children = Build(h6, content).ToArray();
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
                if(blockquote.Source.StartsWith("[!NOTE]")) {
                    attribute = @" class=""note""";
                } 
                else if(blockquote.Source.StartsWith("[!TIP]")) {
                    attribute = @" class=""tip""";
                }
                else if(blockquote.Source.StartsWith("[!IMPORTANT]")) {
                    attribute = @" class=""important""";
                }
                else if(blockquote.Source.StartsWith("[!WARNING]")) {
                    attribute = @" class=""warning""";
                }
                else if(blockquote.Source.StartsWith("[!CAUTION]")) {
                    attribute = @" class=""caution""";
                }
                blockquote.Children = Build(blockquote, blockquote.Source).ToArray();
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
                        ul.Children = Build(ul, content).ToArray();
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
                        ol.Children = Build(ol, content).ToArray();
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
                    li.Children = Build(li, content).ToArray();
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
                    p.Children = Build(p, content).ToArray();
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
                    cite.Children = Build(cite, content).ToArray();
                    cite.Built = @$"<cite id=""cite-{index}""><a href=""#cited-{index}"">({index})</a>. {cite.Children.Build()}</cite>";
                    yield return cite;
                    continue;
                }
            }
            {
                var content = match.Groups["TEXT"].Value;
                if (!string.IsNullOrWhiteSpace(content))
                {
                    var text = new Text
                    {
                        Source = content,
                        Parent = parent,
                        Children = default,
                        Built = Build(content),
                    };
                    yield return text;
                    continue;
                }
            }
            var debug = string.Empty;
        }
    }
}
