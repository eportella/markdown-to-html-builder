using System.Text.RegularExpressions;
using MediatR;
public sealed class InputBuildRequest : IRequest<InputBuildResponse>, ITimmerElapsedLog
{
    public string[]? Args { internal get; init; }
}
internal sealed class InputBuildResponse
{
    public string? ActionPath { get; init; }
    public string? SourcePath { get; init; }
    public string? TargetPath { get; init; }
    public string? TargetFileName { get; init; }
    public string? RepositoryOnwer { get; init; }
    public Uri? BaseUrl { get; init; }
}
internal sealed class InputBuildRequestHandler() : IRequestHandler<InputBuildRequest, InputBuildResponse>
{
    const string PATTERN = @"((?'SOURCE_PATH_KEY'--source-path)(\n|)(?'SOURCE_PATH_VALUE'.+)(\n|))+|((?'TARGET_PATH_KEY'--target-path)(\n|)(?'TARGET_PATH_VALUE'.+)(\n|))+|((?'TARGET_FILE_NAME_KEY'--target-file-name)(\n|)(?'TARGET_FILE_NAME_VALUE'.+)(\n|))+|((?'REPOSITORY_OWNER_KEY'--repository_owner)(\n|)(?'REPOSITORY_OWNER_VALUE'.+)(\n|))+|(((?'SOURCE_URL_BASE_KEY'--source-url-base)(\n|)(?'SOURCE_URL_BASE_VALUE'.+)(\n|)))+|(((?'ACTION_PATH_KEY'--action-path)(\n|)(?'ACTION_PATH_VALUE'.+)(\n|)))+";
    
    public async Task<InputBuildResponse> Handle(InputBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();

        if (request.Args == default)
            throw new ArgumentNullException(nameof(request.Args));

        var matches = Regex.Matches(string.Join("\n", request.Args), PATTERN, RegexOptions.Multiline | RegexOptions.ExplicitCapture).Where(match => match.Success);

        if (!matches.Any())
            throw new ArgumentException($"'{nameof(request.Args)}' not any match");

        return new InputBuildResponse
        {
            ActionPath = matches.Value("--action-path", "ACTION_PATH_KEY", "ACTION_PATH_VALUE"),
            SourcePath = matches.Value("--source-path", "SOURCE_PATH_KEY", "SOURCE_PATH_VALUE"),
            TargetPath = matches.Value("--target-path", "TARGET_PATH_KEY", "TARGET_PATH_VALUE"),
            TargetFileName = matches.Value("--target-file-name", "TARGET_FILE_NAME_KEY", "TARGET_FILE_NAME_VALUE"),
            RepositoryOnwer = matches.Value("--repository_owner", "REPOSITORY_OWNER_KEY", "REPOSITORY_OWNER_VALUE"),
            BaseUrl = new Uri(matches.Value("--source-url-base", "SOURCE_URL_BASE_KEY", "SOURCE_URL_BASE_VALUE")),
        };
    }
}

internal static class MatchesExtensions
{
    internal static string Value(this IEnumerable<Match> matches, string argumentName, string argumentKey, string argumentValue)
    {
        var key = matches.Where(match => match.Groups[argumentKey].Success);

        if (!key.Any())
            throw new ArgumentException($"'{argumentName}' not found");

        if (key.Skip(1).Any())
            throw new ArgumentException($"'{argumentName}' multiple found");

        var itemValue = matches.SingleOrDefault(match => match.Groups[argumentValue].Success);

        if (itemValue?.Success != true)
            throw new ArgumentException($"value of '{argumentName}' not found");

        return itemValue.Value;
    }
}
