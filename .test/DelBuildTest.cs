namespace test;

public class DelBuildTest
{
    [Theory]
    [InlineData(
@"prefix ~~infix strikethrough~~ sufix",
@"prefix <del>infix strikethrough</del> sufix")]
    [InlineData(
@"~~prefix strikethrough~~ infix sufix",
@"<del>prefix strikethrough</del> infix sufix")]
    [InlineData(
@"prefix infix ~~sufix strikethrough~~",
@"prefix infix <del>sufix strikethrough</del>")]
    [InlineData(
@"~~text strikethrough~~",
@"<del>text strikethrough</del>")]
    [InlineData(
@" ~~strikethrough~~ ~~strikethrough~~ ",
@" <del>strikethrough</del> <del>strikethrough</del> ")]
    [InlineData(
@"~~strikethrough~~
~~strikethrough~~",
@"<del>strikethrough</del>
<del>strikethrough</del>")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new DelBuildRequest
        {
            Source = informed
        };

        var result = await new DelBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}
