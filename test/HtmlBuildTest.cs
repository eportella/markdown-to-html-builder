using Moq;

namespace test;

public class HtmlBuildTest
{
    [Fact]
    public async Task EmptyContentSuccess()
    {
        var arrange = new HtmlBuildRequest
        {
            String = string.Empty
        };

        var result = await new HtmlBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal("<html></html>", result);
    }
}