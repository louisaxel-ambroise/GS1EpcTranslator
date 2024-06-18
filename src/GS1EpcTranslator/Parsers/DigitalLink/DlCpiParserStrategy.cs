namespace GS1EpcTranslator.Parsers.DigitalLink;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches CPI in DigitalLink format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class DlCpiParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the DigitalLink CPI format (AI 8010 and 8011)
    /// </summary>
    public string Pattern => "^(?<domain>https?://.*)/(8010|cpi)/(?<cpid>\\d{1,30})/8011/(?<serial>\\d{1,12})$";

    /// <summary>
    /// Transforms the DigitalLink CPI parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the CPI value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["cpid"]);
        var gcp = values["cpid"][..gcpLength];
        var componentType = values["cpid"][gcpLength..];

        return new CpiFormatter(
            gcp: gcp,
            componentType: componentType,
            serial: values["serial"]);
    }
}
