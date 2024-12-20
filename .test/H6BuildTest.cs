using MediatR;
using Moq;

namespace test;

public class H6BuildTest
{
    [Theory]
    [InlineData(
@"######a
######b
######d
",
@"<h6></h6><h6></h6><h6></h6>")]
    [InlineData(
@"######prefix *infix italic* sufix",
@"<h6></h6>")]
    [InlineData(
@"######prefix **infix bold** sufix",
@"<h6></h6>")]
    [InlineData(
@"######prefix **infix bold** *sufix italic*",
@"<h6></h6>")]
    [InlineData(
@"###### prefix infix sufix",
@"<h6></h6>")]

    public async Task Success(string informed, string expected)
    {
        var arrange = new BuildRequest
        {
            Source = informed
        };
        var mediator = Mock.Of<IMediator>();
        static async IAsyncEnumerable<Text> YieldBreak()
        {
            await Task.Yield();
            yield break;
        }
        Mock
            .Get(mediator)
                .Setup(s => s.CreateStream(It.IsAny<TextBuildRequest>(), CancellationToken.None))
                .Returns(YieldBreak());

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
