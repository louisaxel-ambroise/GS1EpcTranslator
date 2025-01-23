using GS1EpcTranslator.Parsers;
using Microsoft.AspNetCore.Mvc;
using GS1CompanyPrefix;
using GS1EpcTranslator.Formatters;
using System.Text.Json.Serialization;
using System.Reflection;
using Gs1EpcTranslator.Api.HostedServices;
using static Gs1EpcTranslator.Api.HostedServices.CompanyPrefixLoaderHostedServices;
using GS1EpcTranslator;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(opt => opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddSingleton<GS1CompanyPrefixProvider>();
builder.Services.AddSingleton<GS1EpcTranslatorContext>();

var parserStrategies = typeof(IEpcParserStrategy)
    .Assembly.GetTypes()
    .Where(x => x.IsClass && !x.IsAbstract)
    .Where(typeof(IEpcParserStrategy).IsAssignableFrom);

// Register all the parser strategies as singleton
foreach(var parserStrategy in parserStrategies)
{
    builder.Services.AddSingleton(typeof(IEpcParserStrategy), parserStrategy);
}

builder.Services.Configure<CompanyPrefixOptions>(builder.Configuration.GetSection(nameof(CompanyPrefixOptions)));
builder.Services.AddHostedService<CompanyPrefixLoaderHostedServices>();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapPost("/translate", ([FromBody] string[] values, [FromServices] GS1EpcTranslatorContext context) =>
{
    var results = values.Select(value => context.TryParse(value, out var result)
            ? result.Format(value)
            : UnknownFormatter.Value.Format(value));

    return Results.Ok(new { Data = results });
});

app.Run();