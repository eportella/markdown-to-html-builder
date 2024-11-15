namespace test;

public class LiBuildTest
{
    [Theory]
    [InlineData(
@"
- list item
",
"<li>list item</li>")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new HtmlLiStringBuildRequest
        {
            String = informed
        };

        var result = await new HtmlLiStringBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}