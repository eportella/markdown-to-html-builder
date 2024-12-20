using MediatR;
using Moq;

namespace test;

public class H2BuildTest
{
    [Theory]
    [InlineData(
@"##a
##b
##d
",
@"<h2></h2><h2></h2><h2></h2>")]
    [InlineData(
@"##prefix *infix italic* sufix",
@"<h2></h2>")]
    [InlineData(
@"##prefix **infix bold** sufix",
@"<h2></h2>")]
    [InlineData(
@"##prefix **infix bold** *sufix italic*",
@"<h2></h2>")]
    [InlineData(
@"## prefix infix sufix",
@"<h2></h2>")]

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
