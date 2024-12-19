namespace test;

public class DelBuildTest
{
    [Theory]
    [InlineData(
@"prefix ~~infix strikethrough~~ sufix",
@"<p>prefix <del>infix strikethrough</del> sufix</p>")]
    [InlineData(
@"~~prefix strikethrough~~ infix sufix",
@"<p><del>prefix strikethrough</del> infix sufix</p>")]
    [InlineData(
@"prefix infix ~~sufix strikethrough~~",
@"<p>prefix infix <del>sufix strikethrough</del></p>")]
    [InlineData(
@"~~text strikethrough~~",
@"<p><del>text strikethrough</del></p>")]
    [InlineData(
@" ~~strikethrough~~ ~~strikethrough~~ ",
@"<p> <del>strikethrough</del> <del>strikethrough</del> </p>")]
    [InlineData(
@"~~strikethrough~~
~~strikethrough~~",
@"<p><del>strikethrough</del>
<del>strikethrough</del></p>")]
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
