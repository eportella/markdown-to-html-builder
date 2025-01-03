using System.Text.RegularExpressions;
using MediatR;
using Moq;

namespace test;

public class H6BuildTest
{
    [Theory]
    [InlineData(
@"######a
######b
######d
",
@"<h6></h6>
<h6></h6>
<h6></h6>
")]
    [InlineData(
@"######prefix *infix italic* sufix",
@"<h6></h6>
")]
    [InlineData(
@"######prefix **infix bold** sufix",
@"<h6></h6>
")]
    [InlineData(
@"######prefix **infix bold** *sufix italic*",
@"<h6></h6>
")]
    [InlineData(
@"###### prefix infix sufix",
@"<h6></h6>
")]

    public async Task Success(string informed, string expected)
    {
        var arrange = new H6BuildRequest
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

        var result = await new H6BuildRequestHandler(
                mediator
            )
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}
