namespace test;

public class BlockquoteBuildBuildTest
{
    [Theory]
    [InlineData(default, default)]
    [InlineData("", "")]
    [InlineData("[!NOTE]\n>text note", @"<blockquote style=""border-color: #1f6feb;""><p style=""display:flex; align-items:center; column-gap:0.4em; font-weight:500;"">[!NOTE]</p></blockquote>")]
    [InlineData("[!TIP]\n>text tip", @"<blockquote style=""border-color: #3fb950;""><p style=""display:flex; align-items:center; column-gap:0.4em; font-weight:500;"">[!TIP]</p></blockquote>")]
    [InlineData("[!IMPORTANT]\n>text important", @"<blockquote style=""border-color: #ab7df8;""><p style=""display:flex; align-items:center; column-gap:0.4em; font-weight:500;"">[!IMPORTANT]</p></blockquote>")]
    [InlineData("[!WARNING]\n>text warning", @"<blockquote style=""border-color: #d29922;""><p style=""display:flex; align-items:center; column-gap:0.4em; font-weight:500;"">[!WARNING]</p></blockquote>")]
    [InlineData("[!CAUTION]\n>text caution", @"<blockquote style=""border-color: #f85149;""><p style=""display:flex; align-items:center; column-gap:0.4em; font-weight:500;"">[!CAUTION]</p></blockquote>")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new BlockquoteBuildRequest
        {
            String = informed
        };

        var result = await new BlockquoteBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}