
namespace test;

public class ABuildTest
{
    [Theory]
    [InlineData(
@"multi link [link1](https://route-1/path/readme.md), [link2](https://route-2/readme.md) e [link3](https://route-3/readme.md) text, contnue text [link4](https://route-4/README.md).",
@"multi link <a href=""https://route-1/path/readme.md"">link1</a>, <a href=""https://route-2/readme.md"">link2</a> e <a href=""https://route-3/readme.md"">link3</a> text, contnue text <a href=""https://route-4/README.md"">link4</a>.")]
    [InlineData(
@"um [hiperlink[^15]](https://teste/) formatado",
@"um <a href=""https://teste/"">hiperlink[^15]</a> formatado")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new ABuildRequest
        {
            Source = informed
        };

        var result = await new ABuildRequestHandler(
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

        Assert.Equal(expected, result!.Target);
    }
}
