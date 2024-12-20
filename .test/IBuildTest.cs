using MediatR;
using Moq;

namespace test;

public class IBuildTest
{
    [Theory]
    [InlineData(
@"prefix *infix italic* sufix",
@"prefix <i>infix italic</i> sufix")]
    [InlineData(
@"*prefix italic* infix sufix",
@"<i>prefix italic</i> infix sufix")]
    [InlineData(
@"prefix infix *sufix italic*",
@"prefix infix <i>sufix italic</i>")]
    [InlineData(
@"*text italic*",
@"<i>text italic</i>")]
    [InlineData(
@" *italic* *italic* ",
@" <i>italic</i> <i>italic</i> ")]
    [InlineData(
@"*italic*
*italic*",
@"<i>italic</i>
<i>italic</i>")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new IBuildRequest
        {
            Source = informed
        };

        var result = await new IBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Contains(expected, result!.Target);
    }
}
