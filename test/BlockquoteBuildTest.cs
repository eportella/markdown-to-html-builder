namespace test;

public class BlockquoteBuildBuildTest
{
    [Theory]
    [InlineData(default, default)]
    [InlineData("", "")]
    [InlineData(">[!NOTE]", "<blockquote><p>[!NOTE]</p></blockquote>")]
    [InlineData("> [!TIP]", "<blockquote><p>[!TIP]</p></blockquote>")]
    [InlineData(">  [!IMPORTANT]", "<blockquote><p>[!IMPORTANT]</p></blockquote>")]
    [InlineData(">    [!WARNING]", "<blockquote><p>[!WARNING]</p></blockquote>")]
    [InlineData(">     [!CAUTION]", "<blockquote><p>[!CAUTION]</p></blockquote>")]
    [InlineData(">paragraph 1\n>paragraph 1.1", "<blockquote><p>paragraph 1</p><p>paragraph 1.1</p></blockquote>")]
    [InlineData("> paragraph 1 \n>   paragraph 1.1       ", "<blockquote><p>paragraph 1</p><p>paragraph 1.1</p></blockquote>")]
    [InlineData(">paragraph 1\n>paragraph 1.1\n\n>paragraph 2", "<blockquote><p>paragraph 1</p><p>paragraph 1.1</p></blockquote>\n<blockquote><p>paragraph 2</p></blockquote>")]
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