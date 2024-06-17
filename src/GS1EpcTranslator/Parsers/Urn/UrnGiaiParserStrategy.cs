using GS1CompanyPrefix;
using GS1EpcTranslator.Formatters;
using GS1EpcTranslator.Helpers;

namespace GS1EpcTranslator.Parsers.Implementations;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches GIAI in URN format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class UrnGiaiParserStrategy(GS1CompanyPrefixProvider gcpProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the URN GIAI format
    /// </summary>
    public string Pattern => "^urn:epc:id:giai:(?<gcp>\\d{6,12})\\.(?<assetRef>\\d*)$";

    /// <summary>
    /// Transforms the URN GIAI parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the GIAI value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        CompanyPrefixValidator.VerifyGcpLength(values["gcp"], gcpProvider);

        return new GiaiFormatter(
            gcp: values["gcp"],
            assetRef: values["assetRef"]);
    }
}
