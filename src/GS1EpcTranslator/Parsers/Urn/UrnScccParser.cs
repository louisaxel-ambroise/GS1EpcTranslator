using GS1CompanyPrefix;
using GS1EpcTranslator.Formatters;
using GS1EpcTranslator.Helpers;

namespace GS1EpcTranslator.Parsers.Implementations;

public sealed class UrnSsccParserStrategy(GS1CompanyPrefixProvider gcpProvider) : IEpcParserStrategy
{
    public string Pattern => "^urn:epc:id:sscc:(?<gcp>\\d{6,12})\\.(?<serialRef>\\d{5,11})(?<=[\\d\\.]{18})$";

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
