using System.Text.RegularExpressions;
using MediatR;
using Moq;

namespace test;

public class H1BuildTest
{
    [Theory]
    [InlineData(
@"#a
#b
#d
",
@"<h1></h1>
<h1></h1>
<h1></h1>
")]
    [InlineData(
@"#prefix *infix italic* sufix",
@"<h1></h1>
")]
    [InlineData(
@"#prefix **infix bold** sufix",
@"<h1></h1>
")]
    [InlineData(
@"#prefix **infix bold** *sufix italic*",
@"<h1></h1>
")]
    [InlineData(
@"# prefix infix sufix",
@"<h1></h1>
")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new H1BuildRequest
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
        
        var result = await new H1BuildRequestHandler(
                mediator
            )
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}
