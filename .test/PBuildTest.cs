using MediatR;
using Moq;

namespace test;

public class PBuildTest
{
    [Theory]
    [InlineData(
@"a",
@"<p>a</p>")]
    [InlineData(
@"ab",
@"<p>ab</p>")]
    [InlineData(
@"abc",
@"<p>abc</p>")]
    [InlineData(
@"a
",
@"<p>a
</p>")]
    [InlineData(
@"a
b
",
@"<p>a
b
</p>")]
    [InlineData(
@"a
b
c
",
@"<p>a
b
c
</p>")]
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
