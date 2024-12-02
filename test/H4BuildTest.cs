namespace test;

public class H4BuildTest
{
    [Theory]
    [InlineData(default, default)]
    [InlineData("#### Headding", "<h4>Headding</h4>")]
    [InlineData("#### #", "<h4>#</h4>")]
    [InlineData("#### #Headding", "<h4>#Headding</h4>")]
    [InlineData("#### # Headding", "<h4># Headding</h4>")]
    [InlineData(@"
# tilte
## tilte
### tilte
#### tilte
##### tilte
###### tilte", @"
# tilte
## tilte
### tilte
<h4>tilte</h4>
##### tilte
###### tilte")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new HtmlH4StringBuildRequest
        {
            String = informed
        };

        var result = await new HtmlH4StringBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}