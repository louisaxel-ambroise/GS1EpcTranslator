using FasTnT.GS1EpcTranslator;
using GS1CompanyPrefix;
using GS1EpcTranslator.Parsers.Implementations;
using GS1EpcTranslator.Parsers;
using Microsoft.AspNetCore.Mvc;
using GS1EpcTranslator.Parsers.ElementString;
using GS1EpcTranslator.Parsers.DigitalLink;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<GS1CompanyPrefixProvider>();
builder.Services.AddSingleton<GS1EpcTranslatorContext>();

builder.Services.AddSingleton<IEpcParserStrategy, UrnSsccParserStrategy>();
builder.Services.AddSingleton<IEpcParserStrategy, DlSsccParserStrategy>();
builder.Services.AddSingleton<IEpcParserStrategy, ElementStringSsccParserStrategy>();
builder.Services.AddSingleton<IEpcParserStrategy, UrnGtinParserStrategy>();
builder.Services.AddSingleton<IEpcParserStrategy, DlGtinParserStrategy>();
builder.Services.AddSingleton<IEpcParserStrategy, ElementStringGtinParserStrategy>();
builder.Services.AddHostedService<CompanyPrefixBackgroundLoader>();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapPost("/translate", ([FromBody] string value, [FromServices] GS1EpcTranslatorContext context) =>
{
    return context.TryParse(value, out var result) 
        ? Results.Ok(result.Format(value)) 
        : Results.BadRequest();
});

app.Run();

public sealed class CompanyPrefixBackgroundLoader(GS1CompanyPrefixProvider companyPrefixProvider) : IHostedService, IDisposable
{
    private Timer? _timer = null;

    public Task StartAsync(CancellationToken stoppingToken)
    {
        _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromMinutes(5));

        return Task.CompletedTask;
    }

    private void DoWork(object? state)
    {
        var httpClient = new HttpClient { BaseAddress = new Uri("https://www.gs1.org/") };
        var request = httpClient.GetAsync("sites/default/files/docs/gcp_length/gcpprefixformatlist.json");
        var response = request.Result;

        using var responseStream = response.Content.ReadAsStream();

        GS1CompanyPrefixLoader.LoadFromJsonStream(companyPrefixProvider, responseStream);
    }

    public Task StopAsync(CancellationToken stoppingToken)
    {
        _timer?.Change(Timeout.Infinite, 0);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        _timer?.Dispose();
    }
}
