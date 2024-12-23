using MediatR;
using Moq;

namespace test;

public class TimeElapsedTest
{
    [Fact]
    public async Task Success()
    {
        var arrange = Mock.Of<ITimmerElapsedLog>();

        var act = new TimeElapsedPipelineBehavior<ITimmerElapsedLog, Unit>();

        var assert = await act.Handle(
            arrange,
            () => Task.FromResult(Unit.Value),
            CancellationToken.None
        );
    }

    [Fact]
    public async Task StreamSuccess()
    {
        var arrange = Mock.Of<ITimmerElapsedLog>();
        var next = Mock.Of<StreamHandlerDelegate<Unit>>();
        Mock.Get(next)
            .Setup(@delegate => @delegate())
            .Returns(AsyncEnumerable);

        var act = new TimeElapsedStreamPipelineBehavior<ITimmerElapsedLog, Unit>();

        await foreach (var assert in act.Handle(
                arrange,
                next,
                CancellationToken.None
            )) ;

        static async IAsyncEnumerable<Unit> AsyncEnumerable()
        {
            await Task.Yield();
            yield break;
        };
    }
}