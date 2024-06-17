using FasTnT.GS1EpcTranslator;
using GS1EpcTranslator.Parsers.Implementations;
using GS1EpcTranslator.Parsers;
using Microsoft.AspNetCore.Mvc;
using GS1EpcTranslator.Parsers.ElementString;
using GS1EpcTranslator.Parsers.DigitalLink;
using GS1CompanyPrefix;
using GS1EpcTranslator.Formatters;
using Gs1EpcTranslator.Api;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services.AddSingleton<GS1CompanyPrefixProvider>();
builder.Services.AddSingleton<GS1EpcTranslatorContext>();

builder.Services.AddSingleton<IEpcParserStrategy, UrnSsccParserStrategy>();
builder.Services.AddSingleton<IEpcParserStrategy, DlSsccParserStrategy>();
builder.Services.AddSingleton<IEpcParserStrategy, ElementStringSsccParserStrategy>();
builder.Services.AddSingleton<IEpcParserStrategy, UrnGtinParserStrategy>();
builder.Services.AddSingleton<IEpcParserStrategy, DlGtinParserStrategy>();
builder.Services.AddSingleton<IEpcParserStrategy, ElementStringGtinParserStrategy>();
builder.Services.AddSingleton<IEpcParserStrategy, UrnSglnParserStrategy>();
builder.Services.AddSingleton<IEpcParserStrategy, DlSglnParserStrategy>();
builder.Services.AddSingleton<IEpcParserStrategy, ElementStringSglnParserStrategy>();

builder.Services.AddHostedService<CompanyPrefixBackgroundLoader>();

var app = builder.Build();

app.UseHttpsRedirection();
app.MapPost("/translate", ([FromBody] string[] values, [FromServices] GS1EpcTranslatorContext context) =>
{
    var results = values.Select(value =>
    {
        if (context.TryParse(value, out var result))
        {
            return result.Format(value);
        }
        else
        {
            return UnknownFormatter.Value.Format(value);
        }
    });
    
    return Results.Ok(results);
});

app.Run();
