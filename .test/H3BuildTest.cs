using System.Text.RegularExpressions;
using MediatR;
using Moq;

namespace test;

public class H3BuildTest
{
    [Theory]
    [InlineData(
@"###a
###b
###d
",
@"<h3></h3>
<h3></h3>
<h3></h3>
")]
    [InlineData(
@"###prefix *infix italic* sufix",
@"<h3></h3>
")]
    [InlineData(
@"###prefix **infix bold** sufix",
@"<h3></h3>
")]
    [InlineData(
@"###prefix **infix bold** *sufix italic*",
@"<h3></h3>
")]
    [InlineData(
@"### prefix infix sufix",
@"<h3></h3>
")]

    public async Task Success(string informed, string expected)
    {
        var arrange = new H3BuildRequest
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

        var result = await new H3BuildRequestHandler(
                mediator
            )
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}
