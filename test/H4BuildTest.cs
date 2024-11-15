namespace test;

public class H4BuildTest
{
    [Theory]
    [InlineData("#### Headding", "<h4>Headding</h4>")]
    [InlineData("#### #", "<h4>#</h4>")]
    [InlineData("#### #Headding", "<h4>#Headding</h4>")]
    [InlineData("#### # Headding", "<h4># Headding</h4>")]
    public async Task Success(string informaed, string expected)
    {
        var arrange = new HtmlH4StringBuildRequest
        {
            String = informaed
        };

        var result = await new HtmlH4StringBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}