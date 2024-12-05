namespace test;

public class StringBuildTest
{
    const string AGE_CURRENT = "39";
    [Theory]
    [InlineData(
default,
default)]
    [InlineData(
@"",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1></body></html>")]
    [InlineData(
@"a",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><p>a</p></body></html>")]
    [InlineData(
@"ab",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><p>ab</p></body></html>")]
    [InlineData(
@"abc",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><p>abc</p></body></html>")]
    [InlineData(
@"`[age-calc]:1985-06-28` abc [link](https://url)",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><p>" + AGE_CURRENT + @" abc <a href=""https://url"">link</a></p></body></html>")]
    [InlineData(
@"
",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1></body></html>")]
    [InlineData(
@"a
",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><p>a
</p></body></html>")]
    [InlineData(
@"a
b
",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><p>a
</p><p>b
</p></body></html>")]
    [InlineData(
@"a
b
c
",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><p>a
</p><p>b
</p><p>c
</p></body></html>")]
    [InlineData(
@"a
#b
c
#d
",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><p>a
</p><h1>b
</h1><p>c
</p><h1>d
</h1></body></html>")]
    [InlineData(
@"#prefix *infix italic* sufix",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h1>prefix <i>infix italic</i> sufix</h1></body></html>")]
    [InlineData(
@"##*prefix italic* infix sufix",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h2><i>prefix italic</i> infix sufix</h2></body></html>")]
    [InlineData(
@"###prefix infix *sufix italic*",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h3>prefix infix <i>sufix italic</i></h3></body></html>")]
    [InlineData(
@"####*text italic*",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h4><i>text italic</i></h4></body></html>")]
    [InlineData(
@"##### *italic*
",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h5><i>italic</i>
</h5></body></html>")]
    [InlineData(
@"###### *italic* ",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h6><i>italic</i> </h6></body></html>")]
    [InlineData(
@"#prefix **infix bold** sufix",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h1>prefix <b>infix bold</b> sufix</h1></body></html>")]
    [InlineData(
@"##**prefix bold** infix sufix",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h2><b>prefix bold</b> infix sufix</h2></body></html>")]
    [InlineData(
@"###prefix infix **sufix bold**",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h3>prefix infix <b>sufix bold</b></h3></body></html>")]
    [InlineData(
@"####**text bold**",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h4><b>text bold</b></h4></body></html>")]
    [InlineData(
@"##### **bold**
",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h5><b>bold</b>
</h5></body></html>")]
    [InlineData(
@"###### **bold** ",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h6><b>bold</b> </h6></body></html>")]
    [InlineData(
@"#prefix **infix bold** *sufix italic*",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h1>prefix <b>infix bold</b> <i>sufix italic</i></h1></body></html>")]
    [InlineData(
@"##**prefix bold** *infix italic* sufix",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h2><b>prefix bold</b> <i>infix italic</i> sufix</h2></body></html>")]
    [InlineData(
@"###*prefix italic* infix **sufix bold**",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h3><i>prefix italic</i> infix <b>sufix bold</b></h3></body></html>")]
    [InlineData(
@"####***text bold italic***",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h4><b><i>text bold italic</i></b></h4></body></html>")]
    [InlineData(
@"##### *italic* **bold** ***bolditalic***",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h5><i>italic</i> <b>bold</b> <b><i>bolditalic</i></b></h5></body></html>")]
    [InlineData(
@"###### **bold** **bold** *italic* *italic*",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h6><b>bold</b> <b>bold</b> <i>italic</i> <i>italic</i></h6></body></html>")]

    [InlineData(
@"#prefix `[age-calc]:1985-06-28` [link](https://url)",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h1>prefix " + AGE_CURRENT + @" <a href=""https://url"">link</a></h1></body></html>")]
    [InlineData(
@"##prefix [`[age-calc]:1985-06-28`](https://url)",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h2>prefix <a href=""https://url"">" + AGE_CURRENT + @"</a></h2></body></html>")]
    [InlineData(
@"###prefix [link](https://url`[age-calc]:1985-06-28`)",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h3>prefix <a href=""https://url" + AGE_CURRENT + @""">link</a></h3></body></html>")]
    [InlineData(
@"####`[age-calc]:1985-06-28`[link](https://url)",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h4>" + AGE_CURRENT + @"<a href=""https://url"">link</a></h4></body></html>")]
    [InlineData(
@"#####[link](https://url)`[age-calc]:1985-06-28`
",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h5><a href=""https://url"">link</a>" + AGE_CURRENT + @"
</h5></body></html>")]
    [InlineData(
@"###### *[link](https://url)italic`[age-calc]:1985-06-28`* ",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h6><i><a href=""https://url"">link</a>italic" + AGE_CURRENT + @"</i> </h6></body></html>")]

    [InlineData(
@"# heading1
## heading2
### heading 3
#### heading 4
##### heading 5
###### heagind 6
",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h1>heading1
</h1><h2>heading2
</h2><h3>heading 3
</h3><h4>heading 4
</h4><h5>heading 5
</h5><h6>heagind 6
</h6></body></html>")]
    [InlineData(
@"###### heagind 6
##### heading 5
#### heading 4
### heading 3
## heading2
# heading1
",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h6>heagind 6
</h6><h5>heading 5
</h5><h4>heading 4
</h4><h3>heading 3
</h3><h2>heading2
</h2><h1>heading1
</h1></body></html>")]
    [InlineData(
@"#heading1
##heading2
###heading 3
######heagind 6
#####heading 5
####heading 4
",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><h1>heading1
</h1><h2>heading2
</h2><h3>heading 3
</h3><h6>heagind 6
</h6><h5>heading 5
</h5><h4>heading 4
</h4></body></html>")]
    [InlineData(
@">a
",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><blockquote><p>a
</p></blockquote></body></html>")]
    [InlineData(
@">a
>b
",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><blockquote><p>a
</p><p>b
</p></blockquote></body></html>")]
    [InlineData(
@">a
>b
>c
",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><blockquote><p>a
</p><p>b
</p><p>c
</p></blockquote></body></html>")]
    [InlineData(
@">quote 1

>quote2
",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><blockquote><p>quote 1
</p></blockquote><blockquote><p>quote2
</p></blockquote></body></html>")]
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
@"`[age-calc]:1985-06-28`[link](https://url)",
@"<html><title>--title--</title><body><h1><a href=""--url--""/>--title--</a></h1><p>" + AGE_CURRENT + @"<a href=""https://url"">link</a></p></body></html>")]


    public async Task Success(string informed, string expected)
    {
        var arrange = new StringBuildRequest
        {
            Title = "--title--",
            Url = "--url--",
            Source = informed
        };

        var result = await new StringBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result.Target?.Html);
    }
}
