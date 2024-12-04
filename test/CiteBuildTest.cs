namespace test;

public class CiteABuildTest
{
    [Theory]
    [InlineData(@"[^1]: Meu primeiro contato.", @"<br/><cite id=""cite-1""><a href=""#cited-1"">(1)</a>. Meu primeiro contato.</cite>")]
    [InlineData(@"<p>[^2]: Meu segundo contato.", @"<br/><cite id=""cite-2""><a href=""#cited-2"">(2)</a>. Meu segundo contato.</cite>")]
    [InlineData(@"</h>[^3]: Meu terceiro contato.", @"<br/><cite id=""cite-3""><a href=""#cited-3"">(3)</a>. Meu terceiro contato.</cite>")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new HtmlCiteStringBuildRequest
        {
            String = informed
        };

        var result = await new HtmlCiteStringBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}