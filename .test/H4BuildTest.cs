using MediatR;
using Moq;

namespace test;

public class H4BuildTest
{
    [Theory]
    [InlineData(
@"####a
####b
####d
",
@"<h4></h4><h4></h4><h4></h4>")]
    [InlineData(
@"####prefix *infix italic* sufix",
@"<h4></h4>")]
    [InlineData(
@"####prefix **infix bold** sufix",
@"<h4></h4>")]
    [InlineData(
@"####prefix **infix bold** *sufix italic*",
@"<h4></h4>")]
    [InlineData(
@"#### prefix infix sufix",
@"<h4></h4>")]

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
