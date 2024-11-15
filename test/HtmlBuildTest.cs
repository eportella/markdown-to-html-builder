using Moq;

namespace test;

public class HtmlBuildTest
{
    [Theory]
    [InlineData("", "<html></html>")]
    public async Task EmptyContentSuccess(string informaed, string expected)
    {
        var arrange = new HtmlBuildRequest
        {
            String = informaed
        };

        var result = await new HtmlBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}