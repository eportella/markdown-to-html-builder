using System.Text.RegularExpressions;
using MediatR;
using Moq;

namespace test;

public class UlOlBuildTest
{
    [Theory]
    [InlineData(
@"- list item 1
- list item 2
- list item 3
- listitem4
-listitem5",
@"<ul><li></li><li></li><li></li><li></li><li></li></ul>")]
    [InlineData(
@"- list item 1

- list item 1
- listitem2

-listitem3
",
@"<ul><li></li></ul><ul><li></li><li></li></ul><ul><li></li></ul>")]
    [InlineData(
@"- li 1.0
- li 1.1
- li 1.2
    - li 2.0
    - li 2.1
    - li 2.2

- li 1.0
- li 1.1
- li 1.2
    1. li 2.0
    1. li 2.1
    1. li 2.2

1. li 1.0
2. li 1.1
3. li 1.2
    - li 2.0
    - li 2.1
    - li 2.2

1. li 1.0
2. li 1.1
3. li 1.2
    1. li 2.0
    9. li 2.1
    8. li 2.2",
@"<ul><li></li><li></li><li></li></ul><ul><li></li><li></li><li></li></ul><ol><li></li><li></li><li></li></ol><ol><li></li><li></li><li></li></ol>")]

    [InlineData(
@">- li 1.0
>- li 1.1
>- li 1.2
>    - li 2.0
>    - li 2.1
>    - li 2.2
>
>- li 1.0
>- li 1.1
>- li 1.2
>    1. li 2.0
>    1. li 2.1
>    1. li 2.2
>
>1. li 1.0
>2. li 1.1
>3. li 1.2
>    - li 2.0
>    - li 2.1
>    - li 2.2
>
>1. li 1.0
>2. li 1.1
>3. li 1.2
>    1. li 2.0
>    9. li 2.1
>    8. li 2.2",
@"<blockquote><ul><li></li><li></li><li></li></ul><ul><li></li><li></li><li></li></ul><ol><li></li><li></li><li></li></ol><ol><li></li><li></li><li></li></ol></blockquote>")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new BuildRequest
        {
            Source = informed
        };
        var mediator = Mock.Of<IMediator>();
        static IEnumerable<Text> YieldBreak()
        {
            yield break;
        }
        Mock
            .Get(mediator)
                .Setup(s => s.CreateStream(It.IsAny<TextBuildRequest>(), CancellationToken.None))
                .Returns(YieldBreak().ToAsyncEnumerable());

        var result = await new BuildRequestHandler(
                new ProjectBuildResponse
                {
                    Title = "--title--",
                    BaseUrl = new Uri("https://github.com"),
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
