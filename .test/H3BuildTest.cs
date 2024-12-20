using MediatR;
using Moq;

namespace test;

public class H3BuildTest
{
    [Theory]
    [InlineData(
@"###a
###b
###d
",
@"<h3></h3><h3></h3><h3></h3>")]
    [InlineData(
@"###prefix *infix italic* sufix",
@"<h3></h3>")]
    [InlineData(
@"###prefix **infix bold** sufix",
@"<h3></h3>")]
    [InlineData(
@"###prefix **infix bold** *sufix italic*",
@"<h3></h3>")]
    [InlineData(
@"### prefix infix sufix",
@"<h3></h3>")]

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
