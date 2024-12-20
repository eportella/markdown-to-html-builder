using System.Globalization;
using System.Text.RegularExpressions;
using MediatR;
internal sealed class AgeCalcBuildRequest : IRequest<AgeCalcBuildResponse?>
{
    internal string? Source { get; init; }
}
internal sealed class AgeCalcBuildResponse
{
    internal string? Target { get; init; }
}
internal sealed class AgeCalcBuildRequestHandler() : IRequestHandler<AgeCalcBuildRequest, AgeCalcBuildResponse?>
{
    const string PATTERN = @"(?'AGE_CALC'`\[age-calc\]:(?'AGE_CALC_CONTENT'[\d]{4}\-[\d]{2}\-[\d]{2})\`)";
    public async Task<AgeCalcBuildResponse?> Handle(AgeCalcBuildRequest request, CancellationToken cancellationToken)
    {
        await Task.Yield();
        if (request.Source == default)
            return default;

        return new AgeCalcBuildResponse
        {
            Target = Build(request.Source),
        };
    }

    private string? Build(string? source)
    {
        if (source == default)
            return source;

        return Regex.Replace(
            source,
            $"({PATTERN})",
            match => AgeCalculate(DateTime.ParseExact(match.Groups["AGE_CALC_CONTENT"].Value, "yyyy-mm-dd", CultureInfo.InvariantCulture)).ToString(),
            RegexOptions.Multiline);
    }

    private static int AgeCalculate(DateTime birthDate)
    {
        DateTime today = DateTime.Today;

        int age = today.Year - birthDate.Year;

        if (birthDate.Date > today.AddYears(-age).Date)
            return age - 1;

        return age;
    }
}
