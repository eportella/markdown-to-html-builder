namespace test;

public class BlockquoteBuildBuildTest
{
    [Theory]
    [InlineData(default, default)]
    [InlineData("", "")]
    [InlineData(">[!NOTE]", @"<blockquote><p>[!NOTE]</p></blockquote>")]
    [InlineData("> [!TIP]", @"<blockquote><p>[!TIP]</p></blockquote>")]
    [InlineData(">  [!IMPORTANT]", @"<blockquote><p>[!IMPORTANT]</p></blockquote>")]
    [InlineData(">    [!WARNING]", @"<blockquote><p>[!WARNING]</p></blockquote>")]
    [InlineData(">     [!CAUTION]", @"<blockquote><p>[!CAUTION]</p></blockquote>")]
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