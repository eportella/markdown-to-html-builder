namespace test;

public class ABuildTest
{
    [Theory]
    [InlineData("[]()", "<a href=\"\"></a>")]
    [InlineData("[link]()", "<a href=\"\">link</a>")]
    [InlineData("[link](/)", "<a href=\"/\">link</a>")]
    [InlineData("[link](https://site.domain.xx)", "<a href=\"https://site.domain.xx\">link</a>")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new HtmlAStringBuildRequest
        {
            String = informed
        };

        var result = await new HtmlAStringBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}