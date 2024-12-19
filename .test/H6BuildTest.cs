namespace test;

public class H6BuildTest
{
    [Theory]
    [InlineData(
@"######a
######b
######d
",
@"<h6>a
</h6><h6>b
</h6><h6>d
</h6>")]
    [InlineData(
@"######prefix *infix italic* sufix",
@"<h6>prefix <i>infix italic</i> sufix</h6>")]
    [InlineData(
@"######prefix **infix bold** sufix",
@"<h6>prefix <b>infix bold</b> sufix</h6>")]
    [InlineData(
@"######prefix **infix bold** *sufix italic*",
@"<h6>prefix <b>infix bold</b> <i>sufix italic</i></h6>")]
    [InlineData(
@"###### prefix infix sufix",
@"<h6>prefix infix sufix</h6>")]

    public async Task Success(string informed, string expected)
    {
        var arrange = new BuildRequest
        {
            Source = informed
        };

        var result = await new BuildRequestHandler(
                new InputBuildResponse
                {
                    BaseUrl = new Uri("https://github.com")
                }, 
                new ProjectBuildResponse
                {
                    Value = "--title--"
                }
            )
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Contains(expected, result.Target?.Built);
    }
}
