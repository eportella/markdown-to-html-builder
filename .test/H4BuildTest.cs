namespace test;

public class H4BuildTest
{
    [Theory]
    [InlineData(
@"####a
####b
####d
",
@"<h4>a
</h4><h4>b
</h4><h4>d
</h4>")]
    [InlineData(
@"####prefix *infix italic* sufix",
@"<h4>prefix <i>infix italic</i> sufix</h4>")]
    [InlineData(
@"####prefix **infix bold** sufix",
@"<h4>prefix <b>infix bold</b> sufix</h4>")]
    [InlineData(
@"####prefix **infix bold** *sufix italic*",
@"<h4>prefix <b>infix bold</b> <i>sufix italic</i></h4>")]
    [InlineData(
@"#### prefix infix sufix",
@"<h4>prefix infix sufix</h4>")]

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
