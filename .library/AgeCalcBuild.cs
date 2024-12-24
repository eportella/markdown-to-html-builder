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
internal sealed partial class AgeCalcBuildRequestHandler() : IRequestHandler<AgeCalcBuildRequest, AgeCalcBuildResponse?>
{
    const string PATTERN = @"(?'AGE_CALC'`\[age-calc\]:(?'AGE_CALC_CONTENT'[\d]{4}\-[\d]{2}\-[\d]{2})\`)";
    [GeneratedRegex(PATTERN, RegexOptions.Multiline)]
    private static partial Regex Regex();
    public async Task<AgeCalcBuildResponse?> Handle(AgeCalcBuildRequest request, CancellationToken cancellationToken)
    {
        if (request.Source == default)
            return default;

        var target = await Regex().ReplaceAsync(
            request.Source,
            match => AgeCalculate(
                DateTime.ParseExact(
                    match.Groups["AGE_CALC_CONTENT"].Value,
                    "yyyy-mm-dd",
                    CultureInfo.InvariantCulture)
                )
                .ToString()
            );

        return new AgeCalcBuildResponse
        {
            Target = target,
        };
    }

    private static int AgeCalculate(DateTime birthDate)
    {
        var today = DateTime.Today;

        int age = today.Year - birthDate.Year;

        if (birthDate.Date > today.AddYears(-age).Date)
            return age - 1;

        return age;
    }
}
