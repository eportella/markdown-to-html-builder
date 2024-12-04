namespace test;

public class H3BuildTest
{
    [Theory]
    [InlineData(default, default)]
    [InlineData("### Headding", "<h3>Headding</h3>")]
    [InlineData("### #", "<h3>#</h3>")]
    [InlineData("### #Headding", "<h3>#Headding</h3>")]
    [InlineData("### # Headding", "<h3># Headding</h3>")]
    [InlineData(@"
# tilte
## tilte
### tilte
#### tilte
##### tilte
###### tilte", @"
# tilte
## tilte
<h3>tilte
</h3>#### tilte
##### tilte
###### tilte")]
    [InlineData(@"### title

>blockquote", @"<h3>title

</h3>>blockquote")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new HtmlH3StringBuildRequest
        {
            String = informed
        };

        var result = await new HtmlH3StringBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}