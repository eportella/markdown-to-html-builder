namespace test;

public class H2BuildTest
{
    [Theory]
    [InlineData(
@"##a
##b
##d
",
@"<h2>a
</h2><h2>b
</h2><h2>d
</h2>")]
    [InlineData(
@"##prefix *infix italic* sufix",
@"<h2>prefix <i>infix italic</i> sufix</h2>")]
    [InlineData(
@"##prefix **infix bold** sufix",
@"<h2>prefix <b>infix bold</b> sufix</h2>")]
    [InlineData(
@"##prefix **infix bold** *sufix italic*",
@"<h2>prefix <b>infix bold</b> <i>sufix italic</i></h2>")]
    [InlineData(
@"## prefix infix sufix",
@"<h2>prefix infix sufix</h2>")]

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
