using MediatR;

namespace test;

public class TimeElapsedTest
{
    [Fact]
    public async Task Success()
    {
        var arrange = new Request();

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
        var arrange = new StreamRequest();

        var act = new TimeElapsedStreamPipelineBehavior<StreamRequest, Unit>()
            .Handle(
                arrange,
                () => Setup(),
                CancellationToken.None
            );


        await foreach (var assert in act) ;

        static async IAsyncEnumerable<Unit> Setup()
        {
            await Task.Yield();
            yield break;
        };
    }
    class Request : IRequest<Unit>
    {

    }
    class StreamRequest : IStreamRequest<Unit>
    {

    }
}