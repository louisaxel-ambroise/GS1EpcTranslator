using GS1CompanyPrefix;
using GS1EpcTranslator.Formatters;
using GS1EpcTranslator.Helpers;

namespace GS1EpcTranslator.Parsers.DigitalLink;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches GIAI in DigitalLink format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class DlGiaiParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the DigitalLink GIAI format (AI 8004)
    /// </summary>
    public string Pattern => "^(?<domain>https?://.*)/(8004|giai)/0(?<giai>\\d+)$";

    /// <summary>
    /// Transforms the DigitalLink GIAI parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the GIAI value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["giai"]);
        var gcp = values["giai"][..gcpLength];
        var assetRef = values["giai"][gcpLength..];

        ArgumentOutOfRangeException.ThrowIfNotEqual(values["cd"], CheckDigit.Compute(values["grai"]));

        return new GiaiFormatter(
            gcp: gcp, 
            assetRef: assetRef);
    }
}
