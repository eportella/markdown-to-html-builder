namespace test;

public class UlBuildTest
{
    [Theory]
    [InlineData(
@"
- a
", 
"<ul><li>a</li>></ul>")]
    public async Task Success(string informaed, string expected)
    {
        var arrange = new HtmlUlStringBuildRequest
        {
            String = informaed
        };

        var result = await new HtmlUlStringBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}