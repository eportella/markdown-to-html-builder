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
        var mediator = Mock.Of<IMediator>();
        static async IAsyncEnumerable<Text> YieldBreak()
        {
            await Task.Yield();
            yield break;
        }
        Mock
            .Get(mediator)
                .Setup(s => s.CreateStream(It.IsAny<TextBuildRequest>(), CancellationToken.None))
                .Returns(YieldBreak());

        var result = await new BuildRequestHandler(
                new ProjectBuildResponse
                {
                    Title = "--title--",
                    BaseUrl = new Uri("https://github.com")
                },
                mediator
            )
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Contains(expected, result.Target?.Built);
    }
}
