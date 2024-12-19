namespace test;

public class ABuildTest
{
    [Theory]
    [InlineData(
@"multi link [link1](https://route-1/path/readme.md), [link2](https://route-2/readme.md) e [link3](https://route-3/readme.md) text, contnue text [link4](https://route-4/README.md).",
@"<p>multi link <a href=""https://route-1/path/readme.md"">link1</a>, <a href=""https://route-2/readme.md"">link2</a> e <a href=""https://route-3/readme.md"">link3</a> text, contnue text <a href=""https://route-4/README.md"">link4</a>.</p>")]
    [InlineData(
@"um [hiperlink[^15]](https://teste/) formatado",
@"<p>um <a href=""https://teste/"">hiperlink<cite id=""cited-15""><a href=""#cite-15""><sup>(15)</sup></a></cite></a> formatado</p>")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new BuildRequest
        {
            Source = informed
        };

        var result = await new BuildRequestHandler(
                new ProjectBuildResponse
                {
                    Title = "--title--",
                    BaseUrl = new Uri("https://route-1")
                }
            )
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Contains(expected, result.Target?.Built);
    }
}
