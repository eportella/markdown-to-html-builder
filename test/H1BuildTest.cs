namespace test;

public class H1BuildTest
{
    [Theory]
    [InlineData("# teste", "<h1>teste</h1>")]
    [InlineData("# #", "<h1>#</h1>")]
    [InlineData("# #teste", "<h1>#teste</h1>")]
    [InlineData("# # teste", "<h1># teste</h1>")]
    public async Task Success(string informaed, string expected)
    {
        var arrange = new HtmlH1StringBuildRequest
        {
            String = informaed
        };

        var result = await new HtmlH1StringBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}