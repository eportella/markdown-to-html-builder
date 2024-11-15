using Moq;

namespace test;

public class HtmlBuildTest
{
    [Fact]
    public async Task EmptyContentSuccess()
    {
        var arrange = Mock.Of<HtmlBuildRequest>();
        Mock.Get(arrange)
            .Setup(x => x.String)
            .Returns(string.Empty);

        var result = await new HtmlBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal("<html></html>", result);
    }
}