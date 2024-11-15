namespace test;

public class H2BuildTest
{
    [Theory]
    [InlineData("## Headding", "<h2>Headding</h2>")]
    [InlineData("## #", "<h2>#</h2>")]
    [InlineData("## #Headding", "<h2>#Headding</h2>")]
    [InlineData("## # Headding", "<h2># Headding</h2>")]
    public async Task Success(string informaed, string expected)
    {
        var arrange = new HtmlH2StringBuildRequest
        {
            String = informaed
        };

        var result = await new HtmlH2StringBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}