namespace test;

public class BodyBuildTest
{
    [Theory]
    [InlineData(default, default, default, @"<body><h1><a href=""""/></a></h1></body>")]
    [InlineData("", "", "", @"<body><h1><a href=""""/></a></h1></body>")]
    [InlineData("a", "b", "c", @"<body><h1><a href=""b""/>c</a></h1>a</body>")]
    [InlineData(@"
a
B
c
D
", "b", "c", @"<body><h1><a href=""b""/>c</a></h1>
a
B
c
D
</body>")]
    public async Task Success(string informed, string url, string title, string expected)
    {
        var arrange = new BodyBuildRequest
        {
            Url = url,
            Title = title,
            String = informed
        };

        var result = await new BodyBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}