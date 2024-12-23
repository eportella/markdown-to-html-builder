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
    public string? Built { get; internal set; }
}
internal class Blockquote : IElement
{
    public string? Built { get; internal set; }
}
internal class Text : IElement
{
    public string? Built { get; init; }
}
