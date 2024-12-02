namespace test;

public class ABuildTest
{
    [Theory]
    [InlineData("[]()", "<a href=\"\"></a>")]
    [InlineData("[link]()", "<a href=\"\">link</a>")]
    [InlineData("[link](/)", "<a href=\"/\">link</a>")]
    [InlineData("[link](https://site.domain.xx)", "<a href=\"https://site.domain.xx\">link</a>")]
    [InlineData("[link](https://site.domain.xx/README.md)", "<a href=\"https://site.domain.xx/\">link</a>")]
    [InlineData("- Também com 16 anos ouvi pela primeira primeira vez a profissão programador de computadores[^2] por um colega de infância que havia iniciado sua trajetória profissional como programador na [*Microsoft*](https://www.microsoft.com/)", "<a href=\"https://www.microsoft.com/\">*Microsoft*</a>")]
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