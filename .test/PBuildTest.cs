using System.Text.RegularExpressions;
using MediatR;
using Moq;

namespace test;

public class PBuildTest
{
    [Theory]
    [InlineData(
@"a",
@"<p></p>")]
    [InlineData(
@"ab",
@"<p></p>")]
    [InlineData(
@"abc",
@"<p></p>")]
    [InlineData(
@"a
",
@"<p></p>")]
    [InlineData(
@"a
b
",
@"<p></p>")]
    [InlineData(
@"a
b
c
",
@"<p></p>")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new BuildRequest
        {
            Source = informed
        };
        var mediator = Mock.Of<IMediator>();
        static IEnumerable<string> YieldBreak()
        {
            yield break;
        }
        Mock
            .Get(mediator)
                .Setup(s => s.CreateStream(It.IsAny<InlineBuildRequest>(), CancellationToken.None))
                .Returns(YieldBreak().ToAsyncEnumerable());

        var result = await new BuildRequestHandler(
                new ProjectBuildResponse
                {
                    Title = "--title--",
                    BaseUrl = new Uri("https://github.com"),
                },
                mediator
            )
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Contains(expected, result);
    }
}
