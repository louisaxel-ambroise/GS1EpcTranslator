using GS1CompanyPrefix;
using GS1EpcTranslator.Formatters;
using GS1EpcTranslator.Helpers;

namespace GS1EpcTranslator.Parsers.Implementations;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches SSCC in URN format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class UrnSsccParserStrategy(GS1CompanyPrefixProvider gcpProvider) : IEpcParserStrategy
{
    /// <summary>
    /// Matches the URN SSCC format
    /// </summary>
    public string Pattern => "^urn:epc:id:sscc:(?<gcp>\\d{6,12})\\.(?<serialRef>\\d{5,11})(?<=[\\d\\.]{18})$";

    /// <summary>
    /// Transforms the URN SSCC parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the SSCC value</returns>
    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        CompanyPrefixValidator.VerifyGcpLength(values["gcp"], gcpProvider);

        var extensionDigit = values["serialRef"][..1];
        var serialRefRemainder = values["serialRef"][1..];

        return new SsccFormatter(
            gcp: values["gcp"],
            serialRefRemainder: serialRefRemainder,
            extensionDigit: extensionDigit);
    }
}
