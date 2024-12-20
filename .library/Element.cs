internal interface IElement
{
    internal IElement? Parent { get; init; }
    internal IElement[]? Children { get; }
    internal string? Built { get; }
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
internal class Html : IElement
{
    public IElement? Parent { get; init; }
    internal string? Source { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
}
internal class Body : IElement
{
    public IElement? Parent { get; init; }
    internal string? Source { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
}
internal class P : IElement
{
    public IElement? Parent { get; init; }
    internal string? Source { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
}

internal class H1 : IElement
{
    public IElement? Parent { get; init; }
    internal string? Source { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
}

internal class H2 : IElement
{
    public IElement? Parent { get; init; }
    internal string? Source { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
}

internal class H3 : IElement
{
    public IElement? Parent { get; init; }
    internal string? Source { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
}

internal class H4 : IElement
{
    public IElement? Parent { get; init; }
    internal string? Source { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
}

internal class H5 : IElement
{
    public IElement? Parent { get; init; }
    internal string? Source { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
}

internal class H6 : IElement
{
    public IElement? Parent { get; init; }
    internal string? Source { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
}

internal class Blockquote : IElement
{
    public IElement? Parent { get; init; }
    internal string? Source { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
}

internal class Ul : IElement
{
    public IElement? Parent { get; init; }
    internal string? Source { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
}

internal class Ol : IElement
{
    public IElement? Parent { get; init; }
    internal string? Source { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
}

internal class LI : IElement
{
    public IElement? Parent { get; init; }
    internal string? Source { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
}

internal class Cite : IElement
{
    public IElement? Parent { get; init; }
    internal string? Source { get; init; }
    public IElement[]? Children { get; internal set; }
    public string? Built { get; internal set; }
}

internal class Text : IElement
{
    public IElement? Parent { get; init; }
    internal string? Source { get; init; }
    public IElement[]? Children { get; init; }
    public string? Built { get; init; }
}
