namespace test;

public class LiBuildTest
{
    [Theory]
    [InlineData(
@"
- list item
",
"<li>list item</li>")]
    [InlineData(
@"
- Conheça a minha [trajetória](trajetoria/README.md).
- Conheça a [Portella LTDA](https://portella-ltda.github.io/).
- Conheça o [Código fonte](https://github.com/eportella/eportella.github.io) dessa página.",
@"<li>Conheça a minha [trajetória](trajetoria/README.md).</li><li>Conheça a [Portella LTDA](https://portella-ltda.github.io/).</li><li>Conheça o [Código fonte](https://github.com/eportella/eportella.github.io) dessa página.</li>")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new HtmlLiStringBuildRequest
        {
            String = informed
        };

        var result = await new HtmlLiStringBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}