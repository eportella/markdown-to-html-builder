namespace test;

public class AgeCalcBuildTest
{
    [Theory]
    [InlineData("`[age-calc]:1985-06-28`", "39")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new AgeCalcBuildRequest
        {
            String = informed
        };

        var result = await new AgeCalcBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}