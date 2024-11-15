using MediatR;
using Moq;

namespace test;

public class TimeElapsedTest
{
    [Fact]
    public async Task Success()
    {
        var arrange = Mock.Of<Request>();

        var act = new TimeElapsedPipelineBehavior<Request, Unit>();

        var assert = await act.Handle(
            arrange,
            () => Task.FromResult(Unit.Value),
            CancellationToken.None
        );
    }

    [Fact]
    public async Task StreamSuccess()
    {
        var arrange = Mock.Of<StreamRequest>();
        var next = Mock.Of<StreamHandlerDelegate<Unit>>();
        
        var act = new TimeElapsedStreamPipelineBehavior<StreamRequest, Unit>();


        await foreach (var assert in act.Handle(
                arrange,
                next,
                CancellationToken.None
            )) ;

        static async IAsyncEnumerable<Unit> Setup()
        {
            await Task.Yield();
            yield break;
        };
    }
    public class Request : IRequest<Unit>
    {

    }
    public class StreamRequest : IStreamRequest<Unit>
    {

    }
}