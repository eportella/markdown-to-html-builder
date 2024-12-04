namespace test;

public class H1BuildTest
{
    [Theory]
    [InlineData(default, default)]
    [InlineData("# Headding", "<h1>Headding</h1>")]
    [InlineData("# #", "<h1>#</h1>")]
    [InlineData("# #Headding", "<h1>#Headding</h1>")]
    [InlineData("# # Headding", "<h1># Headding</h1>")]
    [InlineData(@"
# tilte
## tilte
### tilte
#### tilte
##### tilte
###### tilte", @"
<h1>tilte
</h1>## tilte
### tilte
#### tilte
##### tilte
###### tilte")]
[InlineData(@"# title

>blockquote", @"<h1>title

</h1>>blockquote")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new HtmlH1StringBuildRequest
        {
            String = informed
        };

        var result = await new HtmlH1StringBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}