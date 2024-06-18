namespace GS1EpcTranslator.Parsers.DigitalLink;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches GSIN in DigitalLink format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class DlGsinParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the DigitalLink GSIN format (AI 402)
    /// </summary>
    public string Pattern => "^(?<domain>https?://.*)/(402|gsin)/(?<gsin>\\d{16})(?<cd>\\d)$";

    /// <summary>
    /// Transforms the DigitalLink GSIN parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the GSIN value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["gsin"]);
        var gcp = values["gsin"][..gcpLength];
        var shipperRef = values["gsin"][gcpLength..];

        ArgumentOutOfRangeException.ThrowIfNotEqual(values["cd"], CheckDigit.Compute(values["gsin"]));

        return new GsinFormatter(
            gcp: gcp,
            shipperRef: shipperRef);
    }
}
