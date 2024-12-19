namespace test;

public class H3BuildTest
{
    [Theory]
    [InlineData(
@"###a
###b
###d
",
@"<h3>a
</h3><h3>b
</h3><h3>d
</h3>")]
    [InlineData(
@"###prefix *infix italic* sufix",
@"<h3>prefix <i>infix italic</i> sufix</h3>")]
    [InlineData(
@"###prefix **infix bold** sufix",
@"<h3>prefix <b>infix bold</b> sufix</h3>")]
    [InlineData(
@"###prefix **infix bold** *sufix italic*",
@"<h3>prefix <b>infix bold</b> <i>sufix italic</i></h3>")]
    [InlineData(
@"### prefix infix sufix",
@"<h3>prefix infix sufix</h3>")]

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
