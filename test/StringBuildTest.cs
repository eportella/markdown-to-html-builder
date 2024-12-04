namespace test;

public class StringBuildTest
{
    [Theory]
    [InlineData(
default,
default)]
    [InlineData(
@"",
@"")]
    [InlineData(
@"#",
@"<h1></h1>")]
    [InlineData(
@"# ",
@"<h1></h1>")]
    [InlineData(
@"# a",
@"<h1>a</h1>")]
    [InlineData(
@"# ab",
@"<h1>ab</h1>")]
    [InlineData(
@"##",
@"<h2></h2>")]
    [InlineData(
@"## ",
@"<h2></h2>")]
    [InlineData(
@"## a",
@"<h2>a</h2>")]
    [InlineData(
@"## ab",
@"<h2>ab</h2>")]
    [InlineData(
@"###",
@"<h3></h3>")]
    [InlineData(
@"### ",
@"<h3></h3>")]
    [InlineData(
@"### a",
@"<h3>a</h3>")]
    [InlineData(
@"### ab",
@"<h3>ab</h3>")]
    [InlineData(
@"####",
@"<h4></h4>")]
    [InlineData(
@"#### ",
@"<h4></h4>")]
    [InlineData(
@"#### a",
@"<h4>a</h4>")]
    [InlineData(
@"#### ab",
@"<h4>ab</h4>")]
    [InlineData(
@"#####",
@"<h5></h5>")]
    [InlineData(
@"##### ",
@"<h5></h5>")]
    [InlineData(
@"##### a",
@"<h5>a</h5>")]
    [InlineData(
@"##### ab",
@"<h5>ab</h5>")]
    [InlineData(
@"######",
@"<h6></h6>")]
    [InlineData(
@"###### ",
@"<h6></h6>")]
    [InlineData(
@"###### a",
@"<h6>a</h6>")]
    [InlineData(
@"###### ab",
@"<h6>ab</h6>")]
    [InlineData(
@"-",
@"<ul><li></li></ul>")]
    [InlineData(
@"-
-
",
@"<ul><li>
</li><li>
</li></ul>")]
    [InlineData(
@"- a",
@"<ul><li>a</li></ul>")]
    [InlineData(
@"- a
-b",
@"<ul><li>a
</li><li>b</li></ul>")]
    [InlineData(
@"- a
-a
-a
",
@"<ul><li>a
</li><li>a
</li><li>a
</li></ul>")]
    [InlineData(
@">",
@"<blockquote></blockquote>")]
    [InlineData(
@"> ",
@"<blockquote></blockquote>")]
    [InlineData(
@">
>
",
@"<blockquote>

</blockquote>")]
    [InlineData(
@"> a",
@"<blockquote>a</blockquote>")]
    [InlineData(
@"> a
>b",
@"<blockquote>a
b</blockquote>")]
    [InlineData(
@"> a
>a
>a
",
@"<blockquote>a
a
a
</blockquote>")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new StringBuildRequest
        {
            Source = informed
        };

        var result = await new StringBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}
