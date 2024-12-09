using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using MediatR;
internal sealed class TimeElapsedPipelineBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
        where TRequest : IRequest<TResponse>
{
    Stopwatch Stopwatch { get; }
    public TimeElapsedPipelineBehavior()
    {
        Stopwatch = new Stopwatch();
    }
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        Stopwatch.Start();

        var response = await next();
        Stopwatch.Stop();
        Console.WriteLine($"{typeof(TRequest).FullName} Time Elapsed {Stopwatch.ElapsedMilliseconds}ms");
        return response;
    }
}

internal sealed class TimeElapsedStreamPipelineBehavior<TRequest, TResponse> : IStreamPipelineBehavior<TRequest, TResponse>
        where TRequest : IStreamRequest<TResponse>
{
    Stopwatch Stopwatch { get; }
    public TimeElapsedStreamPipelineBehavior(
    )
    {
        Stopwatch = new Stopwatch();
    }
    public async IAsyncEnumerable<TResponse> Handle(
        TRequest request,
        StreamHandlerDelegate<TResponse> next,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        Stopwatch.Start();
        await foreach (var item in next())
            yield return item;
        Stopwatch.Stop();
        Console.WriteLine($"{typeof(TRequest).FullName} Time Elapsed {Stopwatch.ElapsedMilliseconds}ms");
    }
}

internal sealed class DirectoryInfoGetRequest : IRequest<DirectoryInfo?>
{
    public string? Path { get; set; }
}
internal sealed class RootDirectoryInfoGetRequestHandler : IRequestHandler<DirectoryInfoGetRequest, DirectoryInfo?>
{
    public async Task<DirectoryInfo?> Handle(DirectoryInfoGetRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        return new DirectoryInfo(request.Path!);
    }
}

internal sealed class HtmlFileGetStreamRequest : IStreamRequest<FileInfo>
{
    public DirectoryInfo? DirectoryInfo { get; init; }
}
internal sealed class HtmlFileGetStreamHandler : IStreamRequestHandler<HtmlFileGetStreamRequest, FileInfo>
{
    public async IAsyncEnumerable<FileInfo> Handle(HtmlFileGetStreamRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var item in request.DirectoryInfo!.EnumerateFiles("*.html", new EnumerationOptions() { RecurseSubdirectories = true }))
            yield return item;

        await Task.Yield();
    }
}

internal sealed class MarkdownFileInfoGetStreamRequest : IStreamRequest<FileInfo>
{
    public DirectoryInfo? DirectoryInfo { get; init; }
}
internal sealed class MarkdownFileInfoGetStreamHandler : IStreamRequestHandler<MarkdownFileInfoGetStreamRequest, FileInfo>
{
    public async IAsyncEnumerable<FileInfo> Handle(MarkdownFileInfoGetStreamRequest request, [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        foreach (var item in request.DirectoryInfo!.EnumerateFiles("*.md", new EnumerationOptions() { RecurseSubdirectories = true }))
            yield return item;

        await Task.Yield();
    }
}

internal sealed class FileInfoTextReadRequest : IRequest<string?>
{
    public FileInfo? FileInfo { get; init; }
}
internal sealed class StringGetRequestHandler : IRequestHandler<FileInfoTextReadRequest, string?>
{
    public async Task<string?> Handle(FileInfoTextReadRequest request, CancellationToken cancellationToken)
    {
        using var reader = request.FileInfo!.OpenText();
        return await reader.ReadToEndAsync();
    }
}

internal sealed class MarkdownFileInfoBuildRequest : IRequest
{
    public string? Title { get; init; }
    public string? Url { get; init; }
    public FileInfo? Source { get; init; }
    public FileInfo? Target { get; init; }
}
internal sealed class MarkdownFileInfoBuildRequesttHandler(IMediator mediator) : IRequestHandler<MarkdownFileInfoBuildRequest>
{
    public async Task Handle(MarkdownFileInfoBuildRequest request, CancellationToken cancellationToken)
    {
        var content = (
            await mediator
                .Send(new BuildRequest
                {
                    Title = request.Title,
                    Url = request.Url,
                    Source = await mediator
                            .Send(new FileInfoTextReadRequest
                            {
                                FileInfo = request.Source
                            },
                                cancellationToken
                            ),
                },
                    cancellationToken
                )
            )?
            .Target?
            .Built;

        if (!request.Target!.Directory!.Exists)
            request.Target.Directory.Create();

        var fileInfo = request.Target;
        using var fileStrem = fileInfo!.CreateText();
        await fileStrem.WriteAsync(content);
    }
}
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
internal class Html : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    internal string? Source { get; init; }
    public string? Built { get; internal set; }
    public string? Title { get; init; }
}
internal class Body : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
    public string? Title { get; init; }
    public string? Url { get; init; }
    internal string? Source { get; init; }
}
internal class P : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
    internal string? Source { get; init; }
}

internal class H1 : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
    internal string? Source { get; init; }
}

internal class H2 : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
    internal string? Source { get; init; }
}

internal class H3 : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
    internal string? Source { get; init; }
}

internal class H4 : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
    internal string? Source { get; init; }
}

internal class H5 : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
    internal string? Source { get; init; }
}

internal class H6 : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
    internal string? Source { get; init; }
}

internal class Blockquote : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
    internal string? Source { get; init; }
}

internal class Ul : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
    internal string? Source { get; init; }
}

internal class Ol : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
    internal string? Source { get; init; }
}

internal class LI : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
    internal string? Source { get; init; }
}

internal class Cite : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
    internal string? Source { get; init; }
}

internal class Text : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; init; }
    public string? Built { get; init; }
    internal string? Source { get; init; }
}

public interface IElement
{
    IElement? Parent { get; init; }
    IElement[]? Children { get; }
    string? Built { get; }
}

internal static class IElementExtensions
{
    internal static string? Build(this IElement[]? elements)
    {
        if (elements == default)
            return default;

        return string.Join(string.Empty, elements.BuiltEnumerable());
    }

    private static IEnumerable<string?> BuiltEnumerable(this IElement[] elements)
    {
        foreach (var element in elements)
            yield return element.Built;
    }
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
:root
{
    --color-note-a0: #1f6feb;
    --color-note-a10: #4e7dee;
    --color-note-a20: #6c8cf1;
    --color-note-a30: #859cf3;
    --color-note-a40: #9bacf6;
    --color-note-a50: #b0bcf8;

    --color-tip-a0: #3fb950;
    --color-tip-a10: #5bc164;
    --color-tip-a20: #73c977;
    --color-tip-a30: #89d18a;
    --color-tip-a40: #9ed99d;
    --color-tip-a50: #b2e1b0;

    --color-important-a0: #ab7df8;
    --color-important-a10: #b68bf9;
    --color-important-a20: #c099fa;
    --color-important-a30: #caa8fb;
    --color-important-a40: #d3b6fc;
    --color-important-a50: #dcc4fd;

    --color-warning-a0: #d29922;
    --color-warning-a10: #d9a440;
    --color-warning-a20: #e0af59;
    --color-warning-a30: #e6ba71;
    --color-warning-a40: #ecc588;
    --color-warning-a50: #f1d0a0;

    --color-caution-a0: #f85149;
    --color-caution-a10: #fc685b;
    --color-caution-a20: #ff7e6e;
    --color-caution-a30: #ff9182;
    --color-caution-a40: #ffa496;
    --color-caution-a50: #ffb7aa;

    --color-surface-a0: #121212;
    --color-surface-a10: #282828;
    --color-surface-a20: #3f3f3f;
    --color-surface-a30: #575757;
    --color-surface-a40: #717171;
    --color-surface-a50: #8b8b8b;
    --color-surface-a150: #e7e7e7;
    --color-surface-a200: #f4f4f4;

    --color-note-surface-a0: #232c4b;
    --color-note-surface-a10: #39405d;
    --color-note-surface-a20: #505570;
    --color-note-surface-a30: #676b83;
    --color-note-surface-a40: #7f8297;
    --color-note-surface-a50: #979aab;

    --color-tip-surface-a0: #243f25;
    --color-tip-surface-a10: #3a523a;
    --color-tip-surface-a20: #506650;
    --color-tip-surface-a30: #677a66;
    --color-tip-surface-a40: #7f8f7e;
    --color-tip-surface-a50: #97a496;

    --color-important-surface-a0: #3c304e;
    --color-important-surface-a10: #4f4460;
    --color-important-surface-a20: #645972;
    --color-important-surface-a30: #786f85;
    --color-important-surface-a40: #8d8598;
    --color-important-surface-a50: #a39cac;

    --color-warning-surface-a0: #46361c;
    --color-warning-surface-a10: #594932;
    --color-warning-surface-a20: #6d5e48;
    --color-warning-surface-a30: #817360;
    --color-warning-surface-a40: #958978;
    --color-warning-surface-a50: #a99f92;

    --color-caution-surface-a0: #512722;
    --color-caution-surface-a10: #643c37;
    --color-caution-surface-a20: #77524d;
    --color-caution-surface-a30: #8a6964;
    --color-caution-surface-a40: #9d807c;
    --color-caution-surface-a50: #b19894;

    --color-background-opacity: 0.1;
}

html 
{
    display: flex; 
    justify-content: center;
    color: var(--color-surface-a0);
} 
html > body 
{
    display: flex; 
    flex-direction: column; 
    max-width: 1024px; 
    flex: 1 1 auto;
}
h1
{
    border-bottom: solid 0.085em var(--color-surface-a200);
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
    color: var(--color-surface-a20);
}
blockquote > p
{
    margin-block-start: 0.2em;
    margin-block-end: 0.5em;
}
blockquote > p > span.icon
{
    display: block;
    margin-top: 0.85em;
    margin-bottom: 0.85em;
}
blockquote.note
{
    border-left: solid 0.25em var(--color-note-a0);
    background-color: rgb(from var(--color-note-a50) r g b / var(--color-background-opacity));
}
blockquote.note > p > span.icon
{
    color: var(--color-note-a0);
}
blockquote.note > h1, blockquote.note > h2, blockquote.note > h3, blockquote.note > h4, blockquote.note > h5, blockquote.note > h6
{
    color: var(--color-note-surface-a0);
}
blockquote.tip
{
    border-left: solid 0.25em var(--color-tip-a0);
    background-color: rgb(from var(--color-tip-a50) r g b / var(--color-background-opacity));
}
blockquote.tip > p > span.icon
{
    color: var(--color-tip-a0);
}
blockquote.tip > h1, blockquote.tip > h2, blockquote.tip > h3, blockquote.tip > h4, blockquote.tip > h5, blockquote.tip > h6
{
    color: var(--color-tip-surface-a0);
}
blockquote.important
{
    border-left: solid 0.25em var(--color-important-a0);
    background-color: rgb(from var(--color-important-a50) r g b / var(--color-background-opacity));
}
blockquote.important > p > span.icon
{
    color: var(--color-important-a0);
}
blockquote.important > h1, blockquote.important > h2, blockquote.important > h3, blockquote.important > h4, blockquote.important > h5, blockquote.important > h6
{
    color: var(--color-important-surface-a0);
}
blockquote.warning
{
    border-left: solid 0.25em var(--color-warning-a0);
    background-color: rgb(from var(--color-warning-a50) r g b / var(--color-background-opacity));
}
blockquote.warning > p > span.icon
{
    color: var(--color-warning-a0);
}
blockquote.warning > h1, blockquote.warning > h2, blockquote.warning > h3, blockquote.warning > h4, blockquote.warning > h5, blockquote.warning > h6
{
    color: var(--color-warning-surface-a0);
}
blockquote.caution
{
    border-left: solid 0.25em var(--color-caution-a0);
    background-color: rgb(from var(--color-caution-a50) r g b / var(--color-background-opacity));
}
blockquote.caution > p > span.icon
{
    color: var(--color-caution-a0);
}
blockquote.caution > h1, blockquote.caution > h2, blockquote.caution > h3, blockquote.caution > h4, blockquote.caution > h5, blockquote.caution > h6
{
    color: var(--color-caution-surface-a0);
}
cite
{
    font-size: 0.85em;
    color: var(--color-surface-a30);
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
        element.Built = $@"<!DOCTYPE html><html lang=""pt-BR""><head><title>{element.Title}</title><meta content=""text/html; charset=UTF-8;"" http-equiv=""Content-Type"" /><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""><style>{STYLE}</style></head>{element.Children.Build()}</html>";
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
                "var(--color-note-a0)",
                "M0 8a8 8 0 1 1 16 0A8 8 0 0 1 0 8Zm8-6.5a6.5 6.5 0 1 0 0 13 6.5 6.5 0 0 0 0-13ZM6.5 7.75A.75.75 0 0 1 7.25 7h1a.75.75 0 0 1 .75.75v2.75h.25a.75.75 0 0 1 0 1.5h-2a.75.75 0 0 1 0-1.5h.25v-2h-.25a.75.75 0 0 1-.75-.75ZM8 6a1 1 0 1 1 0-2 1 1 0 0 1 0 2Z"
            );
        }, RegexOptions.Multiline)} <b>Nota</b></span>";

        target = Regex.Replace(target, @$"({SVG_TIP})", (match) =>
        {
            return $@"<span class=""icon"">{SvgBuild(
                "var(--color-tip-a0)",
                "M8 1.5c-2.363 0-4 1.69-4 3.75 0 .984.424 1.625.984 2.304l.214.253c.223.264.47.556.673.848.284.411.537.896.621 1.49a.75.75 0 0 1-1.484.211c-.04-.282-.163-.547-.37-.847a8.456 8.456 0 0 0-.542-.68c-.084-.1-.173-.205-.268-.32C3.201 7.75 2.5 6.766 2.5 5.25 2.5 2.31 4.863 0 8 0s5.5 2.31 5.5 5.25c0 1.516-.701 2.5-1.328 3.259-.095.115-.184.22-.268.319-.207.245-.383.453-.541.681-.208.3-.33.565-.37.847a.751.75 0 0 1-1.485-.212c.084-.593.337-1.078.621-1.489.203-.292.45-.584.673-.848.075-.088.147-.173.213-.253.561-.679.985-1.32.985-2.304 0-2.06-1.637-3.75-4-3.75ZM5.75 12h4.5a.75.75 0 0 1 0 1.5h-4.5a.75.75 0 0 1 0-1.5ZM6 15.25a.75.75 0 0 1 .75-.75h2.5a.75.75 0 0 1 0 1.5h-2.5a.75.75 0 0 1-.75-.75Z"
            );
        }, RegexOptions.Multiline)} <b>Dica</b></span>";

        target = Regex.Replace(target, @$"({SVG_IMPORTANT})", (match) =>
        {
            return $@"<span class=""icon"">{SvgBuild(
                "var(--color-important-a0)",
                "M0 1.75C0 .784.784 0 1.75 0h12.5C15.216 0 16 .784 16 1.75v9.5A1.75 1.75 0 0 1 14.25 13H8.06l-2.573 2.573A1.458 1.458 0 0 1 3 14.543V13H1.75A1.75 1.75 0 0 1 0 11.25Zm1.75-.25a.25.25 0 0 0-.25.25v9.5c0 .138.112.25.25.25h2a.75.75 0 0 1 .75.75v2.19l2.72-2.72a.749.749 0 0 1 .53-.22h6.5a.25.25 0 0 0 .25-.25v-9.5a.25.25 0 0 0-.25-.25Zm7 2.25v2.5a.75.75 0 0 1-1.5 0v-2.5a.75.75 0 0 1 1.5 0ZM9 9a1 1 0 1 1-2 0 1 1 0 0 1 2 0Z"
            );
        }, RegexOptions.Multiline)} <b>Importante</b></span>";

        target = Regex.Replace(target, @$"({SVG_WARNING})", (match) =>
        {
            return $@"<span class=""icon"">{SvgBuild(
                "var(--color-warning-a0)",
                "M6.457 1.047c.659-1.234 2.427-1.234 3.086 0l6.082 11.378A1.75 1.75 0 0 1 14.082 15H1.918a1.75 1.75 0 0 1-1.543-2.575Zm1.763.707a.25.25 0 0 0-.44 0L1.698 13.132a.25.25 0 0 0 .22.368h12.164a.25.25 0 0 0 .22-.368Zm.53 3.996v2.5a.75.75 0 0 1-1.5 0v-2.5a.75.75 0 0 1 1.5 0ZM9 11a1 1 0 1 1-2 0 1 1 0 0 1 2 0Z"
            );
        }, RegexOptions.Multiline)} <b>Aviso</b></span>";

        target = Regex.Replace(target, @$"({SVG_CAUTION})", (match) =>
        {
            return $@"<span class=""icon"">{SvgBuild(
                "var(--color-caution-a0)",
                "M4.47.22A.749.749 0 0 1 5 0h6c.199 0 .389.079.53.22l4.25 4.25c.141.14.22.331.22.53v6a.749.749 0 0 1-.22.53l-4.25 4.25A.749.749 0 0 1 11 16H5a.749.749 0 0 1-.53-.22L.22 11.53A.749.749 0 0 1 0 11V5c0-.199.079-.389.22-.53Zm.84 1.28L1.5 5.31v5.38l3.81 3.81h5.38l3.81-3.81V5.31L10.69 1.5ZM8 4a.75.75 0 0 1 .75.75v3.5a.75.75 0 0 1-1.5 0v-3.5A.75.75 0 0 1 8 4Zm0 8a1 1 0 1 1 0-2 1 1 0 0 1 0 2Z"
            );
        }, RegexOptions.Multiline)} <b>Cuidado</b></span>";

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
