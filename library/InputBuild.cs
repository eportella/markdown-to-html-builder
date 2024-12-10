using System.Text.RegularExpressions;
using MediatR;
public sealed class InputBuildRequest : IRequest<InputBuildResponse>
{
    public string[]? Args { internal get; init; }
}
internal sealed class InputBuildResponse
{
    public string? SourcePath { get; init; }
    public string? TargetPath { get; init; }
    public string? TargetFileName { get; init; }
    public string? RepositoryOnwer { get; init; }
    public string? BaseUrl { get; init; }
}
internal sealed class InputBuildRequestHandler() : IRequestHandler<InputBuildRequest, InputBuildResponse>
{
    public async Task<InputBuildResponse> Handle(InputBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();

        var args = request.Args;

        if (args == default)
            throw new ArgumentNullException(nameof(args));

        var match = Regex.Match(@"(?'SOURCE_PATH_KEY'--source-path){1} (?'SOURCE_PATH_VALUE'.+?)( |$)|(?'TARGET_PATH_KEY'--target-path){1} (?'TARGET_PATH_VALUE'.+?)( |$)|(?'TARGET_FILE_KEY'--target-file-name){1} (?'TARGET_FILE_NAME_VALUE'.+?)( |$)|(?'REPOSITORY_OWNER_KEY'--repository_owner){1} (?'REPOSITORY_OWNER_VALUE'.+?)( |$)|((?'SOURCE_URL_BASE_KEY'--source-url-base){1} (?'SOURCE_URL_BASE_VALUE'.+?)( |$))", string.Join(string.Empty, args));

        if(!match.Success)
            throw new ArgumentException($"'{nameof(args)}' not match");

        if(string.IsNullOrWhiteSpace(match.Groups["SOURCE_PATH_KEY"].Value))
            throw new ArgumentException($"'--source-path' not found");

        if(string.IsNullOrWhiteSpace(match.Groups["SOURCE_PATH_VALUE"].Value))
            throw new ArgumentException($"value of '--source-path' not found");

        if(string.IsNullOrWhiteSpace(match.Groups["TARGET_PATH_KEY"].Value))
            throw new ArgumentException($"'--target-path' not found");

        if(string.IsNullOrWhiteSpace(match.Groups["TARGET_PATH_VALUE"].Value))
            throw new ArgumentException($"value of '--target-path' not found");

        if(string.IsNullOrWhiteSpace(match.Groups["TARGET_FILE_KEY"].Value))
            throw new ArgumentException($"'--target-file-name' not found");

        if(string.IsNullOrWhiteSpace(match.Groups["TARGET_FILE_NAME_VALUE"].Value))
            throw new ArgumentException($"value of '--target-file-name' not found");

        if(string.IsNullOrWhiteSpace(match.Groups["REPOSITORY_OWNER_KEY"].Value))
            throw new ArgumentException($"'--repository_owner' not found");

        if(string.IsNullOrWhiteSpace(match.Groups["REPOSITORY_OWNER_VALUE"].Value))
            throw new ArgumentException($"value of '--repository_owner' not found");

        if(string.IsNullOrWhiteSpace(match.Groups["SOURCE_URL_BASE_KEY"].Value))
            throw new ArgumentException($"'--source-url-base' not found");

        if(string.IsNullOrWhiteSpace(match.Groups["SOURCE_URL_BASE_VALUE"].Value))
            throw new ArgumentException($"value of '--source-url-base' not found");

        return new InputBuildResponse
        {
            SourcePath = match.Groups["SOURCE_PATH_VALUE"].Value,
            TargetPath = match.Groups["TARGET_PATH_VALUE"].Value,
            TargetFileName = match.Groups["TARGET_FILE_NAME_VALUE"].Value,
            RepositoryOnwer = match.Groups["REPOSITORY_OWNER_VALUE"].Value,
            BaseUrl = match.Groups["SOURCE_URL_BASE_VALUE"].Value,
        };
    }
}
