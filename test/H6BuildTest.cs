namespace test;

public class H6BuildTest
{
    [Theory]
    [InlineData("###### Headding", "<h6>Headding</h6>")]
    [InlineData("###### #", "<h6>#</h6>")]
    [InlineData("###### #Headding", "<h6>#Headding</h6>")]
    [InlineData("###### # Headding", "<h6># Headding</h6>")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new HtmlH6StringBuildRequest
        {
            String = informed
        };

        var result = await new HtmlH6StringBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}