namespace test;

public class HtmlBuildTest
{
    [Theory]
    [InlineData(default, default, "<html><title></title></html>")]
    [InlineData("", default, "<html><title></title></html>")]
    [InlineData("a", "A", "<html><title>A</title>a</html>")]
    [InlineData("a\nB\nc\nD\n", "A", "<html><title>A</title>a\nB\nc\nD\n</html>")]
    public async Task Success(string informed, string title, string expected)
    {
        var arrange = new HtmlBuildRequest
        {
            Title = title,
            String = informed
        };

        var result = await new HtmlBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}