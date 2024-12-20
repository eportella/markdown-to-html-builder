using System.Text.RegularExpressions;
using MediatR;
using Moq;

namespace test;

public class H5BuildTest
{
    [Theory]
    [InlineData(
@"#####a
#####b
#####d
",
@"<h5></h5><h5></h5><h5></h5>")]
    [InlineData(
@"#####prefix *infix italic* sufix",
@"<h5></h5>")]
    [InlineData(
@"#####prefix **infix bold** sufix",
@"<h5></h5>")]
    [InlineData(
@"#####prefix **infix bold** *sufix italic*",
@"<h5></h5>")]
    [InlineData(
@"##### prefix infix sufix",
@"<h5></h5>")]

    public async Task Success(string informed, string expected)
    {
        var arrange = new BuildRequest
        {
            Source = informed
        };
        var mediator = Mock.Of<IMediator>();
        static IEnumerable<Text> YieldBreak()
        {
            yield break;
        }
        Mock
            .Get(mediator)
                .Setup(s => s.CreateStream(It.IsAny<TextBuildRequest>(), CancellationToken.None))
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

        Assert.Contains(expected, result.Target?.Built);
    }
}
