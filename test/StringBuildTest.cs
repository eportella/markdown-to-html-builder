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
