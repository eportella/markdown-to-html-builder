using System.Diagnostics;
using System.Globalization;
using System.Runtime.CompilerServices;
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

internal sealed class BlockquoteBuildRequest : IRequest<string?>
{
    public string? String { get; init; }
}

internal sealed class BlockquoteBuildRequestHandler : IRequestHandler<BlockquoteBuildRequest, string?>
{
    static Regex Regex { get; }
    static BlockquoteBuildRequestHandler()
    {
        Regex = new Regex(@"(^> *(.*(\n|)))+$", RegexOptions.Multiline);
    }
    public async Task<string?> Handle(BlockquoteBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.String == default)
            return default;

        var content = request.String!;

        do
        {
            var match = Regex.Match(content);

            if (!match.Success)
                break;

            content = content.Replace(match.Groups[0].Value, $"<blockquote>{string.Join("", match.Groups[2].Captures.Select(c => c.Value))}</blockquote>");
        } while (true);

        return content;
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


internal sealed class AgeCalcBuildRequest : IRequest<string?>
{
    public string? String { get; init; }
}
internal sealed class AgeCalcBuildRequestHandler : IRequestHandler<AgeCalcBuildRequest, string?>
{
    Regex Regex { get; }
    public AgeCalcBuildRequestHandler()
    {
        Regex = new Regex(@"`\[age-calc\]:([\d]{4}\-[\d]{2}\-[\d]{2})\`", RegexOptions.Multiline);
    }
    public async Task<string?> Handle(AgeCalcBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.String == default)
            return default;

        var content = request.String;
        do
        {
            var match = Regex.Match(content);
            if (!match.Success)
                break;
            content = content.Replace(
                match.Groups[0].Value,
                Calculate(DateTime.ParseExact(match.Groups[1].Value, "yyyy-mm-dd", CultureInfo.InvariantCulture)).ToString()
            );

        } while (true);

        return content;
    }

    private static int Calculate(DateTime birthDate)
    {
        DateTime today = DateTime.Today;

        int age = today.Year - birthDate.Year;

        if (birthDate.Date > today.AddYears(-age).Date)
            return age - 1;

        return age;
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
        content = await mediator.Send(new HtmlStringBuildRequest { String = content });

        if (!request.Target.Directory!.Exists)
            request.Target.Directory.Create();
        var fileInfo = request.Target;
        using var fileStrem = fileInfo!.CreateText();
        await fileStrem.WriteAsync(content);
    }
}

internal sealed class HtmlBuildRequest : IRequest<string?>
{
    public string? Title { get; init; }
    public string? String { get; init; }
}

internal sealed class HtmlBuildRequestHandler : IRequestHandler<HtmlBuildRequest, string?>
{
    public async Task<string?> Handle(HtmlBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        return $"<html><title>{request.Title}</title>{request.String}</html>";
    }
}

internal sealed class BodyBuildRequest : IRequest<string?>
{
    public string? String { get; init; }
}

internal sealed class BodyBuildRequestHandler : IRequestHandler<BodyBuildRequest, string?>
{
    public async Task<string?> Handle(BodyBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        return $@"<body><h1><a href=""{Environment.GetCommandLineArgs()[5]}""/>{Environment.GetCommandLineArgs()[4]}</a></h1>{request.String}</body>";
    }
}

internal sealed class HtmlH1StringBuildRequest : IRequest<string>
{
    public string? String { get; init; }
}

internal sealed class HtmlH1StringBuildRequestHandler : IRequestHandler<HtmlH1StringBuildRequest, string?>
{
    static Regex Regex { get; }
    static HtmlH1StringBuildRequestHandler()
    {
        Regex = new Regex(@"^(|>) *# (.*?)(\r|)$", RegexOptions.Multiline);
    }
    public async Task<string?> Handle(HtmlH1StringBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.String == default)
            return request.String;

        return Regex.Replace(request.String, $"<h1>$2</h1>$3");
    }
}

internal sealed class HtmlH2StringBuildRequest : IRequest<string>
{
    public string? String { get; init; }
}
internal sealed class HtmlH2StringBuildRequestHandler : IRequestHandler<HtmlH2StringBuildRequest, string?>
{
    static Regex Regex { get; }
    static HtmlH2StringBuildRequestHandler()
    {
        Regex = new Regex(@"^(|>) *## (.*?)(\r|)$", RegexOptions.Multiline);
    }
    public async Task<string?> Handle(HtmlH2StringBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.String == default)
            return request.String;

        return Regex.Replace(request.String, $"<h2>$2</h2>$3");
    }
}

internal sealed class HtmlH3StringBuildRequest : IRequest<string>
{
    public string? String { get; init; }
}
internal sealed class HtmlH3StringBuildRequestHandler : IRequestHandler<HtmlH3StringBuildRequest, string?>
{
    static Regex Regex { get; }
    static HtmlH3StringBuildRequestHandler()
    {
        Regex = new Regex(@"^(|>) *### (.*?)(\r|)$", RegexOptions.Multiline);
    }
    public async Task<string?> Handle(HtmlH3StringBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.String == default)
            return request.String;

        return Regex.Replace(request.String, $"<h3>$2</h3>$3");
    }
}

internal sealed class HtmlH4StringBuildRequest : IRequest<string>
{
    public string? String { get; init; }
}
internal sealed class HtmlH4StringBuildRequestHandler : IRequestHandler<HtmlH4StringBuildRequest, string?>
{
    static Regex Regex { get; }
    static HtmlH4StringBuildRequestHandler()
    {
        Regex = new Regex(@"^(|>) *#### (.*?)(\r|)$", RegexOptions.Multiline);
    }
    public async Task<string?> Handle(HtmlH4StringBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.String == default)
            return request.String;

        return Regex.Replace(request.String, $"<h4>$2</h4>$3");
    }
}

internal sealed class HtmlH5StringBuildRequest : IRequest<string>
{
    public string? String { get; init; }
}
internal sealed class HtmlH5StringBuildRequestHandler : IRequestHandler<HtmlH5StringBuildRequest, string?>
{
    static Regex Regex { get; }
    static HtmlH5StringBuildRequestHandler()
    {
        Regex = new Regex(@"^(|>) *##### (.*?)(\r|)$", RegexOptions.Multiline);
    }
    public async Task<string?> Handle(HtmlH5StringBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.String == default)
            return request.String;

        return Regex.Replace(request.String, $"<h5>$2</h5>$3");
    }
}

internal sealed class HtmlH6StringBuildRequest : IRequest<string>
{
    public string? String { get; init; }
}
internal sealed class HtmlH6StringBuildRequestHandler : IRequestHandler<HtmlH6StringBuildRequest, string?>
{
    static Regex Regex { get; }
    static HtmlH6StringBuildRequestHandler()
    {
        Regex = new Regex(@"^(|>) *###### (.*?)(\r|)$", RegexOptions.Multiline);
    }
    public async Task<string?> Handle(HtmlH6StringBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.String == default)
            return request.String;

        return Regex.Replace(request.String, $"<h6>$2</h6>$3");
    }
}

internal sealed class HtmlLiStringBuildRequest : IRequest<string>
{
    public string? String { get; init; }
}
internal sealed class HtmlLiStringBuildRequestHandler : IRequestHandler<HtmlLiStringBuildRequest, string?>
{
    static Regex Regex { get; }
    static HtmlLiStringBuildRequestHandler()
    {
        Regex = new Regex("^(- +(.+))$", RegexOptions.Multiline);
    }
    public async Task<string?> Handle(HtmlLiStringBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        var content = request.String!;
        var match = Regex.Match(content);
        do
        {
            if (!match.Success)
                break;

            content = content
                .Replace(
                    match.Groups[1].Value,
                    $"<li>{match.Groups[2].Value}</li>"
                );
            match = match.NextMatch();
        } while (true);

        return content;
    }
}

internal sealed class HtmlUlStringBuildRequest : IRequest<string>
{
    public string? String { get; init; }
}
internal sealed class HtmlUlStringBuildRequestHandler(IMediator mediator) : IRequestHandler<HtmlUlStringBuildRequest, string?>
{
    static Regex Regex { get; }
    static HtmlUlStringBuildRequestHandler()
    {
        Regex = new Regex($"(^- +.+?(\r\n|\n|))+$", RegexOptions.Multiline);
    }
    public async Task<string?> Handle(HtmlUlStringBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.String == default)
            return request.String;
        var match = Regex.Match(request.String);

        return Regex
            .Replace(
                request.String,
                $"<ul>{await mediator.Send(new HtmlLiStringBuildRequest { String = match.Groups[0].Value }, cancellationToken)}</ul>{match.Groups[2].Value}"
            );
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
        Regex = new Regex(@"\[([^\^].*?)\]\((.*?)(README.MD|)\)", RegexOptions.Multiline | RegexOptions.IgnoreCase);
    }
    public async Task<string?> Handle(HtmlAStringBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        var content = request.String!;
        var match = Regex.Match(content);
        do
        {
            if (!match.Success)
                break;

            content = content.Replace(match.Groups[0].Value, $@"<a href=""{match.Groups[2].Value}"">{match.Groups[1].Value}</a>");
            match = match.NextMatch();
        } while (true);

        return content;
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
        Regex = new Regex($"^((?!# *?)(?!> *?)(?!\\- *?)(?!\\[\\^\\d+\\])(?!\r))(.+?)((\r|))$", RegexOptions.Multiline);
    }
    public async Task<string?> Handle(HtmlPStringBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();

        if (request.String == default)
            return request.String;

        return Regex.Replace(request.String, $"<p>$2</p>$3");
    }
}

internal sealed class HtmlCiteStringBuildRequest : IRequest<string>
{
    public string? String { get; init; }
}

internal sealed class HtmlCiteStringBuildRequestHandler : IRequestHandler<HtmlCiteStringBuildRequest, string?>
{
    static Regex Regex { get; }
    static HtmlCiteStringBuildRequestHandler()
    {
        Regex = new Regex(@"^\[\^(\d+)\]: +(.*)", RegexOptions.Multiline);
    }
    public async Task<string?> Handle(HtmlCiteStringBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        var content = request.String!;
        var match = Regex.Match(content);
        do
        {
            if (!match.Success)
                break;

            content = content.Replace(match.Groups[0].Value, @$"<br/><cite id=""cite-{match.Groups[1].Value}""><a href=""#cited-{match.Groups[1].Value}"">({match.Groups[1].Value})</a>. {match.Groups[2].Value}</cite>");

            var citedMatch = Regex.Match(content, @$"\[\^{match.Groups[1].Value}\]", RegexOptions.Multiline);

            if (citedMatch.Success)
                content = content.Replace(citedMatch.Groups[0].Value, @$"<cite id=""cited-{match.Groups[1].Value}""> <a href=""#cite-{match.Groups[1].Value}"">({match.Groups[1].Value})</a></cite>");

            match = match.NextMatch();
        } while (true);

        return content;
    }
}

internal sealed class HtmlStringBuildRequest : IRequest<string>
{
    public string? String { get; init; }
}
internal sealed class HtmlStringBuildRequestHandler(IMediator mediator) : IRequestHandler<HtmlStringBuildRequest, string?>
{
    public async Task<string?> Handle(HtmlStringBuildRequest request, CancellationToken cancellationToken)
    {
        if (request.String == default)
            return default;

        var content = request.String.Replace("\r\n", "\n");
        content = await mediator.Send(new HtmlPStringBuildRequest { String = content }, cancellationToken);
        content = await mediator.Send(new AgeCalcBuildRequest { String = content }, cancellationToken);
        content = await mediator.Send(new SvgBuildRequest { String = content }, cancellationToken);
        content = await mediator.Send(new HtmlBStringBuildRequest { String = content }, cancellationToken);
        content = await mediator.Send(new HtmlIStringBuildRequest { String = content }, cancellationToken);
        content = await mediator.Send(new HtmlCiteStringBuildRequest { String = content }, cancellationToken);
        content = await mediator.Send(new HtmlH1StringBuildRequest { String = content }, cancellationToken);
        content = await mediator.Send(new HtmlH2StringBuildRequest { String = content }, cancellationToken);
        content = await mediator.Send(new HtmlH3StringBuildRequest { String = content }, cancellationToken);
        content = await mediator.Send(new HtmlH4StringBuildRequest { String = content }, cancellationToken);
        content = await mediator.Send(new HtmlH5StringBuildRequest { String = content }, cancellationToken);
        content = await mediator.Send(new HtmlH6StringBuildRequest { String = content }, cancellationToken);
        content = await mediator.Send(new HtmlUlStringBuildRequest { String = content }, cancellationToken);
        content = await mediator.Send(new HtmlAStringBuildRequest { String = content }, cancellationToken);
        content = await mediator.Send(new BlockquoteBuildRequest { String = content }, cancellationToken);
        content = await mediator.Send(new BodyBuildRequest { String = content }, cancellationToken);
        content = await mediator.Send(new HtmlBuildRequest { Title = Environment.GetCommandLineArgs()[4], String = content }, cancellationToken);

        return content;
    }
}
