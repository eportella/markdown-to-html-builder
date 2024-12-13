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
                new InputBuildResponse
                {
                    BaseUrl = new Uri("https://github.com")
                }, 
                new TitleBuildResponse
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
