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
                new InputBuildResponse
                {
                    BaseUrl = new Uri("https://github.com")
                }, 
                new ProjectBuildResponse
                {
                    Value = "--title--"
                }
            )
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Contains(expected, result.Target?.Built);
    }
}
