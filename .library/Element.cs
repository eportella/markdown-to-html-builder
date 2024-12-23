internal static class StringExtensions
{
    internal static string Build(this IEnumerable<string?> elements)
    {
        return string.Join(string.Empty, elements);
    }
}
