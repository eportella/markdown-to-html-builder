namespace test;

public class AgeCalcBuildTest
{
    const string AGE_CURRENT = "39";
    [Theory]
    [InlineData(
@"prefix `[age-calc]:1985-06-28` sufix",
@"prefix " + AGE_CURRENT + @" sufix")]
    [InlineData(
@"prefix`[age-calc]:1985-06-28` sufix",
@"prefix" + AGE_CURRENT + @" sufix")]
    [InlineData(
@"prefix`[age-calc]:1985-06-28`sufix",
@"prefix" + AGE_CURRENT + @"sufix")]
    [InlineData(
@"prefix `[age-calc]:1985-06-28`sufix",
@"prefix " + AGE_CURRENT + @"sufix")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new AgeCalcBuildRequest
        {
            Source = informed
        };

        var result = await new AgeCalcBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}
