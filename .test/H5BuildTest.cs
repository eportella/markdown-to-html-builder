namespace test;

public class H5BuildTest
{
    [Theory]
    [InlineData(
@"#####a
#####b
#####d
",
@"<h5>a
</h5><h5>b
</h5><h5>d
</h5>")]
    [InlineData(
@"#####prefix *infix italic* sufix",
@"<h5>prefix <i>infix italic</i> sufix</h5>")]
    [InlineData(
@"#####prefix **infix bold** sufix",
@"<h5>prefix <b>infix bold</b> sufix</h5>")]
    [InlineData(
@"#####prefix **infix bold** *sufix italic*",
@"<h5>prefix <b>infix bold</b> <i>sufix italic</i></h5>")]
    [InlineData(
@"##### prefix infix sufix",
@"<h5>prefix infix sufix</h5>")]

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
