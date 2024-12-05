using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using HtmlAgilityPack;
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

internal sealed class BlockquoteFormatRequest : IRequest<string?>
{
    public FileInfo? FileInfo { get; init; }
}
internal sealed class BlockquoteFormatRequestHandler : IRequestHandler<BlockquoteFormatRequest, string?>
{
    public async Task<string?> Handle(BlockquoteFormatRequest request, CancellationToken cancellationToken)
    {
        using var stream = request.FileInfo!.OpenRead();
        await Task.Yield();
        var htmlDocument = new HtmlDocument();
        htmlDocument.Load(stream);

        var blockquotes = htmlDocument.DocumentNode.SelectNodes("//blockquote");

        if (blockquotes == null)
            return htmlDocument.DocumentNode.OuterHtml;

        foreach (var blockquote in blockquotes)
        {
            var p = blockquote.SelectSingleNode("p");
            if (p == null)
                continue;

            if (p.InnerText.Trim().StartsWith("[!NOTE]"))
            {
                var highlight = new Highlight
                {
                    Key = "[!NOTE]",
                    Color = "#1f6feb",
                    Name = "Note"
                };
                blockquote.SetAttributeValue("style", $"border-color: {highlight.Color};");
                Format(p, highlight);
                continue;
            }
            if (p.InnerText.Trim().StartsWith("[!TIP]"))
            {
                var highlight = new Highlight
                {
                    Key = "[!TIP]",
                    Color = "#3fb950",
                    Name = "Tip"
                };
                blockquote.SetAttributeValue("style", $"border-color: {highlight.Color};");
                Format(p, highlight);
                continue;
            }
            if (p.InnerText.Trim().StartsWith("[!IMPORTANT]"))
            {
                var highlight = new Highlight
                {
                    Key = "[!IMPORTANT]",
                    Color = "#ab7df8",
                    Name = "Important"
                };
                blockquote.SetAttributeValue("style", $"border-color: {highlight.Color};");
                Format(p, highlight);
                continue;
            }
            if (p.InnerText.Trim().StartsWith("[!WARNING]"))
            {
                var highlight = new Highlight
                {
                    Key = "[!WARNING]",
                    Color = "#d29922",
                    Name = "Warning"
                };
                blockquote.SetAttributeValue("style", $"border-color: {highlight.Color};");
                Format(p, highlight);
                continue;
            }
            if (p.InnerText.Trim().StartsWith("[!CAUTION]"))
            {
                var highlight = new Highlight
                {
                    Key = "[!CAUTION]",
                    Color = "#f85149",
                    Name = "Caution"
                };
                blockquote.SetAttributeValue("style", $"border-color: {highlight.Color};");
                Format(p, highlight);
                continue;
            }
        }

        return htmlDocument.DocumentNode.OuterHtml;
    }

    private static void Format(HtmlNode p, Highlight highlight)
    {
        p.SetAttributeValue("style", "display:flex; align-items:center; column-gap:0.4em; font-weight:500;");

        if (highlight.Key != default)
            p.InnerHtml = p.InnerHtml.Replace(highlight.Key, string.Empty);

        if (highlight.Color != default)
        {
            var span = HtmlNode.CreateNode($"<span style='color:{highlight.Color};'>{highlight.Name}</span>");
            p.PrependChild(span);
            p.PrependChild(HtmlNode.CreateNode(highlight.Key));
        }
    }

    public struct Highlight
    {
        public string? Key { get; set; }
        public string? Color { get; set; }
        public string? Name { get; set; }
    }
}

internal sealed class SvgBuildRequest : IRequest<string?>
{
    public string? String { get; init; }
}
internal sealed class SvgBuildRequestHandler : IRequestHandler<SvgBuildRequest, string?>
{
    public async Task<string?> Handle(SvgBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.String == default)
            return default;

        return request
            .String
            .Replace(
                "[!NOTE]",
                SvgCreate("#1f6feb", "M0 8a8 8 0 1 1 16 0A8 8 0 0 1 0 8Zm8-6.5a6.5 6.5 0 1 0 0 13 6.5 6.5 0 0 0 0-13ZM6.5 7.75A.75.75 0 0 1 7.25 7h1a.75.75 0 0 1 .75.75v2.75h.25a.75.75 0 0 1 0 1.5h-2a.75.75 0 0 1 0-1.5h.25v-2h-.25a.75.75 0 0 1-.75-.75ZM8 6a1 1 0 1 1 0-2 1 1 0 0 1 0 2Z"))
            .Replace(
                "[!TIP]",
                SvgCreate("#3fb950", "M8 1.5c-2.363 0-4 1.69-4 3.75 0 .984.424 1.625.984 2.304l.214.253c.223.264.47.556.673.848.284.411.537.896.621 1.49a.75.75 0 0 1-1.484.211c-.04-.282-.163-.547-.37-.847a8.456 8.456 0 0 0-.542-.68c-.084-.1-.173-.205-.268-.32C3.201 7.75 2.5 6.766 2.5 5.25 2.5 2.31 4.863 0 8 0s5.5 2.31 5.5 5.25c0 1.516-.701 2.5-1.328 3.259-.095.115-.184.22-.268.319-.207.245-.383.453-.541.681-.208.3-.33.565-.37.847a.751.75 0 0 1-1.485-.212c.084-.593.337-1.078.621-1.489.203-.292.45-.584.673-.848.075-.088.147-.173.213-.253.561-.679.985-1.32.985-2.304 0-2.06-1.637-3.75-4-3.75ZM5.75 12h4.5a.75.75 0 0 1 0 1.5h-4.5a.75.75 0 0 1 0-1.5ZM6 15.25a.75.75 0 0 1 .75-.75h2.5a.75.75 0 0 1 0 1.5h-2.5a.75.75 0 0 1-.75-.75Z"))
            .Replace(
                "[!IMPORTANT]",
                SvgCreate("#ab7df8", "M0 1.75C0 .784.784 0 1.75 0h12.5C15.216 0 16 .784 16 1.75v9.5A1.75 1.75 0 0 1 14.25 13H8.06l-2.573 2.573A1.458 1.458 0 0 1 3 14.543V13H1.75A1.75 1.75 0 0 1 0 11.25Zm1.75-.25a.25.25 0 0 0-.25.25v9.5c0 .138.112.25.25.25h2a.75.75 0 0 1 .75.75v2.19l2.72-2.72a.749.749 0 0 1 .53-.22h6.5a.25.25 0 0 0 .25-.25v-9.5a.25.25 0 0 0-.25-.25Zm7 2.25v2.5a.75.75 0 0 1-1.5 0v-2.5a.75.75 0 0 1 1.5 0ZM9 9a1 1 0 1 1-2 0 1 1 0 0 1 2 0Z"))
            .Replace(
                "[!WARNING]",
                SvgCreate("#d29922", "M6.457 1.047c.659-1.234 2.427-1.234 3.086 0l6.082 11.378A1.75 1.75 0 0 1 14.082 15H1.918a1.75 1.75 0 0 1-1.543-2.575Zm1.763.707a.25.25 0 0 0-.44 0L1.698 13.132a.25.25 0 0 0 .22.368h12.164a.25.25 0 0 0 .22-.368Zm.53 3.996v2.5a.75.75 0 0 1-1.5 0v-2.5a.75.75 0 0 1 1.5 0ZM9 11a1 1 0 1 1-2 0 1 1 0 0 1 2 0Z"))
            .Replace(
                "[!CAUTION]",
                SvgCreate("#f85149", "M4.47.22A.749.749 0 0 1 5 0h6c.199 0 .389.079.53.22l4.25 4.25c.141.14.22.331.22.53v6a.749.749 0 0 1-.22.53l-4.25 4.25A.749.749 0 0 1 11 16H5a.749.749 0 0 1-.53-.22L.22 11.53A.749.749 0 0 1 0 11V5c0-.199.079-.389.22-.53Zm.84 1.28L1.5 5.31v5.38l3.81 3.81h5.38l3.81-3.81V5.31L10.69 1.5ZM8 4a.75.75 0 0 1 .75.75v3.5a.75.75 0 0 1-1.5 0v-3.5A.75.75 0 0 1 8 4Zm0 8a1 1 0 1 1 0-2 1 1 0 0 1 0 2Z")
            );
    }

    static string SvgCreate(string color, string shape)
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
}

internal sealed class StringGetdRequest : IRequest<string?>
{
    public FileInfo? FileInfo { get; init; }
}
internal sealed class StringGetRequestHandler : IRequestHandler<StringGetdRequest, string?>
{
    public async Task<string?> Handle(StringGetdRequest request, CancellationToken cancellationToken)
    {
        using var reader = request.FileInfo!.OpenText();
        return await reader.ReadToEndAsync();
    }
}

internal sealed class MarkdownFileInfoBuildRequest : IRequest
{
    public FileInfo? Source { get; init; }
    public FileInfo? Target { get; init; }
}
internal sealed class MarkdownFileInfoBuildRequesttHandler(IMediator mediator) : IRequestHandler<MarkdownFileInfoBuildRequest>
{
    public async Task Handle(MarkdownFileInfoBuildRequest request, CancellationToken cancellationToken)
    {
        var content = await mediator.Send(new StringGetdRequest { FileInfo = request.Source });
        // content = await mediator.Send(new HtmlStringBuildRequest
        // {
        //     Title = Environment.GetCommandLineArgs()[4],
        //     Url = Environment.GetCommandLineArgs()[5],
        //     String = content,
        // });
        content = (await mediator.Send(new StringBuildRequest
        {
            Title = Environment.GetCommandLineArgs()[4],
            Url = Environment.GetCommandLineArgs()[5],
            Source = content,
        }))?.Target?.Html;

        if (!request.Target.Directory!.Exists)
            request.Target.Directory.Create();
        var fileInfo = request.Target;
        using var fileStrem = fileInfo!.CreateText();
        await fileStrem.WriteAsync(content);
    }
}
internal sealed class HtmlAStringBuildRequest : IRequest<string>
{
    public string? String { get; init; }
}
internal sealed class HtmlAStringBuildRequestHandler : IRequestHandler<HtmlAStringBuildRequest, string?>
{
    static Regex Regex { get; }
    static HtmlAStringBuildRequestHandler()
    {
        Regex = new Regex(@"\[(?'content'[^\^].*?)\]\((?'href'.*?)(README.MD|)\)", RegexOptions.Multiline | RegexOptions.IgnoreCase);
    }
    public async Task<string?> Handle(HtmlAStringBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.String == default)
            return request.String;

        return Regex.Replace(
            request.String,
            match => $@"<a href=""{match.Groups["href"].Value}"">{match.Groups["content"].Value}</a>"
        );
    }
}

internal sealed class HtmlIStringBuildRequest : IRequest<string>
{
    public string? String { get; init; }
}
internal sealed class HtmlIStringBuildRequestHandler : IRequestHandler<HtmlIStringBuildRequest, string?>
{
    static Regex Regex { get; }
    static HtmlIStringBuildRequestHandler()
    {
        Regex = new Regex(@"\*{1}([^\*| ].+?)\*{1}", RegexOptions.Multiline);
    }
    public async Task<string?> Handle(HtmlIStringBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        var content = request.String!;
        var match = Regex.Match(content);
        do
        {
            if (!match.Success)
                break;

            content = content.Replace(match.Groups[0].Value, $@"<i>{match.Groups[1].Value}</i>");
            match = match.NextMatch();
        } while (true);

        return content;
    }
}

internal sealed class HtmlBStringBuildRequest : IRequest<string>
{
    public string? String { get; init; }
}
internal sealed class HtmlBStringBuildRequestHandler : IRequestHandler<HtmlBStringBuildRequest, string?>
{
    static Regex Regex { get; }
    static HtmlBStringBuildRequestHandler()
    {
        Regex = new Regex(@"\*{2}([^\*| ].+?)\*{2}", RegexOptions.Multiline);
    }
    public async Task<string?> Handle(HtmlBStringBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        var content = request.String!;
        var match = Regex.Match(content);
        do
        {
            if (!match.Success)
                break;

            content = content.Replace(match.Groups[0].Value, $@"<b>{match.Groups[1].Value}</b>");
            match = match.NextMatch();
        } while (true);

        return content;
    }
}

internal sealed class HtmlPStringBuildRequest : IRequest<string>
{
    public string? String { get; init; }
}

internal sealed class HtmlPStringBuildRequestHandler : IRequestHandler<HtmlPStringBuildRequest, string?>
{
    static Regex Regex { get; }
    static HtmlPStringBuildRequestHandler()
    {
        Regex = new Regex(@"^((?!# *?)(?!> *?)(?!\- *?)(?!([ ]{4})+\- *?)(?!\[\^\d+\])(?!\r?\n))(.+(\r?\n|)+)", RegexOptions.Multiline);
    }
    public async Task<string?> Handle(HtmlPStringBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();

        if (request.String == default)
            return request.String;

        return Regex.Replace(request.String, $"<p>$3</p>");
    }
}

internal sealed class HtmlCiteStringBuildRequest : IRequest<string>
{
    public string? String { get; init; }
}

internal sealed class HtmlCiteStringBuildRequestHandler : IRequestHandler<HtmlCiteStringBuildRequest, string?>
{
    static Regex CiteRegex { get; }
    static Regex CitedRegex { get; }
    static HtmlCiteStringBuildRequestHandler()
    {
        CiteRegex = new Regex(@"^(?'start'</?.+>|)\[\^(?'cite'\d+)\]: +(?'content'.*)", RegexOptions.Multiline);
        CitedRegex = new Regex(@$"(?'start'</?.+>|)\[\^(?'cite'\d+)\]", RegexOptions.Multiline);
    }
    public async Task<string?> Handle(HtmlCiteStringBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.String == default)
            return request.String;

        var content = CiteRegex.Replace(
            request.String,
            match => @$"{match.Groups["start"].Value}<br/><cite id=""cite-{match.Groups["cite"].Value}""><a href=""#cited-{match.Groups["cite"].Value}"">({match.Groups["cite"].Value})</a>. {match.Groups["content"].Value}</cite>"
        );

        content = CitedRegex.Replace(
            content,
            match => @$"{match.Groups["start"].Value}<cite id=""cited-{match.Groups["cite"].Value}""> <a href=""#cite-{match.Groups["cite"].Value}"">({match.Groups["cite"].Value})</a></cite>"
        );

        return content;
    }
}

internal sealed class HtmlStringBuildRequest : IRequest<string>
{
    public string? String { get; init; }
    public string? Url { get; init; }
    public string? Title { get; init; }
}
internal sealed class HtmlStringBuildRequestHandler(IMediator mediator) : IRequestHandler<HtmlStringBuildRequest, string?>
{
    public async Task<string?> Handle(HtmlStringBuildRequest request, CancellationToken cancellationToken)
    {
        if (request.String == default)
            return default;

        var content = request.String;
        content = await mediator.Send(new HtmlPStringBuildRequest { String = content }, cancellationToken);
        content = await mediator.Send(new SvgBuildRequest { String = content }, cancellationToken);
        content = await mediator.Send(new HtmlBStringBuildRequest { String = content }, cancellationToken);
        content = await mediator.Send(new HtmlIStringBuildRequest { String = content }, cancellationToken);
        content = await mediator.Send(new HtmlCiteStringBuildRequest { String = content }, cancellationToken);
        content = await mediator.Send(new HtmlAStringBuildRequest { String = content }, cancellationToken);

        return content;
    }
}

internal sealed class StringBuildRequest : IRequest<StringBuildResponse>
{
    public string? Title { get; init; }
    public string? Source { get; init; }
    public string? Url { get; internal set; }
}
internal sealed class StringBuildResponse
{
    internal HtmlElement? Target { get; init; }
}
internal class HtmlElement : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    internal string? Source { get; init; }
    public string? Html { get => $"<html><title>{Title}</title>{Children.Html()}</html>"; }
    public string? Title { get; init; }
}
internal class BodyElement : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Html { get => @$"<body><h1><a href=""{Url}""/>{Title}</a></h1>{Children.Html()}</body>"; }
    public string? Title { get; init; }
    public string? Url { get; init; }
    internal string? Source { get; init; }
}
internal class PElement : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Html { get; internal set; }
    internal string? Source { get; init; }
}

internal class H1Element : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Html { get => $"<h1>{Children.Html()}</h1>"; }
    internal string? Source { get; init; }
}

internal class H2Element : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Html { get => $"<h2>{Children.Html()}</h2>"; }
    internal string? Source { get; init; }
}

internal class H3Element : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Html { get => $"<h3>{Children.Html()}</h3>"; }
    internal string? Source { get; init; }
}

internal class H4Element : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Html { get => $"<h4>{Children.Html()}</h4>"; }
    internal string? Source { get; init; }
}

internal class H5Element : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Html { get => $"<h5>{Children.Html()}</h5>"; }
    internal string? Source { get; init; }
}

internal class H6Element : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Html { get => $"<h6>{Children.Html()}</h6>"; }
    internal string? Source { get; init; }
}

internal class BlockquoteElement : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Html { get; internal set; }
    internal string? Source { get; init; }
}

internal class UlElement : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Html { get => $"<ul>{Children.Html()}</ul>"; }
    internal string? Source { get; init; }
}

internal class LIElement : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Html { get => $"<li>{Children.Html()}</li>"; }
    internal string? Source { get; init; }
}

internal class TextElement : IElement
{
    public IElement? Parent { get; init; }
    public IElement[]? Children { get; init; }
    public string? Html { get; init; }
    internal string? Source { get; init; }
}

public interface IElement
{
    IElement? Parent { get; init; }
    IElement[]? Children { get; }
    string? Html { get; }
}

internal static class IElementExtensions
{
    internal static string? Html(this IElement[]? elements)
    {
        if (elements == default)
            return default;

        return string.Join(string.Empty, elements.HtmlEnumerable());
    }

    private static IEnumerable<string?> HtmlEnumerable(this IElement[] elements)
    {
        foreach (var element in elements)
            yield return element.Html;
    }
}

internal sealed class StringBuildRequestHandler : IRequestHandler<StringBuildRequest, StringBuildResponse>
{
    const string P = @"^(?'P'(?!(#|>| *-)).+(\r?\n|)*)";
    const string H1 = @"^(?'H1'# *(?'H1_CONTENT'(?!#).+(\r?\n|)))";
    const string H2 = @"^(?'H2'## *(?'H2_CONTENT'(?!#).+(\r?\n|)))";
    const string H3 = @"^(?'H3'### *(?'H3_CONTENT'(?!#).+(\r?\n|)))";
    const string H4 = @"^(?'H4'#### *(?'H4_CONTENT'(?!#).+(\r?\n|)))";
    const string H5 = @"^(?'H5'##### *(?'H5_CONTENT'(?!#).+(\r?\n|)))";
    const string H6 = @"^(?'H6'###### *(?'H6_CONTENT'(?!#).+(\r?\n|)))";
    const string BLOCKQUOTE = @"^(?'BLOCKQUOTE'> *(?'BLOCKQUOTE_CONTENT'.*(\r?\n|)))+";
    const string UL = @"^(?'UL'( *- *.+(\r?\n|))+(\r?\n|))";
    const string LI = @"^- *(?'LI'(.*(\r?\n|)+(?!\-))+(\r?\n|))";
    const string I = @"(?'I'\*{1}(?'I_CONTENT'[^\*| ].+?)\*{1})";
    const string B = @"(?'B'\*{2}(?'B_CONTENT'[^\*| ].+?)\*{2})";
    const string BI = @"(?'BI'\*{3}(?'BI_CONTENT'[^\*| ].+?)\*{3})";
    const string TEXT = @"^(?'TEXT'((.*(\r?\n|))*))";
    const string AGE_CALC = @"(?'AGE_CALC'`\[age-calc\]:(?'AGE_CALC_CONTENT'[\d]{4}\-[\d]{2}\-[\d]{2})\`)";
    const string A = @"(?'A'\[(?!.*(\[).*)(?'A_CONTENT'.*)\]\((?'A_HREF'.*)(README.MD|)\))";
    public async Task<StringBuildResponse> Handle(StringBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return new StringBuildResponse();

        return new StringBuildResponse
        {
            Target = Build(request),
        };
    }

    private HtmlElement Build(StringBuildRequest request)
    {
        var element = new HtmlElement
        {
            Title = request.Title,
            Source = request.Source,
            Parent = default,
        };
        element.Children = Build(element, request);
        return element;
    }

    private IElement[] Build(HtmlElement html, StringBuildRequest request)
    {
        var body = new BodyElement
        {
            Title = request.Title,
            Url = request.Url,
            Source = request.Source,
            Parent = html,
        };
        body.Children = Build(body, request.Source).ToArray();
        return [body];
    }

    private static IEnumerable<IElement> Build(IElement? parent, string? source)
    {
        if (source == default)
            yield break;

        foreach (IElement element in Build(parent, Regex.Matches(source, @$"({P}|{H1}|{H2}|{H3}|{H4}|{H5}|{H6}|{BLOCKQUOTE}|{UL})", RegexOptions.Multiline)))
            yield return element;
    }

    private static IEnumerable<IElement> Build(BlockquoteElement? parent, string? source)
    {
        if (source == default)
            yield break;

        foreach (IElement element in Build(parent, Regex.Matches(source, @$"({P}|{H1}|{H2}|{H3}|{H4}|{H5}|{H6}|{BLOCKQUOTE}|{UL})", RegexOptions.Multiline)))
            yield return element;
    }

    private static IEnumerable<IElement> Build(LIElement? parent, string? source)
    {
        if (source == default)
            yield break;

        foreach (IElement element in Build(parent, Regex.Matches(source, @$"({TEXT})", RegexOptions.Singleline)))
            yield return element;
    }

    private static IEnumerable<IElement> Build(UlElement? parent, string? source)
    {
        if (source == default)
            yield break;

        foreach (IElement element in Build(parent, Regex.Matches(source, @$"({LI})", RegexOptions.Multiline)))
            yield return element;
    }

    private static IEnumerable<IElement> Build(H1Element? parent, string? source)
    {
        if (source == default)
            yield break;

        foreach (IElement element in Build(parent, Regex.Matches(source, @$"({TEXT})", RegexOptions.Multiline)))
            yield return element;
    }

    private static IEnumerable<IElement> Build(H2Element? parent, string? source)
    {
        if (source == default)
            yield break;

        foreach (IElement element in Build(parent, Regex.Matches(source, @$"({TEXT})", RegexOptions.Multiline)))
            yield return element;
    }

    private static IEnumerable<IElement> Build(H3Element? parent, string? source)
    {
        if (source == default)
            yield break;

        foreach (IElement element in Build(parent, Regex.Matches(source, @$"({TEXT})", RegexOptions.Multiline)))
            yield return element;
    }

    private static IEnumerable<IElement> Build(H4Element? parent, string? source)
    {
        if (source == default)
            yield break;

        foreach (IElement element in Build(parent, Regex.Matches(source, @$"({TEXT})", RegexOptions.Multiline)))
            yield return element;
    }

    private static IEnumerable<IElement> Build(H5Element? parent, string? source)
    {
        if (source == default)
            yield break;

        foreach (IElement element in Build(parent, Regex.Matches(source, @$"({TEXT})", RegexOptions.Multiline)))
            yield return element;
    }

    private static IEnumerable<IElement> Build(H6Element? parent, string? source)
    {
        if (source == default)
            yield break;

        foreach (IElement element in Build(parent, Regex.Matches(source, @$"({TEXT})", RegexOptions.Multiline)))
            yield return element;
    }

    private static IEnumerable<IElement> Build(PElement? parent, string? source)
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

        target = Regex.Replace(target, @$"({BI})", (match) =>
        {
            return $"<b><i>{match.Groups["BI_CONTENT"].Value}</i></b>";
        });

        target = Regex.Replace(target, @$"({B})", (match) =>
        {
            return $"<b>{match.Groups["B_CONTENT"].Value}</b>";
        });

        target = Regex.Replace(target, @$"({I})", (match) =>
        {
            return $"<i>{match.Groups["I_CONTENT"].Value}</i>";
        });

        target = Regex.Replace(target, @$"({A})", (match) =>
        {
            return $@"<a href=""{match.Groups["A_HREF"].Value}"">{match.Groups["A_CONTENT"].Value}</a>";
        });

        target = Regex.Replace(target, @$"({AGE_CALC})", (match) =>
        {
            return AgeCalculate(DateTime.ParseExact(match.Groups["AGE_CALC_CONTENT"].Value, "yyyy-mm-dd", CultureInfo.InvariantCulture)).ToString();
        });

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

    private static IEnumerable<IElement> Build(IElement? parent, MatchCollection matches)
    {
        foreach (Match match in matches)
        {
            if (!string.IsNullOrWhiteSpace(match.Groups["H1"].Value))
            {
                var h1 = new H1Element
                {
                    Source = match.Groups["H1_CONTENT"].Value,
                    Parent = parent,
                };
                h1.Children = Build(h1, match.Groups["H1_CONTENT"].Value).ToArray();
                yield return h1;
                continue;
            }
            if (!string.IsNullOrWhiteSpace(match.Groups["H2"].Value))
            {
                var h2 = new H2Element
                {
                    Source = match.Groups["H2_CONTENT"].Value,
                    Parent = parent,
                };
                h2.Children = Build(h2, match.Groups["H2_CONTENT"].Value).ToArray();
                yield return h2;
                continue;
            }
            if (!string.IsNullOrWhiteSpace(match.Groups["H3"].Value))
            {
                var h3 = new H3Element
                {
                    Source = match.Groups["H3_CONTENT"].Value,
                    Parent = parent,
                };
                h3.Children = Build(h3, match.Groups["H3_CONTENT"].Value).ToArray();
                yield return h3;
                continue;
            }
            if (!string.IsNullOrWhiteSpace(match.Groups["H4"].Value))
            {
                var h4 = new H4Element
                {
                    Source = match.Groups["H4_CONTENT"].Value,
                    Parent = parent,
                };
                h4.Children = Build(h4, match.Groups["H4_CONTENT"].Value).ToArray();
                yield return h4;
                continue;
            }
            if (!string.IsNullOrWhiteSpace(match.Groups["H5"].Value))
            {
                var h5 = new H5Element
                {
                    Source = match.Groups["H5_CONTENT"].Value,
                    Parent = parent,
                };
                h5.Children = Build(h5, match.Groups["H5_CONTENT"].Value).ToArray();
                yield return h5;
                continue;
            }
            if (!string.IsNullOrWhiteSpace(match.Groups["H6"].Value))
            {
                var h6 = new H6Element
                {
                    Source = match.Groups["H6_CONTENT"].Value,
                    Parent = parent,
                };
                h6.Children = Build(h6, match.Groups["H6_CONTENT"].Value).ToArray();
                yield return h6;
                continue;
            }
            if (!string.IsNullOrWhiteSpace(match.Groups["BLOCKQUOTE"].Value))
            {
                var blockquote = new BlockquoteElement
                {
                    Source = string.Join(string.Empty, match.Groups["BLOCKQUOTE_CONTENT"].Captures.Select(c => c.Value)),
                    Parent = parent,
                    Children = default,
                };
                blockquote.Children = Build(blockquote, blockquote.Source).ToArray();
                blockquote.Html = $"<blockquote>{blockquote.Children.Html()}</blockquote>";
                yield return blockquote;
                continue;
            }

            if (!string.IsNullOrWhiteSpace(match.Groups["UL"].Value))
            {
                var ul = new UlElement
                {
                    Source = match.Groups["UL"].Value,
                    Parent = parent,
                };
                ul.Children = Build(ul, match.Groups["UL"].Value).ToArray();
                yield return ul;
                continue;
            }

            if (!string.IsNullOrWhiteSpace(match.Groups["LI"].Value))
            {
                var li = new LIElement
                {
                    Source = match.Groups["LI"].Value,
                    Parent = parent,
                };
                li.Children = Build(li, match.Groups["LI"].Value).ToArray();
                yield return li;
                continue;
            }

            if (!string.IsNullOrWhiteSpace(match.Groups["P"].Value))
            {
                var p = new PElement
                {
                    Source = match.Groups["P"].Value,
                    Parent = parent,
                    Html = $"<p>{Build(match.Groups["P"].Value)}</p>"
                };
                p.Children = Build(p, match.Groups["P"].Value).ToArray();
                yield return p;
                continue;
            }

            if (!string.IsNullOrWhiteSpace(match.Groups["TEXT"].Value))
            {
                var text = new TextElement
                {
                    Source = match.Groups["TEXT"].Value,
                    Parent = parent,
                    Children = default,
                    Html = Build(match.Groups["TEXT"].Value),
                };
                yield return text;
                continue;
            }
            var t = string.Empty;
        }
    }
}
