using FasTnT.GS1EpcTranslator;
using GS1EpcTranslator.Parsers.Implementations;
using GS1EpcTranslator.Parsers;
using Microsoft.AspNetCore.Mvc;
using GS1EpcTranslator.Parsers.ElementString;
using GS1EpcTranslator.Parsers.DigitalLink;
using GS1CompanyPrefix;
using GS1EpcTranslator.Formatters;
using Gs1EpcTranslator.Api;

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
app.MapPost("/translate", ([FromBody] string[] values, [FromServices] GS1EpcTranslatorContext context) =>
{
    var results = new EpcResult[values.Length];

    for(var i=0; i<values.Length; i++)
    {
        if (context.TryParse(values[i], out var result))
        {
            results[i] = result.Format(values[i]);
        }
        else
        {
            results[i] = UnknownFormatter.Value.Format(values[i]);
        }
    }
    
    return Results.Ok(results);
});

app.Run();
