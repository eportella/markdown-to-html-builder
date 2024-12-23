using System.Text.RegularExpressions;
using MediatR;
using Moq;

namespace test;

public class H2BuildTest
{
    [Theory]
    [InlineData(
@"##a
##b
##d
",
@"<h2></h2>
<h2></h2>
<h2></h2>
")]
    [InlineData(
@"##prefix *infix italic* sufix",
@"<h2></h2>
")]
    [InlineData(
@"##prefix **infix bold** sufix",
@"<h2></h2>
")]
    [InlineData(
@"##prefix **infix bold** *sufix italic*",
@"<h2></h2>
")]
    [InlineData(
@"## prefix infix sufix",
@"<h2></h2>
")]

    public async Task Success(string informed, string expected)
    {
        var arrange = new H2BuildRequest
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
        
        var result = await new H2BuildRequestHandler(
                mediator
            )
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}
