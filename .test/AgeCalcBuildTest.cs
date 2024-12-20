using MediatR;
using Moq;

namespace test;

public class AgeCalcBuildTest
{
    const string AGE_CURRENT = "39";
    [Theory]
    [InlineData(
@"prefix `[age-calc]:1985-06-28` sufix",
@"<p>prefix " + AGE_CURRENT + @" sufix</p>")]
    [InlineData(
@"prefix`[age-calc]:1985-06-28` sufix",
@"<p>prefix" + AGE_CURRENT + @" sufix</p>")]
    [InlineData(
@"prefix`[age-calc]:1985-06-28`sufix",
@"<p>prefix" + AGE_CURRENT + @"sufix</p>")]
    [InlineData(
@"prefix `[age-calc]:1985-06-28`sufix",
@"<p>prefix " + AGE_CURRENT + @"sufix</p>")]
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
                    BaseUrl = new Uri("https://github.com")
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
