namespace test;

public class HtmlBuildTest
{
    
    [Theory]
    [InlineData(
default,
default)]
    [InlineData(
@"",
@"<!DOCTYPE html><html lang=""pt-BR""><head><title>--title--</title><meta content=""text/html; charset=UTF-8;"" http-equiv=""Content-Type"" /><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""><meta name=""color-scheme"" content=""dark light""><link rel=""stylesheet"" href=""https://domain/stylesheet.css""></style></head><body><h1><a href=""https://domain/""/>--title--</a></h1></body></html>")]
    [InlineData(
@"
",
@"<!DOCTYPE html><html lang=""pt-BR""><head><title>--title--</title><meta content=""text/html; charset=UTF-8;"" http-equiv=""Content-Type"" /><meta name=""viewport"" content=""width=device-width, initial-scale=1.0""><meta name=""color-scheme"" content=""dark light""><link rel=""stylesheet"" href=""https://domain/stylesheet.css""></style></head><body><h1><a href=""https://domain/""/>--title--</a></h1></body></html>")]
    public async Task Success(string informed, string expected)
    {
        var arrange = new BuildRequest
        {
            Source = informed
        };

        var result = await new BuildRequestHandler(
                new ProjectBuildResponse
                {
                    Title = "--title--",
                    BaseUrl = new Uri("https://github.com"),
                }
            )
            .Handle(
                arrange,
                CancellationToken.None
            );

        Assert.Equal(expected, result.Target?.Built);
    }
}
