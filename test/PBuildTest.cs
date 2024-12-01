namespace test;

public class PBuildTest
{
    [Theory]
    [InlineData("a", "<p>a</p>")]
    [InlineData("a\nB\nc\nD\n", "<p>a</p><p>B</p><p>c</p><p>D</p>")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new HtmlPStringBuildRequest
        {
            String = informed
        };

        var result = await new HtmlPStringBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}