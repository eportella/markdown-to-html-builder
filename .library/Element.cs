internal interface IElement
{
    internal string? Built { get; }
}

internal static class IElementExtensions
{
    internal static string Build(this IEnumerable<IElement> elements)
    {
        return string.Join(string.Empty, elements.Select(element => element.Built));
    }
}
internal class Html : IElement
{
    internal string? Source { get; init; }
    public string? Built { get; internal set; }
}
internal class Body : IElement
{
    internal string? Source { get; init; }
    public string? Built { get; internal set; }
}
internal class P : IElement
{
    internal string? Source { get; init; }
    public string? Built { get; internal set; }
}

internal class H1 : IElement
{
    internal string? Source { get; init; }
    public string? Built { get; internal set; }
}

internal class H2 : IElement
{
    internal string? Source { get; init; }
    public string? Built { get; internal set; }
}

internal class H3 : IElement
{
    internal string? Source { get; init; }
    public string? Built { get; internal set; }
}

internal class H4 : IElement
{
    internal string? Source { get; init; }
    public string? Built { get; internal set; }
}

internal class H5 : IElement
{
    internal string? Source { get; init; }
    public string? Built { get; internal set; }
}

internal class H6 : IElement
{
    internal string? Source { get; init; }
    public string? Built { get; internal set; }
}

internal class Blockquote : IElement
{
    internal string? Source { get; init; }
    public string? Built { get; internal set; }
}

internal class Ul : IElement
{
    internal string? Source { get; init; }
    public string? Built { get; internal set; }
}

internal class Ol : IElement
{
    internal string? Source { get; init; }
    public string? Built { get; internal set; }
}

internal class LI : IElement
{
    internal string? Source { get; init; }
    public string? Built { get; internal set; }
}

internal class Cite : IElement
{
    internal string? Source { get; init; }
    public string? Built { get; internal set; }
}

internal class Text : IElement
{
    internal string? Source { get; init; }
    public string? Built { get; init; }
}
