using MediatR;
using Moq;

namespace test;

public class HtmlBuildTest
{
    [Theory]
    [InlineData(default, "<html></html>")]
    [InlineData("", "<html></html>")]
    [InlineData("a", "<html>a</html>")]
    [InlineData("a\nB\nc\nD\n", "<html>a\nB\nc\nD\n</html>")]
    public async Task Success(string informed, string expected)
    {
        var mediator = Mock.Of<IMediator>();
        Mock.Get(mediator).Setup(s=>s.Send(It.IsAny<BodyBuildRequest>(), CancellationToken.None)).ReturnsAsync(informed);
        var arrange = new HtmlBuildRequest
        {
            String = informed
        };

        var result = await new HtmlBuildRequestHandler(mediator)
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}