using MediatR;
using Moq;

namespace test;

public class IBuildTest
{
    [Theory]
    [InlineData(
@"prefix *infix italic* sufix",
@"<p>prefix <i>infix italic</i> sufix</p>")]
    [InlineData(
@"*prefix italic* infix sufix",
@"<p><i>prefix italic</i> infix sufix</p>")]
    [InlineData(
@"prefix infix *sufix italic*",
@"<p>prefix infix <i>sufix italic</i></p>")]
    [InlineData(
@"*text italic*",
@"<p><i>text italic</i></p>")]
    [InlineData(
@" *italic* *italic* ",
@"<p> <i>italic</i> <i>italic</i> </p>")]
    [InlineData(
@"*italic*
*italic*",
@"<p><i>italic</i>
<i>italic</i></p>")]
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
                },
                Mock.Of<IMediator>()
            )
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Contains(expected, result.Target?.Built);
    }
}
