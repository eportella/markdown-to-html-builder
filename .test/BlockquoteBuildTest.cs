namespace test;

public class BlockquoteBuildTest
{
    [Theory]
    [InlineData(
@">a
",
@"<blockquote><p>a
</p></blockquote>")]
    [InlineData(
@">a
>b
",
@"<blockquote><p>a
b
</p></blockquote>")]
    [InlineData(
@">a
>b
>c
",
@"<blockquote><p>a
b
c
</p></blockquote>")]
    [InlineData(
@">quote 1

>quote2
",
@"<blockquote><p>quote 1
</p></blockquote><blockquote><p>quote2
</p></blockquote>")]
    [InlineData(
@">a
>>b
>c
",
@"<blockquote><p>a
</p><blockquote><p>b
</p></blockquote><p>c
</p></blockquote>")]
    [InlineData(
@">a
>>b
>>>c
>>>d
>e",
@"<blockquote><p>a
</p><blockquote><p>b
</p><blockquote><p>c
d
</p></blockquote></blockquote><p>e</p></blockquote>")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new BuildRequest
        {
            Source = informed
        };

        var result = await new BuildRequestHandler(
                new InputBuildResponse
                {
                    BaseUrl = new Uri("https://github.com")
                }, 
                new ProjectBuildResponse
                {
                    Value = "--title--"
                }
            )
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Contains(expected, result.Target?.Built);
    }
}
