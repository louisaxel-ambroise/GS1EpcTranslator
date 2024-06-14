using GS1CompanyPrefix;
using GS1EpcTranslator.Formatters;
using GS1EpcTranslator.Helpers;

namespace GS1EpcTranslator.Parsers.DigitalLink;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches SSCC in DigitalLink format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class DlSsccParserStrategy(GS1CompanyPrefixProvider companyPrefixProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the DigitalLink SSCC format (AI 00)
    /// </summary>
    public string Pattern => "^(?<domain>https?://.*)/(00|sscc)/(?<ext>\\d)(?<sscc>\\d{16})(?<cd>\\d)$";

    /// <summary>
    /// Transforms the DigitalLink SSCC parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the SSCC value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        var gcpLength = companyPrefixProvider.GetCompanyPrefixLength(values["sscc"]);
        var gcp = values["sscc"][..gcpLength];
        var serialRefRemainder = values["sscc"][gcpLength..];

        ArgumentOutOfRangeException.ThrowIfLessThan(gcpLength, 0);
        ArgumentOutOfRangeException.ThrowIfNotEqual(values["cd"], CheckDigit.Compute(values["ext"] + values["sscc"]));

        return new SsccFormatter(
            gcp: gcp, 
            serialRefRemainder: serialRefRemainder, 
            extensionDigit: values["ext"]);
    }
}
