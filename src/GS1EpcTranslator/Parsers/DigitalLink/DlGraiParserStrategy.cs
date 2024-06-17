using GS1CompanyPrefix;
using GS1EpcTranslator.Formatters;
using GS1EpcTranslator.Helpers;

namespace GS1EpcTranslator.Parsers.DigitalLink;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches GRAI in DigitalLink format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class DlGraiParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the DigitalLink GRAI format (AI 8003)
    /// </summary>
    public string Pattern => "^(?<domain>https?://.*)/(8003|grai)/0(?<grai>\\d{12})(?<cd>\\d)(?<sn>\\d+)$";

    /// <summary>
    /// Transforms the DigitalLink GRAI parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the GRAI value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["grai"]);
        var gcp = values["grai"][..gcpLength];
        var assetType = values["grai"][gcpLength..];

        ArgumentOutOfRangeException.ThrowIfNotEqual(values["cd"], CheckDigit.Compute(values["grai"]));

        return new GraiFormatter(
            gcp: gcp, 
            assetType: assetType, 
            serialNumber: values["sn"]);
    }
}
