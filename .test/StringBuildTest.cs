namespace test;

public class StringBuildTest
{
    const string AGE_CURRENT = "39";
    [Theory]
    [InlineData(
@"- list item 1
- list item 2
- list item 3
- listitem4
-listitem5",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><ul><li>list item 1
</li><li>list item 2
</li><li>list item 3
</li><li>listitem4
</li><li>listitem5</li></ul></body></html>")]
    [InlineData(
@"- list item 1

- list item 1
- listitem2

-listitem3
",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><ul><li>list item 1

</li></ul><ul><li>list item 1
</li><li>listitem2

</li></ul><ul><li>listitem3
</li></ul></body></html>")]
    [InlineData(
@"prefix **infix bold** *sufix italic*",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><p>prefix <b>infix bold</b> <i>sufix italic</i></p></body></html>")]
    [InlineData(
@"**prefix bold** *infix italic* sufix",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><p><b>prefix bold</b> <i>infix italic</i> sufix</p></body></html>")]
    [InlineData(
@"*prefix italic* infix **sufix bold**",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><p><i>prefix italic</i> infix <b>sufix bold</b></p></body></html>")]
    [InlineData(
@"***text bold italic***",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><p><b><i>text bold italic</i></b></p></body></html>")]
    [InlineData(
@" *italic* **bold** ***bolditalic*** ",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><p> <i>italic</i> <b>bold</b> <b><i>bolditalic</i></b> </p></body></html>")]
    [InlineData(
@" **bold** **bold** *italic* *italic* ",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><p> <b>bold</b> <b>bold</b> <i>italic</i> <i>italic</i> </p></body></html>")]
    [InlineData(
@"**bold**
**bold**
*italic*    
    *italic*",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><p><b>bold</b>
</p><p><b>bold</b>
</p><p><i>italic</i>    
</p><p>    <i>italic</i></p></body></html>")]
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
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><ul><li>li 1.0
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
</li><li>li 2.2</li></ol></li></ol></body></html>")]

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
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><blockquote><ul><li>li 1.0
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
</li><li>li 2.2</li></ol></li></ol></blockquote></body></html>")]
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

        Assert.Equal(expected, result.Target?.Built);
    }
}
