using System.Text.RegularExpressions;

namespace test;

public class PBuildTest
{
    [Theory]
    [InlineData("a", "<p>a</p>")]
    [InlineData(@"
a
B
c
D
", @"
<p>a
</p><p>B
</p><p>c
</p><p>D
</p>")]
    [InlineData(@"
# markdown-to-html-builder
## markdown-to-html-builder
### markdown-to-html-builder
#### markdown-to-html-builder
##### markdown-to-html-builder
###### markdown-to-html-builder
paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0.
paragraph 1 paragraph 1 paragraph 1 paragraph 1 paragraph 1.

paragraph 2 paragraph 2 paragraph 2 paragraph 2 paragraph 2.


paragraph 3 paragraph 3 paragraph 3 paragraph 3 paragraph 3.



*italic*
**bold**
***bold and italic***

[]()
[google](https://google.com)", @"
# markdown-to-html-builder
## markdown-to-html-builder
### markdown-to-html-builder
#### markdown-to-html-builder
##### markdown-to-html-builder
###### markdown-to-html-builder
<p>paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0 paragraph 0.
</p><p>paragraph 1 paragraph 1 paragraph 1 paragraph 1 paragraph 1.

</p><p>paragraph 2 paragraph 2 paragraph 2 paragraph 2 paragraph 2.


</p><p>paragraph 3 paragraph 3 paragraph 3 paragraph 3 paragraph 3.



</p><p>*italic*
</p><p>**bold**
</p><p>***bold and italic***

</p><p>[]()
</p><p>[google](https://google.com)</p>")]
    [InlineData(@"
- li 1.0
- li 1.1
- li 1.2
    - li 2.0
    - li 2.1
    - li 2.2

paragrapfh", @"
- li 1.0
- li 1.1
- li 1.2
    - li 2.0
    - li 2.1
    - li 2.2

<p>paragrapfh</p>")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new HtmlPStringBuildRequest
        {
            String = informed
        };

        var result = await new HtmlPStringBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}