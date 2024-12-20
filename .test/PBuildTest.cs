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
        Mock
            .Get(mediator)
            .Setup(s => s.Send(It.IsAny<TextBuildRequest>(), CancellationToken.None))
            .ReturnsAsync(new Text { });

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

        Assert.Contains(expected, result.Target?.Built);
    }
}
