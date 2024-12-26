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
internal sealed partial class InputBuildRequestHandler() : IRequestHandler<InputBuildRequest, InputBuildResponse>
{
    const string ACTION_PATH = "--action-path";
    const string SOURCE_PATH = "--source-path";
    const string TARGET_PATH = "--target-path";
    const string TARGET_FILE_NAME = "--target-file-name";
    const string REPOSITORY_OWNER = "--repository_owner";
    const string SOURCE_URL_BASE = "--source-url-base";
    const string PATTERN =
        @"((?'SOURCE_PATH_KEY'" + SOURCE_PATH + ")(\n|)(?'SOURCE_PATH_VALUE'.+)(\n|))+|" +
         "((?'TARGET_PATH_KEY'" + TARGET_PATH + ")(\n|)(?'TARGET_PATH_VALUE'.+)(\n|))+|" +
         "((?'TARGET_FILE_NAME_KEY'" + TARGET_FILE_NAME + ")(\n|)(?'TARGET_FILE_NAME_VALUE'.+)(\n|))+|" +
         "((?'REPOSITORY_OWNER_KEY'" + REPOSITORY_OWNER + ")(\n|)(?'REPOSITORY_OWNER_VALUE'.+)(\n|))+|" +
         "(((?'SOURCE_URL_BASE_KEY'" + SOURCE_URL_BASE + ")(\n|)(?'SOURCE_URL_BASE_VALUE'.+)(\n|)))+|" +
         "(((?'ACTION_PATH_KEY'" + ACTION_PATH + ")(\n|)(?'ACTION_PATH_VALUE'.+)(\n|)))+";

    [GeneratedRegex(PATTERN, RegexOptions.Multiline | RegexOptions.ExplicitCapture)]
    private static partial Regex Regex();


    public async Task<InputBuildResponse> Handle(InputBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();

        if (request.Args == default)
            throw new ArgumentNullException(nameof(request.Args));

        var matches = Regex().Matches(string.Join("\n", request.Args)).Where(match => match.Success);

        if (!matches.Any())
            throw new ArgumentException($"'{nameof(request.Args)}' not any match");

        return new InputBuildResponse
        {
            ActionPath = matches.Value(ACTION_PATH, "ACTION_PATH_KEY", "ACTION_PATH_VALUE"),
            SourcePath = matches.Value(SOURCE_PATH, "SOURCE_PATH_KEY", "SOURCE_PATH_VALUE"),
            TargetPath = matches.Value(TARGET_PATH, "TARGET_PATH_KEY", "TARGET_PATH_VALUE"),
            TargetFileName = matches.Value(TARGET_FILE_NAME, "TARGET_FILE_NAME_KEY", "TARGET_FILE_NAME_VALUE"),
            RepositoryOnwer = matches.Value(REPOSITORY_OWNER, "REPOSITORY_OWNER_KEY", "REPOSITORY_OWNER_VALUE"),
            BaseUrl = new Uri(matches.Value(SOURCE_URL_BASE, "SOURCE_URL_BASE_KEY", "SOURCE_URL_BASE_VALUE")),
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
