namespace test;

public class BrBuildTest
{
    [Theory]
    [InlineData("a b", "a b")]
    [InlineData("a\nb", "a<br />b")]
    [InlineData("\na\nb\n", "<br />a<br />b<br />")]
    [InlineData("\n\na\n\nb\n\n", "<br />a<br />b<br />")]
    [InlineData("\n\n\na\n\n\nb\n\n\n", "<br />a<br />b<br />")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new HtmlBrStringBuildRequest
        {
            String = informed
        };

        var result = await new HtmlBrStringBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}