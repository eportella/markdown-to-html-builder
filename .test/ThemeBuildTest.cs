using MediatR;
using Moq;

namespace test;

public class ThemeBuildTest
{
    [Theory]
    [InlineData(
@"[!.BD0]padrão",
@"<span class=""theme d-b-0"">padrão</span>")]
    [InlineData(
@"[!.FD0]padrão",
@"<span class=""theme d-f-0"">padrão</span>")]
[InlineData(
@"[!.BD0.FD0]padrão",
@"<span class=""theme d-b-0 d-f-0"">padrão</span>")]
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
