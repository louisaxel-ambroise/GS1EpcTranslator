namespace GS1EpcTranslator.Parsers.ElementString;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches SGTIN pattern in ElementString format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class ElementStringSgtinPatternParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the ElementString SGTIN pattern format (AI 01)
    /// </summary>
    public string Pattern => "^\\(01\\)(?<indicator>\\d)(?<gtin>\\d{13})$";

    /// <summary>
    /// Transforms the ElementString SGTIN pattern parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the SGTIN pattern value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["gtin"]);
        var gcp = values["gtin"][..gcpLength];
        var gtin = values["gtin"][gcpLength..];

        return new SgtinPatternFormatter(values["indicator"], gcp, gtin);
    }
}
