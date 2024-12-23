using System.Text.RegularExpressions;
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
@"<blockquote><p></p></blockquote><p></p><blockquote><p></p></blockquote>")]
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
        var arrange = new BlockBuildRequest
        {
            Source = informed
        };
        var mediator = Mock.Of<IMediator>();
        static IEnumerable<string> YieldBreak()
        {
            yield break;
        }
        Mock
            .Get(mediator)
                .Setup(s => s.CreateStream(It.IsAny<InlineBuildRequest>(), CancellationToken.None))
                .Returns(YieldBreak().ToAsyncEnumerable());

        var result = await new BlockBuildRequestHandler(
                mediator
            )
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}
