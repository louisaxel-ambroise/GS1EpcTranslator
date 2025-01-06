namespace GS1EpcTranslator.Parsers.DigitalLink;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches GTIN pattern in DigitalLink format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class DlSgtinPatternParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the DigitalLink GTIN pattern format (AI 01)
    /// </summary>
    public string Pattern => "^https?://.*/01/(?<indicator>\\d)(?<gtin>\\d{13})$";

    /// <summary>
    /// Transforms the DigitalLink GTIN pattern parsed values into a <see cref="IEpcIdentifier"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcIdentifier"/> for the GTIN pattern value</returns>
    public IEpcIdentifier Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["gtin"]);
        var gcp = values["gtin"][..gcpLength];
        var gtin = values["gtin"][gcpLength..];

        return new GtinPattern(values["indicator"], gcp, gtin);
    }
}
