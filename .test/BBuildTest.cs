namespace test;

public class BBuildTest
{
    [Theory]
    [InlineData(
@"prefix **infix bold** sufix",
@"<p>prefix <b>infix bold</b> sufix</p>")]
    [InlineData(
@"**prefix bold** infix sufix",
@"<p><b>prefix bold</b> infix sufix</p>")]
    [InlineData(
@"prefix infix **sufix bold**",
@"<p>prefix infix <b>sufix bold</b></p>")]
    [InlineData(
@"**text bold**",
@"<p><b>text bold</b></p>")]
    [InlineData(
@" **bold** **bold** ",
@"<p> <b>bold</b> <b>bold</b> </p>")]
    [InlineData(
@"**bold**
**bold**",
@"<p><b>bold</b>
<b>bold</b></p>")]
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
                    BaseUrl = new Uri("https://github.com")
                }
            )
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Contains(expected, result.Target?.Built);
    }
}
