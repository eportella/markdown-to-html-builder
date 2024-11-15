using MediatR;
using Moq;

namespace test;

public class UlBuildTest
{
    [Theory]
    [InlineData(
@"
- list item
",
"<ul>#empty#</ul>")]
    public async Task Success(string informed, string expected)
    {
        var mediator = Mock.Of<IMediator>();
        Mock.Get(mediator)
            .Setup(s => s.Send(It.IsAny<HtmlLiStringBuildRequest>(), CancellationToken.None))
            .ReturnsAsync("#empty#");

        var arrange = new HtmlUlStringBuildRequest
        {
            String = informed
        };

        var result = await new HtmlUlStringBuildRequestHandler(mediator)
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}