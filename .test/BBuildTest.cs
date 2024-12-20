using MediatR;
using Moq;

namespace test;

public class BBuildTest
{
    [Theory]
    [InlineData(
@"prefix **infix bold** sufix",
@"prefix <b>infix bold</b> sufix")]
    [InlineData(
@"**prefix bold** infix sufix",
@"<b>prefix bold</b> infix sufix")]
    [InlineData(
@"prefix infix **sufix bold**",
@"prefix infix <b>sufix bold</b>")]
    [InlineData(
@"**text bold**",
@"<b>text bold</b>")]
    [InlineData(
@" **bold** **bold** ",
@" <b>bold</b> <b>bold</b> ")]
    [InlineData(
@"**bold**
**bold**",
@"<b>bold</b>
<b>bold</b>")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new BBuildRequest
        {
            Source = informed
        };

        var result = await new BBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Contains(expected, result!.Target);
    }
}
