namespace test;

public class H3BuildTest
{
    [Theory]
    [InlineData("### Headding", "<h3>Headding</h3>")]
    [InlineData("### #", "<h3>#</h3>")]
    [InlineData("### #Headding", "<h3>#Headding</h3>")]
    [InlineData("### # Headding", "<h3># Headding</h3>")]
    public async Task Success(string informaed, string expected)
    {
        var arrange = new HtmlH3StringBuildRequest
        {
            String = informaed
        };

        var result = await new HtmlH3StringBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}