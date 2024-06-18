using FasTnT.GS1EpcTranslator;
using GS1EpcTranslator.Parsers;
using Microsoft.AspNetCore.Mvc;
using GS1CompanyPrefix;
using GS1EpcTranslator.Formatters;
using Gs1EpcTranslator.Api;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(opt => opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddSingleton<GS1CompanyPrefixProvider>();
builder.Services.AddSingleton<GS1EpcTranslatorContext>();

var parserStrategies = typeof(IEpcParserStrategy)
    .Assembly.GetTypes()
    .Where(typeof(IEpcParserStrategy).IsAssignableFrom)
    .Where(x => x.IsClass && !x.IsAbstract);

foreach(var parserStrategy in parserStrategies)
{
    // Register all the parser stratesgies as singleton
    builder.Services.AddSingleton(typeof(IEpcParserStrategy), parserStrategy);
}

builder.Services.AddHostedService<CompanyPrefixBackgroundLoader>();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapPost("/translate", ([FromBody] string[] values, [FromServices] GS1EpcTranslatorContext context) =>
{
    var results = values.Select(value => context.TryParse(value, out var result)
            ? result.Format(value)
            : UnknownFormatter.Value.Format(value));

    return Results.Ok(results);
});

app.Run();