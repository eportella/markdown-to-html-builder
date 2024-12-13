namespace test;

public class InputBuildTest
{
    [Theory]
    [InlineData(
@"/home/runner/work/_actions/eportella/markdown-to-html-builder/main/console/bin/Debug/net8.0/console.dll --source-path /home/runner/work/markdown-to-html-builder/markdown-to-html-builder --target-path /home/runner/work/markdown-to-html-builder/markdown-to-html-builder/_site --target-file-name index.html --repository_owner eportella --source-url-base https://eportella.github.io/markdown-to-html-builder --action-path /home/runner/work/markdown-to-html-builder")]
    public async Task Success(string informed)
    {
        var arrange = new InputBuildRequest
        {
            Args = informed.Split(' '),
        };

        var result = await new InputBuildRequestHandler()
            .Handle(
                arrange,
                CancellationToken.None
            );

        
    }
}
