using GS1CompanyPrefix;
using GS1EpcTranslator.Formatters;
using GS1EpcTranslator.Helpers;

namespace GS1EpcTranslator.Parsers.Implementations;

/// <summary>
/// Implementation of <see cref="IEpcParserStrategy"/> that matches GTIN in URN format
/// </summary>
/// <param name="companyPrefixProvider">The GCP prefix provider</param>
public sealed class UrnGtinParserStrategy(GS1CompanyPrefixProvider gcpProvider) : IEpcParserStrategy
{
    static readonly string SgtinPattern = "(?<gcp>\\d{6,12})\\.(?<itemRef>\\d{1,7})(?<=[\\d\\.]{14})\\.(?<ext>\\w{1,20})";
    static readonly string LgtinPattern = "(?<gcp>\\d{6,12})\\.(?<itemRef>\\d{1,7})(?<=[\\d\\.]{14})\\.(?<lot>\\w{1,20})";
    static readonly string UnserializedPattern = "(?<gcp>\\d{6,12})\\.(?<itemRef>\\d{1,7})(?<=[\\d\\.]{14})\\.\\*";

    /// <summary>
    /// Matches the URN GTIN format. Can be serialized (sgtin + ext, lgtin + lot) or non-serialized (sgtin without ext)
    /// </summary>
    public string Pattern => $"^urn:epc:id((:sgtin:{SgtinPattern})|(:lgtin:{LgtinPattern})|(pat:sgtin:{UnserializedPattern}))$";

    /// <summary>
    /// Transforms the URN GTIN parsed values into a <see cref="IEpcFormatter"/>
    /// </summary>
    /// <param name="values">The values retrieved from the regex match</param>
    /// <returns>The <see cref="IEpcFormatter"/> for the GTIN value</returns>
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