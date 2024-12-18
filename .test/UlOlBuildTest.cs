namespace test;

public class UlOlBuildTest
{
    [Theory]
    [InlineData(
@"- list item 1
- list item 2
- list item 3
- listitem4
-listitem5",
@"<ul><li>list item 1
</li><li>list item 2
</li><li>list item 3
</li><li>listitem4
</li><li>listitem5</li></ul>")]
    [InlineData(
@"- list item 1

- list item 1
- listitem2

-listitem3
",
@"<ul><li>list item 1

</li></ul><ul><li>list item 1
</li><li>listitem2

</li></ul><ul><li>listitem3
</li></ul>")]
    [InlineData(
@"- li 1.0
- li 1.1
- li 1.2
    - li 2.0
    - li 2.1
    - li 2.2

- li 1.0
- li 1.1
- li 1.2
    1. li 2.0
    1. li 2.1
    1. li 2.2

1. li 1.0
2. li 1.1
3. li 1.2
    - li 2.0
    - li 2.1
    - li 2.2

1. li 1.0
2. li 1.1
3. li 1.2
    1. li 2.0
    9. li 2.1
    8. li 2.2",
@"<ul><li>li 1.0
</li><li>li 1.1
</li><li>li 1.2
<ul><li>li 2.0
</li><li>li 2.1
</li><li>li 2.2

</li></ul></li></ul><ul><li>li 1.0
</li><li>li 1.1
</li><li>li 1.2
<ol><li>li 2.0
</li><li>li 2.1
</li><li>li 2.2

</li></ol></li></ul><ol><li>li 1.0
</li><li>li 1.1
</li><li>li 1.2
<ul><li>li 2.0
</li><li>li 2.1
</li><li>li 2.2

</li></ul></li></ol><ol><li>li 1.0
</li><li>li 1.1
</li><li>li 1.2
<ol><li>li 2.0
</li><li>li 2.1
</li><li>li 2.2</li></ol></li></ol>")]

    [InlineData(
@">- li 1.0
>- li 1.1
>- li 1.2
>    - li 2.0
>    - li 2.1
>    - li 2.2
>
>- li 1.0
>- li 1.1
>- li 1.2
>    1. li 2.0
>    1. li 2.1
>    1. li 2.2
>
>1. li 1.0
>2. li 1.1
>3. li 1.2
>    - li 2.0
>    - li 2.1
>    - li 2.2
>
>1. li 1.0
>2. li 1.1
>3. li 1.2
>    1. li 2.0
>    9. li 2.1
>    8. li 2.2",
@"<blockquote><ul><li>li 1.0
</li><li>li 1.1
</li><li>li 1.2
<ul><li>li 2.0
</li><li>li 2.1
</li><li>li 2.2

</li></ul></li></ul><ul><li>li 1.0
</li><li>li 1.1
</li><li>li 1.2
<ol><li>li 2.0
</li><li>li 2.1
</li><li>li 2.2

</li></ol></li></ul><ol><li>li 1.0
</li><li>li 1.1
</li><li>li 1.2
<ul><li>li 2.0
</li><li>li 2.1
</li><li>li 2.2

</li></ul></li></ol><ol><li>li 1.0
</li><li>li 1.1
</li><li>li 1.2
<ol><li>li 2.0
</li><li>li 2.1
</li><li>li 2.2</li></ol></li></ol></blockquote>")]
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
                new TitleBuildResponse
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
