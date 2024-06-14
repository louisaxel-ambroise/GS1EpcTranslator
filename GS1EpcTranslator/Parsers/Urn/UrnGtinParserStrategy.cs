using GS1CompanyPrefix;
using GS1EpcTranslator.Formatters;
using GS1EpcTranslator.Helpers;

namespace GS1EpcTranslator.Parsers.Implementations;

public sealed class UrnGtinParserStrategy(GS1CompanyPrefixProvider gcpProvider) : IEpcParserStrategy
{
    static readonly string SgtinPattern = "(?<gcp>\\d{6,12})\\.(?<itemRef>\\d{1,7})(?<=[\\d\\.]{14})\\.(?<ext>\\w{1,20})";
    static readonly string LgtinPattern = "(?<gcp>\\d{6,12})\\.(?<itemRef>\\d{1,7})(?<=[\\d\\.]{14})\\.(?<lot>\\w{1,20})";
    static readonly string UnserializedPattern = "(?<gcp>\\d{6,12})\\.(?<itemRef>\\d{1,7})(?<=[\\d\\.]{14})\\.\\*";
    public string Pattern => $"^urn:epc:id((:sgtin:{SgtinPattern})|(:lgtin:{LgtinPattern})|(pat:sgtin:{UnserializedPattern}))$";

    public IEpcFormatter Transform(IDictionary<string, string> values)
    {
        CompanyPrefixValidator.VerifyGcpLength(values["gcp"], gcpProvider);

        return new GtinFormatter(
            gcp: values["gcp"],
            itemRef: values["itemRef"],
            ext: values["ext"],
            lot: values["lot"]);
    }
}