namespace test;

public class HtmlBuildTest
{
    [Theory]
    [InlineData(default, "<html></html>")]
    [InlineData("", "<html></html>")]
    [InlineData("a", "<html>a</html>")]
    [InlineData("a\nB\nc\nD\n", "<html>a\nB\nc\nD\n</html>")]
    public async Task Success(string informaed, string expected)
    {
        var arrange = new HtmlBuildRequest
        {
            String = informaed
        };

        var result = await new HtmlBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}