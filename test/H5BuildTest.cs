namespace test;

public class H5BuildTest
{
    [Theory]
    [InlineData(default, default)]
    [InlineData("##### Headding", "<h5>Headding</h5>")]
    [InlineData("##### #", "<h5>#</h5>")]
    [InlineData("##### #Headding", "<h5>#Headding</h5>")]
    [InlineData("##### # Headding", "<h5># Headding</h5>")]
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
#### tilte
<h5>tilte
</h5>###### tilte")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new HtmlH5StringBuildRequest
        {
            String = informed
        };

        var result = await new HtmlH5StringBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}