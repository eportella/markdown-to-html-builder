namespace test;

public class H1BuildTest
{
    [Theory]
    [InlineData(
@"#a
#b
#d
",
@"<h1>a
</h1><h1>b
</h1><h1>d
</h1>")]
    [InlineData(
@"#prefix *infix italic* sufix",
@"<h1>prefix <i>infix italic</i> sufix</h1>")]
    [InlineData(
@"#prefix **infix bold** sufix",
@"<h1>prefix <b>infix bold</b> sufix</h1>")]
    [InlineData(
@"#prefix **infix bold** *sufix italic*",
@"<h1>prefix <b>infix bold</b> <i>sufix italic</i></h1>")]
    [InlineData(
@"# prefix infix sufix",
@"<h1>prefix infix sufix</h1>")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new BuildRequest
        {
            Source = informed
        };

        var result = await new BuildRequestHandler(
                new ProjectBuildResponse
                {
                    Title = "--title--",
                    BaseUrl = new Uri("https://github.com"),
                }
            )
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Contains(expected, result.Target?.Built);
    }
}
