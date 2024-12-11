using System;
using System.ComponentModel.DataAnnotations;
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
    public Uri? BaseUrl { get; init; }
}
internal sealed class InputBuildRequestHandler() : IRequestHandler<InputBuildRequest, InputBuildResponse>
{
    public async Task<InputBuildResponse> Handle(InputBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();

        if (request.Args == default)
            throw new ArgumentNullException(nameof(request.Args));

        var matches = Regex.Matches(string.Join("\n", request.Args), @"((?'SOURCE_PATH_KEY'--source-path)(\n|)(?'SOURCE_PATH_VALUE'.+)(\n|))+|((?'TARGET_PATH_KEY'--target-path)(\n|)(?'TARGET_PATH_VALUE'.+)(\n|))+|((?'TARGET_FILE_NAME_KEY'--target-file-name)(\n|)(?'TARGET_FILE_NAME_VALUE'.+)(\n|))+|((?'REPOSITORY_OWNER_KEY'--repository_owner)(\n|)(?'REPOSITORY_OWNER_VALUE'.+)(\n|))+|(((?'SOURCE_URL_BASE_KEY'--source-url-base)(\n|)(?'SOURCE_URL_BASE_VALUE'.+)(\n|)))+", RegexOptions.Multiline | RegexOptions.ExplicitCapture).Where(match => match.Success);

        if (!matches.Any())
            throw new ArgumentException($"'{nameof(request.Args)}' not any match");

        if (!matches.Any(match => match.Groups["SOURCE_PATH_KEY"].Success))
            throw new ArgumentException($"'--source-path' not found");

        if (!matches.Any(match => match.Groups["SOURCE_PATH_VALUE"].Success))
            throw new ArgumentException($"value of '--source-path' not found");

        if (!matches.Any(match => match.Groups["TARGET_PATH_KEY"].Success))
            throw new ArgumentException($"'--target-path' not found");

        if (!matches.Any(match => match.Groups["TARGET_PATH_VALUE"].Success))
            throw new ArgumentException($"value of '--target-path' not found");

        if (!matches.Any(match => match.Groups["TARGET_FILE_NAME_KEY"].Success))
            throw new ArgumentException($"'--target-file-name' not found");

        if (!matches.Any(match => match.Groups["TARGET_FILE_NAME_VALUE"].Success))
            throw new ArgumentException($"value of '--target-file-name' not found");

        if (!matches.Any(match => match.Groups["REPOSITORY_OWNER_KEY"].Success))
            throw new ArgumentException($"'--repository_owner' not found");

        if (!matches.Any(match => match.Groups["REPOSITORY_OWNER_VALUE"].Success))
            throw new ArgumentException($"value of '--repository_owner' not found");

        if (!matches.Any(match => match.Groups["SOURCE_URL_BASE_KEY"].Success))
            throw new ArgumentException($"'--source-url-base' not found");

        if (!matches.Any(match => match.Groups["SOURCE_URL_BASE_VALUE"].Success))
            throw new ArgumentException($"value of '--source-url-base' not found");

        return new InputBuildResponse
        {
            SourcePath = matches.Select(match => match.Groups["SOURCE_PATH_VALUE"]).Single(group => group.Success).Value,
            TargetPath = matches.Select(match => match.Groups["TARGET_PATH_VALUE"]).Single(group => group.Success).Value,
            TargetFileName = matches.Select(match => match.Groups["TARGET_FILE_NAME_VALUE"]).Single(group => group.Success).Value,
            RepositoryOnwer = matches.Select(match => match.Groups["REPOSITORY_OWNER_VALUE"]).Single(group => group.Success).Value,
            BaseUrl = new Uri(matches.Select(match => match.Groups["SOURCE_URL_BASE_VALUE"]).Single(group => group.Success).Value),
        };
    }
}
