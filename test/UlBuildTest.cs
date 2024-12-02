using MediatR;
using Moq;

namespace test;

public class UlBuildTest
{
    [Theory]
        [InlineData(
@"- list item 1",
@"<ul></ul>")]
    [InlineData(
@"
- list item 1",
@"
<ul></ul>")]
    [InlineData(
@"- list item 1
- list item 2",
@"<ul></ul>")]
    [InlineData(
@"- list item 1
- list item 2
- list item 3",
"<ul></ul>")]
    [InlineData(
@"- list item 1
",
@"<ul></ul>
")]
    [InlineData(
@"- list item 1
- list item 2
",
@"<ul></ul>
")]
    [InlineData(
@"- list item 1
- list item 2
- list item 3
",
@"<ul></ul>
")]
    [InlineData(
@"
- list item 1
",
@"
<ul></ul>
")]
    [InlineData(
@"
- list item 1
- list item 2
",
@"
<ul></ul>
")]
    [InlineData(
@"
- list item 1
- list item 2
- list item 3
",
@"
<ul></ul>
")]
    public async Task Success(string informed, string expected)
    {
        var mediator = Mock.Of<IMediator>();
        Mock.Get(mediator)
            .Setup(s => s.Send(It.IsAny<HtmlLiStringBuildRequest>(), CancellationToken.None))
            .ReturnsAsync(string.Empty);

        var arrange = new HtmlUlStringBuildRequest
        {
            String = informed
        };

        var result = await new HtmlUlStringBuildRequestHandler(mediator)
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result);
    }
}
