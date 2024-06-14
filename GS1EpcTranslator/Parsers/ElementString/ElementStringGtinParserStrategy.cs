using GS1CompanyPrefix;
using GS1EpcTranslator.Formatters;

namespace GS1EpcTranslator.Parsers.ElementString;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches GTIN in ElementString format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class ElementStringGtinParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the DigitalLink GTIN format (AI 01 and 21 or 10)
    /// </summary>
    public string Pattern => "^\\(01\\)(?<gtin>\\d{13})((\\(21\\)(?<ext>.{1,20}))|(\\(10\\)(?<lot>.{1,20})))?$";

    /// <summary>
    /// Transforms the ElementString GTIN parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the GTIN value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["gtin"]);
        var gcp = values["gtin"][..gcpLength];
        var gtin = values["gtin"][gcpLength..];

        return new GtinFormatter(gcp, gtin, values["ext"], values["lot"]);
    }
}
