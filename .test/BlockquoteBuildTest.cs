using MediatR;
using Moq;

namespace test;

public class BlockquoteBuildTest
{
    [Theory]
    [InlineData(
@">a
",
@"<blockquote></blockquote>")]
    [InlineData(
@">a
>b
",
@"<blockquote></blockquote>")]
    [InlineData(
@">a
>b
>c
",
@"<blockquote></blockquote>")]
    [InlineData(
@">quote 1

>quote2
",
@"<blockquote></blockquote>
<blockquote></blockquote>")]
    [InlineData(
@">a
>>b
>c
",
@"<blockquote></blockquote>")]
    [InlineData(
@">a
>>b
>>>c
>>>d
>e",
@"<blockquote></blockquote>")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new BlockquoteBuildRequest
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

        var result = await new BlockquoteBuildRequestHandler(
                mediator
            )
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}
