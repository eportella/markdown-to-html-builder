using MediatR;
using Moq;

namespace test;

public class BlockquoteBuildTest
{
    [Theory]
    [InlineData(
@">a
",
@"<blockquote><p></p></blockquote>")]
    [InlineData(
@">a
>b
",
@"<blockquote><p></p></blockquote>")]
    [InlineData(
@">a
>b
>c
",
@"<blockquote><p></p></blockquote>")]
    [InlineData(
@">quote 1

>quote2
",
@"<blockquote><p></p></blockquote><blockquote><p></p></blockquote>")]
    [InlineData(
@">a
>>b
>c
",
@"<blockquote><p></p><blockquote><p></p></blockquote><p></p></blockquote>")]
    [InlineData(
@">a
>>b
>>>c
>>>d
>e",
@"<blockquote><p></p><blockquote><p></p><blockquote><p></p></blockquote></blockquote><p></p></blockquote>")]
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
                    BaseUrl = new Uri("https://github.com")
                },
                Mock.Of<IMediator>()
            )
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Contains(expected, result.Target?.Built);
    }
}
