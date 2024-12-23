using System.Diagnostics;
using System.Runtime.CompilerServices;
using MediatR;
public sealed class TimeElapsedPipelineBehavior<MarkdownToHtmlBuildRequest, TResponse> : IPipelineBehavior<MarkdownToHtmlBuildRequest, TResponse>
        where MarkdownToHtmlBuildRequest : notnull
{
    Stopwatch Stopwatch { get; }
    public TimeElapsedPipelineBehavior()
    {
        Stopwatch = new Stopwatch();
    }
    public async Task<TResponse> Handle(MarkdownToHtmlBuildRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        Stopwatch.Start();

        var response = await next();
        Stopwatch.Stop();
        Console.WriteLine($"{typeof(MarkdownToHtmlBuildRequest).FullName} Time Elapsed {Stopwatch.ElapsedMilliseconds}ms");
        return response;
    }
}

public sealed class TimeElapsedStreamPipelineBehavior<MarkdownToHtmlBuildRequest, TResponse> : IStreamPipelineBehavior<MarkdownToHtmlBuildRequest, TResponse>
        where MarkdownToHtmlBuildRequest : IStreamRequest<TResponse>
{
    Stopwatch Stopwatch { get; }
    public TimeElapsedStreamPipelineBehavior(
    )
    {
        Stopwatch = new Stopwatch();
    }
    public async IAsyncEnumerable<TResponse> Handle(
        MarkdownToHtmlBuildRequest request,
        StreamHandlerDelegate<TResponse> next,
        [EnumeratorCancellation] CancellationToken cancellationToken
    )
    {
        Stopwatch.Start();
        await foreach (var item in next())
            yield return item;
        Stopwatch.Stop();
        Console.WriteLine($"{typeof(MarkdownToHtmlBuildRequest).FullName} Time Elapsed {Stopwatch.ElapsedMilliseconds}ms");
    }
}