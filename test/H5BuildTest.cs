namespace test;

public class H5BuildTest
{
    [Theory]
    [InlineData("##### Headding", "<h5>Headding</h5>")]
    [InlineData("##### #", "<h5>#</h5>")]
    [InlineData("##### #Headding", "<h5>#Headding</h5>")]
    [InlineData("##### # Headding", "<h5># Headding</h5>")]
    public async Task Success(string informaed, string expected)
    {
        var arrange = new HtmlH5StringBuildRequest
        {
            String = informaed
        };

        var result = await new HtmlH5StringBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}