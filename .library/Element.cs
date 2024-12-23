internal interface IElement
{
    internal string? Built { get; }
}

internal static class IElementExtensions
{
    internal static string Build(this IEnumerable<string?> elements)
    {
        return string.Join(string.Empty, elements);
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
