namespace test;

public class BodyBuildTest
{
    [Theory]
    [InlineData(default, "<body></body>")]
    [InlineData("", "<body></body>")]
    [InlineData("a", "<body>a</body>")]
    [InlineData("a\nB\nc\nD\n", "<body>a\nB\nc\nD\n</body>")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new BodyBuildRequest
        {
            String = informed
        };

        var result = await new BodyBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}