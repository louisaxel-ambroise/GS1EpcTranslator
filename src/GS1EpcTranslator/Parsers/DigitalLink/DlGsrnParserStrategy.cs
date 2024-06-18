namespace GS1EpcTranslator.Parsers.DigitalLink;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches GSRN in DigitalLink format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class DlGsrnParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the DigitalLink GSRN format (AI 8018)
    /// </summary>
    public string Pattern => "^(?<domain>https?://.*)/(8018|gsrn)/(?<gsrn>\\d{17})(?<cd>\\d)$";

    /// <summary>
    /// Transforms the DigitalLink GSRN parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the GSRN value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["gsrn"]);
        var gcp = values["gsrn"][..gcpLength];
        var serviceReference = values["gsrn"][gcpLength..];

        ArgumentOutOfRangeException.ThrowIfNotEqual(values["cd"], CheckDigit.Compute(values["gsrn"]));

        return new GsrnFormatter(
            gcp: gcp,
            serviceReference: serviceReference);
    }
}
